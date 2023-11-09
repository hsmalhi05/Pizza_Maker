using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MainPage : Page
    {
        ExamData data;
        public MainPage()
        {
            this.InitializeComponent();
            data = new ExamData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            data.LoggedUser = null;

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Password.ToString();

                if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    txtMsg.Text = "Username/Password are required!";
                }
                else
                {
                    User loggedUser = data.ValidateUser(username, password);

                    if(loggedUser != null)
                    {
                        data.LoggedUser = loggedUser;
                        if(loggedUser.UserType.ToUpper() == "ADMIN")
                        {
                            this.Frame.Navigate(typeof(ProductDashboard), data);
                        }
                        else
                        {
                            this.Frame.Navigate(typeof(CustomerDashboard), data);
                        }
                       
                    }
                    else
                    {
                        txtMsg.Text = "Invalid Credentials! Please try again";
                    }
                }
            }catch (Exception ex)
            {
                txtMsg.Text = "Error: " + ex.Message;
            }
        }

        private void btnGuest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Frame.Navigate(typeof(CustomerDashboard), data);
            }
            catch (Exception ex)
            {
                txtMsg.Text = "Error: " + ex.Message;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fullName = txtFullName.Text;
                string email = txtEmail.Text;
                string phone = txtPhone.Text;
                string password = txtRegPassword.Password.ToString();

                if(string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
                {
                    txtMsg.Text = "All Fields are required to register";
                }
                else
                {
                    bool checkEmail = data.CheckUserExists(email);

                    if (checkEmail)
                    {
                        txtMsg.Text = "Email already exists! Please try different email.";
                    }
                    else
                    {
                        User newUser = new User();
                        newUser.Email = email;
                        newUser.Phone = phone;
                        newUser.Password = password;
                        newUser.FullName = fullName;
                        newUser.UserType = "CUSTOMER";

                        bool isInserted = data.InsertUser(newUser);

                        if (isInserted)
                        {
                            txtMsg.Text = "You have been successfully registered!";

                            txtFullName.Text = "";
                            txtEmail.Text = "";
                            txtRegPassword.Password = "";
                            txtPhone.Text = "";
                        }
                        else
                        {
                            txtMsg.Text = "Failed to register!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtMsg.Text = "Error: " + ex.Message;
            }
        }


    }
}
