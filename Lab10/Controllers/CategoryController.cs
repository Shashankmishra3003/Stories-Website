using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lab10.Data;
using Lab10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab10.Controllers
{   [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly ApplicationDbContext context_;
        private const string session_id = "SessionId";

        public CategoryController(ApplicationDbContext context,IHostingEnvironment appEnvironment)
        {
            context_ = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateCategory(int id)
        {
            var model = new Categories();
            return View(model);
        }

        //----< posts back new courses details >---------------------

        [HttpPost]
        public async Task<IActionResult> CreateCategory(int id, Categories crs,IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("Image not Selected");
            string path_root = _appEnvironment.WebRootPath;
            string path_to_Images = path_root + "\\FileStorage\\Images\\" + file.FileName;
            //Moving file to target
            using (var stream = new FileStream(path_to_Images, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            crs.ImageName = file.FileName;
            context_.Categories.Add(crs);
            context_.SaveChanges();
            return RedirectToAction("readStory", "Category",crs.CategoriesID);
        }



        public IActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            try
            {
                var category = context_.Categories.Find(id);
                if (category != null)
                {
                    var bookList = context_.Stories.Where(s => s.CategoriesID == id);
                    if(bookList.Any())
                    {
                        ViewBag.MyErrorMessage = "The Category Contains Story, Delet the stories then delete category";
                        return View();
                    }
                    if(!bookList.Any())
                    {
                        context_.Remove(category);
                        context_.SaveChanges();
                    }
                    
                }
            }
            catch (Exception)
            {
                // nothing for now
            }
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Categories category = context_.Categories.Find(id);
            if (category == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategory(int? id, Categories catg)
        {

            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var category = context_.Categories.Find(id);
            if (category != null)
            {
                category.categoryName = catg.categoryName;
                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }
            return RedirectToAction("Index","Home");
        }
        [AllowAnonymous]
        public IActionResult readStory()
        {
            try
            {
                return View(context_.Categories.ToList<Categories>());
            }

            catch
            {
                return RedirectToAction("Error", "Home");
            }
            
        }
    }
}