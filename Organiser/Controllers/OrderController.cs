using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Organiser.Actions;
using Organiser.Data.EnumType;
using Organiser.Data.Models;
using Organiser.Data.UnitOfWork;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Organiser.Controllers.HelperMethods;

namespace Organiser.Controllers
{
    public class OrderController : Controller
    {
        public IHostingEnvironment _hostingEnvironment;
        public IUnitOfWork _unitOfWork;
        public IOrderActions _orderActions;
        public OrderController(IHostingEnvironment hostingEnv,
            IUnitOfWork unitOfWork,
            IOrderActions orderActions)
        {
            _hostingEnvironment = hostingEnv;
            _unitOfWork = unitOfWork;
            _orderActions = orderActions;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? page, string SearchID = "")
        {
            PaginatedList<Order> _orderList = await _orderActions.Index(page, SearchID);
            return View(_orderList);
        }

        public async Task<IActionResult> AvailableWork(int locationNameNum, int? page)
        {
            OrderStateViewModel _model = await _orderActions.AvailableWork(locationNameNum, page);
            return View(_model);
        }



        // GET: Order/Create
        [Authorize]
        public IActionResult Create()
        {

            if (!User.IsInRole("Orders"))
            {
                return Error("You don't have the necessary permissions to do this.");
            }
            List<DepartmentState> _dStates = new List<DepartmentState>();
            List<SelectListItem> _dStatesDefaults = LocationDefaults();
            foreach (var ds in _dStatesDefaults)
            {
                _dStates.Add(new DepartmentState() { NameNum = 0 });
            }

            var _order = new Order() { DepartmentStates = _dStates };
            TempData["Locations"] = _dStatesDefaults;
            return View(_order);
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)//[Bind("Customer, OrderId, OrderNumber, EntityType, EntityCount, DeadLineDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                using (_unitOfWork)
                {
                    if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name,
                        GetLocationIntValue("Orders")))

                    {
                        return RedirectToAction("Logout", "Account");
                    }

                    if (_unitOfWork.OrderRepository.Find(e => e.OrderNumber == order.OrderNumber).Any())
                    {
                        ViewBag.ErrorMessage = "Error: order with order number " + order.OrderNumber + " already exists.";
                        TempData["Locations"] = LocationDefaults();
                        return View(order);
                    }


                    order.CreatedAt = DateTime.Now;
                    order.DepartmentStates = order.DepartmentStates.Where(x => x.NameNum != 0).ToList();
                    List<DepartmentState> _departmentStates = new List<DepartmentState>();

                    if (order.DepartmentStates.Any())
                    {
                        _departmentStates = CreateOrderDepartmentStates(order);
                    }
                    else
                    {
                        order.DepartmentStates.Add(new DepartmentState() { NameNum = 7 }); //drivers
                        _departmentStates = CreateOrderDepartmentStates(order);
                    }

                    order.EntitiesNotProcessed = order.EntityCount;
                    order.DepartmentStates = _departmentStates;
                    order.Status = ((Enums.Statuses)1).ToString();
                    _unitOfWork.OrderRepository.Add(order);
                    await _unitOfWork.CompleteAsync();

                    _unitOfWork.LogRepository.CreateLog(
                        HttpContext.User.Identity.Name,
                        "Order created.",
                        DateTime.Now,
                        order.OrderNumber);
                    return RedirectToAction("Index");
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

            if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
            {
                return RedirectToAction("Logout", "Account");
            }

