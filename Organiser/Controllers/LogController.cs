using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Organiser.Models;
using Organiser.ViewModels;
using static Organiser.Controllers.HelperMethods;

namespace Organiser.Controllers
{
    public class LogController : Controller
    {
        public AppDbContext _appDbContext;
        public ILogRepository _logRepository;
        public IUserRepository _userRepository;

        public LogController(
            AppDbContext appDbContext,
            IUserRepository userRepository,
            ILogRepository logRepository)
        {
            _appDbContext = appDbContext;
            _logRepository = logRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index(string orderNumber, string userName, int? page, string message = "", int messageType = 0)
        {
            IQueryable<Log> Logs;
            int pageSize = 20;
            if (!UserIsAdmin())
            {
                RedirectToAction("Index", "Order");
            }

            if (orderNumber != null)
            {
                Logs = _logRepository.GetActionRecordsByOrderNumber(orderNumber);
                if (Logs == null)
                {
                    Logs = _logRepository.GetAllLogs();
                    ViewBag.errorMessage = "There are no event records related to order with order number: " + orderNumber;
                }
            }
            else if (userName != null)
            {
                Logs = _logRepository.GetActionRecordsByUserName(userName);
                if (Logs == null)
                {
                    Logs = _logRepository.GetAllLogs();
                    ViewBag.errorMessage = "There are no event records related to user with user name: " + userName;
                }
            }
            else
            {
                Logs = _logRepository.GetAllLogs();
            }

            if (message != "")
            {
                if (messageType == 1)
                {
                    ViewBag.errorMessage = message;
                }
                else
                {
                    ViewBag.successMessage = message;
                }
            }

            return View(new LogsViewModel()
            {
                Logs = await PaginatedList<Log>.CreateAsync(Logs.AsNoTracking(), page ?? 1, pageSize)
            });
        }



        [HttpPost]
        public IActionResult EraseLogs(DateTime eraseTo)
        {
            if (!UserIsAdmin())
            {
                return Error("You need admin privileges.");
            }
            if (eraseTo != DateTime.MinValue)
            {

                _logRepository.EraseLogsOlderThanDate(eraseTo);
                return RedirectToAction("Index", new { message = "Log context older than " + eraseTo.ToShortDateString() + " deleted.", messageType = 2 });
            }
            else
            {
                return RedirectToAction("Index", new { message = "Please select a date first.", messageType = 1 });
            }
        }

        private bool UserIsAdmin()
        {
            string UserName = HttpContext.User.Identity.Name;
            return _userRepository.IsAdmin(UserName);
        }

        private IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }
    }
    
}
