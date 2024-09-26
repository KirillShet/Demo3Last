using System;
using System.Collections.Generic;
using System.Linq;
using LastDemo.Models;
using LastDemo.EntityModels;
using System.Text;
using System.Threading.Tasks;

namespace LastDemo
{
    public static class ProductAction
    {
        public static ShetininContext DBContext { get; set; } = new ShetininContext();
        public static List<Product> ListProducts { get; set;} = DBContext.Products.ToList();
        public static List<Manufacturer> ListManufacturers { get; set; } = DBContext.Manufacturers.ToList();
    }
}
