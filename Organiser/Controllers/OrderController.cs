using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Models;
using Organiser.ViewModels;
using static Organiser.Controllers.HelperMethods;

//just testing git
namespace Organiser.Controllers
{
    public class OrderController : Controller
    {
        public AppDbContext _appDbContext;
        public IOrderRepository _orderRepository;
        public IUserRepository _userRepository;
        public IDepartmentStateRepository _DepartmentStateRepository;
        public IHostingEnvironment _hostingEnvironment;
        public ILogRepository _logRepository;

        public OrderController(
            IHostingEnvironment hostingEnv,
            IOrderRepository orderList,
            AppDbContext appDbContext,
            IUserRepository userRepository,
            IDepartmentStateRepository DepartmentStateRepository,
            ILogRepository logRepository)
        {
            _appDbContext = appDbContext;
            _orderRepository = orderList;
            _userRepository = userRepository;
            _DepartmentStateRepository = DepartmentStateRepository;
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
                orders = _orderRepository.GetOrdersAndDepartmentStatesBySearchId(SearchID);

                return View(new OrderStateViewModel
                {
                    OrderListPaginated = await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize),
                });
            }

            orders = _orderRepository.OrdersAndDepartmentStates;

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
            if (!_userRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
            {
                return RedirectToAction("Logout", "Account");
            }

            TempData["Locations"] = LocationDefaults();
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Customer, OrderId, OrderNumber, EntityType, EntityCount, DeadLineDate")] Order order,
            int locname0, int locname1, int locname2, int locname3,
            int locname4, int locname5, int locname6)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!_userRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
                    {
                        return RedirectToAction("Logout", "Account");
                    }

                    if (_orderRepository.OrderExistsByOrderNumber(order.OrderNumber))
                    {
                        return Error("Error: order with order number " + order.OrderNumber + " already exists.");
                    }
                    List<int> DepartmentStatesUnorganised = new List<int> { locname0, locname1, locname2, locname3, locname4, locname5, locname6 };

                    order.CreatedAt = DateTime.Now;

                    List<int> DepartmentStatesOrganised = DepartmentStateOrganiser(DepartmentStatesUnorganised);
                    List<DepartmentState> DepartmentStateObjects = new List<DepartmentState>();

                    if (DepartmentStatesOrganised.Count() > 0)
                    {
                        DepartmentStateObjects = CreateOrderDepartmentStates(DepartmentStatesOrganised, order);
                    }
                    else
                    {
                        DepartmentStatesOrganised.Add(7); //drivers
                        DepartmentStateObjects = CreateOrderDepartmentStates(DepartmentStatesOrganised, order);
                    }

                    order.EntitiesNotProcessed = order.EntityCount;
                    order.DepartmentStates = DepartmentStateObjects;
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

