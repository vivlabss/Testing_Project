using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using MathNet.Numerics;

namespace Google_Chart
{
    public partial class GoogleChartTEsting02 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Bind Gridview  
                //BindGvData();

                // Bind Charts  
                BindChart();
            }
        }
        private void BindGvData()
        {
            //asp 그리드 뷰에 데이터 매핑시키기
            //gvData.DataSource = GetChartData();
            //gvData.DataBind();
        }

        private void BindChart()
        {
            DataTable dsChartData = new DataTable();
            StringBuilder strScript = new StringBuilder();
            List<double> xData = new List<double>();
            List<double> tData = new List<double>();
            List<double> LRData = new List<double>();

            dsChartData = GetChartData();

            LinearRegression(dsChartData, xData, tData, LRData);

            //data.addColumn('number', 'LR');
            //strScript.Append("data.setCell(" + (i - 1) + "," + 2 + "," + LRData[i] + ") ;");
            List<double> testData = new List<double>();
            for (int i =0; i < dsChartData.Rows.Count ; i++)
            {
                testData.Add(85000 + i);
            }

            try
            {
                //다중 차트 그리기
                #region 구글차트 스크립트
                strScript.Append(@"<script type='text/javascript'> 
                google.load( 'visualization', '1', {'packages':['corechart']});
                google.setOnLoadCallback(drawChart);
                function drawChart() {
                var data = new google.visualization.DataTable();
                data.addColumn('string', 'Date');
                data.addColumn('number', 'Price');
                data.addColumn('number', 'LR');
              
                data.addRows(" + (dsChartData.Rows.Count - 1) + ");");

                for (int i = 1; i < dsChartData.Rows.Count; i++)
                {
                    strScript.Append("data.setCell( " + (i - 1) + "," + 0 + ",'" + dsChartData.Rows[i]["DATE"] + "');");
                    strScript.Append("data.setCell(" + (i - 1) + "," + 1 + "," + dsChartData.Rows[i]["PRICE_CLOSE"] + ") ;");
                    strScript.Append("data.setCell(" + (i - 1) + "," + 2 + "," + LRData[dsChartData.Rows.Count - i] + ") ;");

                }
                strScript.Append("data.sort({ column: 0, asc: true});");
                strScript.Append(
                    " var options = {" +
                        "title : 'SK Innovation Stock Price'," +
                        "legend: { position: 'bottom'}," +
                        "series: { " +
                            "0 : { color: 'rgb(220,90,90)'}, " + "1 : { color:'rgb(120,50,50)'}" +
                                "}" +
                    "};");
                strScript.Append(" var chart = new google.visualization.LineChart(document.getElementById('chart_div'));");
                strScript.Append(" chart.draw(data, options);}");
                strScript.Append("$(window).resize(function(){drawChart();});");
                strScript.Append("</script>");
                ltScripts.Text = strScript.ToString(); //ASP 리터럴 테그에 위의 자바스크릅트 삽입 !!    
                #endregion
            }
            catch
            {
            }
            finally
            {
                dsChartData.Dispose();
                strScript.Clear();
            }
        }
        // 선형회귀
        private static void LinearRegression(DataTable dsChartData, List<double> xData, List<double> tData, List<double> LRData)
        {
            for (int i = 1; i < dsChartData.Rows.Count; i++)
            {
                xData.Add(double.Parse(dsChartData.Rows[i]["PRICE_CLOSE"].ToString()));
                tData.Add(i);
            }
            xData.Sort();
            double[] xxData = xData.ToArray();
            double[] ttData = tData.ToArray();
            Tuple<double, double> p = Fit.Line(xxData, ttData);
            double intercept = p.Item1;
            double slope = p.Item2;
            for (int i = 0; i < dsChartData.Rows.Count; i++)
            {
                LRData.Add(i * slope + intercept);
            }
        }

        private DataTable GetChartData()
        {
            DataSet dsData = new DataSet();
            try
            {
                //저장 프로시저를 통한 데이터 불러오기 그래야 빠르다.
                SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
                SqlDataAdapter sqlCmd = new SqlDataAdapter("Get_Data", sqlCon);
                sqlCmd.SelectCommand.CommandType = CommandType.StoredProcedure;

                sqlCon.Open();

                sqlCmd.Fill(dsData);

                sqlCon.Close();
            }
            catch
            {
                throw;
            }
            return dsData.Tables[0]; // DB 조회 내용을 테이블 형식으로 리턴해준다.           
        }
    }
}