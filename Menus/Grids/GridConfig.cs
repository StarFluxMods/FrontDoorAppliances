using System.Collections.Generic;
using Kitchen.Modules;
using UnityEngine;

namespace FrontDoorAppliances.Menus.Grids
{
    public class GridConfig : GridMenuConfig
    {
        public override GridMenu Instantiate(Transform container, int player, bool has_back)
        {
            return new MenuGridMenu(Menus, container, player, has_back);
        }

        public List<GridMenuConfig> Menus;
    }
}