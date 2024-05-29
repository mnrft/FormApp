using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BilgiYarısması
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int soruNo = 0;
        int cevapDogru = 0;
        int cevapYanlıs = 0;
        int sure;
       


        private void button1_Click(object sender, EventArgs e)
        {
            label4.Text = "20";
            timer1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;

            button1.Text = "İleri";
            soruNo++;

            label1.Text =soruNo.ToString();

            if(soruNo == 1)
            {
                richTextBox1.Text = "Atatürk kaç yılında doğdu?";
                button2.Text = "1781";
                button3.Text = "1678";
                button4.Text = "1881";
                button5.Text = "1938";
                
            }
            if (soruNo == 2)
            {
                richTextBox1.Text = "Atatürk'ün annesinin adı nedir?";
                button2.Text = "Makbule";
                button3.Text = "Fatma";
                button4.Text = "Zübeyde";
                button5.Text = "Ayşe Fatma";
                
            }
            if (soruNo == 3)
            {
                richTextBox1.Text = "Atatürk kaç yılında ölmüştür?";
                button2.Text = "1781";
                button3.Text = "1938";
                button4.Text = "1927";
                button5.Text = "1918";
                button1.Text = "Bitir";
               
            }
            if (soruNo == 4)
            { 
                panel1.Visible = false; 
                label7.Text = cevapDogru.ToString();
                label8.Text = cevapYanlıs.ToString();
                groupBox1.Visible = true;
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

            if (soruNo == 3)
            {
                cevapDogru++;
   
            }
            if (soruNo == 1 || soruNo == 2)
            {
                cevapYanlıs++;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

            if (soruNo == 1 || soruNo==2)
            {
                cevapDogru++;
            }
            if (soruNo == 3)
            {
                cevapYanlıs++;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

            if (soruNo == 1 || soruNo == 2 || soruNo == 3)
            {
                cevapYanlıs++;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button2.Enabled=false;  
            button3.Enabled=false;  
            button4.Enabled=false;
            button5.Enabled=false;

            if (soruNo == 1 || soruNo == 2 || soruNo==3)
            {
                cevapYanlıs++;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sure = Convert.ToInt32(label4.Text);
            sure = sure - 1;
            label4.Text = sure.ToString();
            if (sure == 0)
            {
                timer1.Enabled = false;
                cevapYanlıs--;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }
    }
}
