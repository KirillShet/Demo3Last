using System;
using Avalonia.Media.Imaging;
using System.Collections.Generic;

namespace LastDemo.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public float Cost { get; set; }

    public string? Description { get; set; }

    public string? Mainimagepath { get; set; }

    public int Isactive { get; set; }

    public int? Manufacturerid { get; set; }
    public Bitmap Image
    {
        get
        {
            try
            {
                return new Bitmap($"{Environment.CurrentDirectory}/{Mainimagepath}");
            }
            catch (Exception)
            {
                return new Bitmap($"{Environment.CurrentDirectory}/Товары школы/no-img.png");
            }
        }
    }
    public bool Active
    {
        get
        {
            if (Isactive == 1)
                return true;
            else return false;
        }
    }

    public string Color
    {
        get
        {
            if (Active) return "White"; else return "Gainsboro";
        }
    }
    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<Productphoto> Productphotos { get; set; } = new List<Productphoto>();

    public virtual ICollection<Productsale> Productsales { get; set; } = new List<Productsale>();

    public virtual ICollection<Product> Attachedproducts { get; set; } = new List<Product>();

    public virtual ICollection<Product> Mainproducts { get; set; } = new List<Product>();
}
