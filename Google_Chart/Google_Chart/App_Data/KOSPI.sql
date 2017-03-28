CREATE TABLE KOSPI(
	[ID]          VARCHAR (10) NOT NULL,
    [DATE]        DATETIME     NOT NULL,
    [PRICE_CLOSE] INT          NOT NULL,
    [VOLUME]      INT          NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

select * from KOSPI order by DATE desc;

/*
 SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source =.\SQLEXPRESS; Initial Catalog = Stock_Data; Integrated Security = True";
            conn.Open();

            now_month = (Int32.Parse(now_month) - 1).ToString();
            aMonth = (Int32.Parse(now_month) - 1).ToString();
            bDay = (Int32.Parse(now_day) - 1).ToString();
            strUrl += "%5EKS11";
            strUrl += "&a=" + 0 + "&b=" + 1 + "&c=" + 2010 + "&d=" + now_month + "&e=" + bDay + "&f=" + now_year + "g=d&ignore=.csv";
            Console.WriteLine(strUrl + '\n');
            using (WebClient wc = new WebClient())
            {
                try
                {
                    reuslt = wc.DownloadString(strUrl);
                    list_data = reuslt.Split('\n').ToList<string>();
                    for(int i =0; i<list_data.Count; i++)
                    {
                        //Console.WriteLine(list_data[i]);
                    }
                }
                catch
                {
                    Console.WriteLine("데이터 다운 에러");
                }
            }
             
            for (int j = 1; j < list_data.Count; j++)
            {
                        string insertTable = "INSERT INTO KOSPI" +
                        "(ID, DATE, PRICE_CLOSE, VOLUME)VALUES("
                        + (j) + ",'"
                        + list_data[j].Split(',')[0] + "',"
                        + list_data[j].Split(',')[4] + ","
                        + list_data[j].Split(',')[5] + ")";
                        Console.WriteLine(insertTable);
                        SqlCommand insertCmd = new SqlCommand(insertTable, conn);
                        insertCmd.ExecuteNonQuery();
            }                                           
*/