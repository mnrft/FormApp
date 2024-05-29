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
    public partial class PizzaHome : Form
    {
        public PizzaHome()
        {
            InitializeComponent();
        }
        string secim1,secim2,secim3; //label atanarak da yapılabilir.

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            secim1 ="sucuk";
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            secim2 = "kaşar";
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            secim3 = "zeytin";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();   
            comboBox1.Text = ""; //comboboxta clear yok
            secim1 = "";
            secim2 = "";
            secim3 = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adres.Items.Add(textBox3.Text);
            içecek.Items.Add(comboBox2.Text);
            malzeme.Items.Add(secim1 + secim2 + secim3);
            secim1 = "";
            secim2 = "";
            secim3 = "";

        }
        private void PizzaHome_Load(object sender, EventArgs e)
        {

        }
        
    }
}
