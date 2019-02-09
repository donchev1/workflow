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
        public AppDbContext _context;

        public LogController( AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index(string orderNumber, string userName, int? page, string message = "", int messageType = 0)
        {
            using (UnitOfWork uow = new UnitOfWork(_context))
            {
                IQueryable<Log> Logs;


                int pageSize = 20;
                if (!UserIsAdmin())
                {
                    RedirectToAction("Index", "Order");
                }

                if (orderNumber != null)
                {
                    Logs = uow.LogRepository.GetFilteredToIQuerable(x => x.OrderNumber == orderNumber);
                    if (Logs == null)
                    {
                        Logs = uow.LogRepository.GetAllToIQuerable();
                        ViewBag.errorMessage = "There are no event records related to order with order number: " + orderNumber;
                    }
                }
                else if (userName != null)
                {
                    Logs = uow.LogRepository.GetFilteredToIQuerable(x => x.UserName == userName);
                    if (Logs == null)
                    {
                        Logs = uow.LogRepository.GetAllToIQuerable();
                        ViewBag.errorMessage = "There are no event records related to user with user name: " + userName;
                    }
                }
                else
                {
                    Logs = uow.LogRepository.GetAllToIQuerable();
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
                    uow.LogRepository.RemoveRange(x => x.CreatedAt < eraseTo );
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
            using(var uow = new UnitOfWork(_context)) 
            {
            string username = HttpContext.User.Identity.Name;
                var user = uow.UserRepository.GetFilteredToIQuerable((x => x.UserName == username));
                return user.Select(x => x.IsAdmin == true).FirstOrDefault();
            }
        }

        private IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }
    }

}
