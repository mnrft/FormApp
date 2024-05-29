using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using MySql.Data.MySqlClient;

// Zaman ayarlı mesaj gösteren bir form sınıfı tanımlıyoruz.
public class TimedMessageForm : Form
{
    // Kontrolleri sınıf seviyesinde tanımlıyoruz ki diğer metodlardan da erişilebilsin.
    private TextBox messageTextBox;
    private NumericUpDown secondsNumericUpDown;
    private Button startTimerButton;
    private Label displayLabel;
    private System.Timers.Timer messageTimer;
    private MySqlConnection databaseConnection;

    // Formun yapıcı metodu. Form oluşturulduğunda otomatik olarak çağrılır.
    public TimedMessageForm()
    {
        // Kontrolleri başlatıyoruz.
        InitializeComponents();
        // Veritabanı bağlantısını yapıyoruz.
        InitializeDatabaseConnection();
        // Veritabanından en son kaydedilen mesaj ve zaman aralığını alıyoruz.
        RetrieveMessageAndInterval();
    }

    // Form üzerindeki kontrolleri ve bunların özelliklerini başlatmak için kullanılan metod.
    private void InitializeComponents()
    {
        // Formun boyutunu ve başlık metnini ayarlıyoruz.
        this.Size = new Size(500, 200);
        this.Text = "Zaman Ayarlı Mesaj";
        // Formu ekranın ortasında başlatıyoruz.
        this.StartPosition = FormStartPosition.CenterScreen;

        // Kullanıcıdan mesaj girmesini istediğimiz TextBox'ı oluşturuyoruz.
        messageTextBox = new TextBox
        {
            Size = new Size(200, 25),
            Location = new Point(20, 20),
            // Kullanıcıya rehberlik etmek için gri renkli bir yer tutucu metin koyuyoruz.
            Text = "Mesajınızı yazın...",
            ForeColor = Color.Gray
        };
        // Kullanıcı TextBox'a odaklandığında yer tutucu metni temizliyoruz.
        messageTextBox.Enter += (sender, e) => {
            if (messageTextBox.Text == "Mesajınızı yazın...")
            {
                messageTextBox.Text = "";
                messageTextBox.ForeColor = Color.Black;
            }
        };
        // Kullanıcı TextBox'tan odaklanmayı kaybettiğinde yer tutucu metni geri koyuyoruz.
        messageTextBox.Leave += (sender, e) => {
            if (string.IsNullOrEmpty(messageTextBox.Text))
            {
                messageTextBox.Text = "Mesajınızı yazın...";
                messageTextBox.ForeColor = Color.Gray;
            }
        };

        // Kullanıcının mesajı ne kadar süre sonra göstermek istediğini seçebileceği NumericUpdown kontrolünü oluşturuyoruz.
        secondsNumericUpDown = new NumericUpDown
        {
            Size = new Size(70, 25),
            Location = new Point(230, 20),
            // Kullanıcının seçebileceği minimum ve maximum süreleri belirliyoruz.
            Minimum = 1,
            Maximum = 60
        };

        // Zamanlayıcıyı başlatmak için bir buton oluşturuyoruz.
        startTimerButton = new Button
        {
            Text = "Zamanlayıcıyı Başlat",
            Location = new Point(310, 20),
            Size = new Size(150, 25),
        };
        // Butona tıklama olayı ekliyoruz, bu olay zamanlayıcıyı başlatacak.
        startTimerButton.Click += StartTimerButton_Click;

        // Zamanlayıcı tamamlandığında mesajı göstermek için bir label oluşturuyoruz.
        displayLabel = new Label
        {
            Size = new Size(450, 25),
            Location = new Point(20, 60),
            // Label'ın fontunu ve rengini ayarlıyoruz.
            Font = new Font("Segoe UI", 14),
            ForeColor = Color.DarkSlateGray
        };

        // Oluşturduğumuz kontrol elemanlarını forma ekliyoruz.
        this.Controls.Add(messageTextBox);
        this.Controls.Add(secondsNumericUpDown);
        this.Controls.Add(startTimerButton);
        this.Controls.Add(displayLabel);
    }

    // Butona tıklama olayını işleyen metod.
    private void StartTimerButton_Click(object sender, EventArgs e)
    {
        // NumericUpdown'dan alınan değeri kullanarak zaman aralığını hesaplıyoruz.
        int interval = (int)secondsNumericUpDown.Value;
        // TextBox'tan alınan mesaj metnini bir değişkene atıyoruz.
        string message = messageTextBox.Text == "Mesajınızı yazın..." ? "" : messageTextBox.Text;

        // Mesajı ve zaman aralığını veritabanına kaydediyoruz.
        SaveMessageAndInterval(message, interval);

        // Sistemin Timer'ını kullanarak belirlenen süre sonunda bir eylem gerçekleştirecek zamanlayıcıyı ayarlıyoruz.
        messageTimer = new System.Timers.Timer(interval * 1000);
        messageTimer.Elapsed += (src, evt) =>
        {
            // Zamanlayıcı süresi dolduğunda timer'ı durduruyoruz.
            messageTimer.Stop();
            // Kontroller başka bir thread'de güncellendiği için Invoke metodunu kullanarak güvenli bir şekilde erişiyoruz.
            displayLabel.Invoke(new Action(() =>
            {
                // Zamanlayıcı tamamlandığında mesajı Label üzerinde gösteriyoruz.
                displayLabel.Text = message;
            }));
        };
        // Zamanlayıcıyı başlatıyoruz.
        messageTimer.Start();
    }

    // MySQL veritabanı bağlantısını başlatmak için kullanılan metod.
    private void InitializeDatabaseConnection()
    {
        // MySQL veritabanı için bağlantı dizesini belirliyoruz.
        string connectionString = "server=localhost;port=3306;database=timed_messages;user=root;password=;";
        // MySqlConnection nesnesi oluşturuyoruz.
        databaseConnection = new MySqlConnection(connectionString);
        try
        {
            // Veritabanı bağlantısını açıyoruz.
            databaseConnection.Open();
        }
        catch (MySqlException ex)
        {
            // Veritabanı bağlantı hatasını yakalayıp, kullanıcıya gösteriyoruz.
            MessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message);
        }
    }

    // Veritabanına mesaj ve zaman aralığını kaydeden metod.
    private void SaveMessageAndInterval(string message, int interval)
    {
        // SQL sorgumuzu yazıyoruz.
        string query = "INSERT INTO timed_messages (message, interval_seconds) VALUES (@message, @interval)";
        using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
        {
            // Sorgu parametrelerini ekliyoruz.
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@interval", interval);
            // Sorguyu çalıştırıyoruz.
            command.ExecuteNonQuery();
        }
    }

    // Veritabanından en son kaydedilen mesajı ve zaman aralığını çeken metod.
    private void RetrieveMessageAndInterval()
    {
        // SQL sorgumuzu yazıyoruz.
        string query = "SELECT message, interval_seconds FROM timed_messages ORDER BY id DESC LIMIT 1";
        using (MySqlCommand command = new MySqlCommand(query, databaseConnection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Okunan değerleri kontrol elemanlarına yerleştiriyoruz.
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
        // Görsel stil özelliklerini etkinleştiriyoruz.
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        // Ana form olarak TimedMessageForm sınıfının bir örneğini başlatıyoruz.
        Application.Run(new TimedMessageForm());
    }
}
