using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Context;
using Organiser.Data.UnitOfWork;
using Organiser.Models;
using Organiser.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Organiser.Data.Models;

namespace Organiser.Controllers
{
    public class LogController : Controller
    {
        public AppDbContext_Old _appDbContext;
        public AppDbContext _context;
        public ILogRepository_Old _logRepository;
        public IUserRepository _userRepository;

        public LogController(
        AppDbContext context,
        AppDbContext_Old appDbContext,
        IUserRepository userRepository,
        ILogRepository_Old logRepository)
        {
            _context = context;
            _appDbContext = appDbContext;
            _logRepository = logRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index(string orderNumber, string userName, int? page, string message = "", int messageType = 0)
        {
            using (var uow = new UnitOfWork(_context))
            {

                IQueryable<Log_Old> Logs;
                var Logs2 = new object();
                

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
                        Logs2 = uow.LogRepository.GetAllToList();
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
                    Logs = await PaginatedList<Log_Old>.CreateAsync(Logs.AsNoTracking(), page ?? 1, pageSize)
                });
            }
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
