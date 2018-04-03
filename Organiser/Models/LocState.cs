﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Organiser.Controllers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Organiser.Models
{
    public class LocState 
    {
        public int LocStateId { get; set; }
        public string Name { get; set; }
        public int EntitiesPassedThrought { get; set; }
        public int EntitiesInProgress { get; set; }
        public int EntitiesReadyForCollection { get; set; }
        public int TotalEntityCount { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public int LocationPosition { get; set; }
        public string Status { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
