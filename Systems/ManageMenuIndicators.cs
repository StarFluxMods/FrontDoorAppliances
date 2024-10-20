using FrontDoorAppliances.Components;
using Kitchen;
using KitchenLib.Utils;
using KitchenMods;
using Unity.Entities;

namespace FrontDoorAppliances.Systems
{
    public class ManageMenuIndicators : PlayerSpecificUIIndicator<CMenuIndicator, CMenuIndicatorInfo>, IModSystem
    {
        protected override ViewType ViewType => (ViewType)VariousUtils.GetID("RestaurantMenuIndicator");

        protected override CMenuIndicatorInfo GetInfo(Entity source, CMenuIndicator selector, CTriggerPlayerSpecificUI trigger, CPlayer player)
        {
            return new CMenuIndicatorInfo
            {
                Player = player,
                PlayerEntity = trigger.TriggerEntity
            };
        }
    }
}