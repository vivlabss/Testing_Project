using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace MachineLearning_Test
{
    class Stock_Data_Batch
    {
        static List<string> codes = new List<string>();
        static List<string> companies = new List<string>();
        static List<string> fullCodes = new List<string>();
        static List<string> list_data = new List<string>();
        static int flag = 0;

        static string strUrl = "http://ichart.yahoo.com/table.csv?s=";
        static string now_year = System.DateTime.Now.Year.ToString();
        static string now_month = System.DateTime.Now.Month.ToString();
        static string now_day = System.DateTime.Now.Day.ToString();
        static string aMonth = "";
        static string bDay = "";
        static string cYear = "";
        static string reuslt;

        static void Main(string[] args)
        {
            parseCodeHtml(downloadCode("1")); // 종목코드와 회사명 받아오기

            #region 코드정보 입력
            //while (flag != 1)
            //{
            //    input_code = Console.ReadLine();
            //    if (codes.Contains(input_code) == false)
            //    {
            //        Console.WriteLine("종목정보가 존재하지 않습니다. 다시 입력해주세요.");
            //    }
            //    else
            //    {                   
            //        flag = 1;
            //    }
            //}
            //flag = 0;
            #endregion

            #region 날짜입력 및 처리
            Console.WriteLine("받아오고 싶은 과거의 날짜를 입력 \nex) 2010(년)\n5(월)\n1(일) 부터 현재까지");
            Console.WriteLine("년도를 입력하세요");
            while (flag != 1)
            {
                cYear = Console.ReadLine();
                if (cYear.Length > 4)
                {
                    Console.WriteLine("4자리 수로 입력해주세요.");
                }
                else if (Int32.Parse(cYear) < Int32.Parse(System.DateTime.Now.Year.ToString()) - 10)
                {
                    Console.WriteLine("10년 범위만 지원합니다.");
                }
                else
                {
                    flag = 1;
                }
            }
            flag = 0;
            Console.WriteLine("월을 입력하세요 ex) 1월은 1, 10월은 10");
            while (flag != 1)
            {
                aMonth = Console.ReadLine();
                if (aMonth.Length > 2)
                {
                    Console.WriteLine("잘못 입력하셨습니다.");
                }
                else if (Int32.Parse(aMonth) - 1 > 11 || (Int32.Parse(aMonth) - 1 < 0))
                {
                    Console.WriteLine("잘못 입력하셨습니다.");
                }
                else
                {
                    flag = 1;
                }
            }
            flag = 0;
            Console.WriteLine("일을 입력하세요 ex) 1일은 1, 10일은 10");
            while (flag != 1)
            {
                bDay = Console.ReadLine();
                if (bDay.Length > 2)
                {
                    Console.WriteLine("잘못 입력하셨습니다.");
                }
                else if (Int32.Parse(bDay) < 0 || Int32.Parse(bDay) > 31)
                {
                    Console.WriteLine("잘못 입력하셨습니다.");
                }
                else
                {
                    flag = 1;
                }
            }
            Console.WriteLine("입력하신 날짜는 {0}년 {1}월 {2}일 입니다.", cYear, aMonth, bDay);
            aMonth = (Int32.Parse(aMonth) - 1).ToString();
            now_month = (Int32.Parse(now_month) - 1).ToString();
            #endregion

            Console.WriteLine(aMonth + " " + bDay + " " + cYear);
            Console.WriteLine("종목갯수 : " + codes.Count + "\n" + companies.Count + "\n" + fullCodes.Count);

            //downloadStockData(input_code);           
            //dbWriter();
            //dataWriter(aMonth, bDay, cYear);
            updateData();
            //Console.WriteLine(companies.IndexOf("희성전선1"));
        }
        #region 종목코드와 회사명 받아오기
        // 주식종목코드 받아오기. MarketType (1=코스피 , 2=코스닥)
        static string downloadCode(string market_type)
        {
            string url = "http://datamall.koscom.co.kr/servlet/infoService/SearchIssue";
            using (WebClient client = new WebClient())
            {
                //한글깨짐 인코딩 작업
                Encoding encKr = Encoding.GetEncoding("euc-kr");
                EncodingInfo[] encods = Encoding.GetEncodings();
                Encoding destEnc = Encoding.UTF8;

                byte[] response =
                client.UploadValues(url, new NameValueCollection()
                {
                   { "flag","SEARCH" },
                   { "marketDisabled" , "null" },
                   { "marketBit", market_type }
                });
                response = Encoding.Convert(encKr, destEnc, response); //최종 인코딩
                string result = System.Text.Encoding.UTF8.GetString(response); //마크업 스트링 변환                           
                return result;
            }
        }
        static void parseCodeHtml(string html)
        {
            Regex firstRegex = new Regex("<option value=\"KR.*\">.*?</option>");
            Match firstMatch = firstRegex.Match(html);
            MatchCollection firstCollection = firstRegex.Matches(html);
            foreach (Match fc in firstCollection)
            {
                Regex secondRegex = new Regex("<.*?>");
                Regex lastRegex = new Regex(@"<option value=" + "\"" + "|" + "\"" + ">.*?</option>");
                string sr = secondRegex.Replace(fc.Value, "");
                string lr = lastRegex.Replace(fc.Value, "");
                string tr = Regex.Replace(sr, @"\D", ""); // 코드 분류 완료
                sr = Regex.Replace(sr, @"\(.*?\)", ""); // 회사이름 분류 완료               
                if (tr.Length >= 7)
                {
                    tr = tr.Substring(0, 6);
                }
                codes.Add(tr); // 코드 리스트에 삽입
                companies.Add(sr); // 회사이름 리스트에 삽입
                fullCodes.Add(lr); //회사 풀코드 리스트에 삽입

            }
            //StreamWriter sw = new StreamWriter(@"C:\Users\youli\Desktop\code1.txt");
            //for (int i = 0; i < codes.Count; i++)
            //{
            //    sw.Write(codes[i] + " " + fullCodes[i] + " " + companies[i] + "\n");
            //}
            //sw.Close();

        }
        #endregion

        #region 주가데이터 받아오기
        static void downloadStockData(string code)
        {
            #region 입력받은 코드와 날짜를 바탕으로 주가데이터 다운로드
            strUrl += code + ".KS";
            strUrl += "&a=" + aMonth + "&b=" + bDay + "&c=" + cYear + "&d=" + now_month + "&e=" + now_day + "&f=" + now_year + "g=d&x=.csv";

            using (WebClient wc = new WebClient())
            {
                reuslt = wc.DownloadString(strUrl);
                list_data = reuslt.Split('\n').ToList<string>();
            }
            #endregion

        }
        #endregion

        #region 주가정보 테이블 동적생성
        static void dbWriter()
        {
            SqlConnection conn = new SqlConnection();
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Stock_Data;Integrated Security=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            conn.ConnectionString = connectionString;
            conn.Open();

            for (int i = 0; i < companies.Count; i++)
            {
                string createTable = "CREATE TABLE dbo." + companies[i] +
              "( ID varchar(10) NOT NULL PRIMARY KEY," +
              "DATE datetime NOT NULL," +
              "PRICE_CLOSE integer NOT NULL," +
              "VOLUME integer)";
                SqlCommand createCmd = new SqlCommand(createTable, conn);
                try
                {
                    createCmd.ExecuteNonQuery();
                }
                catch (SqlException)
                {

                }
            }
            conn.Close();
        }
        #endregion

        #region 생성한 주가정보 테이블에 데이터를 동적으로 입력
        static void dataWriter(string aMonth, string bDay, string cYear)
        {
            SqlConnection conn = new SqlConnection();
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Stock_Data;Integrated Security=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            conn.ConnectionString = connectionString;
            conn.Open();
            for (int i = 0; i < codes.Count; i++)
            {
                //문제점은 이곳 strUrl의 코드값이 제대로 들어가고 있지 않다.... 왜이러지?
                strUrl += codes[i] + ".KS";
                strUrl += "&a=" + aMonth + "&b=" + bDay + "&c=" + cYear + "&d=" + now_month + "&e=" + now_day + "&f=" + now_year + "g=d&x=.csv";
                Console.WriteLine(strUrl + '\n');
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        reuslt = wc.DownloadString(strUrl);
                        list_data = reuslt.Split('\n').ToList<string>();
                    }
                    catch
                    {
                        Console.WriteLine("데이터 다운 에러");
                    }

                }
                strUrl = "http://ichart.yahoo.com/table.csv?s=";
                try
                {
                    for (int j = 1; j < list_data.Count; j++)
                    {
                        string insertTable = "INSERT INTO " + companies[i] +
                        "(ID, DATE, PRICE_CLOSE, VOLUME)VALUES("
                        + (j) + ",'"
                        + list_data[j].Split(',')[0] + "',"
                        + list_data[j].Split(',')[4] + ","
                        + list_data[j].Split(',')[5] + ")";
                        //Console.WriteLine(insertTable);
                        SqlCommand insertCmd = new SqlCommand(insertTable, conn);
                        insertCmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("데이터 삽입 실패");
                    continue;
                }
            }
            // Console.WriteLine(strUrl);
            conn.Close();
        }
        #endregion

        #region 동적 주가정보 갱신
        static void updateData()
        {
            SqlConnection conn = new SqlConnection();
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Stock_Data;Integrated Security=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            conn.ConnectionString = connectionString;
            conn.Open();

            string selectTable = "SELECT MAX(DATE) FROM 희성전선1";
            SqlCommand selectCmd = new SqlCommand(selectTable, conn);
            SqlDataReader dr = selectCmd.ExecuteReader();
            string pandan_date = dr.Read().ToString();
            if (DateTime.Parse(dr[0].ToString()) < DateTime.Now)
            {
                for (int i = 0; i < companies.Count; i++)
                {
                    try
                    {
                        string updateTable = "UPDATE " + companies[i] + " "
                        + "SET "
                        + "ID = ID + 1";
                        SqlCommand insertCmd = new SqlCommand(updateTable, conn);
                        insertCmd.ExecuteNonQuery();
                        //현재 날짜의 데이터를 불러오고 저장해야함
                        string insertTable = "INSERT " + companies[i] + " ";

                    }
                    catch
                    {
                    }
                }
            }
        }
        #endregion
    }
}
