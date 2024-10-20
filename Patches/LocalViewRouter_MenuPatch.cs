using System.Collections.Generic;
using System.Reflection;
using FrontDoorAppliances.Menus.Grids;
using FrontDoorAppliances.Utils;
using FrontDoorAppliances.Views;
using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using KitchenLib.Utils;
using UnityEngine;

namespace FrontDoorAppliances.Patches
{
    [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
    public class LocalViewRouter_MenuPatch
    {
        private static GameObject NewUIPrefab = null;
        private static GameObject container;

        private static FieldInfo _AssetDirectory = ReflectionUtils.GetField<LocalViewRouter>("AssetDirectory");
        private static FieldInfo _Container = ReflectionUtils.GetField<CostumeChangeIndicator>("Container");

        public static List<GridMenuConfig> MENUSTOSHOW = new List<GridMenuConfig>();

        static bool Prefix(LocalViewRouter __instance, ViewType view_type, ref GameObject __result)
        {
            if (view_type != (ViewType)VariousUtils.GetID("RestaurantMenuIndicator"))
            {
                return true;
            }

            if (container == null)
            {
                container = new GameObject("temp");
                container.SetActive(false);
            }

            if (NewUIPrefab != null)
            {
                __result = NewUIPrefab;
                return false;
            }
            
            AssetDirectory AssetDirectory = (AssetDirectory)_AssetDirectory.GetValue(__instance);
            NewUIPrefab = GameObject.Instantiate(AssetDirectory.ViewPrefabs[ViewType.CostumeChangeInfo], container.transform);
            CostumeChangeIndicator costumeChangeIndicator = NewUIPrefab.GetComponent<CostumeChangeIndicator>();

            if (costumeChangeIndicator != null)
            {
                MenuIndicator upgradeIndicator = NewUIPrefab.AddComponent<MenuIndicator>();
                upgradeIndicator.Container = (Transform)_Container?.GetValue(costumeChangeIndicator);
                
                GridConfig rootMenu = ScriptableObject.CreateInstance<GridConfig>();
                rootMenu.Menus = FrontDoorMenus.Menus;
                
                upgradeIndicator.RootMenuConfig = rootMenu;
                
                Component.DestroyImmediate(costumeChangeIndicator);
            }

            __result = NewUIPrefab;
            return false;
        }
    }
}