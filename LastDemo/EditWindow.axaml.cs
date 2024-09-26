using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using LastDemo.Models;
using LastDemo.EntityModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LastDemo;

public partial class EditWindow : Window
{
    int index = -1;
    List<string> manf = Actions.PublicContext.Manufacturers.Select(m => m.Name).ToList();
    string _imageName = "";
    string _imagePath = "";
    public List<Product> ExtraProducts = new List<Product>();
    string cost = "";
    public EditWindow()
    {
        InitializeComponent();
        SalesButton.IsVisible = false;
        Actions.Path.Add(-1);
        Man.ItemsSource = manf;
        Available.ItemsSource = Actions.PublicContext.Products.Where(p => p.Isactive == 1);
    }
    public EditWindow(int id)
    {
        Product product;
        index = id;
        InitializeComponent();
        Man.ItemsSource = manf;
        if (Actions.Path.Count() == 0 || Actions.Path.Last() != id)
        {
            Actions.Path.Add(id);
        }
        if (Actions.Path.Last() == -1)
        {
            product = Actions.ProductToAdd;
            SalesButton.IsVisible = false;
        }
        else
        {
            product = Actions.PublicContext.Products.FirstOrDefault(p => p.Id == index);
        }
        IsActive.IsChecked =  product.Isactive == 1;
        Id.Text = id.ToString();
        Name.Text = product.Title;
        Cost.Text = product.Cost.ToString();
        Desc.Text = product.Description;
        Image.Source = product.Image;
        if (Actions.PublicContext.Manufacturers.FirstOrDefault(m => m.Id == product.Manufacturerid) != null)
        {
            Man.SelectedIndex = manf.IndexOf(Actions.PublicContext.Manufacturers.FirstOrDefault(m => m.Id == product.Manufacturerid).Name);
        }
        if (index != -1)
        {
            ExtraProducts = Actions.PublicContext.Products.Include(x => x.Attachedproducts).FirstOrDefault(p => p.Id == id).Attachedproducts.ToList();
        }
        else
        {
            ExtraProducts = Actions.ProductToAdd.Attachedproducts.ToList();
        }
        Extra.ItemsSource = ExtraProducts;
        Available.ItemsSource = Actions.PublicContext.Products.Where(p => p.Isactive == 1 && p.Id != index);
    }

