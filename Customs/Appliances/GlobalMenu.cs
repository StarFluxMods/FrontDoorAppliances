using System;
using System.Collections.Generic;
using FrontDoorAppliances.Components;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using Shapes;
using TMPro;
using UnityEngine;
using Font = Kitchen.Font;

namespace FrontDoorAppliances.Customs.Appliances
{
    public class GlobalMenu : CustomAppliance
    {
        public override string UniqueNameID => "GlobalMenu";
        public override GameObject Prefab => Mod.Bundle.LoadAsset<GameObject>("GlobalMenu").AssignMaterialsByNames();

        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>
        {
            new CImmovable(),
            new CFixedRotation(),
            new CDestroyApplianceAtDay
            {
                HideBin = true
            },
            new CTriggerPlayerSpecificUI(),
            new CMenuIndicator()
        };

        public override void OnRegister(Appliance gameDataObject)
        {
            base.OnRegister(gameDataObject);

            Rectangle rectangle = gameDataObject.Prefab.GetChild("Floor_Label/Rectangle").AddComponent<Rectangle>();
            FontUpdater fontUpdater = gameDataObject.Prefab.GetChild("Floor_Label/Label").AddComponent<FontUpdater>();
            AutoGlobalLocal autoGlobalLocal = gameDataObject.Prefab.GetChild("Floor_Label/Label").AddComponent<AutoGlobalLocal>();

            autoGlobalLocal.Text = "Menus";

            rectangle.Color = new Color(0.3074376f, 0.2764863f, 0.4622642f, 0.4392157f);
            rectangle.Type = Rectangle.RectangleType.RoundedSolid;
            rectangle.CornerRadiii = new Vector4(0.15f, 0.15f, 0.15f, 0.15f);
            rectangle.Thickness = 0.05f;
        }
    }

    public class FontUpdater : MonoBehaviour
    {
        private void Awake()
        {
            Appliance prac = GameData.Main.Get<Appliance>(ApplianceReferences.PracticeModeTrigger);
            TextMeshPro tmp = prac.Prefab.GetComponentInChildren<TextMeshPro>();
            TextMeshPro textMeshPro = GetComponent<TextMeshPro>();
            textMeshPro.font = tmp.font;
        }
    }
}