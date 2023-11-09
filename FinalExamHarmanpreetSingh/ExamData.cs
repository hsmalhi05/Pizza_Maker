using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FinalExamHarmanpreetSingh
{
    public class ExamData
    {
        public ObservableCollection<Product> ProductList { get; set; }
        public User LoggedUser { get; set; }

        private string ConnectionString { get => GetConnectionString("2022ExamSQLServer"); }
       
        public ExamData()
        {
            ProductList = new ObservableCollection<Product>();

            LoadProducts();
        }
        private string GetConnectionString(string connectionStringName)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("config.json");
            IConfiguration config = builder.Build();
            return config["ConnectionStrings:" + connectionStringName];
        }

        public User ValidateUser(string email, string password)
        {
            User user = null;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT * FROM users WHERE Email = @Email AND Password = @Password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("Password", password);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    user = new User();
                    user.UserId = reader.GetInt32(0);
                    user.FullName = reader.GetString(1);
                    user.Email = reader.GetString(2);
                    user.Phone = reader.GetString(3);
                    user.Password = reader.GetString(4);
                    user.UserType = reader.GetString(5);
                }
            }
            return user;
        }

        public bool CheckUserExists(string email)
        {
            User user = null;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT * FROM users WHERE Email = @Email";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("Email", email);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    user = new User();
                    user.UserId = reader.GetInt32(0);
                    user.FullName = reader.GetString(1);
                    user.Email = reader.GetString(2);
                    user.Phone = reader.GetString(3);
                    user.Password = reader.GetString(4);
                    user.UserType = reader.GetString(5);
                }
            }
            return user != null ? true : false;
        }
        public void LoadProducts()
        {
            ProductList = new ObservableCollection<Product>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT * FROM products";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product();
                    product.ProductId = reader.GetInt32(0);
                    product.Name = reader.GetString(1);
                    product.Category = reader.GetString(2);
                    product.Price = reader.GetDouble(3);

                    ProductList.Add(product);
                }
            }
        }

        public Product GetProductById(int id)
        {
            Product product = new Product();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"SELECT * FROM products WHERE ProductId = @ProductId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("ProductId", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    
                    product.ProductId = reader.GetInt32(0);
                    product.Name = reader.GetString(1);
                    product.Category = reader.GetString(2);
                    product.Price = reader.GetDouble(3);

                }
            }
            return product;
        }


        public bool InsertUser(User user)
        {
            string query = @"Insert into users(FullName, Email, Phone, Password, UserType) 
                         VALUES(@FullName, @Email, @Phone, @Password, @UserType)";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("FullName", user.FullName);
                cmd.Parameters.AddWithValue("Email", user.Email);
                cmd.Parameters.AddWithValue("Phone", user.Phone);
                cmd.Parameters.AddWithValue("Password", user.Password);
                cmd.Parameters.AddWithValue("UserType", user.UserType);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        public bool InsertProduct(Product product)
        {
            string query = @"Insert into products(Name, Category, Price) 
                         VALUES(@Name, @Category, @Price)";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("Name", product.Name);
                cmd.Parameters.AddWithValue("Category", product.Category);
                cmd.Parameters.AddWithValue("Price", product.Price);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        public bool UpdateProduct(Product product)
        {
            string query = @"UPDATE products SET Name = @Name, Category = @Category, Price = @Price WHERE ProductId = @ProductId";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("ProductId", product.ProductId);
                cmd.Parameters.AddWithValue("Name", product.Name);
                cmd.Parameters.AddWithValue("Category", product.Category);
                cmd.Parameters.AddWithValue("Price", product.Price);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        public bool DeleteProduct(int id)
        {
            string query = @"DELETE FROM products WHERE ProductId = @ProductId";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("ProductId", id);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }


    }
}
