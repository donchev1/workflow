using Microsoft.AspNetCore.Mvc;
using Organiser.Controllers;
using Organiser.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Organiser.Data.Models;

namespace Organiser.ViewModels
{
    public class LogsViewModel 
    {
       public PaginatedList<Log> Logs { get; set; }
       // public PaginatedList<Log> Logs2 { get; set; }
    }

}
