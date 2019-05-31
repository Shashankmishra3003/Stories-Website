using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab10.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab10.Components
{ 
    public class MenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public MenuViewComponent (ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int id = 1;
           var model = _context.Stories.ToList();
       
           var story = _context.Stories.Include(l => l.Categories).Where(m => m.CategoriesID == id);
           var storyOrder = story.OrderBy(l => l.storyName)
                .OrderBy(l => l.Categories)
                .Select(l => l);
            if(id==0)
            {
                return await Task.FromResult((IViewComponentResult)View("Default", model));
            }
            return await Task.FromResult((IViewComponentResult)View("Default", storyOrder));
        }
    }
}
