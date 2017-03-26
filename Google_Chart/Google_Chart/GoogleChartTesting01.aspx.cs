using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Google_Chart
{
    public partial class GoogleChartTesting01 : System.Web.UI.Page
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
            dsChartData = GetChartData();

            try
            {
                //dsChartData = GetChartData();

                strScript.Append(@"<script type='text/javascript'> 
                google.load( 'visualization', '1', {'packages':['corechart']});
                google.setOnLoadCallback(drawChart);
                function drawChart() {
                var data = new google.visualization.DataTable();
                data.addColumn('string', 'Date');
                data.addColumn('number', 'Price');
              
                data.addRows(" + (dsChartData.Rows.Count - 1) + ");");

                for (int i = 1; i < dsChartData.Rows.Count; i++)
                {
                    strScript.Append("data.setCell( " + (i - 1) + "," + 0 + ",'" + dsChartData.Rows[i]["DATE"] + "');");
                    strScript.Append("data.setCell(" + (i - 1) + "," + 1 + "," + dsChartData.Rows[i]["PRICE_CLOSE"] + ") ;");

                }
                strScript.Append("data.sort({ column: 0, asc: true});");
                strScript.Append(" var options = {title : 'SK Innovation Stock Price', legend: { position: 'bottom'}, series: { 0 : { color: 'rgb(220,90,90)'} }};");
                strScript.Append(" var chart = new google.visualization.LineChart(document.getElementById('chart_div'));");
                strScript.Append(" chart.draw(data, options);}");
                strScript.Append("</script>");
                ltScripts.Text = strScript.ToString(); //ASP 리터럴 테그에 위의 자바스크릅트 삽입 !!              
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
        
    }
}