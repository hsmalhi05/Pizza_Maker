using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public partial class ProductDashboard : Page
    {
        protected ExamData data;
        protected Product SelectedProduct = null;
        public ProductDashboard()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.data = (ExamData)e.Parameter;
            //   productCollection.Source = this.data.ProductList;
            productGrid.ItemsSource = data.ProductList;
        
        }

        private void productGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product selectedProduct = (Product)productGrid.SelectedItem;

            if(selectedProduct != null)
            {
                this.SelectedProduct = selectedProduct;

                txtProductName.Text = selectedProduct.Name;
                txtProductCategory.Text = selectedProduct.Category;
                txtProductPrice.Text = selectedProduct.Price + "";
            }
            
        }

        private async void ShowMessageAsync(string message)
        {
            MessageDialog messageDialog = new MessageDialog(message, "Information");

             await messageDialog.ShowAsync();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = txtProductName.Text;
                string category = txtProductCategory.Text;
                string price = txtProductPrice.Text;
                this.SelectedProduct = null;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(price))
                {
                    ShowMessageAsync("All fields are required!");
                }
                else
                {
                    Product product = new Product();
                    product.Name = name;
                    product.Category = category;
                    product.Price = Convert.ToDouble(price);

                    bool isInserted = data.InsertProduct(product);
                    if (isInserted)
                    {
                        ShowMessageAsync("Product Added successfully!");
                        data.LoadProducts();
                        productGrid.ItemsSource = null;
                        productGrid.ItemsSource = data.ProductList;
                    }
                    else
                    {
                        ShowMessageAsync("Failed to add new product...");
                    }
                }

            }
            catch (Exception ex)
            {
                ShowMessageAsync(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = txtProductName.Text;
                string category = txtProductCategory.Text;
                string price = txtProductPrice.Text;

                if(this.SelectedProduct != null)
                {
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) || string.IsNullOrEmpty(price))
                    {
                        ShowMessageAsync("All fields are required!");
                    }
                    else
                    {
                        Product product = this.SelectedProduct;
                        product.Name = name;
                        product.Category = category;
                        product.Price =  Convert.ToDouble(price);

                        bool isUpdated = data.UpdateProduct(product);
                        if (isUpdated)
                        {
                            ShowMessageAsync("Product Updated successfully!");

                            data.LoadProducts();
                            productGrid.ItemsSource = data.ProductList;
                        }
                        else
                        {
                            ShowMessageAsync("Failed to Update product...");
                        }
                        this.SelectedProduct = null;
                    }
                }
                else
                {
                    ShowMessageAsync("Please select product before updating..");
                }

                
            }
            catch (Exception ex)
            {
                ShowMessageAsync(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SelectedProduct != null)
                {
                    bool isDeleted = data.DeleteProduct(this.SelectedProduct.ProductId);

                    if (isDeleted)
                    {
                        ShowMessageAsync("Product deleted successfully!");

                        data.LoadProducts();
                        productGrid.ItemsSource = data.ProductList;
                        this.SelectedProduct = null;
                    }
                    else
                    {
                        ShowMessageAsync("Failed to delete product...");
                    }
                    this.SelectedProduct = null;
                }
                else
                {
                    ShowMessageAsync("Please select product before deleting..");
                }

            }
            catch (Exception ex)
            {
                ShowMessageAsync(ex.Message);
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
