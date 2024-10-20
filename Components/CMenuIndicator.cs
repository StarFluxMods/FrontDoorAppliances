using KitchenData;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace FrontDoorAppliances.Components
{
    public struct CMenuIndicator : IApplianceProperty, IAttachableProperty, IComponentData, IPlayerSpecificUISource, IModComponent
    {
        Vector3 IPlayerSpecificUISource.DrawLocation => DrawLocation;
        public Vector3 DrawLocation;
    }
}