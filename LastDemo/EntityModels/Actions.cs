using Avalonia.Controls.ApplicationLifetimes;
using LastDemo.EntityModels;
using LastDemo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastDemo.EntityModels
{
    public static class Actions
    {
        public static List<int> Path = new List<int>();
        public static Product ProductToAdd;
        public static ShetininContext PublicContext = new ShetininContext();
        public static List<Product> Products = PublicContext.Products.Include(x => x.Attachedproducts).ToList();
        public static List<string> Manufacturers = new List<string>() { "Все элементы" }.Concat(PublicContext.Manufacturers.Select(m => m.Name)).ToList();
        public static string Amount = $"{Products.Count}/{PublicContext.Products.Count()}";

        public static void ChangeProductList(string search, int filtr, int sort)
        {
            Products.Clear();
            Products = PublicContext.Products.Include(x => x.Attachedproducts).ToList();
            string[] words = search.Split(' ');
            foreach (string word in words)
            {
                Products = Products.Where(w => w.Title.ToLower().Contains(word.ToLower()) || w.Description.ToLower().Contains(word.ToLower())).ToList();
            }
            if (filtr != 0)
            {
                Products = Products.Where(p => p.Manufacturerid == filtr).ToList();
            }
            switch (sort)
            {
                case 1:
                    Products = Products.OrderBy(p => p.Cost).ToList(); break;
                case 2:
                    Products = Products.OrderByDescending(p => p.Cost).ToList(); break;
            }
            Amount = $"{Products.Count()}/{PublicContext.Products.Count()}";
        }
    }
}
