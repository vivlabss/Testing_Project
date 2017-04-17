/* 동적 저장프로시저 http://blog.daum.net/turnnig-pointer/16498100 */
CREATE PROC getStockData 
	@table_id varchar(200),
	@dateStart varchar(50),
	@dateEnd varchar(50)
AS
BEGIN
	DECLARE
		@strSql nvarchar(200)
	SET @strSql = 'SELECT PRICE_CLOSE FROM ' + @table_id + ' WHERE DATE BETWEEN ' + @dateStart + ' AND ' + @dateEnd + ' ' + 'ORDER BY DATE ASC'
	EXEC sp_executesql @strSql
END

GO
DROP PROC getStockData;