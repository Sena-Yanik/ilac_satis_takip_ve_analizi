using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient; 
using System.Configuration; 

namespace ilac_takip
{
    /// <summary>
    /// login.xaml etkileşim mantığı
    /// </summary>
    public partial class login : Window
    {
        public login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            string username = txtKullaniciAdi.Text; 
            string password = pwdSifre.Password; 

            // Boş alan kontrolü
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş bırakılamaz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

           
            string connectionString = ConfigurationManager.ConnectionStrings["VeritabaniBaglantisi"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL Injection'ı önlemek için parametreli sorgu 
                string query = "SELECT COUNT(1) FROM giris WHERE txtKullaniciAdi = @Username AND pwdSifre = @Password"; 

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password); 

                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            // ANA EKRANA GEÇİŞ
                            main mainWindow = new main(); // main.xaml'deki pencerenizin sınıf adı 'main'
                            mainWindow.Show();
                            this.Close(); // Mevcut login penceresini kapat

                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Veritabanı bağlantı hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Beklenmedik bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}