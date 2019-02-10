using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Data.EnumType;

namespace Organiser.Controllers
{
  
    public class HelperMethods
    {
        //starting from 0
        public static int NumberOfUserRoles = 8;

        public static int GetLocationIntValue(string location)
        {
            switch (location)
            {
                case "Folirung":
                    return 1;
                case "Handarbeit":
                    return 2;
                case "Inkchet":
                    return 3;
                case "Falcen":
                    return 4;
                case "Covertirung":
                    return 5;
                case "Lager":
                    return 6;
                case "Fahrer":
                    return 7;
                case "Orders":
                    return 8;
                default:
                    return 0;
            };

        }

        public static int GetStatusIntValue(string Status)
        {
            switch (Status)
            {
                case "NotStarted":
                    return 1;
                case "InProgress":
                    return 2;
                case "Finished":
                    return 3;
                default:
                    return 0;
            };

        }

        public static List<SelectListItem> RoleDefaults()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "- - Select - -", Value = "0"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)1).ToString(), Value = "1"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)2).ToString(), Value = "2"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)3).ToString(), Value = "3"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)4).ToString(), Value = "4"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)5).ToString(), Value = "5"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)6).ToString(), Value = "6"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)7).ToString(), Value = "7"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)8).ToString(), Value = "8"}
            };
        }
        public static List<SelectListItem> LocationDefaults()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "- - Select - -", Value = "0"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)1).ToString(), Value = "1"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)2).ToString(), Value = "2"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)3).ToString(), Value = "3"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)4).ToString(), Value = "4"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)5).ToString(), Value = "5"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)6).ToString(), Value = "6"},
                new SelectListItem { Selected = false, Text = ((Enums.Department)7).ToString(), Value = "7"},
            };
        }

        public static List<SelectListItem> StatusDefaults(int selectedStatus = 0)
        {
            if (selectedStatus != 0)
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Selected = selectedStatus == 1, Text = ((Enums.Statuses)1).ToString(), Value = ((Enums.Statuses)1).ToString()},
                    new SelectListItem { Selected = selectedStatus == 2, Text = ((Enums.Statuses)2).ToString(), Value = ((Enums.Statuses)2).ToString()},
                    new SelectListItem { Selected = selectedStatus == 3, Text = ((Enums.Statuses)3).ToString(), Value = ((Enums.Statuses)3).ToString()}
                };
            }

            return new List<SelectListItem>
            {
                new SelectListItem { Selected = false, Text = ((Enums.Statuses)1).ToString(), Value = ((Enums.Statuses)1).ToString()},
                new SelectListItem { Selected = false, Text = ((Enums.Statuses)2).ToString(), Value = ((Enums.Statuses)2).ToString()},
                new SelectListItem { Selected = false, Text = ((Enums.Statuses)3).ToString(), Value = ((Enums.Statuses)3).ToString()}
            };
        }

        public static List<List<SelectListItem>> LocationDropdownsWithSelectedLocations(List<int> userRoles)
        {
            List<List<SelectListItem>> dropDowns = new List<List<SelectListItem>>();
            for (int i = 0; i < NumberOfUserRoles; i++)
            {
                List<SelectListItem> selectListItems = LocationDefaults();
                if (userRoles.Count() > i)
                {
                    selectListItems.FirstOrDefault(x => x.Value == userRoles[i].ToString()).Selected = true;
                    selectListItems[0].Selected = false;
                }
                dropDowns.Add(selectListItems);
            }
            return dropDowns;
        }

        public static List<List<SelectListItem>> RoleDropdownsWithSelectedRoles(List<int> userRoles)
        {
            List<List<SelectListItem>> dropDowns = new List<List<SelectListItem>>();
            for (int i = 0; i < NumberOfUserRoles; i++)
            {
                List<SelectListItem> selectListItems = RoleDefaults();
                if (userRoles.Count() > i)
                {
                    selectListItems.FirstOrDefault(x => x.Value == userRoles[i].ToString()).Selected = true;
                    selectListItems[0].Selected = false;
                }
                dropDowns.Add(selectListItems);
            }
            return dropDowns;
        }

        public static List<SelectListItem> DisplayUserRolesDropDown(List<string> userRoles)
        {
            List<SelectListItem> userRolesDropDown = new List<SelectListItem>();
            userRolesDropDown.Add(new SelectListItem { Selected = true, Text = "--User Roles--" });
            for (int i = 0; i < userRoles.Count(); i++)
            {
                userRolesDropDown.Add(new SelectListItem { Selected = false, Text = userRoles[i]});
            }
            if (userRolesDropDown.Count == 1)
            {
                return new List<SelectListItem>()
                {
                    new SelectListItem { Selected = true, Text = "--No Roles--" }
                };
            }

            return userRolesDropDown;
        }
    }
}


