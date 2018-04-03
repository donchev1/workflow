using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Models;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Organiser.Controllers.HelperMethods;

namespace Organiser.Controllers
{
    public class MessagesController : Controller
    {
        private INoteRepository _noteRepository;
        private readonly AppDbContext _appDbContext;
        private IUserRepository _userRepository;
        public MessagesController(
            AppDbContext context,
            INoteRepository noteRepository,
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _noteRepository = noteRepository;
            _appDbContext = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(int id, int? page, string message = "", int messageType = 0)
        {

            if (!User.IsInRole(((Locations)id).ToString()))
            {
                return Error("You don't have persmissions to view this page. Contact your administrator for more information.");
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

            var notes = _noteRepository.GetNotesForLocation(id);
            int pageSize = 5;
            
            _noteRepository.UpdateMonitor(id, false);

            return View(new MessagesViewModel() {
                Notes = await PaginatedList<Note>.CreateAsync(notes.AsNoTracking(), page ?? 1, pageSize),
                LocationName = ((Locations)id).ToString(),
                LocationNameNum = id,
                userIsAdmin = UserIsAdmin()
            });            
        }

        [Authorize]
        [HttpGet]
        public IActionResult WriteNote(int locStateId)
        {
            if (locStateId > 0 && locStateId < 8)
            {
                return View(new Note
                {
                    LocationName = ((Locations)locStateId).ToString(),
                    Location = locStateId
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
            Note oldestNote = _noteRepository.GetOldestNote();

            if (ModelState.IsValid)
            {
                note.CreatedAt = DateTime.Now;
                note.Author = HttpContext.User.Identity.Name;

                if (!_noteRepository.MonitorIsCreated())
                {
                    _noteRepository.CreateMonitor();
                }

               
                _noteRepository.UpdateMonitor(note.Location, true);

                _appDbContext.Add(note);
                _appDbContext.SaveChanges();
                note.LocationName = ((Locations)note.Location).ToString();
                ViewBag.successMessage = "Message sent!";
                return View(note);
            }
            else
            {
                return View(note);
            }
        }

        [HttpPost]
        public bool CheckForNewMessages(int locStateId)
        {
            if (!_noteRepository.MonitorIsCreated())
            {
                _noteRepository.CreateMonitor();
            }
            bool result = _noteRepository.HasNewMessages(locStateId);
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

               _noteRepository.EraseNotesOlderThanDate(eraseTo);
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
            return _userRepository.IsAdmin(UserName);
        }

    }
}
