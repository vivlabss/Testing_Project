using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxKHOpenAPILib;
using KHOpenAPILib;

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
            int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);
            string data1 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목코드");
            string data2 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, 0, "종목명");
            ListViewItem li = new ListViewItem(new string[] { data1, data2 });
            listView1.Items.Add(li);
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

        private void button2_Click(object sender, EventArgs e)
        {
            axKHOpenAPI1.SetInputValue("종목코드", "000050");
            int nRet = axKHOpenAPI1.CommRqData("주식기본정보", "OPT10001", 0, "1001");
                   
        }
    }
}
