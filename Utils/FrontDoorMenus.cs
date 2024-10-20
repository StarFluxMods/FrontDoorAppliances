using System.Collections.Generic;
using Kitchen.Modules;

namespace FrontDoorAppliances.Utils
{
    public class FrontDoorMenus
    {
        internal static List<GridMenuConfig> Menus = new List<GridMenuConfig>();
        
        public static bool Register(GridMenuConfig menu)
        {
            if (!Menus.Contains(menu))
            {
                Menus.Add(menu);
                return true;
            }

            return false;
        }
        
        public static bool ForceRegister(GridMenuConfig menu)
        {
            Menus.Add(menu);
            return true;
        }
    }
}