using FrontDoorAppliances.Components;
using Kitchen;
using KitchenMods;

namespace FrontDoorAppliances.Systems
{
    public class ActivateMenuIndicator : InteractionSystem, IModSystem
    {
        protected override bool AllowActOrGrab => true;

        protected override InteractionMode RequiredMode => InteractionMode.Appliances;

        protected override bool ShouldAct(ref InteractionData data)
        {
            if (!Require(data.Target, out Editor)) return false;

            bool flag = data.Attempt.Type == InteractionType.Grab;
            bool flag2 = base.ShouldAct(ref data);
            return Editor.UseGrab == flag && flag2;
        }

        protected override bool IsPossible(ref InteractionData data)
        {
            return Has<CMenuIndicator>(data.Target) && Require(data.Target, out Editor) && (!Require(data.Target, out OwnedByPlayer) || !(OwnedByPlayer.Player != data.Interactor));
        }

        protected override void Perform(ref InteractionData data)
        {
            Editor.IsTriggered = true;
            Editor.TriggerEntity = data.Interactor;
            SetComponent(data.Target, Editor);
        }

        private CTriggerPlayerSpecificUI Editor;

        private COwnedByPlayer OwnedByPlayer;
    }
}