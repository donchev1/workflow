using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Organiser.Controllers
{
    public enum Statuses { NotStarted = 1, InProgress = 2, Finished = 3 }
    public enum Locations
    {
        Folirane = 1,
        ManualWork = 2,
        InkChet = 3,
        Falcing = 4,
        Kovertirane = 5,
        Sklad = 6,
        Drivers = 7,
        Orders = 8
    }
    public class HelperMethods
    {
        public IUserRepository _userRepository;
        public static int NumberOfUserRoles = 8;

        public static int GetLocationIntValue(string location)
        {
            switch (location)
            {
                case "Folirane":
                    return 1;
                case "ManualWork":
                    return 2;
                case "InkChet":
                    return 3;
                case "Falcing":
                    return 4;
                case "Kovertirane":
                    return 5;
                case "Sklad":
                    return 6;
                case "Drivers":
                    return 7;
                case "Orders":
                    return 8;
                default:
                    return 0;
            };

        }
        public static List<SelectListItem> RoleDefaults()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "- - Select - -", Value = "0"},
                new SelectListItem { Selected = false, Text = ((Locations)1).ToString(), Value = "1"},
                new SelectListItem { Selected = false, Text = ((Locations)2).ToString(), Value = "2"},
                new SelectListItem { Selected = false, Text = ((Locations)3).ToString(), Value = "3"},
                new SelectListItem { Selected = false, Text = ((Locations)4).ToString(), Value = "4"},
                new SelectListItem { Selected = false, Text = ((Locations)5).ToString(), Value = "5"},
                new SelectListItem { Selected = false, Text = ((Locations)6).ToString(), Value = "6"},
                new SelectListItem { Selected = false, Text = ((Locations)7).ToString(), Value = "7"},
                new SelectListItem { Selected = false, Text = ((Locations)8).ToString(), Value = "8"}
            };
        }
        public static List<SelectListItem> LocationDefaults()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "- - Select - -", Value = "0"},
                new SelectListItem { Selected = false, Text = ((Locations)1).ToString(), Value = "1"},
                new SelectListItem { Selected = false, Text = ((Locations)2).ToString(), Value = "2"},
                new SelectListItem { Selected = false, Text = ((Locations)3).ToString(), Value = "3"},
                new SelectListItem { Selected = false, Text = ((Locations)4).ToString(), Value = "4"},
                new SelectListItem { Selected = false, Text = ((Locations)5).ToString(), Value = "5"},
                new SelectListItem { Selected = false, Text = ((Locations)6).ToString(), Value = "6"},
                new SelectListItem { Selected = false, Text = ((Locations)7).ToString(), Value = "7"},
            };
        }

        public static List<SelectListItem> StatusDefaults()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Selected = false, Text = ((Statuses)1).ToString(), Value = ((Statuses)1).ToString()},
                new SelectListItem { Selected = false, Text = ((Statuses)2).ToString(), Value = ((Statuses)2).ToString()},
                new SelectListItem { Selected = false, Text = ((Statuses)3).ToString(), Value = ((Statuses)3).ToString()}
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


