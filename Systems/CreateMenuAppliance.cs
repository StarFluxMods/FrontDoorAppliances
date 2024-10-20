using System;
using FrontDoorAppliances.Customs.Appliances;
using Kitchen;
using KitchenLib.Utils;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FrontDoorAppliances.Systems
{
    [UpdateAfter(typeof(CreatePracticeTrigger))]
    [UpdateAfter(typeof(CreateRerollTrigger))]
    public class CreateMenuAppliance : NightSystem, IModSystem
    {
        private EntityQuery _cPositions;
        protected override void Initialise()
        {
            base.Initialise();
            _cPositions = GetEntityQuery(typeof(CPosition));
        }

        protected override void OnUpdate()
        {
            if (HasSingleton<SCreateMenuAppliance>())
            {
                return;
            }

            if (GetNextAvailableCPosition(4, out Vector3 result, 10))
            {
                Entity entity = EntityManager.CreateEntity(typeof(CCreateAppliance), typeof(CPosition), typeof(SCreateMenuAppliance), typeof(CDoNotPersist));
                EntityManager.SetComponentData(entity, new CCreateAppliance
                {
                    ID = GDOUtils.GetCustomGameDataObject<GlobalMenu>().ID
                });
                EntityManager.SetComponentData(entity, new CPosition(result));
            }
            else
            {
                Mod.Logger.LogError("Failed to find viable spawn position");
            }
        }
        
        public bool GetNextAvailableCPosition(int depth, out Vector3 result, int maxdepth = 10)
        {
            if (depth > maxdepth)
            {
                result = Vector3.zero;
                return false;
            }

            Vector3 frontDoor = GetFrontDoor();
            int num = frontDoor.x > 0f ? -1 : 1;

            Vector3 checkPosition = frontDoor + new Vector3(num * depth, 0f, -1f);
            
            if (IsPositionInUse(checkPosition))
            {
                return GetNextAvailableCPosition(depth + 1, out result, maxdepth);
            }

            result = checkPosition;
            return true;
        }

        public bool IsPositionInUse(Vector3 checkPosition, float TOLERANCE = 0.1f)
        {
            using (NativeArray<Entity> cPositions = _cPositions.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity cPosition in cPositions)
                {
                    if (Require(cPosition, out CPosition position))
                    {
                        if ((Math.Abs(checkPosition.x - position.Position.x) < TOLERANCE) && (Math.Abs(checkPosition.y - position.Position.y) < TOLERANCE) && (Math.Abs(checkPosition.z - position.Position.z) < TOLERANCE))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private struct SCreateMenuAppliance : IComponentData, IModComponent
        {
        }
    }
}