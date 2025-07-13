using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace ilac_takip
{
    
    public partial class main : Window
    {
        public main()
        {
            InitializeComponent();
            LoadDoctors();
        }

        private void LoadDoctors()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["VeritabaniBaglantisi"].ConnectionString;
            List<string> doctorNames = new List<string>(); // Doktor isimlerini tutacak liste

            string query = "SELECT isim FROM doktor ORDER BY isim"; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            doctorNames.Add(reader["isim"].ToString());
                        }
                        reader.Close();
                        // Eğer doktor sayısı, TextBlock sayısından azsa veya fazla olursa dikkatli olun
                        if (doctorNames.Count > 0)
                        {
                            isim1_Kopyala.Text = doctorNames[0];
                        }
                        if (doctorNames.Count > 1)
                        {
                            isim2.Text = doctorNames[1];
                        }
                        if (doctorNames.Count > 2)
                        {
                            isim3.Text = doctorNames[2];
                        }
                        if (doctorNames.Count > 3)
                        {
                            isim4.Text = doctorNames[3];
                        }
                        if (doctorNames.Count > 4)
                        {
                            isim5.Text = doctorNames[4];
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

        private void BtnAnasayfa_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                TextBlock tb = clickedButton.Content as TextBlock;
                if (tb != null)
                {
                    string doctorName = tb.Text;
                    // drdetay sayfasına doktor adını ileterek yeni bir örnek oluştur ve Frame içinde göster.
                    if (mainFrame != null) 
                    {
                        mainFrame.Navigate(new drdetay(doctorName));
                    }
                    else
                    {
                        MessageBox.Show("mainFrame kontrolü bulunamadı. Lütfen main.xaml dosyanızı kontrol edin.");
                    }
                }
            }
        }
    }
}
