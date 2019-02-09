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
        public IUnitOfWork _unitOfWork;

        public LogController( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public async Task<IActionResult> Index(string orderNumber, string userName, int? page, string message = "", int messageType = 0)
        {
            using (_unitOfWork)
            {

                IQueryable<Log> Logs;


                int pageSize = 20;
                if (!UserIsAdmin())
                {
                    RedirectToAction("Index", "Order");
                }

                if (orderNumber != null)
                {
                    Logs = _unitOfWork.LogRepository.GetFilteredToIQuerable(x => x.OrderNumber == orderNumber);
                    if (Logs == null)
                    {
                        Logs = _unitOfWork.LogRepository.GetAllToIQuerable();
                        ViewBag.errorMessage = "There are no event records related to order with order number: " + orderNumber;
                    }
                }
                else if (userName != null)
                {
                    Logs = _unitOfWork.LogRepository.GetFilteredToIQuerable(x => x.UserName == userName);
                    if (Logs == null)
                    {
                        Logs = _unitOfWork.LogRepository.GetAllToIQuerable();
                        ViewBag.errorMessage = "There are no event records related to user with user name: " + userName;
                    }
                }
                else
                {
                    Logs = _unitOfWork.LogRepository.GetAllToIQuerable();
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
                    _unitOfWork.LogRepository.RemoveRange(x => x.CreatedAt < eraseTo );
                    return RedirectToAction("Index", new { message = "Log context older than " + eraseTo.ToShortDateString() + " deleted.", messageType = 2 });
            }
            else
            {
                return RedirectToAction("Index", new { message = "Please select a date first.", messageType = 1 });
            }
        }

        private bool UserIsAdmin()
        {
            string username = HttpContext.User.Identity.Name;
                var user = _unitOfWork.UserRepository.GetFilteredToIQuerable((x => x.UserName == username));
                return user.Select(x => x.IsAdmin == true).FirstOrDefault();
        }

        private IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }
    }

}
