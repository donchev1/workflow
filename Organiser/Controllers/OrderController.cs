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
    public class OrderController : Controller
    {
        public AppDbContext _appDbContext;
        public IOrderRepository _orderRepository;
        public IUserRepository _userRepository;
        public ILocStateRepository _locStateRepository;
        public IHostingEnvironment _hostingEnvironment;
        public ILogRepository _logRepository;




        public OrderController(
            IHostingEnvironment hostingEnv,
            IOrderRepository orderList,
            AppDbContext appDbContext,
            IUserRepository userRepository,
            ILocStateRepository locStateRepository,
            ILogRepository logRepository)
        {
            _appDbContext = appDbContext;
            _orderRepository = orderList;
            _userRepository = userRepository;
            _locStateRepository = locStateRepository;
            _hostingEnvironment = hostingEnv;
            _logRepository = logRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? page, string SearchID = "")
        {
            IQueryable<Order> orders;
            int pageSize = 15;
            if (SearchID != "" && SearchID != null)
            {
                orders = _orderRepository.GetOrdersAndLocStatesBySearchId(SearchID);

                return View(new OrderStateViewModel
                {
                    OrderListPaginated = await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize),
                });
            }

            orders = _orderRepository.OrdersAndLocStates;

            return View(new OrderStateViewModel
            {
                OrderListPaginated = await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize)
            });
        }

        public async Task<IActionResult> AvailableWork(int locationNameNum, int? page)
        {
            int pageSize = 15;

            IQueryable<Order> orders = _orderRepository.GetAvailableOrdersForWork(locationNameNum);
            return View(new OrderStateViewModel
            {
                LocationNameNum = locationNameNum,
                LocationName = ((Locations)locationNameNum).ToString(),
                OrderListPaginated = await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize)
            });
        }



        // GET: Order/Create
        [Authorize]
        public IActionResult Create()
        {
            TempData["Locations"] = LocationDefaults();
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId, OrderNumber, EntityType, EntityCount")] Order order,
            int locname0, int locname1, int locname2, int locname3,
            int locname4, int locname5, int locname6)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_orderRepository.OrderExistsByOrderNumber(order.OrderNumber))
                    {
                        return Error("Error: order with order number " + order.OrderNumber + " already exists.");
                    }
                    List<int> locStatesUnorganised = new List<int> { locname0, locname1, locname2, locname3, locname4, locname5, locname6 };

                    order.CreatedAt = DateTime.Now;

                    List<int> locStatesOrganised = locStateOrganiser(locStatesUnorganised);
                    List<LocState> locStateObjects = new List<LocState>();

                    if (locStatesOrganised.Count() > 0)
                    {
                        locStateObjects = CreateOrderLocStates(locStatesOrganised, order);
                    }
                    else
                    {
                        locStatesOrganised.Add(7); //drivers
                        locStateObjects = CreateOrderLocStates(locStatesOrganised, order);
                    }

                    order.EntitiesNotProcessed = order.EntityCount;
                    order.LocStates = locStateObjects;
                    order.Status = ((Statuses)1).ToString();
                    _appDbContext.Add(order);
                    await _appDbContext.SaveChangesAsync();

                    _logRepository.CreateLog(
                        HttpContext.User.Identity.Name,
                        "Order created.",
                        DateTime.Now,
                        order.OrderNumber);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex;
                    return View("Error");
                }
            }
            TempData["Locations"] = LocationDefaults();
            return View("Create", order);
        }

        //[HttpGet]
        //public IActionResult CreateLocList(List<int?> orderIDandCount)
        //{
        //    TempData["Locations"] = HelperMethods.locationDefaults();
        //    return View(orderIDandCount);
        //}

        [HttpGet]
        public void TestMethod()
        {
            int order = 4003;
            int num = 1;
            LocState targetLocState = _locStateRepository.GetLocStateByOrderIdAndLocNum(order, num);

        }



        // GET: Order/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _appDbContext.Orders.SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            order.StatusDefaultsDropdown = StatusDefaults();
            return View(order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId, OrderNumber, EntityType, Status")] Order order)
        {

            if (!_userRepository.GetUserRolesByUserName(HttpContext.User.Identity.Name).Contains(GetLocationIntValue("Orders")))
            {
                return Error("You don't have permissions to do this.");
            }


            if (_orderRepository.GetOrderByOrderNumber(order.OrderNumber) != null && _orderRepository.GetOrderByOrderNumber(order.OrderNumber).OrderId != order.OrderId)
            {
                ViewBag.errorMessage = "Error: order with order number " + order.OrderNumber + " already exists.";
                order.StatusDefaultsDropdown = StatusDefaults();
                return View(order);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Order oldOrderState = _orderRepository.GetOrderAndLocStatesById(order.OrderId);

                    if (order.Status != oldOrderState.Status)
                    {
                        List<LocState> newLocStates = oldOrderState.LocStates;
                        if (order.Status == ((Statuses)3).ToString())
                        {
                            foreach (LocState ls in newLocStates)
                            {
                                ls.Status = order.Status;
                                ls.EntitiesInProgress = 0;
                                ls.EntitiesReadyForCollection = 0;
                            }
                        }
                        oldOrderState.EntitiesNotProcessed = 0;
                        oldOrderState.LocStates = newLocStates;
                    }
                    UpdateOrderValues(ref oldOrderState, order);

                    _appDbContext.Update(oldOrderState);
                    await _appDbContext.SaveChangesAsync();
                    _logRepository.CreateLog(
                            HttpContext.User.Identity.Name,
                            "Edited order.",
                            DateTime.Now,
                        order.OrderNumber);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_orderRepository.OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = order.OrderId });
            }
            return View(order);
        }

        [HttpPost]
        [Authorize]
        public PassEntitiesResultModel PassEntities(EntityOrganiserViewModel model)
        {
            LocState sourceLocState = _locStateRepository.GetLocStateById(model.LocStateId);
            LocState targetLocState = _locStateRepository.GetLocStateByOrderIdAndLocNum(sourceLocState.OrderId, sourceLocState.LocationPosition + 1);

            if (sourceLocState.EntitiesReadyForCollection >= model.EntitiesPassed)
            {
                sourceLocState.EntitiesReadyForCollection -= model.EntitiesPassed;
            }
            else
            {
                Error("Entity count insufficient!");
            }

            if (targetLocState.Status == ((Statuses)1).ToString())
            {
                targetLocState.Status = ((Statuses)2).ToString();
                targetLocState.Start = DateTime.Now;
            }

            targetLocState.EntitiesInProgress += model.EntitiesPassed;

            _appDbContext.Update(sourceLocState);
            _appDbContext.Update(targetLocState);

            _appDbContext.SaveChanges();
            _logRepository.CreateLog(
                        HttpContext.User.Identity.Name,
                        "Moved " + model.EntitiesPassed.ToString() + " " + _orderRepository.GetOrderById(sourceLocState.OrderId).EntityType + " from " + sourceLocState.Name + " to " + targetLocState.Name + ".",
                        DateTime.Now,
                        _orderRepository.GetOrderNumberByOrderId(sourceLocState.OrderId));

            PassEntitiesResultModel resultModel = new PassEntitiesResultModel()
            {
                SourceEntitiesReadyForCollection = sourceLocState.EntitiesReadyForCollection,
                TargetEntitiesInProgress = targetLocState.EntitiesInProgress,
                TargetLocStateId = targetLocState.LocStateId,
                TargetLocStateStatus = targetLocState.Status
            };

            return resultModel;
        }



        // GET: Order/Details/5
        [Authorize]
        public IActionResult Details(int id, int messageType = 0, string message = "")
        {
            if (message != "")
            {
                if (messageType == 1)
                {
                    ViewBag.successMessage = message;
                }
                else
                {
                    ViewBag.errorMessage = message;
                }
            }
            var order = _orderRepository.GetOrderAndLocStatesById(id);
            //string userRole = ((UserRoles)_userRepository.GetUserByName(HttpContext.User.Identity.Name).Role).ToString();

            if (order == null)
            {
                ViewBag.ErrorMessage = "Order doesn't exist.";
                return View("Error");
            }
            var webRoot = _hostingEnvironment.WebRootPath;
            if (!Directory.Exists(webRoot + "/OrderFiles/"))
            {
                Directory.CreateDirectory(webRoot + "/OrderFiles/");
            }
            if (!Directory.Exists(webRoot + "/OrderFiles/" + order.OrderId.ToString() + "/"))
            {
                Directory.CreateDirectory(webRoot + "/OrderFiles/" + order.OrderId.ToString() + "/");
            }
            string[] strfiles = Directory.GetFiles(webRoot + "/OrderFiles/" + order.OrderId.ToString() + "/", "*.*");
            Dictionary<string, string> fileNamesUrls = new Dictionary<string, string>();

            if (strfiles.Length > 0)
            {
                string fileName;
                for (int i = 0; i < strfiles.Length; i++)
                {
                    fileName = Path.GetFileName(strfiles[i]);

                    string _CurrentFile = strfiles[i].ToString();
                    if (System.IO.File.Exists(_CurrentFile))
                    {
                        string tempFileURL = "/OrderFiles/" + order.OrderId.ToString() + "/" + Path.GetFileName(_CurrentFile).Replace(" ", "%20");
                        fileNamesUrls.Add(Path.GetFileName(_CurrentFile), tempFileURL);
                    }

                }
            }

            string UserName = HttpContext.User.Identity.Name;
            List<int> userRoles = _userRepository.GetUserRolesByUserName(UserName);
            List<int> allowedLocationPositions = new List<int>();
            foreach (LocState ls in order.LocStates)
            {
                if (userRoles.Contains(GetLocationIntValue(ls.Name)) && ls.Status != ((Statuses)3).ToString())
                {
                    allowedLocationPositions.Add(ls.LocationPosition - 1);
                }
            }
            OrderStateViewModel model = new OrderStateViewModel
            {
                OrderDetails = order,
                AllowedPositions = allowedLocationPositions
            };
            if (fileNamesUrls.Count() > 0)
            {
                model.FileNamesUrls = fileNamesUrls;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult FirstPickUp([Bind("OrderId, EntitiesPassed")] OrderStateViewModel model)
        {
            Order order = new Order();
            LocState firstLocState;
            string errorMessage = "";

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Oops, something went wrong, refresh the page and try again.";
                return View("Error");
            }

            if (_orderRepository.OrderExists(model.OrderId))
            {
                order = _orderRepository.GetOrderById(model.OrderId);
            }
            else
            {
                ViewBag.ErrorMessage = "Order doesn't exist.";
                return View("Error");
            }

            if (order.EntitiesNotProcessed < 1)
            {
                return RedirectToAction("Details", new { id = model.OrderId, message = "All entities related to this order have been processed." });
            }

            if (model.EntitiesPassed > order.EntitiesNotProcessed ||
                model.EntitiesPassed < 1)
            {
                return RedirectToAction("Details", new { id = model.OrderId, message = "Number of entities must be between 1 and " + order.EntitiesNotProcessed.ToString() });
            }



            order.EntitiesNotProcessed -= model.EntitiesPassed;
            firstLocState = _appDbContext.LocStates.FirstOrDefault(ls => ls.OrderId == order.OrderId && ls.LocationPosition == 1);
            firstLocState.EntitiesInProgress += model.EntitiesPassed;
            order.LocStates.Add(firstLocState);
            if (firstLocState.Status == ((Statuses)1).ToString())
            {
                firstLocState.Status = ((Statuses)2).ToString();
                firstLocState.Start = DateTime.Now;
            }
            if (order.Status != ((Statuses)2).ToString())
            {
                order.StartedAt = DateTime.Now;
                order.Status = ((Statuses)2).ToString();
            }

            _appDbContext.Update(order);
            _appDbContext.SaveChanges();

            _logRepository.CreateLog(
            HttpContext.User.Identity.Name,
            "Started processing " + model.EntitiesPassed.ToString() + " " + _orderRepository.GetOrderById(model.OrderId).EntityType + " in " + firstLocState.Name + ".",
            DateTime.Now,
            order.OrderNumber);

            return RedirectToAction("Details", new { id = model.OrderId, messageType = 1, message = model.EntitiesPassed.ToString() + " entities successfully passed to " + firstLocState.Name });
        }

        // GET: Order/Delete/5
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _orderRepository.GetOrderById(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(new OrderStateViewModel() { OrderDetails = order });
        }


        // POST: Order/Delete/5



        [Authorize]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _orderRepository.GetOrderById(id);
            _appDbContext.Orders.Remove(order);

            _appDbContext.SaveChanges();

            _logRepository.CreateLog(
           HttpContext.User.Identity.Name,
           "Deleted order.",
           DateTime.Now,
           order.OrderNumber);
            if (!Directory.Exists(_hostingEnvironment.WebRootPath + "/OrderFiles/" + id + "/"))
            {
                var dirPath = Path.Combine(
                         Directory.GetCurrentDirectory(),
                         "wwwroot" + "/OrderFiles/" + Convert.ToString(id) + "/");
                Directory.Delete(dirPath, true);
            }

            return RedirectToAction("Index");
        }


        private List<LocState> CreateOrderLocStates(List<int> locations, Order order)
        {
            List<LocState> orderNewLocStates = new List<LocState>();
            int count = 0;
            foreach (int l in locations)
            {
                orderNewLocStates.Add(new LocState
                {
                    Name = ((Locations)l).ToString(),
                    LocationPosition = count + 1,
                    Status = ((Statuses)1).ToString(),
                    TotalEntityCount = order.EntityCount
                });

                ++count;
            }
            return orderNewLocStates;
        }

        private List<int> locStateOrganiser(List<int> locStates)
        {
            List<int> organisedList = new List<int>();
            for (int i = 0; i < locStates.Count(); i++)
            {
                if (locStates[i] != 0)
                {
                    organisedList.Add(locStates[i]);
                }
            }
            return organisedList;
        }

        public IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }

        private void UpdateOrderValues(ref Order order, Order newOrderDetails)
        {
            order.EntityType = newOrderDetails.EntityType;
            order.Status = newOrderDetails.Status;
            order.OrderNumber = newOrderDetails.OrderNumber;
        }
    }
}
