using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab10.Models;

namespace Lab10.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            if(context.Categories.Any())
                {
                return;
            }

            var category = new Categories[]
            {
                new Categories{categoryName="Projects"},
                new Categories{categoryName="Experiences"}
            };

            foreach(Categories c in category)
            {
                context.Categories.Add(c);
            }
            context.SaveChanges();

            var story = new Stories[]
            {
                new Stories{storyName="Multi User Test Harness",storyDescription="C++ Based Testing tool designed using WPF and C++ CLI",
                            CategoriesID=1},
                new Stories{storyName="Journey from India to USA",storyDescription="Story about the steps and experiences of obtaining a Masters Degree",
                            CategoriesID=2}
            };
            foreach(Stories s in story)
            {
                context.Stories.Add(s);
            }
            context.SaveChanges();

            var storyBlock = new StoryBlock[]
            {
                new StoryBlock{ FileName="new.txt",ImageName="desert.jpg",StoriesID=1}
            };
            foreach (StoryBlock s in storyBlock)
            {
                context.StoryBlocks.Add(s);
            }
            context.SaveChanges();
        }
    }
}
