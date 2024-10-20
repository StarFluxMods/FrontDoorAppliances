using System.Collections.Generic;
using Kitchen.Modules;
using KitchenLib.UI.PlateUp.Grids;
using UnityEngine;

namespace FrontDoorAppliances.Menus.Grids
{
    public class MenuGridMenu : KLPageGridMenu<GridMenuConfig>
    {
        public MenuGridMenu(List<GridMenuConfig> cosmetics, Transform container, int player, bool has_back) : base(cosmetics, container, player, has_back)
        {
        }

        protected override int RowLength => 5;
        protected override int ColumnLength => 2;

        protected override void SetupElement(GridMenuConfig item, GridMenuElement element, int playerID)
        {
            element.Set(item.Icon);
        }

        protected override void OnSelect(GridMenuConfig item)
        {
            if (Player != 0 && item != null)
            {
                RequestNewMenu(item);
            }
        }
    }
}