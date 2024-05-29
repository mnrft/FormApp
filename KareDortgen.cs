using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pizzacı
{
    public partial class KareDortgen : Form
    {
        public KareDortgen()
        {
            InitializeComponent();
        }

        private void btnkare_Click(object sender, EventArgs e)
        {
            textBox1.Clear(); //işlemler arası geçiş yapıldığında textboxları, labelları da temizleyebiliriz.

            textBox2.Clear();   
            panel1.Visible = true;
            label1.Text = "KARE";
            label6.Text = "Kenar uzunluğu:";
            textBox2.Visible = false;
            label7.Visible = false;
            btnhesaplad.Visible = false;
            btnhesaplak.Visible = true;
        }

        private void btndortgen_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            panel1.Visible = true;
            label1.Text = "DİKDÖRTGEN";
            label6.Text = "Kısa kenarı gir:";
            textBox2.Visible = true;
            label7.Visible = true;
            btnhesaplak.Visible = false;
            btnhesaplad.Visible = true;
        }

        private void KareDortgen_Load(object sender, EventArgs e)
        {
            //form yüklendiği zaman panel visible olur.
            panel1.Visible = false;
        }


        private void btnhesaplad_Click(object sender, EventArgs e)
        {
            int kisakenar, uzunkenar, alan, cevre;
            kisakenar = Convert.ToInt32(textBox1.Text);
            uzunkenar = Convert.ToInt32(textBox2.Text);
            alan = kisakenar * uzunkenar;
            cevre = 2 * (uzunkenar + kisakenar);
            label4.Text = alan.ToString();
            label5.Text = cevre.ToString();

        }

        private void btnhesaplak_Click_1(object sender, EventArgs e)
        {
            int kenar, alan, cevre;
            kenar = Convert.ToInt32(textBox1.Text);
            alan = kenar * kenar;
            cevre = kenar * 4;
            label4.Text = alan.ToString();
            label5.Text = cevre.ToString();
        }


    }
}
