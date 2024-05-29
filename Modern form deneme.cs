using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using MySql.Data.MySqlClient;

// Zaman ayarlı mesaj gösteren bir form sınıfı tanımlanıyor.
public class TimedMessageForm : Form
{
    // Diğer metodlardan erişim sağlanabilir.
    private TextBox messageTextBox;
    private NumericUpDown secondsNumericUpDown;
    private Button startTimerButton;
    private Label displayLabel;
    private System.Timers.Timer messageTimer;
    private MySqlConnection databaseConnection;

    // Yapıcı metodu. Form oluşturulduğunda otomatik olarak çağrılır.
    public TimedMessageForm()
    {
        // Kontrolleri başlatılıyor.
        InitializeComponents();
        // Veritabanı bağlantısını yapılıyor.
        InitializeDatabaseConnection();
        // Veritabanından en son kaydedilen mesaj ve zaman aralığı alınır.
        RetrieveMessageAndInterval();
    }

    // Form üzerindeki kontrolleri ve bunların özelliklerini başlatmak için kullanılan metod.
    private void InitializeComponents()
    {
        this.Size = new Size(500, 200);
        this.Text = "Zaman Ayarlı Mesaj";
        this.StartPosition = FormStartPosition.CenterScreen;

       
        messageTextBox = new TextBox
        {
            Size = new Size(200, 25),
            Location = new Point(20, 20),
            // Kullanıcıya rehberlik etmek için gri renkli bir yer tutucu metin tanımlanıyor.
            Text = "Mesajınızı yazın...",
            ForeColor = Color.Gray
        };
        // Kullanıcı TextBox'a focuslandığında yer tutucu metin temizlenir.
        messageTextBox.Enter += (sender, e) => {
            if (messageTextBox.Text == "Mesajınızı yazın...")
            {
                messageTextBox.Text = "";
                messageTextBox.ForeColor = Color.Black;
            }
        };
        // Kullanıcı TextBox'tan odaklanmayı bıraktığında yer tutucu metin tekrar eklenir.
        messageTextBox.Leave += (sender, e) => {
            if (string.IsNullOrEmpty(messageTextBox.Text))
            {
                messageTextBox.Text = "Mesajınızı yazın...";
                messageTextBox.ForeColor = Color.Gray;
            }
        };

        // Kullanıcının mesajı ne kadar süre sonra göstermek istediğini seçebileceği NumericUpdown kontrolü oluşturuluyor
        secondsNumericUpDown = new NumericUpDown
        {
            Size = new Size(70, 25),
            Location = new Point(230, 20),
            // Kullanıcının seçebileceği minimum ve maximum süreler belirleniyor.
            Minimum = 1,
            Maximum = 60
        };

        // Zamanlayıcıyı başlatmak için bir buton oluşturuluyor.
        startTimerButton = new Button
        {
            Text = "Zamanlayıcıyı Başlat",
            Location = new Point(310, 20),
            Size = new Size(150, 25),
        };
        // Butona tıklama olayı ekliyoruz, bu olay zamanlayıcıyı başlatacak.
        startTimerButton.Click += StartTimerButton_Click;

        // Zamanlayıcı tamamlandığında mesajı göstermek için bir label oluşturulur.
        displayLabel = new Label
        {
            Size = new Size(450, 25),
            Location = new Point(20, 60),
            Font = new Font("Segoe UI", 14),
            ForeColor = Color.DarkSlateGray
        };

        // Oluşturulan kontrol elemanları forma ekleniyor.
        this.Controls.Add(messageTextBox);
        this.Controls.Add(secondsNumericUpDown);
        this.Controls.Add(startTimerButton);
        this.Controls.Add(displayLabel);
    }

    // Butona tıklama olayını işleyen metod.
    private void StartTimerButton_Click(object sender, EventArgs e)
    {
        // NumericUpdown'dan alınan değeri kullanarak zaman aralığı hesaplanıyor.
        int interval = (int)secondsNumericUpDown.Value;
        // TextBox'tan alınan mesaj metni bir değişkene atılıyor.
        string message = messageTextBox.Text == "Mesajınızı yazın..." ? "" : messageTextBox.Text;

        // Mesajı ve zaman aralığı veritabanına kaydedilir.
        SaveMessageAndInterval(message, interval);

        // Sistemin Timer'ını kullanarak belirlenen süre sonunda bir eylem gerçekleştirecek zamanlayıcı ayarlanıyor.
        messageTimer = new System.Timers.Timer(interval * 1000);
        messageTimer.Elapsed += (src, evt) =>
        {
            // Zamanlayıcı süresi dolduğunda timer durdurulur.
            messageTimer.Stop();
            // Kontroller başka bir thread'de güncellendiği için Invoke metodu kullanılarak güvenli bir şekilde erişiliyor.
            displayLabel.Invoke(new Action(() =>
            {
                // Zamanlayıcı tamamlandığında mesajı Label üzerinde gösterme.
                displayLabel.Text = message;
            }));
        };
        // Zamanlayıcı başlatılıyor.
        messageTimer.Start();
    }

    // MySQL veritabanı bağlantısını başlatmak için kullanılan metod.
    private void InitializeDatabaseConnection()
    {
        // MySQL veritabanı için bağlantı dizesi girilr.
        string connectionString = "server=localhost;port=3306;database=timed_messages;user=root;password=;";
        // MySqlConnection nesnesi oluşturulur.
        databaseConnection = new MySqlConnection(connectionString);
        try
        {
            databaseConnection.Open();
        }
        catch (MySqlException ex)
        {
            // Veritabanının bağlantı hatasını yakalanıp, kullanıcıya gösterilir..
            MessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message);
        }
    }

    // Veritabanına mesaj ve zaman aralığını kaydeden metod.
    private void SaveMessageAndInterval(string message, int interval)
    {
        
        string query = "INSERT INTO timed_messages (message, interval_seconds) VALUES (@message, @interval)";
        using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
        {
            // Sorgu parametreleri ekleniyor.
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@interval", interval);
            // Sorgu çalıştırılıyor.
            command.ExecuteNonQuery();
        }
    }

    // Veritabanından en son kaydedilen mesajı ve zaman aralığını çeken metod.
    private void RetrieveMessageAndInterval()
    {
    
        string query = "SELECT message, interval_seconds FROM timed_messages ORDER BY id DESC LIMIT 1";
        using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Okunan değerler kontrol elemanlarına yerleştirilir.
                    messageTextBox.Text = reader.GetString("message");
                    secondsNumericUpDown.Value = Convert.ToDecimal(reader.GetInt32("interval_seconds"));


                }
            }
        }
    }

    // Uygulamanın giriş noktası.
    [STAThread]
    static void Main()
    {
        // Görsel stil özelliklerini etkinleştirir.
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        // Ana form olarak TimedMessageForm sınıfının bir örneğini başlatır.
        Application.Run(new TimedMessageForm());
    }
}
