using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

// Not Hesaplama ve Veritabanına Kaydetme Form Sınıfı
public class NotHesaplamaFormu : Form
{
    private Label vizeLabel, finalLabel, odevLabel, ortalamaLabel, statusLabel, signatureLabel;
    private TextBox vizeTextBox, finalTextBox, odevTextBox;
    private Button hesaplaButton, kaydetButton;
    private PictureBox logoPictureBox;
    private MySqlConnection veritabaniBaglantisi;

    public NotHesaplamaFormu()
    {
        InitializeComponent();
        InitializeDatabaseConnection();
    }

    private void InitializeComponent()
    {
        // Form özelliklerini ayarlanıyor.
        this.Text = "Not Hesaplama Programı";
        this.BackColor = Color.WhiteSmoke;
        this.Size = new Size(400, 350);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Kontroller ve etiketler oluşturuluyor.
        vizeLabel = new Label { Text = "Vize Notu:", Location = new Point(10, 20), AutoSize = true };
        finalLabel = new Label { Text = "Final Notu:", Location = new Point(10, 60), AutoSize = true };
        odevLabel = new Label { Text = "Ödev Notu:", Location = new Point(10, 100), AutoSize = true };
        ortalamaLabel = new Label { Text = "Ortalama: -", Location = new Point(10, 220), AutoSize = true };
        statusLabel = new Label { Location = new Point(10, 250), Size = new Size(380, 20), Text = "Veritabanı durumu: Bağlanmadı" };
        signatureLabel = new Label
        {
            Text = "Dr. Öğr. Üyesi Yakup Kutlu",
            AutoSize = true,
            ForeColor = Color.DimGray,
            Font = new Font("Arial", 8, FontStyle.Italic),
            Location = new Point(10, this.Height - 40)
        };

        vizeTextBox = new TextBox { Location = new Point(120, 20), Width = 100 };
        finalTextBox = new TextBox { Location = new Point(120, 60), Width = 100 };
        odevTextBox = new TextBox { Location = new Point(120, 100), Width = 100 };

        // Hesapla ve Kaydet butonları oluşturuluyor.
        hesaplaButton = new Button { Text = "Ortalama Hesapla", Location = new Point(240, 20), Size = new Size(130, 25) };
        hesaplaButton.Click += HesaplaButton_Click;

        kaydetButton = new Button { Text = "Veritabanına Kaydet", Location = new Point(240, 60), Size = new Size(130, 25), Enabled = false };
        kaydetButton.Click += KaydetButton_Click;

        // Logo PictureBox'ı oluşturuluyor.
        logoPictureBox = new PictureBox
        {
            Image = Image.FromFile(@"C:\Users\fatma\Masaüstü\Çalışma dosyaları\iste_arma (1).png"), // Logonun dosya yolu
            SizeMode = PictureBoxSizeMode.Zoom, // Resmi orantılı olarak sığdırır
            Location = new Point(240, 180), // PictureBox'ın form üzerindeki konumu
            Size = new Size(50, 50) // PictureBox'ın boyutları, logo boyutunuza göre ayarlama
        };


        
        this.Controls.Add(vizeLabel);
        this.Controls.Add(finalLabel);
        this.Controls.Add(odevLabel);
        this.Controls.Add(ortalamaLabel);
        this.Controls.Add(vizeTextBox);
        this.Controls.Add(finalTextBox);
        this.Controls.Add(odevTextBox);
        this.Controls.Add(hesaplaButton);
        this.Controls.Add(kaydetButton);
        this.Controls.Add(statusLabel);
        this.Controls.Add(signatureLabel);
        this.Controls.Add(logoPictureBox);
    }

    private void HesaplaButton_Click(object sender, EventArgs e)
    {
        
        if (double.TryParse(vizeTextBox.Text, out double vize) &&
            double.TryParse(finalTextBox.Text, out double final) &&
            double.TryParse(odevTextBox.Text, out double odev))
        {
            // Vize %30, Final %60, Ödev %10 ağırlıkta hesaplanıyor.
            double ortalama = (vize * 0.3) + (final * 0.6) + (odev * 0.1);
            ortalamaLabel.Text = $"Ortalama: {ortalama:F2}";
            kaydetButton.Enabled = true; 
        }
        else
        {
            MessageBox.Show("Lütfen geçerli not değerleri girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ortalamaLabel.Text = "Ortalama: -";
            kaydetButton.Enabled = false;
        }
    }


    private void KaydetButton_Click(object sender, EventArgs e)
    {
        // Ortalama veritabanına kaydediliyor.
        string sorgu = "INSERT INTO notlar (vize, final, odev, ortalama) VALUES (@vize, @final, @odev, @ortalama)";
        using (MySqlCommand komut = new MySqlCommand(sorgu, veritabaniBaglantisi))
        {
            komut.Parameters.AddWithValue("@vize", double.Parse(vizeTextBox.Text));
            komut.Parameters.AddWithValue("@final", double.Parse(finalTextBox.Text));
            komut.Parameters.AddWithValue("@odev", double.Parse(odevTextBox.Text));
            komut.Parameters.AddWithValue("@ortalama", double.Parse(ortalamaLabel.Text.Substring(10)));
            komut.ExecuteNonQuery();
            MessageBox.Show("Notlar başarıyla veritabanına kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void InitializeDatabaseConnection()
    {
        string connectionString = "server=localhost;port=3306;database=nothesapla;user=root;password=;";
        veritabaniBaglantisi = new MySqlConnection(connectionString);
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

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // Form kapatıldığında veritabanı bağlantısı da kapatılıyor.
        veritabaniBaglantisi?.Close();
        base.OnFormClosed(e);
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new NotHesaplamaFormu());
    }
}
