using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NMlib64;

namespace TestTrainer64
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            NeutronMemoryLibrary.IslemEkle("İŞLEM İSMİ");
            if (NeutronMemoryLibrary.ProcessRunning)
            {
                label1.Text = "İşlem çalışıyor.";
            }
            else
            {
                label1.Text = "İşlem çalışmıyor.";
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            NeutronMemoryLibrary.OpcodeKullan(0x18B59E, "90 90 90 90");
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            NeutronMemoryLibrary.PointerKullan(0x3B4FA0, 0xB10, 0x180, 0x219, 1);
        }
    }
}
