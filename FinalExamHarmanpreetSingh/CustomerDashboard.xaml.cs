using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FinalExamHarmanpreetSingh
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class CustomerDashboard : Page
    {
        protected ExamData data;
        protected ObservableCollection<Order> OrderList { get; set; }
        public double Subtotal { get; set; }
        public double Discount { get => Subtotal * 0.10; }
        public double Tax { get => Subtotal * 0.13; }
        public double Total { get; set; }
        public CustomerDashboard()
        {
            this.InitializeComponent();
            OrderList = new ObservableCollection<Order>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.data = (ExamData)e.Parameter;
            productGrid.ItemsSource = data.ProductList;
            OrderList = new ObservableCollection<Order>();
            if(data.LoggedUser != null)
            {
                txtCustomer.Text = data.LoggedUser.FullName;
            }
            else
            {
                txtCustomer.Text = "Guest";
            }
           
        }

        private void productGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product selectedProduct = (Product)productGrid.SelectedItem;

            if (selectedProduct != null)
            {

                Order order = OrderList.Where(item => item.ProductId == selectedProduct.ProductId).FirstOrDefault();

                if(order != null)
                {
                    order.Quantity += 1;
                }
                else
                {
                    order = new Order();
                    order.ProductId = selectedProduct.ProductId;
                    order.CustomerName = data.LoggedUser != null ? data.LoggedUser.FullName : "Guest";
                    order.ProductName = selectedProduct.Name;
                    order.ProductPrice = selectedProduct.Price;
                    order.Quantity = 1;
                    OrderList.Add(order);
                }
                orderGrid.ItemsSource = null;
                orderGrid.ItemsSource = OrderList;

                
                productGrid.SelectedItem = null;
                selectedProduct = null;

                Subtotal = 0;
                foreach (var item in OrderList)
                {
                    Subtotal += (item.Quantity * item.ProductPrice);
                }

                if(data.LoggedUser != null)
                {
                    Total = (Subtotal + Tax)-Discount;
                    txtDiscount.Text = Discount.ToString("C");
                }
                else
                {
                    Total = (Subtotal + Tax);
                    txtDiscount.Text = 0.ToString("C");
                }

                txtSubtotal.Text = Subtotal.ToString("C");
                txtTax.Text = Tax.ToString("C");
                txtTotal.Text = Total.ToString("C");
            }
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string customerName = data.LoggedUser != null ? data.LoggedUser.FullName : "Guest";
                string fileName = "Receipt-" + customerName + ".txt";
                string destinationPath = Path.Combine("C:", fileName);

                if (!File.Exists(destinationPath))
                {
                    using (StreamWriter sw = File.CreateText(destinationPath))
                    {
                        sw.WriteLine("Hello, " + customerName);
                        sw.WriteLine("\nYour Order Details\n\n");
                        sw.WriteLine($"{"Product",-20} | {"Category",-20} | {"Quantity",10} | {"Price",7}");
                        foreach (var item in OrderList)
                        {
                            sw.WriteLine($"{item.ProductName,-20} | {item.ProductCategory,-20} | {item.Quantity,10} | {item.ProductPrice.ToString("C"),7}");
                        }
                        sw.WriteLine("\n");
                        sw.WriteLine($"{"Subtotal",-10} {":",-2} {Subtotal.ToString("C"),-10}");
                        sw.WriteLine($"{"Tax",-10} {":",-2} {Tax.ToString("C"),-10}");
                        if (data.LoggedUser != null)
                        {
                            sw.WriteLine($"{"Discount",-10} {":",-2} {Discount.ToString("C"),-10}");
                        }
                        else
                        {
                            sw.WriteLine($"{"Discount",-10} {":",-2} {0.ToString("C"),-10}");
                        }

                        sw.WriteLine($"{"Total",-10} {":",-2} {Subtotal.ToString("C"),-10}");
                    }
                    ShowMessageAsync("Reciept Created at path: " + destinationPath);
                }
                else
                {
                    ShowMessageAsync("Failed to create file");
                }

            }
            catch (Exception ex)
            {
                ShowMessageAsync("Error: " + ex.Message);
            }
            
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void ShowMessageAsync(string message)
        {
            MessageDialog messageDialog = new MessageDialog(message, "Information");

            await messageDialog.ShowAsync();
        }
    }
}
