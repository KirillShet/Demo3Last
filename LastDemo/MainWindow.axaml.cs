using Avalonia.Controls;
using System.Linq;
using LastDemo.Models;
using LastDemo.EntityModels;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.Linq;

namespace LastDemo
{
    public partial class MainWindow : Window
    {
        int filtr = 0;
        int sort = 0;
        string search = "";
        public MainWindow()
        {
            InitializeComponent();
            Filtr.ItemsSource = Actions.Manufacturers;
            Filtr.SelectedIndex = 0;
            Sort.SelectedIndex = 0;
        }
        public void Update()
        {
            ProductList.ItemsSource = Actions.Products.ToList();
            Amount.Text = Actions.Amount.ToString();
        }
        private void SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Name == "Filtr")
            {
                filtr = Filtr.SelectedIndex;
            }
            else
            {
                sort = Sort.SelectedIndex;
            }
            Actions.ChangeProductList(search, filtr, sort);
            Update();
        }

        private void Add(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            new EditWindow().Show();
            this.Close();

        }

        private void Change(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            new EditWindow(Int32.Parse((sender as Border).Tag.ToString())).Show();
            this.Close();
        }

        private void Delete(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                Actions.PublicContext.Products.Remove(ProductList.SelectedItem as Product);
                Actions.PublicContext.SaveChanges();
            }
            catch { }
            Filtr.SelectedIndex = 0;
            Sort.SelectedIndex = 0;
            Search.Text = "";
        }

        private void ListBox_SelectionChanged_1(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            if (ProductList.SelectedItem != null)
            {
                DeleteButton.IsVisible = true;
            }
            else
            {
                DeleteButton.IsVisible = false;
            }
        }

        private void TextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            search = Search.Text;
            Actions.ChangeProductList(search, filtr, sort);
            Update();
        }
    }
}