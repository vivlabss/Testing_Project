﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KiumAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            long Result;
            Result = axKHOpenAPI1.CommConnect();
            if (Result != 0)
            {
                MessageBox.Show("Login창 열림 Fail");
            }
        }
    }
}
