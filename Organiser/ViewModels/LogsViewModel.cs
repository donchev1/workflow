﻿using Microsoft.AspNetCore.Mvc;
using Organiser.Controllers;
using Organiser.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.ViewModels
{
    public class LogsViewModel 
    {
        public PaginatedList<Log_Old> Logs { get; set; }
    }

}
