using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Organiser.Data.UnitOfWork;

namespace Organiser.Controllers
{
    public class FileManagerController : Controller
    {
        public readonly IHostingEnvironment _hostingEnvironment;
        public IUnitOfWork _unitOfWork;

        public FileManagerController(
            IHostingEnvironment hostingEnvironment,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddFile(int? id)
        {
            using (_unitOfWork)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var order = _unitOfWork.OrderRepository.GetById((int)id);
                if (order == null)
                {
                    return NotFound();
                }
                var webRoot = _hostingEnvironment.WebRootPath;

                return View(order);

            }        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormCollection form)
        {
            if (form.Files == null || form.Files[0].Length == 0)
                return RedirectToAction("Details", "Order", new { id = Convert.ToString(form["OrderDetails.OrderId"]), messageType = 2, message = "Select a file first." });

            string orderId = Convert.ToString(form["OrderDetails.OrderId"]);
            var webRoot = _hostingEnvironment.WebRootPath;

            if (!Directory.Exists(webRoot + "/OrderFiles/"))
            {
                Directory.CreateDirectory(webRoot + "/OrderFiles/");
            }
            if (!Directory.Exists(webRoot + "/OrderFiles/" + orderId + "/"))
            {
                Directory.CreateDirectory(webRoot + "/OrderFiles/" + orderId + "/");
            }

           var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot" + "/OrderFiles/" + orderId + "/",
                        form.Files[0].FileName);

            if (GetContentType(path) == "")
            {
                return RedirectToAction("Details", "Order", new { id = Convert.ToString(orderId), messageType = 2, message = "Only files with these extensions are allowed: pdf/png/jpg/jpeg/gif." });

            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await form.Files[0].CopyToAsync(stream);
            }


            int orderIdInt = Int32.Parse(orderId);

            _unitOfWork.LogRepository.CreateLog(
           HttpContext.User.Identity.Name,
           "Uploaded file: " + form.Files[0].FileName,
           DateTime.Now,
          _unitOfWork.OrderRepository.GetOrderNumberByOrderId(orderIdInt));

            return RedirectToAction("Details", "Order", new { id = Convert.ToString(orderId), messageType = 1, message = "File Uploaded." });
        }

        [HttpGet]
        public IActionResult Delete(int orderId, string fileName)
        {
            return View(new Tuple<int, string>(orderId, fileName));
        }

        public async Task<IActionResult> DeleteConfirmed(int orderId, string fileName)
        {
            var dirPath = Path.Combine(
                          Directory.GetCurrentDirectory(),
                          "wwwroot" + "\\OrderFiles\\" + Convert.ToString(orderId) + "\\" + fileName);
            string sbb = "B:\\Freelance\\current\\Organiser\\Organiser\\wwwroot\\OrderFiles\\4003\\Спортна_програма.pdf";
            if (System.IO.File.Exists(dirPath))
            {
                System.IO.File.Delete(dirPath);
            }

            _unitOfWork.LogRepository.CreateLog(
           HttpContext.User.Identity.Name,
           "Deleted file: " + fileName,
           DateTime.Now,
           _unitOfWork.OrderRepository.GetOrderNumberByOrderId(orderId));

            return RedirectToAction("Details", "Order", new { id = orderId, messageType = 1, message = "File Deleted." });

        }

        public async Task<IActionResult> Download(int orderId, string fileName)
        {
            if (fileName == null)
                return Content("The file has been erased.");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot" + "/OrderFiles/" + orderId + "/", fileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            _unitOfWork.LogRepository.CreateLog(
           HttpContext.User.Identity.Name,
           "Downloaded file: " + fileName,
           DateTime.Now,
           _unitOfWork.OrderRepository.GetOrderNumberByOrderId(orderId));
            if (GetContentType(path) == "")
            {
                return Error("Only files with these extensions are allowed: pdf/png/jpg/jpeg/gif");
            }
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            string ss = "";
            if (types.ContainsKey(ext))
            { 
             ss = types[ext];
            }
            return ss;
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".pdf", "File/pdf"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"}
            };
        }

        public IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }
    }
}
