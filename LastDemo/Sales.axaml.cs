using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LastDemo.Models;
using LastDemo.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace LastDemo;

public partial class Sales : Window
{
    int index = -1;
    List<Product> _products = Actions.PublicContext.Products.ToList();
    public Sales()
    {
        InitializeComponent();
    }
    public Sales(int i)
    {
        index = i;
        InitializeComponent();
        Prod.ItemsSource = _products;
        Prod.SelectedItem = Actions.PublicContext.Products.FirstOrDefault(p => p.Id == index);
    }



    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new EditWindow(index).Show();
        this.Close();
    }

    private void ComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        List<Productsale> productsales = Actions.PublicContext.Productsales.Where(ps => ps.Productid == ((sender as ComboBox).SelectedItem as Product).Id).OrderBy(ps => ps.Saledate).ToList();
        SaleList.ItemsSource = productsales;
    }
}