            var order = await _unitOfWork.OrderRepository.Find(x => x.OrderId == id).SingleOrDefaultAsync();
            if (order == null)
            {
                return Error("Order doesn't exist.");
            }
            //Tino:ToDo
            order.StatusDefaultsDropdown = StatusDefaults(GetStatusIntValue(order.Status));
            return View(order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Customer, OrderId, OrderNumber, EntityType, Status")] Order order)
        {
            using (_unitOfWork)
            {

                if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
                {
                    return RedirectToAction("Logout", "Account");
                }

                if (_unitOfWork.OrderRepository.Find(x => x.OrderNumber == order.OrderNumber).FirstOrDefault() is null
                && _unitOfWork.OrderRepository.Find(x => x.OrderNumber == order.OrderNumber).FirstOrDefault().OrderId != order.OrderId)
                {
                    ViewBag.errorMessage =
                        "Error: order with order number " + order.OrderNumber + " already exists.";
                    //Tino:ToDo
                    order.StatusDefaultsDropdown = StatusDefaults(GetStatusIntValue(order.Status));
                    return View(order);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        Order oldOrderState = _unitOfWork.OrderRepository.GetAllToIQuerable().Include(x => x.DepartmentStates)
                            .FirstOrDefault(o => o.OrderId == order.OrderId);

                        if (order.Status != oldOrderState.Status)
                        {
                            List<DepartmentState> newDepartmentStates = oldOrderState.DepartmentStates;
                            if (order.Status == ((Enums.Statuses)3).ToString())
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

                        _unitOfWork.Update(oldOrderState);
                        await _unitOfWork.CompleteAsync();
                        _unitOfWork.LogRepository.CreateLog(
                            HttpContext.User.Identity.Name,
                            "Edited order.",
                            DateTime.Now,
                            order.OrderNumber);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        using (_unitOfWork)
                        {
                            if (!_unitOfWork.OrderRepository.Find(o => o.OrderId == order.OrderId).Any())
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }

                    return RedirectToAction("Details", new { id = order.OrderId });
                }

                //Tino:ToDo
                order.StatusDefaultsDropdown = StatusDefaults(GetStatusIntValue(order.Status));
                return View(order);
            }
        }

        [HttpPost]
        [Authorize]
        public PassEntitiesResultModel PassEntities(EntityOrganiserViewModel model)
        {
            DepartmentState sourceDepartmentState = _unitOfWork.DepartmentStateRepository.GetDepartmentStateById(model.DepartmentStateId);
            DepartmentState targetDepartmentState = _unitOfWork.DepartmentStateRepository.GetDepartmentStateByOrderIdAndLocNum(sourceDepartmentState.OrderId, sourceDepartmentState.LocationPosition + 1);

            if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue(targetDepartmentState.Name)))
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

            if (targetDepartmentState.Status == ((Enums.Statuses)1).ToString())
            {
                targetDepartmentState.Status = ((Enums.Statuses)2).ToString();
                targetDepartmentState.Start = DateTime.Now;
            }

            targetDepartmentState.EntitiesInProgress += model.EntitiesPassed;

            _unitOfWork.Update(sourceDepartmentState);
            _unitOfWork.Update(targetDepartmentState);

            _unitOfWork.Complete();
            _unitOfWork.LogRepository.CreateLog(
                        HttpContext.User.Identity.Name,
                        "Moved " + model.EntitiesPassed.ToString() + " " + _unitOfWork.OrderRepository.Find(x => x.OrderId == sourceDepartmentState.OrderId).FirstOrDefault().EntityType + " from " + sourceDepartmentState.Name + " to " + targetDepartmentState.Name + ".",
                        DateTime.Now,
                _unitOfWork.OrderRepository.Find(o => o.OrderId == sourceDepartmentState.OrderId)
                    .Select(o => o.OrderNumber).FirstOrDefault());




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
            using (_unitOfWork)
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
                Order order = _unitOfWork.OrderRepository.GetOrderAndDepartmentStatesById(id);
                //string userRole = ((UserRoles)_unitOfWork.UserRepository.GetUserByName(HttpContext.User.Identity.Name).Role).ToString();

                if (order == null)
                {
                    ViewBag.ErrorMessage = "Order doesn't exist.";
                    return View("Error");
                }
                string webRoot = _hostingEnvironment.WebRootPath;
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
                List<int> userRoles = _unitOfWork.UserRepository.GetUserRolesByUserName(UserName);
                List<int> allowedLocationPositions = new List<int>();
                foreach (DepartmentState ls in order.DepartmentStates)
                {
                    if (userRoles.Contains(GetLocationIntValue(ls.Name)) && ls.Status != ((Enums.Statuses)3).ToString())
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
        }

        [HttpPost]
        [Authorize]
        public IActionResult FirstPickUp([Bind("OrderId, EntitiesPassed")] OrderStateViewModel model)
        {
            using (_unitOfWork)
            {
                Order order = new Order();
                DepartmentState firstDepartmentState;

                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Oops, something went wrong, refresh the page and try again.";
                    return View("Error");
                }
                if (_unitOfWork.OrderRepository.Find(o => o.OrderId == model.OrderId).Any())
                {
                    order = _unitOfWork.OrderRepository.GetById(model.OrderId);
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

                firstDepartmentState = _unitOfWork.DepartmentStateRepository.Find(ls => ls.OrderId == order.OrderId && ls.LocationPosition == 1).FirstOrDefault();

                if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue(firstDepartmentState.Name)))
                {
                    return RedirectToAction("Logout", "Account");
                }

                order.EntitiesNotProcessed -= model.EntitiesPassed;
                firstDepartmentState.EntitiesInProgress += model.EntitiesPassed;
                order.DepartmentStates.Add(firstDepartmentState);
                if (firstDepartmentState.Status == ((Enums.Statuses)1).ToString())
                {
                    firstDepartmentState.Status = ((Enums.Statuses)2).ToString();
                    firstDepartmentState.Start = DateTime.Now;
                }
                if (order.Status != ((Enums.Statuses)2).ToString())
                {
                    order.StartedAt = DateTime.Now;
                    order.Status = ((Enums.Statuses)2).ToString();
                }

                _unitOfWork.Update(order);
                _unitOfWork.Complete();

                _unitOfWork.LogRepository.CreateLog(
                HttpContext.User.Identity.Name,
                "Started processing " + model.EntitiesPassed.ToString() + " " + _unitOfWork.OrderRepository.GetById(model.OrderId).EntityType + " in " + firstDepartmentState.Name + ".",
                DateTime.Now,
                order.OrderNumber);

                return RedirectToAction("Details", new { id = model.OrderId, messageType = 1, message = model.EntitiesPassed.ToString() + " entities successfully passed to " + firstDepartmentState.Name });

            }
        }

        // GET: Order/Delete/5
        [Authorize]
        public IActionResult Delete(int? id)
        {
            using (_unitOfWork)
            {
                if (id == null)
                {
                    return NotFound();
                }

                if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
                {
                    return RedirectToAction("Logout", "Account");
                }

                Order order = _unitOfWork.OrderRepository.GetById((int)id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(new OrderStateViewModel() { OrderDetails = order });
            }
        }


        // POST: Order/Delete/5



        [Authorize]
        public IActionResult DeleteConfirmed(int id)
        {

            using (_unitOfWork)
            {
                if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue("Orders")))
                {
                    return RedirectToAction("Logout", "Account");
                }
                Order order = _unitOfWork.OrderRepository.GetById(id);
                _unitOfWork.OrderRepository.Remove(order);

                _unitOfWork.Complete();

                _unitOfWork.LogRepository.CreateLog(
               HttpContext.User.Identity.Name,
               "Deleted order.",
               DateTime.Now,
               order.OrderNumber);
                if (!Directory.Exists(_hostingEnvironment.WebRootPath + "/OrderFiles/" + id + "/"))
                {
                    string dirPath = Path.Combine(
                             Directory.GetCurrentDirectory(),
                             "wwwroot" + "/OrderFiles/" + Convert.ToString(id) + "/");
                    Directory.Delete(dirPath, true);
                }

                return RedirectToAction("Index");

            }
        }


        private List<DepartmentState> CreateOrderDepartmentStates(Order order)
        {
            List<DepartmentState> orderNewDepartmentStates = new List<DepartmentState>();
            int count = 0;
            foreach (var ds in order.DepartmentStates)
            {
                ++count;
                orderNewDepartmentStates.Add(new DepartmentState
                {
                    Name = ((Enums.Department)ds.NameNum).ToString(),
                    LocationPosition = count,
                    Status = ((Enums.Statuses)1).ToString(),
                    TotalEntityCount = order.EntityCount
                });
            }
            return orderNewDepartmentStates;
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
