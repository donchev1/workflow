using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Context;
using Organiser.Data.Models;
using Organiser.Data.UnitOfWork;
using Organiser.Models;
using Organiser.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            using (UnitOfWork uow = new UnitOfWork(_context))
            {
                IQueryable<Log> Logs2;


                int pageSize = 20;
                if (!UserIsAdmin())
                {
                    RedirectToAction("Index", "Order");
                }

                if (orderNumber != null)
                {
                    Logs2 = uow.LogRepository.GetFilteredToIQuerable(x => x.OrderNumber == orderNumber);
                    if (Logs2 == null)
                    {
                        Logs2 = uow.LogRepository.GetAllToIQuerable();
                        ViewBag.errorMessage = "There are no event records related to order with order number: " + orderNumber;
                    }
                }
                else if (userName != null)
                {
                    Logs2 = uow.LogRepository.GetFilteredToIQuerable(x => x.UserName == userName);
                    if (Logs2 == null)
                    {
                        Logs2 = uow.LogRepository.GetAllToIQuerable();
                        ViewBag.errorMessage = "There are no event records related to user with user name: " + userName;
                    }
                }
                else
                {
                    Logs2 = uow.LogRepository.GetAllToIQuerable();
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
                    //Logs = await PaginatedList<Log_Old>.CreateAsync(Logs.AsNoTracking(), page ?? 1, pageSize);
                    Logs2 = await PaginatedList<Log>.CreateAsync(Logs2.AsNoTracking(), page ?? 1, pageSize)
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
                using (UnitOfWork uow = new UnitOfWork(_context))
                {
                    _logRepository.EraseLogsOlderThanDate(eraseTo);
                    uow.LogRepository.RemoveRange(x => x. );
                    return RedirectToAction("Index", new { message = "Log context older than " + eraseTo.ToShortDateString() + " deleted.", messageType = 2 });
                }
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
