using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Organiser.Models;
using Organiser.ViewModels;
using static Organiser.Controllers.HelperMethods;

namespace Organiser.Controllers
{


    [Authorize]
    public class LocStateController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private IOrderRepository _orderRepository;
        private IUserRepository _userRepository;
        private ILocStateRepository _locStateRepository;
        private INoteRepository _noteRepository;
        private ILogRepository _logRepository;

        public LocStateController(AppDbContext context,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            ILocStateRepository locStateRepository,
            INoteRepository noteRepository,
            ILogRepository logRepository)
            
        {
            _noteRepository = noteRepository;
            _appDbContext = context;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _locStateRepository = locStateRepository;
            _logRepository = logRepository;
        }

        [Authorize]
        public Dictionary<string, string> MarkAsReady(EntityOrganiserViewModel model)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            
            if (!ModelState.IsValid)
            { 
                result.Add("MessageType", "Error");
                result.Add("Message", "Invalid selection. Please refresh the page and try again.");
                return result;
            }

            try
            { 
            LocState locState = _locStateRepository.GetLocStateById(model.LocStateId);
            Order order = _orderRepository.GetOrderById(locState.OrderId);
            string additionalMessage = "";

            if (locState.EntitiesInProgress < model.EntitiesPassed)
            {
                result.Add("MessageType", "Error");
                result.Add("Message", "There are only " + locState.EntitiesInProgress.ToString() + " entities in progress. Refresh the page.");
                return result;
            }
           
            locState.EntitiesReadyForCollection += model.EntitiesPassed;
            locState.EntitiesInProgress -= model.EntitiesPassed;
            locState.EntitiesPassedThrought += model.EntitiesPassed;

            if (locState.TotalEntityCount ==  locState.EntitiesPassedThrought)
            {
                locState.Status = ((Statuses)3).ToString();
                locState.Finish = DateTime.Now;
            }

            //TODO test this if statement
            
            if ( _locStateRepository.IsLastOrderLocState(locState.OrderId, locState.LocStateId))
            {
                order.EntitiesCompleted += model.EntitiesPassed;
                if (locState.EntitiesReadyForCollection == order.EntityCount)
                {
                    order.FinshedAt = DateTime.Now;
                    order.Status = ((Statuses)3).ToString();
                    additionalMessage = "Order status: " + ((Statuses)3).ToString() + ".";
                }
                 _appDbContext.Update(order);

            }



             _appDbContext.Update(locState);
            _appDbContext.SaveChanges();
            result.Add("MessageType", "Success");

            result.Add("Message", model.EntitiesPassed.ToString() + 
                " entities successfully marked as ready for collection!" + additionalMessage);

                _logRepository.CreateLog(
                  HttpContext.User.Identity.Name,
                  "Marked " + model.EntitiesPassed.ToString() + " " + order.EntityType + " as ready for collection.",
                  DateTime.Now,
                  order.OrderNumber);

                return result;
            }
            catch(Exception ex)
            {
                result.Add("MessageType", "Error");
                result.Add("Message", "Invalid selection. Please refresh the page and try again.");
                return result;
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult MyWork(int locStateId, string errorType = "", string message = "", bool showMessages = false)
        {
            string UserName = HttpContext.User.Identity.Name;
            List<int> userRoles = _userRepository.GetUserRolesByUserName(UserName);

            if (!userRoles.Contains(locStateId))
            {
               return Error("You don't have the right privileges to view this page. Speak to your administrator for more information.");
            }

            if (errorType != "")
            {
                if (errorType == "Success")
                {
                    ViewBag.successMessage = message;
                }
                else
                {
                    ViewBag.errorMessage = message;
                }
            }

            List<Order> orderList = _orderRepository.GetAllActiveOrdersForLocation(locStateId);

            if (orderList.Count() <1)
            {
                return View( new OrderStateViewModel
                {
                    LocationName = ((Locations)locStateId).ToString(),
                    LocationNameNum = GetLocationIntValue(((Locations)locStateId).ToString()),
                    ShowMessages = showMessages
                });
            }

            return View( new OrderStateViewModel
            {
                LocationNameNum = GetLocationIntValue(orderList[0].LocStates[0].Name),
                LocationName = orderList[0].LocStates[0].Name, 
                OrderList = orderList,
                ShowMessages = showMessages
            });
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _appDbContext.LocStates.ToListAsync());
        }


        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locState = await _appDbContext.LocStates
                .SingleOrDefaultAsync(m => m.LocStateId == id);
            if (locState == null)
            {
                return NotFound();
            }

            return View(locState);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocStateId,Name,Status,EntityCount,Start,Finish,LocationPosition")] LocState locState)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.Add(locState);
                await _appDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(locState);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locState = await _appDbContext.LocStates.SingleOrDefaultAsync(m => m.LocStateId == id);
            if (locState == null)
            {
                return NotFound();
            }
            return View(locState);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocStateId,Name,Status,EntityCount,Start,Finish,LocationPosition")] LocState locState)
        {
            if (id != locState.LocStateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _appDbContext.Update(locState);
                    await _appDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_locStateRepository.LocStateExists(locState.LocStateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(locState);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locState = await _appDbContext.LocStates
                .SingleOrDefaultAsync(m => m.LocStateId == id);
            if (locState == null)
            {
                return NotFound();
            }

            return View(locState);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locState = await _appDbContext.LocStates.SingleOrDefaultAsync(m => m.LocStateId == id);
            _appDbContext.LocStates.Remove(locState);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }
    }
}
 