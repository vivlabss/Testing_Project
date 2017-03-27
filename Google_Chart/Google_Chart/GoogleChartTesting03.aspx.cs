using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

using Accord;
using Accord.Statistics;

namespace Google_Chart
{
    public partial class GoogleChartTesting03 : System.Web.UI.Page
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

        private void BindChart()
        {
            DataTable dsChartData = new DataTable();
            DataTable dsChartData2 = new DataTable();
            StringBuilder strScript = new StringBuilder();

            dsChartData = GetChartData();
            dsChartData2 = GetChartData2();

            try
            {
                #region 구글차트 스크립트
                strScript.Append(@"<script type='text/javascript'> 
                google.load( 'visualization', '1', {'packages':['corechart']});
                google.setOnLoadCallback(drawChart);
                function drawChart() {
                var data = new google.visualization.DataTable();
                data.addColumn('string', 'Date');
                data.addColumn('number', 'SK_Price');
                data.addColumn('number', 'LG-화학');
              
                data.addRows(" + (dsChartData.Rows.Count - 1) + ");");

                for (int i = 1; i < dsChartData.Rows.Count; i++)
                {
                    strScript.Append("data.setCell( " + (i - 1) + "," + 0 + ",'" + dsChartData.Rows[i]["DATE"] + "');");
                    strScript.Append("data.setCell(" + (i - 1) + "," + 1 + "," + dsChartData.Rows[i]["PRICE_CLOSE"] + ") ;");
                    strScript.Append("data.setCell(" + (i - 1) + "," + 2 + "," + dsChartData2.Rows[i]["PRICE_CLOSE"] + ") ;");

                }
                strScript.Append("data.sort({ column: 0, asc: true});");
                strScript.Append(
                    " var options = {" +
                        "title : 'Stock Price'," +
                        "legend: { position: 'bottom'}," +
                        "series: { " +
                            "0 : { color: 'rgb(220,90,90)'}," +"1 : {color: 'rgb(200,120,40)'}" +
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

        private DataTable GetChartData2() //새로운 저장프로시저 정의
        {
            DataSet dsData = new DataSet();
            try
            {
                SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
                SqlDataAdapter sqlCmd = new SqlDataAdapter("Get_Data_LGCH", sqlCon);
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

        //상관관계
        private double[,] Calc_correlation(DataTable dsChartData, DataTable dsChartData2)
        {
            double[,] datazip = new double[2,dsChartData.Rows.Count-1];
            double[,] result = new double[2, dsChartData.Rows.Count-1];
            for(int i = 0; i < 2; i++)
            {               
                for(int j = 0; j < dsChartData2.Rows.Count-1; j++)
                {
                    if(j== 1838)
                    {
                        break;
                    }
                    if(i == 0)
                    {
                        datazip[i,j] = double.Parse(dsChartData.Rows[j+1]["PRICE_CLOSE"].ToString());
                    }
                    else if ( i == 1)
                    {
                        datazip[i, j] = double.Parse(dsChartData2.Rows[j+1]["PRICE_CLOSE"].ToString());
                    }
                }
            }

                for(int j = 0; j < dsChartData.Rows.Count-1; j++)
                {
                    Response.Write(datazip[0,j] + " " + datazip[1,j] + "<br />" );
                }

            
            result = Measures.Correlation(datazip);
            for(int j = 0; j < dsChartData.Rows.Count-1; j++)
            {
                Response.Write(result[0,j] + " " + result[1,j] + "<br />");
            }
            return result;
            
        }
    }
}
