using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

using AxKHOpenAPILib;
using KHOpenAPILib;



namespace KiumAPI
{
    public partial class Form1 : Form
    {
        // 참고자료 : http://blog.naver.com/rkdwnsdud555/220814310043

        List<string> codeList;
        List<string> companyList = new List<string>();
        int counter = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void axKHOpenAPI1_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if(e.sRQName == "주식기본정보") { 
                int nCnt = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRQName);
                for(int nIdx = 0; nIdx < nCnt; nIdx++)
                {
                    string data1 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nIdx, "종목명");
                    string data2 = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRQName, nIdx, "종목코드");
                    ListViewItem li = new ListViewItem(new string[] { data1, data2 });
                    listView1.Items.Add(li);
                }
            }
            for(int cnt = 0; cnt < codeList.Count; cnt++)
            {
               
            }

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

        private void button3_Click(object sender, EventArgs e)
        {
           // 상장사의 코드만 전부다 받아올 수 있다.
           string code = axKHOpenAPI1.GetCodeListByMarket("0");
           codeList = code.Split(';').ToList<string>();         // 1295 개
           MessageBox.Show(codeList.Count.ToString() + codeList[0]); 
           //axKHOpenAPI1.SetInputValue("종목코드", codeList[cnt]);
           int nRet = axKHOpenAPI1.CommKwRqData(code, 0, 100, 0, "주식기본정보", "1001");

            //MessageBox.Show(companyList.Count.ToString());
            try
            {
                for (int cnt = 0; cnt < codeList.Count; cnt++)
                {
                    companyList.Add(axKHOpenAPI1.GetMasterCodeName(codeList[cnt])); 
                }
                MessageBox.Show(companyList[1200]);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show(companyList[1030]);

        }

        private void btnBatch_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = @"Data Source=.\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=KiumAPI";
                con.Open();

                    for (int cnt = 0; cnt < companyList.Count; cnt++)
                    {
                        try { 
                        SqlCommand cmd = new SqlCommand("CREATE_STOCK_TABLE", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter("@NAME", System.Data.SqlDbType.NVarChar);
                        param.Value = "\"" + companyList[cnt] + "\"";
                        cmd.Parameters.Add(param);
                        cmd.ExecuteNonQuery();
                        counter++;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message + "\n counter :" + counter);
                        }
                       
                    }
                
            }           
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("\n counter :" + counter);


        }
    }
}
