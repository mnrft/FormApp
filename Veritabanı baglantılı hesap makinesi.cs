using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

// Veritabanı bağlantılı ve kullanıcı arayüzüne sahip bir hesap makinesi sınıfı tanımlandı.
public class VeritabaniHesapMakinesi : Form
{
    private TextBox input1, input2;
    private Button toplaButton, cikarButton, carpButton, bolButton;
    private Label sonucLabel, statusLabel, signatureLabel;
    private PictureBox logoPictureBox;
    private MySqlConnection veritabaniBaglantisi;

    public VeritabaniHesapMakinesi()
    {
        InitializeComponent();
        VeritabaniBaglantisiKur();
    }

    private void InitializeComponent()
    {
        // Formun genel ayarlarını yapılıyor.
        this.Text = "Veritabanı Bağlantılı Hesap Makinesi";
        this.BackColor = Color.WhiteSmoke;
        this.Size = new Size(320, 240);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Giriş kutuları
        input1 = new TextBox { Location = new Point(10, 10), Text = "0" };
        input2 = new TextBox { Location = new Point(170, 10), Text = "0" };

        // İşlem butonlarını oluşturuluyor.
        toplaButton = new Button { Text = "Topla", Location = new Point(10, 40) };
        cikarButton = new Button { Text = "Çıkar", Location = new Point(170, 40) };
        carpButton = new Button { Text = "Çarp", Location = new Point(10, 70) };
        bolButton = new Button { Text = "Böl", Location = new Point(170, 70) };

        // Butonların olay işleyicilerini (event handlers) atama.
        toplaButton.Click += (sender, e) => Hesapla((x, y) => x + y);
        cikarButton.Click += (sender, e) => Hesapla((x, y) => x - y);
        carpButton.Click += (sender, e) => Hesapla((x, y) => x * y);
        bolButton.Click += (sender, e) => Hesapla((x, y) => x / y);

        // Sonuç etiketi oluşturma.
        sonucLabel = new Label { Text = "Sonuç: 0", Location = new Point(10, 140), AutoSize = true };

        // Veritabanı bağlantı durumu etiketi oluşturma.
        statusLabel = new Label { Location = new Point(10, 100), Size = new Size(300, 20) };

        // İmza etiketini oluşturma.
        signatureLabel = new Label
        {
            Text = "Dr. Öğr. Üyesi Ayşe Birelli",
            AutoSize = true,
            ForeColor = Color.DimGray,
            Font = new Font("Segoe UI", 7, FontStyle.Italic),
            Location = new Point(10, 180)
        };

        // Logo için PictureBox oluşturuluyor.
        logoPictureBox = new PictureBox
        {
            Image = Image.FromFile(@"C:\Users\fatma\Masaüstü\Çalışma dosyaları\iste_arma.png"), // Logo yolu
            SizeMode = PictureBoxSizeMode.StretchImage,
            Location = new Point(250, 180),
            Size = new Size(50, 50)
        };

        // Oluşturulan kontroller forma ekleniyor.
        this.Controls.Add(input1);
        this.Controls.Add(input2);
        this.Controls.Add(toplaButton);
        this.Controls.Add(cikarButton);
        this.Controls.Add(carpButton);
        this.Controls.Add(bolButton);
        this.Controls.Add(sonucLabel);
        this.Controls.Add(statusLabel);
        this.Controls.Add(signatureLabel);
        this.Controls.Add(logoPictureBox);
    }

    private void VeritabaniBaglantisiKur()
    {
        // MySQL veritabanı bağlantı dizesi.
        string baglantiDizesi = "server=localhost;port=3306;database=hesapmakinesi;user=root;password=;";
        veritabaniBaglantisi = new MySqlConnection(baglantiDizesi);

        // Veritabanı bağlantısı açma ve durum kontrolü.
        try
        {
            veritabaniBaglantisi.Open();
            statusLabel.Text = "Veritabanı bağlantı durumu: Bağlı.";
        }
        catch (MySqlException ex)
        {
            statusLabel.Text = "Veritabanı bağlantı hatası: " + ex.Message;
        }
    }

    private void Hesapla(Func<double, double, double> islem)
    {
        try
        {
            // Giriş kutularından sayılar alınıyor.
            double sayi1 = double.Parse(input1.Text);
            double sayi2 = double.Parse(input2.Text);

            // Verilen işlemi kullanarak hesaplama yapılıyor.
            double sonuc = islem(sayi1, sayi2);

            // Sonu. etikette gösteriliyor.
            sonucLabel.Text = "Sonuç: " + sonuc;

            // Sonuç veritabanına kaydediliyor.
            string sorgu = "INSERT INTO results (num1, num2, result) VALUES (@num1, @num2, @result)";
            using (MySqlCommand komut = new MySqlCommand(sorgu, veritabaniBaglantisi))
            {
                // Parametreler ekleniyor.
                komut.Parameters.AddWithValue("@num1", sayi1);
                komut.Parameters.AddWithValue("@num2", sayi2);
                komut.Parameters.AddWithValue("@result", sonuc);

                // SQL komutu çalıştırılıyor.
                komut.ExecuteNonQuery();
            }
        }
        catch (FormatException)
        {
            MessageBox.Show("Lütfen geçerli sayılar giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (MySqlException ex)
        {
            MessageBox.Show("Veritabanı işlem hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (veritabaniBaglantisi.State == System.Data.ConnectionState.Open)
        {
            veritabaniBaglantisi.Close();
        }
        base.OnFormClosed(e);
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new VeritabaniHesapMakinesi());
    }
}
