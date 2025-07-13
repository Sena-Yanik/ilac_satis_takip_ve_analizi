# İlaç Mümessili Satış ve Doktor Takip Sistemi (Pharmaceutical Rep Sales & Doctor Tracking System)

# 
Bu masaüstü uygulaması, ilaç şirketlerinin mümessiller aracılığıyla doktorlara yapılan ilaç satışlarını takip etmelerini ve hangi doktorun hangi ilacı ne kadar yazdığını kaydetmelerini sağlar. C# ve WPF ile geliştirilmiş olup, güvenli giriş, detaylı doktor bilgileri ve satış performans grafiklerini içerir.

## İçerik Tablosu
- [Hakkında](#hakkında)
- [Özellikler](#özellikler)
- [Ekran Görüntüleri](#ekran-görüntüleri)
- [Kurulum](#kurulum)
  - [Ön Koşullar](#ön-koşullar)
  - [Yükleme Adımları](#yükleme-adımları)
- [Kullanım](#kullanım)
- [Proje Yapısı](#proje-yapısı)
- [Katkıda Bulunma](#katkıda-bulunma)
- [Lisans](#lisans)
- [İletişim](#iletişim)

---

## Hakkında
Bu proje, ilaç şirketleri için tasarlanmış bir Windows Presentation Foundation (WPF) uygulamasıdır. Temel amacı, ilaç mümessillerinin doktor ziyaretleri ve bu ziyaretler sonucunda gerçekleşen ilaç satışlarını etkin bir şekilde takip etmektir. Uygulama, hangi doktorun belirli ilaçlardan ne kadar reçete ettiğini kaydetmeye ve bu verileri görsel olarak sunmaya yardımcı olur. Böylece ilaç şirketleri, satış performanslarını izleyebilir ve doktor bazında ilaç yazım alışkanlıklarını analiz edebilirler. Uygulama, veri yönetimi için bir SQL Server veritabanına bağlanır.

## Özellikler
* **Kullanıcı Kimlik Doğrulaması**: Güvenli kullanıcı adı ve şifre ile giriş sistemi.
* **Mümessil/Doktor Takibi**: Veritabanından çekilen doktor listesini görüntüler.
* **Doktor Detayları ve İlaç Yazım Takibi**: Her bir doktor için detaylı bilgi sağlar, yazılan ilaç adedini ve ilgili şirket bilgilerini gösterir.
* **Satış/Reçete Grafikleri**: İlaç yazım veya satış verilerini grafiklerle görselleştirerek aylık performans analizine olanak tanır.
* **Veritabanı Entegrasyonu**: Uygulama verilerini (kullanıcılar, doktorlar, ilaç satışları) bir SQL Server veritabanında depolar ve yönetir.
* **Yapılandırma Yönetimi**: Veritabanı bağlantı dizelerini `App.config` dosyasından okur.

## Ekran Görüntüleri
| Giriş Ekranı | Doktor Detayları - Deniz | Doktor Detayları - Fatma |
|---|---|---|
| ![Giriş Ekranı](https://github.com/Sena-Yanik/ilac_satis_takip_ve_analizi/blob/main/ilac_takip/images/Ekran%20g%C3%B6r%C3%BCnt%C3%BCs%C3%BC%202025-07-13%20153007.png) | ![Deniz Doktor Detayları](https://github.com/Sena-Yanik/ilac_satis_takip_ve_analizi/blob/main/ilac_takip/images/Ekran%20g%C3%B6r%C3%BCnt%C3%BCs%C3%BC%202025-07-13%20153048.png) | ![Fatma Doktor Detayları](https://github.com/Sena-Yanik/ilac_satis_takip_ve_analizi/blob/main/ilac_takip/images/Ekran%20g%C3%B6r%C3%BCnt%C3%BCs%C3%BC%202025-07-13%20153137.png) |

## Kurulum

### Ön Koşullar
Başlamadan önce aşağıdaki gereksinimleri karşıladığınızdan emin olun:
* Visual Studio (2019 veya üzeri önerilir)
* .NET Framework 4.7.2
* SQL Server (veya SQL Server Express, App.config'daki bağlantı dizenize bağlı olarak)
* C#, WPF ve SQL Server hakkında temel bilgi.

### Yükleme Adımları

1.  **Depoyu Klonlayın:**
    ```bash
    git clone [https://github.com/your-username/your-repository-name.git](https://github.com/your-username/your-repository-name.git)
    ```
2.  **Visual Studio'da Açın:**
    Klonladığınız dizine gidin ve `.sln` dosyasını Visual Studio'da açın.
3.  **NuGet Paketlerini Geri Yükleyin:**
    Çözüm Gezgini'nde çözüme sağ tıklayın ve `LiveCharts`, `LiveCharts.Wpf` ve `System.Data.SqlClient` paketlerini yüklemek için "NuGet Paketlerini Geri Yükle"yi seçin.
4.  **Veritabanını Yapılandırın:**
    * `App.config` dosyasını açın.
    * `connectionStrings` altındaki `connectionString` değerini kendi SQL Server örneğinize ve veritabanınıza işaret edecek şekilde güncelleyin.
        ```xml
        <add name="VeritabaniBaglantisi"
             connectionString="Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Trusted_Connection=True;"
             providerName="System.Data.SqlClient" />
        ```
        `YOUR_SERVER_NAME` ve `YOUR_DATABASE_NAME` yerine kendi SQL Server bilgilerinizi girin.
    * SQL Server veritabanınızda (`ilac_sati_takip` olarak App.config'da belirtildiği gibi) giriş ve doktor verileri için uygun şemaya sahip `giris`, `doktor` ve ilaç satış detaylarını tutacak tabloların bulunduğundan emin olun.
        * `giris` tablosu, giriş için `txtKullaniciAdi` ve `pwdSifre` sütunlarına sahip olmalıdır.
        * `doktor` tablosu, doktor isimleri için `isim` sütununa sahip olmalıdır.
        * Doktor detay ekranlarını tam olarak desteklemek için ilaç satışları (tarih, firma adı, ilaç adı, adet gibi) bilgilerini içerecek ek tablolar gerekecektir.

## Kullanım
1.  Uygulamayı Visual Studio'dan **çalıştırın**.
2.  **Giriş Ekranı**: Kullanıcı adınızı ve şifrenizi girin. Bu kimlik bilgileri, veritabanınızdaki `giris` tablosuna göre doğrulanacaktır.
    * *Örnek*: Eğer `giris` tablonuzda `txtKullaniciAdi = "test"` ve `pwdSifre = "password"` değerlerine sahip bir kullanıcı varsa, bunları kullanabilirsiniz.
3.  **Ana Pencere**: Başarılı girişten sonra, doktor listesini gösteren ana pencereye yönlendirileceksiniz.
4.  **Doktor Detayları**: İlaç satışları ve grafikler de dahil olmak üzere detaylı bilgilerini görüntülemek için listeden bir doktorun adına tıklayın.

## Proje Yapısı
* `login.xaml.cs`: Kullanıcı kimlik doğrulama mantığını içerir ve başarılı girişte ana pencereye yönlendirme yapar.
* `main.xaml.cs`: Doktor isimlerinin görüntülenmesini yönetir ve doktor detay sayfalarına yönlendirmeyi sağlar.
* `App.xaml.cs`: Standart uygulama giriş noktasıdır.
* `App.config`: Veritabanı bağlantı dizeleri için yapılandırma dosyasıdır.
* `packages.config`: NuGet paket bağımlılıklarını listeler.
* `Ekran görüntüsü 2025-07-13 153007.png`: Giriş ekranının ekran görüntüsü.
* `Ekran görüntüsü 2025-07-13 153048.png`: Deniz adlı doktorun detaylarının ekran görüntüsü.
* `Ekran görüntüsü 2025-07-13 153137.png`: Fatma adlı doktorun detaylarının ekran görüntüsü.
* `drdetay.xaml.cs` (sağlanmamış ancak çıkarım yapılmıştır): Bireysel doktor detaylarını ve grafikleri görüntülemek için mantığı içermesi beklenen dosya, muhtemelen `main.xaml.cs` tarafından örneklendirilir.

## Katkıda Bulunma
Katkılarınızı bekliyoruz! Lütfen depoyu çatallamaktan (fork), yeni bir dal (branch) oluşturmaktan ve herhangi bir geliştirme veya hata düzeltmesi için çekme isteği (pull request) göndermekten çekinmeyin.

## Lisans
Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır - daha fazla bilgi için LİSANS dosyasına bakın. 

## İletişim
Sena Yanık - sena.yanik.srk@gmail.com
Proje Bağlantısı: (https://github.com/Sena-Yanik/ilac_satis_takip_ve_analizi)
```"
