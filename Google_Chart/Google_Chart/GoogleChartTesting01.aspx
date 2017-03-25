<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoogleChartTesting01.aspx.cs" Inherits="Google_Chart.GoogleChartTesting01" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="chart_div" style="width: 100%; height: 700px;">  
         <asp:Literal ID="ltScripts" runat="server"></asp:Literal>  
        </div>  
        <asp:GridView ID="gvData" runat="server">  
        </asp:GridView>  
        <br />  
        <br />  
    </div>
    </form>
</body>
</html>
