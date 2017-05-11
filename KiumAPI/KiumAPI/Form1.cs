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
using System.Net;

using AxKHOpenAPILib;
using KHOpenAPILib;



namespace KiumAPI
{
    public partial class Form1 : Form
    {
        // 참고자료 : http://blog.naver.com/rkdwnsdud555/220814310043

        List<string> codeList;
        List<string> companyList = new List<string>();

        // 야후 API 주가정보 관련 변수들 //
        string strUrl = "http://ichart.yahoo.com/table.csv?s=";
        string now_year = System.DateTime.Now.Year.ToString();
        string now_month = System.DateTime.Now.Month.ToString();
        string now_day = System.DateTime.Now.Day.ToString();
        string reuslt;
        int temp;

        int counter = 0;

        public Form1()
        {
            InitializeComponent();
        }


        // KiumAPI에서 데이터가 요청되고 반환된 데이터를 처리할 때 사용하는 함수
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

        #region DB 배치 실행관련 함수
        
        private void btnBatch_Click(object sender, EventArgs e)
        {
            // 상장사의 코드만 전부다 받아올 수 있다.
            //string code = axKHOpenAPI1.GetCodeListByMarket("0");
            //codeList = code.Split(';').ToList<string>();         // 1295 개
            MessageBox.Show(codeList.Count.ToString() + codeList[0]);

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

        private void btnScrap_Click(object sender, EventArgs e)
        {
            List<string> list_data = new List<string>();
            now_month = (Int32.Parse(now_month) - 1).ToString();

            string code = axKHOpenAPI1.GetCodeListByMarket("0");
            codeList = code.Split(';').ToList<string>();         // 1295 개

            try
            {
                for (int cnt = 0; cnt < codeList.Count; cnt++)
                {
                    companyList.Add(axKHOpenAPI1.GetMasterCodeName(codeList[cnt]));
                }
                //MessageBox.Show(companyList[1200]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // MessageBox.Show(companyList[1030]);

            SqlConnection conn = new SqlConnection();
            string connectionString = @"Data Source=.\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=KiumAPI";
            conn.ConnectionString = connectionString;
            conn.Open();
            MessageBox.Show("연결성공");
            for (int i = 0; i < codeList.Count; i++)
            {
                strUrl += codeList[i] + ".KS";
                strUrl += "&a=0&b=1&c=2014&d=" + now_month + "&e=" + now_day + "&f=" + now_year + "&g=d&x=.csv";
                //MessageBox.Show(strUrl);
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        reuslt = wc.DownloadString(strUrl);
                        list_data = reuslt.Split('\n').ToList<string>();
                        //MessageBox.Show(list_data.Count.ToString());
                        //MessageBox.Show(list_data[1]);
                    }
                    catch(Exception ex)
                    {
                       MessageBox.Show(ex.Message);
                        continue;
                    }

                }
                strUrl = "http://ichart.yahoo.com/table.csv?s=";

                try
                {
                    for (int j = 1; j < list_data.Count; j++)
                    {

                        string insertTable = "INSERT INTO " + companyList[i] +
                            "(Date, Price)VALUES('"
                            + list_data[j].Split(',')[0] + "','"
                            + list_data[j].Split(',')[4]
                            + "')";

                        SqlCommand insertCmd = new SqlCommand(insertTable, conn);
                        insertCmd.ExecuteNonQuery();

                        //SqlCommand cmd = new SqlCommand("GET_STOCK_DATA", conn);
                        //cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        //SqlParameter param1 = new SqlParameter("@tableName", System.Data.SqlDbType.NVarChar);
                        //param1.Value = companyList[i];
                        //cmd.Parameters.Add(param1);

                        //SqlParameter param2 = new SqlParameter("@price", System.Data.SqlDbType.NVarChar);
                        ////MessageBox.Show(Convert.ToInt16(list_data[j].Split(',')[4]).ToString());
                        //// temp = (int)float.Parse(list_data[j].Split(',')[4]);
                        //param2.Value = list_data[j].Split(',')[4];
                        //cmd.Parameters.Add(param2);


                        //SqlParameter param3 = new SqlParameter("@date", System.Data.SqlDbType.NVarChar);
                        //param3.Value = list_data[j].Split(',')[0];
                        //MessageBox.Show(param3.Value.ToString());
                        //cmd.Parameters.Add(param3);
                        //cmd.ExecuteNonQuery();
                    }
                }

                catch
                {
                    //MessageBox.Show(ex2.Message);
                    continue;
                }
            }
            // Console.WriteLine(strUrl);
            conn.Close();
        }

        #endregion
    }
}