    private void Comfirm(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Name.Text) && !string.IsNullOrEmpty(Cost.Text) && Man.SelectedIndex != null)
        {
            if (string.IsNullOrEmpty(Desc.Text))
            {
                Desc.Text = "";
            }
            if (index == -1)
            {
                Product product = new Product() { Title = Name.Text, Cost = float.Parse(Cost.Text), Description = Desc.Text, 
                    Manufacturerid = Actions.PublicContext.Manufacturers.FirstOrDefault(m => m.Name == manf[Man.SelectedIndex]).Id, 
                    Isactive = (Convert.ToInt32(IsActive.IsChecked) + 1) * -1 + 3, Attachedproducts = ExtraProducts
                };
            
                if (_imagePath != "")
                {
                    File.Copy(_imagePath, Environment.CurrentDirectory + "/" + _imageName);
                    product.Mainimagepath = _imageName;
                }
                Actions.PublicContext.Products.Add(product);
                Actions.PublicContext.SaveChanges();
            }
            else
            {
                Product product = Actions.PublicContext.Products.FirstOrDefault(p => p.Id == index);
                product.Title  = Name.Text;
                product.Cost = float.Parse(Cost.Text);
                product.Description  = Desc.Text;
                product.Manufacturerid = Actions.PublicContext.Manufacturers.FirstOrDefault(m => m.Name == manf[Man.SelectedIndex]).Id;
                product.Isactive = (Convert.ToInt32(IsActive.IsChecked) + 1) * -1 + 3;
                if (_imagePath != "")
                {
                    File.Copy(_imagePath, Environment.CurrentDirectory + "/" + _imageName); 
                    product.Mainimagepath = _imageName;
                }
                Actions.PublicContext.Products.Update(product);
                Actions.PublicContext.SaveChanges();
            }
            if (Actions.Path.Count() == 1)
            {
                Actions.Path.Clear();
                new MainWindow().Show();
                this.Close();
            }
            else
            {
                Actions.Path.RemoveAt(Actions.Path.Count()-1);
                new EditWindow(Actions.Path.Last()).Show();
                Close();
            }

        }
    }

    private async void AddImage(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title="Выберите изображение"});
        if (files.Count() != 0)
        {
            try
            {
                _imagePath = files[0].Path.LocalPath;
                Image.Source = new Bitmap(_imagePath);
                _imageName = $"Товары школы/{Guid.NewGuid()}{_imagePath.Substring(_imagePath.LastIndexOf('.'), _imagePath.Length - _imagePath.LastIndexOf('.'))}";
            }
            catch { }
        }
    }

    private void CostTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if ((float.TryParse((sender as TextBox).Text, out float result) && result > 0) || string.IsNullOrEmpty((sender as TextBox).Text))
        {
            cost = (sender as TextBox).Text;
        }
        else
        {
            (sender as TextBox).Text = cost;
        }
    }

    private void ExtraChoosed(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if ((sender as ComboBox).SelectedItem != null)
        {
            Product product = (sender as ComboBox).SelectedItem as Product;
            ExtraImage.Source = product.Image;
        }
    }

    private void AddExtra(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Available.SelectedItem != null)
        {
            ExtraProducts.Add(Available.SelectedItem as Product);
            Extra.ItemsSource = ExtraProducts.ToList();
            ExtraImage.Source = null;
            if (index != -1)
            {
                Product pr = Actions.PublicContext.Products.FirstOrDefault(p => p.Id == index);
                pr.Attachedproducts.Add((Available.SelectedItem as Product));
                Actions.PublicContext.Update(pr);
                Actions.PublicContext.SaveChanges();
            }
            Available.SelectedIndex = -1;
        }
    }

    private void ExtraTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if ((sender as Border).Tag != null)
        {
            if (index == -1)
            {
                Product product = new Product();
                product.Title = Name.Text;
                try
                {
                    product.Cost = float.Parse(Cost.Text);
                }
                catch
                {}
                product.Description = Desc.Text;
                try
                {
                    product.Manufacturerid = Actions.PublicContext.Manufacturers.FirstOrDefault(m => m.Name == manf[Man.SelectedIndex]).Id;
                }
                catch
                { }
                product.Attachedproducts = ExtraProducts;
                product.Isactive = (Convert.ToInt32(IsActive.IsChecked) + 1) * -1 + 3;
                if (_imagePath != "")
                {
                    File.Copy(_imagePath, Environment.CurrentDirectory + "/" + _imageName);
                    product.Mainimagepath = _imageName;
                }
                Actions.ProductToAdd = product;
            }
            new EditWindow(Int32.Parse((sender as Border).Tag.ToString())).Show();
            this.Close();
        }
    }

    private void Exit(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Actions.Path.Clear();
        new MainWindow().Show();
        this.Close();
    }

    private void ListBox_SelectionChanged_1(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if (Extra.SelectedItem != null)
        {
            DeleteExtra.IsVisible = true;
        }
        else
        {
            DeleteExtra.IsVisible = false;
        }
    }

    private void Delete(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (index == -1)
        {
            ExtraProducts.Remove(Extra.SelectedItem as Product);
            Extra.ItemsSource = ExtraProducts.ToList();
        }
        else
        {
            Product pr = Actions.PublicContext.Products.FirstOrDefault(p => p.Id == index);
            pr.Attachedproducts.Remove((Extra.SelectedItem as Product));
            Actions.PublicContext.Update(pr);
            Actions.PublicContext.SaveChanges();
            ExtraProducts = Actions.PublicContext.Products.Include(x => x.Attachedproducts).FirstOrDefault(p => p.Id == index).Attachedproducts.ToList();
            Extra.ItemsSource = ExtraProducts.ToList();
        }
    }

    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new Sales(index).Show();
        this.Close();
    }
}