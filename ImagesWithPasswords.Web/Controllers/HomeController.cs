using ImagesWithPasswords.Data;
using ImagesWithPasswords.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImagesWithPasswords.Web.Controllers
{
    public class HomeController : Controller
    {

        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=ImagesWithPasswords; Integrated Security=true;";
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
           
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {
            string fileName = $"{Guid.NewGuid()}-{image.FileName}";
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(imagePath, FileMode.CreateNew);
            image.CopyTo(fs);
            Images img = new Images();
            img.ImagePath = fileName;
            img.Password = password;
            ImagesRepository repo = new ImagesRepository(_connectionString);
            img.Id = repo.AddImage(img);
            UploadImageViewModel vm = new UploadImageViewModel()
            {
                Image = img
            };
            return View(vm);
        }
     
        
        public IActionResult ViewImage(int imageId)
        {
            ImagesRepository repo = new ImagesRepository(_connectionString);
            Images img = repo.GetImageById(imageId);
            ViewImageVm vm = new ViewImageVm()
            {
                Image = img
            };
            List<int> ids = HttpContext.Session.Get<List<int>>("Ids");
            if(ids == null)
            {
                ids = new List<int>();
            }
            if(ids.Contains(imageId))
            {
                repo.UpdateViews(imageId);
                vm.CorrectPassword = true;
            }

            return View(vm);
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            List<int> ids = HttpContext.Session.Get<List<int>>("Ids");
            if(ids == null)
            {
                ids = new List<int>();
            }
            bool correctPassword = false;
            ImagesRepository repo = new ImagesRepository(_connectionString);
            Images img = repo.GetImageById(id);
            if(password == img.Password)
            {
                correctPassword = true;
                ids.Add(id);
            }
            string message = (string)TempData["Message"];
           
            HttpContext.Session.Set("Ids", ids);
            if(ids.Contains(id))
            {
                correctPassword = true;
            }
            if (correctPassword == true)
            {
               repo.UpdateViews(id);
            }
            ViewImageVm vm = new ViewImageVm()
            {
                Image = img,
                CorrectPassword = correctPassword
            };
            if (password != img.Password)
            {
                TempData["Message"] = "Invalid Password";
            }
            vm.Message = message;

            return View(vm);
        }


    }
}
