using System.Collections.Generic;
using Controllers;
using FrontDoorAppliances.Components;
using Kitchen;
using Kitchen.Modules;
using KitchenMods;
using MessagePack;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


namespace FrontDoorAppliances.Views
{
    public class MenuIndicator : ResponsiveObjectView<MenuIndicator.ViewData, MenuIndicator.ResponseData>, IInputConsumer
    {
        public class UpdateView : ResponsiveViewSystemBase<ViewData, ResponseData>, IModSystem
        {
            private EntityQuery _indicators;

            protected override void Initialise()
            {
                base.Initialise();
                _indicators = GetEntityQuery(typeof(CLinkedView), typeof(CMenuIndicatorInfo));
            }

            protected override void OnUpdate()
            {
                using (NativeArray<Entity> indicators = _indicators.ToEntityArray(Allocator.Temp))
                {
                    foreach (Entity indicator in indicators)
                    {
                        if (!Require(indicator, out CLinkedView linkedView)) continue;
                        if (!Require(indicator, out CMenuIndicatorInfo craneEditorInfo)) continue;

                        SendUpdate(linkedView.Identifier, new ViewData
                        {
                            Player = craneEditorInfo.Player.PlayerID
                        });
                        ResponseData result = default(ResponseData);
                        if (ApplyUpdates(linkedView.Identifier, delegate(ResponseData data) { result = data; }, only_final_update: true))
                        {
                            craneEditorInfo.IsComplete = result.IsComplete;
                            Set(indicator, craneEditorInfo);
                        }
                    }
                }
            }
        }

        public GridMenuConfig RootMenuConfig;
        public Transform Container;
        private bool IsComplete;
        private int PlayerID;
        private InputLock.Lock Lock;
        private GridMenu GridMenu;
        private Stack<MenuStackElement> MenuStack = new Stack<MenuStackElement>();
        
        [MessagePackObject(false)]
        public struct ViewData : IViewData, IViewResponseData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int Player;

            public bool IsChangedFrom(ViewData check)
            {
                return Player != check.Player;
            }
        }

        [MessagePackObject(false)]
        public struct ResponseData : IResponseData, IViewResponseData
        {
            [Key(0)] public bool IsComplete;
        }

        private struct MenuStackElement
        {
            public GridMenuConfig Config;

            public int Index;
        }

        protected override void UpdateData(ViewData data)
        {
            if (InputSourceIdentifier.DefaultInputSource != null)
            {
                if (!Players.Main.Get(data.Player).IsLocalUser)
                {
                    gameObject.SetActive(value: false);
                    return;
                }
                gameObject.SetActive(value: true);
                InitialiseForPlayer(data.Player);
            }
        }
        
        private void InitialiseForPlayer(int player)
        {
            LocalInputSourceConsumers.Register(this);
            if (Lock.Type != 0)
            {
                InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
            }
            PlayerID = player;
            SetNewMenu(RootMenuConfig, 0, 0);
            Lock = InputSourceIdentifier.DefaultInputSource.SetInputLock(PlayerID, PlayerLockState.NonPause);
        }

        public override bool HasStateUpdate(out IResponseData state)
        {
            state = null;
            if (IsComplete)
            {
                state = new ResponseData
                {
                    IsComplete = IsComplete
                };
            }
            return IsComplete;
        }

        public InputConsumerState TakeInput(int player_id, InputState state)
        {
            if (PlayerID != 0 && player_id == PlayerID)
            {
                if (state.MenuTrigger == ButtonState.Pressed)
                {
                    IsComplete = true;
                    InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
                    return InputConsumerState.Terminated;
                }
                if (GridMenu != null && !GridMenu.HandleInteraction(state) && state.MenuCancel == ButtonState.Pressed)
                {
                    CloseMenu();
                }
                if (!IsComplete)
                {
                    return InputConsumerState.Consumed;
                }
                return InputConsumerState.Terminated;
            }
            return InputConsumerState.NotConsumed;
        }
        
        private void CloseMenu()
        {
            if (MenuStack.Count > 1)
            {
                int index = MenuStack.Pop().Index;
                MenuStackElement menuStackElement = MenuStack.Pop();
                SetNewMenu(menuStackElement.Config, index, menuStackElement.Index);
            }
            else
            {
                Remove();
            }
        }
        
        public override void Remove()
        {
            IsComplete = true;
            InputSourceIdentifier.DefaultInputSource.ReleaseLock(PlayerID, Lock);
            base.Remove();
        }
        
        private void OnDestroy()
        {
            LocalInputSourceConsumers.Remove(this);
        }
        
        private void SetNewMenu(GridMenuConfig menu, int new_index, int previous_index)
        {
            GridMenu?.Destroy();
            GridMenu = menu.Instantiate(Container, PlayerID, MenuStack.Count > 0);
            GridMenu.OnRequestMenu += delegate(GridMenuConfig c)
            {
                SetNewMenu(c, 0, GridMenu?.SelectedIndex() ?? 0);
            };
            GridMenu.OnGoBack += CloseMenu;
            GridMenu.SelectByIndex(new_index);
            MenuStack.Push(new MenuStackElement
            {
                Config = menu,
                Index = previous_index
            });
        }
    }
}