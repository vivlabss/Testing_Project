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
using Accord.MachineLearning;
using Accord.Statistics;
using Accord.Statistics.Models.Regression.Linear;

namespace Google_Chart
{
	public partial class LinearRegression02 : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindChart();
            }
        }

        private void BindChart()
        {
            DataTable dsChartData = new DataTable();
            DataTable dsChartData2 = new DataTable();
            StringBuilder strScript = new StringBuilder();
            double[] LR;

            dsChartData = GetChartData();
            dsChartData2 = GetChartData2();

            LR = LinearRegression(dsChartData, dsChartData2);

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
                data.addColumn('number', 'KOSPI');
                data.addColumn('number', 'LR');
              
                data.addRows(" + (dsChartData2.Rows.Count - 1) + ");");

                for (int i = 1; i < dsChartData2.Rows.Count; i++)
                {
                    strScript.Append("data.setCell( " + (i - 1) + "," + 0 + ",'" + dsChartData.Rows[i]["DATE"] + "');");
                    strScript.Append("data.setCell(" + (i - 1) + "," + 1 + "," + dsChartData.Rows[i]["PRICE_CLOSE"] + ") ;");
                    strScript.Append("data.setCell(" + (i - 1) + "," + 2 + "," + dsChartData2.Rows[i]["PRICE_CLOSE"] + ") ;");
                    strScript.Append("data.setCell(" + (i - 1) + "," + 3 + "," + LR[i - 1] + ") ;");

                }
                strScript.Append("data.sort({ column: 0, asc: true});");
                strScript.Append(
                    " var options = {" +             
                        "vAxes : {0: {viewWindow : {max : 3200000, min: 100000}} , 1: {viewWindow : {max :270000, min : 60000}} }, "+
                        "title : 'Stock Price'," +
                        "legend: { position: 'bottom'}," +
                        "series: { " +
                            "0 : {targetAxisIndex:1}," + "1 : {targetAxisIndex:1}," + "2 : {targetAxisIndex:0}" +
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
                SqlDataAdapter sqlCmd = new SqlDataAdapter("Get_Data_KOSPI", sqlCon);
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

        //회귀 데이터가 매우 이상하다. 검증할 필요가 있다.
        private double[] LinearRegression(DataTable dsChartData, DataTable dsChartData2)
        {
            double[] inputs = new double[dsChartData2.Rows.Count - 1];
            double[] outputs = new double[dsChartData2.Rows.Count - 1];
            double[] result = new double[dsChartData2.Rows.Count - 1];

            for (int i = 1; i < dsChartData2.Rows.Count; i++)
            {
                inputs[i - 1] = double.Parse(dsChartData2.Rows[i]["PRICE_CLOSE"].ToString());
                outputs[i - 1] = double.Parse(dsChartData.Rows[i]["PRICE_CLOSE"].ToString());
            }

            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
            SimpleLinearRegression lr = ols.Learn(inputs, outputs);

            for (int i = 0; i < dsChartData2.Rows.Count - 1; i++)
            {
                result[i] = lr.Transform(outputs[i]);
            }
            return result;
        }
    }
}