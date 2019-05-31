using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab10.Data;
using Lab10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab10.Controllers
{

    public class StoryController : Controller
    {
        private readonly ApplicationDbContext context_;
        private const string session_id = "SessionId";

        public StoryController(ApplicationDbContext context)
        {
            context_ = context;
        }

        [Authorize(Roles = "Admin,User")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,User")]
        public IActionResult Stories(int id)
        {
            if (!(context_.Categories.Any(o => o.CategoriesID == id)))
            {
                return RedirectToAction("Error", "Home");
            }
            try
            {
                var story = context_.Stories.Include(l => l.Categories).Where(m => m.CategoriesID == id);
                var storyOrder = story.OrderBy(l => l.storyName)
                  .OrderBy(l => l.Categories)
                  .Select(l => l);
                return View(storyOrder);
            }
         
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpGet]
        public IActionResult AddStories(int id)
        {
            HttpContext.Session.SetInt32(session_id, id);
            Categories category = context_.Categories.Find(id);
            if (category == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var model = new Stories();
            return View(model);
        }

        [HttpPost]
        public IActionResult AddStories(int? id, Stories story)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            int? categoryID = HttpContext.Session.GetInt32(session_id);
            var category = context_.Categories.Find(categoryID);
            if (category != null)
            {
                if (category.Stories == null)
                {
                    List<Stories> stories = new List<Stories>();
                    category.Stories = stories;
                }

                category.Stories.Add(story);
            }
            try
            {
                context_.SaveChanges();
            }
            catch
            {

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult EditStory(int? id)
        {
            if (id == null)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Stories story = context_.Stories.Find(id);

            if (story == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(story);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditStory(int? id, Stories str)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var story = context_.Stories.Find(id);

            if (story != null)
            {
                story.storyName = str.storyName;
                story.storyDescription = str.storyDescription;

                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }
            return RedirectToAction("Stories","Story",new { id= story.CategoriesID});
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteStory(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            try
            {
                var story = context_.Stories.Find(id);
                if (story != null)
                {
                    context_.Remove(story);
                    context_.SaveChanges();

                    return RedirectToAction("Stories", "Story", new { id = story.CategoriesID });
                }
            }
            catch (Exception)
            {
                // nothing for now
            }
            return RedirectToAction("Index","Home");
        }


    }
}