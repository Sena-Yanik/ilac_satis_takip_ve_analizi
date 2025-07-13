using System;
using System.Collections.Generic;
using System.Configuration; 
using System.Data; 
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts; 
using LiveCharts.Wpf;

namespace ilac_takip
{
    public partial class drdetay : Page
    {
        private string _doctorName;

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public drdetay(string doctorName)
        {
            InitializeComponent();
            _doctorName = doctorName;

            drDetayHeader.Text = $"{doctorName} Doktor Detayları"; 
            LoadDoctorRegion(doctorName); 
            LoadDoctorSalesData(doctorName); 
            LoadMonthlySalesChart(doctorName); 
            LoadTotalDrugSales(doctorName); 

            DataContext = this;
        }

        // Doktorun bölge bilgisini yükleyen metot
        private void LoadDoctorRegion(string doctorName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["VeritabaniBaglantisi"].ConnectionString;
            string region = string.Empty;

            string query = "SELECT bölge FROM dbo.doktor WHERE isim = @DoctorName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorName", doctorName);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            region = result.ToString();
                        }
                        txtDoktorBolge.Text = $"Bölge: {region}"; // Bölgeyi güncelle
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Veritabanı bağlantı hatası (Bölge): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Beklenmedik bir hata oluştu (Bölge): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void LoadDoctorSalesData(string doctorName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["VeritabaniBaglantisi"].ConnectionString;
            DataTable dt = new DataTable();

            string query = @"
                SELECT
                    s.tarih,
                    i.firma_adi,
                    i.ilac_adi,
                    s.adet
                FROM
                    dbo.satis AS s
                INNER JOIN
                    dbo.doktor AS d ON s.drid = d.drid
                INNER JOIN
                    dbo.ilac AS i ON s.ilacid = i.ilacid
                WHERE
                    d.isim = @DoctorName
                ORDER BY s.tarih DESC;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorName", doctorName);

                    try
                    {
                        connection.Open();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(dt);

                        dataGridIlacDetay.ItemsSource = dt.DefaultView;
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Veritabanı bağlantı hatası (DataGrid): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Beklenmedik bir hata oluştu (DataGrid): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void LoadMonthlySalesChart(string doctorName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["VeritabaniBaglantisi"].ConnectionString;
            Dictionary<string, double> monthlySales = new Dictionary<string, double>();

            DateTime oneYearAgo = DateTime.Now.AddYears(-1);

            string query = @"
                SELECT
                    MONTH(s.tarih) AS Ay,
                    YEAR(s.tarih) AS Yil,
                    SUM(s.adet) AS ToplamAdet
                FROM
                    dbo.satis AS s
                INNER JOIN
                    dbo.doktor AS d ON s.drid = d.drid
                WHERE
                    d.isim = @DoctorName AND s.tarih >= @OneYearAgo
                GROUP BY
                    MONTH(s.tarih), YEAR(s.tarih)
                ORDER BY
                    Yil ASC, Ay ASC;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorName", doctorName);
                    command.Parameters.AddWithValue("@OneYearAgo", oneYearAgo);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            try
                            {
                                int month = reader.IsDBNull(reader.GetOrdinal("Ay")) ? 1 : Convert.ToInt32(reader["Ay"]);
                                int year = reader.IsDBNull(reader.GetOrdinal("Yil")) ? DateTime.Now.Year : Convert.ToInt32(reader["Yil"]);
                                double totalAdet = reader.IsDBNull(reader.GetOrdinal("ToplamAdet")) ? 0.0 : Convert.ToDouble(reader["ToplamAdet"]);

                                string monthYear = new DateTime(year, month, 1).ToString("MMMM yyyy");
                                monthlySales[monthYear] = totalAdet;
                            }
                            catch (InvalidCastException ex)
                            {
                                MessageBox.Show($"Veri dönüşüm hatası: '{ex.Message}'. Sütun değerlerini kontrol edin. Ay: {reader["Ay"]}, Yil: {reader["Yil"]}, ToplamAdet: {reader["ToplamAdet"]}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Beklenmedik bir hata oluştu (Grafik Veri Okuma): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                        reader.Close();

                        List<string> labelsList = new List<string>();
                        List<double> valuesList = new List<double>();

                        for (int i = 0; i < 12; i++)
                        {
                            DateTime currentMonth = DateTime.Now.AddMonths(-i);
                            string monthKey = currentMonth.ToString("MMMM yyyy");
                            labelsList.Insert(0, monthKey);

                            double currentMonthAdet = 0;
                            if (monthlySales.TryGetValue(monthKey, out currentMonthAdet))
                            {
                                valuesList.Insert(0, currentMonthAdet);
                            }
                            else
                            {
                                valuesList.Insert(0, 0);
                            }
                        }

                        SeriesCollection = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Toplam Satış Adedi",
                                Values = new ChartValues<double>(valuesList)
                            }
                        };

                        Labels = labelsList.ToArray();
                        Formatter = value => value.ToString("N0");

                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Veritabanı bağlantı hatası (Grafik): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Beklenmedik bir hata oluştu (Grafik Ana): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void LoadTotalDrugSales(string doctorName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["VeritabaniBaglantisi"].ConnectionString;

            Dictionary<string, double> drugTotalSales = new Dictionary<string, double>();

            string query = @"
                SELECT
                    i.ilac_adi,
                    SUM(s.adet) AS ToplamAdet
                FROM
                    dbo.satis AS s
                INNER JOIN
                    dbo.doktor AS d ON s.drid = d.drid
                INNER JOIN
                    dbo.ilac AS i ON s.ilacid = i.ilacid
                WHERE
                    d.isim = @DoctorName
                GROUP BY
                    i.ilac_adi;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorName", doctorName);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            string ilacAdi = reader["ilac_adi"].ToString();
                            double toplamAdet = reader.IsDBNull(reader.GetOrdinal("ToplamAdet")) ? 0.0 : Convert.ToDouble(reader["ToplamAdet"]);
                            drugTotalSales[ilacAdi.ToUpper()] = toplamAdet;
                        }
                        reader.Close();

                        double count;

                        if (drugTotalSales.TryGetValue("PAROL", out count))
                            txtParolAdet.Text = $"Adet: {count:N0}";
                        else
                            txtParolAdet.Text = "Adet: 0";

                        if (drugTotalSales.TryGetValue("ADVİL", out count))
                            txtAdvilAdet.Text = $"Adet: {count:N0}";
                        else
                            txtAdvilAdet.Text = "Adet: 0";

                        if (drugTotalSales.TryGetValue("NOVALGİN", out count))
                            txtNovalginAdet.Text = $"Adet: {count:N0}";
                        else
                            txtNovalginAdet.Text = "Adet: 0";

                        if (drugTotalSales.TryGetValue("İBURAMİN", out count))
                            txtIburaminAdet.Text = $"Adet: {count:N0}";
                        else
                            txtIburaminAdet.Text = "Adet: 0";

                        if (drugTotalSales.TryGetValue("ARVELES", out count))
                            txtArvelesAdet.Text = $"Adet: {count:N0}";
                        else
                            txtArvelesAdet.Text = "Adet: 0";
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Veritabanı bağlantı hatası (İlaç Adetleri): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Beklenmedik bir hata oluştu (İlaç Adetleri): {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
    }
}