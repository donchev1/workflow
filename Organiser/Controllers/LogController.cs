using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Actions;
using Organiser.Actions.ActionObjects;
using Organiser.Data.Models;
using Organiser.Data.UnitOfWork;
using Organiser.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Controllers
{
    public class LogController : Controller
    {
        public IUnitOfWork _unitOfWork;
        private readonly ILogActions _logActions;

        public LogController(ILogActions logActions, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logActions = logActions;
        }

        [Authorize]
        public async Task<IActionResult> Index(string orderNumber, string userName, int? page, string message = "", int messageType = 0)
        {
            if (!UserIsAdmin())
            {
                Error("You don't have the right permissions to do this.");
            }
            LogIndexActionObject _actionObject = await _logActions.Index(orderNumber, userName, page);

            return View(new LogsViewModel()
            {
                Logs = _actionObject.LogList
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
                _logActions.EraseLogs(eraseTo);
                return RedirectToAction("Index", new { message = "Log context older than " + eraseTo.ToShortDateString() + " deleted.", messageType = 2 });
            }
            else
            {
                return RedirectToAction("Index", new { message = "Please select a date first.", messageType = 1 });
            }
        }

        private bool UserIsAdmin()
        {
            return User.IsInRole("admin");
        }

        private IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }
    }

}