        // GET: Order/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Error("Order doesn't exist.");
            }

            if (!_userRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
            {
                return RedirectToAction("Logout", "Account");
            }

            var order = await _appDbContext.Orders.SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return Error("Order doesn't exist.");
            }
            order.StatusDefaultsDropdown = StatusDefaults(GetStatusIntValue(order.Status));
            return View(order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Customer, OrderId, OrderNumber, EntityType, Status")] Order order)
        {

            if (!_userRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
            {
                return RedirectToAction("Logout", "Account");
            }


            if (_orderRepository.GetOrderByOrderNumber(order.OrderNumber) != null && _orderRepository.GetOrderByOrderNumber(order.OrderNumber).OrderId != order.OrderId)
            {
                ViewBag.errorMessage = "Error: order with order number " + order.OrderNumber + " already exists.";
                order.StatusDefaultsDropdown = StatusDefaults(GetStatusIntValue(order.Status));
                return View(order);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Order oldOrderState = _orderRepository.GetOrderAndDepartmentStatesById(order.OrderId);

                    if (order.Status != oldOrderState.Status)
                    {
                        List<DepartmentState> newDepartmentStates = oldOrderState.DepartmentStates;
                        if (order.Status == ((Statuses)3).ToString())
                        {
                            if (order.StartedAt == DateTime.MinValue)
                            {
                                order.StartedAt = DateTime.Now;
                            }
                            order.FinshedAt = DateTime.Now;
                            foreach (DepartmentState ls in newDepartmentStates)
                            {
                                ls.Status = order.Status;
                                ls.EntitiesInProgress = 0;
                                ls.EntitiesRFC = 0;
                            }
                        }
                        oldOrderState.EntitiesNotProcessed = 0;
                        oldOrderState.DepartmentStates = newDepartmentStates;
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
            order.StatusDefaultsDropdown = StatusDefaults(GetStatusIntValue(order.Status));
            return View(order);
        }

        [HttpPost]
        [Authorize]
        public PassEntitiesResultModel PassEntities(EntityOrganiserViewModel model)
        {
            DepartmentState sourceDepartmentState = _DepartmentStateRepository.GetDepartmentStateById(model.DepartmentStateId);
            DepartmentState targetDepartmentState = _DepartmentStateRepository.GetDepartmentStateByOrderIdAndLocNum(sourceDepartmentState.OrderId, sourceDepartmentState.LocationPosition + 1);

            if (!_userRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue(targetDepartmentState.Name)))
            {
                Error("Something went wrong, logout and log back in to fix the issue.");
            }

            if (sourceDepartmentState.EntitiesRFC >= model.EntitiesPassed)
            {
                sourceDepartmentState.EntitiesRFC -= model.EntitiesPassed;
            }
            else
            {
                Error("Entity count insufficient!");
            }

            if (targetDepartmentState.Status == ((Statuses)1).ToString())
            {
                targetDepartmentState.Status = ((Statuses)2).ToString();
                targetDepartmentState.Start = DateTime.Now;
            }

            targetDepartmentState.EntitiesInProgress += model.EntitiesPassed;

            _appDbContext.Update(sourceDepartmentState);
            _appDbContext.Update(targetDepartmentState);

            _appDbContext.SaveChanges();
            _logRepository.CreateLog(
                        HttpContext.User.Identity.Name,
                        "Moved " + model.EntitiesPassed.ToString() + " " + _orderRepository.GetOrderById(sourceDepartmentState.OrderId).EntityType + " from " + sourceDepartmentState.Name + " to " + targetDepartmentState.Name + ".",
                        DateTime.Now,
                        _orderRepository.GetOrderNumberByOrderId(sourceDepartmentState.OrderId));

            PassEntitiesResultModel resultModel = new PassEntitiesResultModel()
            {
                SourceEntitiesRFC = sourceDepartmentState.EntitiesRFC,
                TargetEntitiesInProgress = targetDepartmentState.EntitiesInProgress,
                TargetDepartmentStateId = targetDepartmentState.DepartmentStateId,
                TargetDepartmentStateStatus = targetDepartmentState.Status,
                StartTime = targetDepartmentState.Start.ToString("dd/MM/yy (HH:mm)")
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
            var order = _orderRepository.GetOrderAndDepartmentStatesById(id);
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
            foreach (DepartmentState ls in order.DepartmentStates)
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
            DepartmentState firstDepartmentState;
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

            firstDepartmentState = _appDbContext.DepartmentStates.FirstOrDefault(ls => ls.OrderId == order.OrderId && ls.LocationPosition == 1);

            if (!_userRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue(firstDepartmentState.Name)))
            {
                return RedirectToAction("Logout", "Account");
            }

            order.EntitiesNotProcessed -= model.EntitiesPassed;
            firstDepartmentState.EntitiesInProgress += model.EntitiesPassed;
            order.DepartmentStates.Add(firstDepartmentState);
            if (firstDepartmentState.Status == ((Statuses)1).ToString())
            {
                firstDepartmentState.Status = ((Statuses)2).ToString();
                firstDepartmentState.Start = DateTime.Now;
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
            "Started processing " + model.EntitiesPassed.ToString() + " " + _orderRepository.GetOrderById(model.OrderId).EntityType + " in " + firstDepartmentState.Name + ".",
            DateTime.Now,
            order.OrderNumber);

            return RedirectToAction("Details", new { id = model.OrderId, messageType = 1, message = model.EntitiesPassed.ToString() + " entities successfully passed to " + firstDepartmentState.Name });
        }

        // GET: Order/Delete/5
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!_userRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
            {
                return RedirectToAction("Logout", "Account");
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

            if (!_userRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
            {
                return RedirectToAction("Logout", "Account");
            }
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


        private List<DepartmentState> CreateOrderDepartmentStates(List<int> locations, Order order)
        {
            List<DepartmentState> orderNewDepartmentStates = new List<DepartmentState>();
            int count = 0;
            foreach (int l in locations)
            {
                orderNewDepartmentStates.Add(new DepartmentState
                {
                    Name = ((Locations)l).ToString(),
                    LocationPosition = count + 1,
                    Status = ((Statuses)1).ToString(),
                    TotalEntityCount = order.EntityCount
                });

                ++count;
            }
            return orderNewDepartmentStates;
        }

        private List<int> DepartmentStateOrganiser(List<int> DepartmentStates)
        {
            List<int> organisedList = new List<int>();
            for (int i = 0; i < DepartmentStates.Count(); i++)
            {
                if (DepartmentStates[i] != 0)
                {
                    organisedList.Add(DepartmentStates[i]);
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
