using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Models;
using Organiser.ViewModels;
using System;
using System.Threading.Tasks;
using Organiser.Data.EnumType;
using Organiser.Data.UnitOfWork;
using static Organiser.Controllers.HelperMethods;

namespace Organiser.Controllers
{
    public class MessagesController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public MessagesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(int id, int? page, string message = "", int messageType = 0)
        {

            using (_unitOfWork)
            {
                if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue(((Enums.Locations)id).ToString())))
                {
                    return RedirectToAction("Logout", "Account");
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

                var notes = _unitOfWork.NoteRepository.GetNotesForLocation(id);
                int pageSize = 5;

                _unitOfWork.NoteRepository.UpdateMonitor(id, false);

                return View(new MessagesViewModel()
                {
                    Notes = await PaginatedList<Note>.CreateAsync(notes.AsNoTracking(), page ?? 1, pageSize),
                    LocationName = ((Enums.Locations)id).ToString(),
                    LocationNameNum = id,
                    userIsAdmin = UserIsAdmin()
                });

            }        }

        [Authorize]
        [HttpGet]
        public IActionResult WriteNote(int DepartmentStateId)
        {
            if (DepartmentStateId > 0 && DepartmentStateId < 8)
            {
                return View(new Note
                {
                    LocationName = ((Enums.Locations)DepartmentStateId).ToString(),
                    Location = DepartmentStateId
                });
            }
            else
            {
                return Error("Click on the Messages button on the top navigation menu!");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult WriteNote(Note note)
        {
            DateTime present = DateTime.Now;
            Note oldestNote = _unitOfWork.NoteRepository.GetOldestNote();

            if (ModelState.IsValid)
            {
                note.CreatedAt = DateTime.Now;
                note.Author = HttpContext.User.Identity.Name;

                if (!_unitOfWork.NoteRepository.MonitorIsCreated())
                {
                    _unitOfWork.NoteRepository.CreateMonitor();
                }

               
                _unitOfWork.NoteRepository.UpdateMonitor(note.Location, true);

                _unitOfWork.NoteRepository.Add(note);
                _unitOfWork.Complete();
                note.LocationName = ((Enums.Locations)note.Location).ToString();
                ViewBag.successMessage = "Message sent!";
                return View(note);
            }
            else
            {
                return View(note);
            }
        }

        [HttpPost]
        public bool CheckForNewMessages(int DepartmentStateId)
        {
            if (!_unitOfWork.NoteRepository.MonitorIsCreated())
            {
                _unitOfWork.NoteRepository.CreateMonitor();
            }
            bool result = _unitOfWork.NoteRepository.HasNewMessages(DepartmentStateId);
            return result;
        }

        [HttpPost]
        public IActionResult EraseMessages(DateTime eraseTo, int locationNameNum)
        {
            if (!UserIsAdmin())
            {
                return Error("You need admin privileges.");
            }
            if(eraseTo != DateTime.MinValue)
            {

               _unitOfWork.NoteRepository.EraseNotesOlderThanDate(eraseTo);
                return RedirectToAction("Index", new { id = locationNameNum, message = "Messages older than " + eraseTo.ToShortDateString() + " deleted.", messageType = 2 });
            }
            else
            {
                return RedirectToAction("Index", new { id = locationNameNum , message = "Please select a date first.", messageType = 1 });
            }
        }

        private IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }

        private bool UserIsAdmin()
        {
            string UserName = HttpContext.User.Identity.Name;
            return _unitOfWork.UserRepository.IsAdmin(UserName);
        }

    }
}
