
----Tables Creation------
CREATE TABLE dbo.user_info ---Customer table
(
Sno INT IDENTITY(1,1),
Userid VARCHAR(10),
[Password] NVARCHAR(20),
[Name] VARCHAR(50),
Mobile BIGINT,
Email NVARCHAR(50),
[Address] VARCHAR(1000),
City varchar(20),
Pincode BIGINT,
UserType VARCHAR(10),
[Status] VARCHAR(10),
Created_date DATETIME
)
-------
CREATE TABLE dbo.User_Administrator -- Admin and sub admins master table
(
Sno INT IDENTITY(1,1),
ID varchar(10),
[Password] NVARCHAR(20),
[Name] VARCHAR(50),
Email NVARCHAR(50),
UserType VARCHAR(10),
[Status] VARCHAR(10),
Created_date DATETIME
)
-------------------------------
Create TABLE dbo.Ordermangement ---Ordermaster table
(
Sno INT IDENTITY(1,1),
Order_ID BIGINT,
ProductID INT,
ProductName VARCHAR(50),
Quantity INT,
OrderStatus VARCHAR(10),
CustomerId varchar(10),
CustomerName VARCHAR(50),
ShippingAddress NVARCHAR(1000),
Mobile BIGINT,
Barcode nvarchar(200),
[Status] VARCHAR(10),
Created_date DATETIME 
)

-----------------------
Create TABLE dbo.Products --Products table
(
Sno INT IDENTITY(1,1),
ProductID INT,
ProductName VARCHAR(50),
Height VARCHAR(10),
[Weight] VARCHAR(10),
[Image] NVARCHAR(50),
[Status] VARCHAR(10),
Created_date DATETIME 

)
--------------------
--Store Procedures--
--------------------
--In this Store Procedure we have add,update,delete orderdetails---
CREATE PROCEDURE dbo.Order_Transactions
(
@sno VARCHAR(20),
@Order_ID VARCHAR(20),
@ProductID VARCHAR(20),
@ProductName VARCHAR(100),
@Quantity VARCHAR(20),
@OrderStatus VARCHAR(100),
@CustomerId VARCHAR(10),
@CustomerName VARCHAR(50),
@ShippingAddress NVARCHAR(1000),
@Mobile VARCHAR(10),
@QRcode NVARCHAR(200),
@Condition VARCHAR(20)


)
AS
BEGIN
	if (@Condition='AddOrders')                                                
		Begin  
			DECLARE @Random INT                                                 
			DECLARE @Upper INT                                                 
			DECLARE @Lower INT   
			SET @Lower = 99999                                                
			SET @Upper = 99999999                                             
			SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0)                                                
 
			;WITH cte AS (SELECT VALUE,ROW_NUMBER()OVER(ORDER BY (SELECT 0)) ROW FROM dbo.ufn_Split(@ProductID,',')),                                                 
			cte1 AS (SELECT VALUE,ROW_NUMBER()OVER(ORDER BY (SELECT 0)) ROW FROM dbo.ufn_Split(@ProductName,',')),                                                 
			cte2 AS (SELECT VALUE,ROW_NUMBER()OVER(ORDER BY (SELECT 0)) ROW FROM dbo.ufn_Split(@Quantity,',')),                                                 
			cte3 AS (SELECT VALUE,ROW_NUMBER()OVER(ORDER BY (SELECT 0)) ROW FROM dbo.ufn_Split(@OrderStatus,','))                                      
                                               
			INSERT INTO Ordermangement(Order_ID,ProductID,ProductName,Quantity,OrderStatus,CustomerId,CustomerName,ShippingAddress,                                                
			Mobile,Status,Created_date)                                                
				SELECT @Random,c.VALUE ,c1.VALUE ,c2.VALUE , c3.VALUE,@CustomerId,@CustomerName,@ShippingAddress,@Mobile,
				'Active',GETUTCDATE()                                                 
				 FROM cte c inner join cte1 c1 ON c.ROW=c1.ROW                                                 
				 INNER JOIN cte2 c2 ON c1.ROW=c2.ROW                                  
				 LEFT OUTER JOIN  cte3 c3 ON c2.ROW=c3.ROW    
					SELECT 'True' Result,o.*,p.Height,p.Weight,p.Image from Ordermangement o
					INNER JOIN Products p ON o.productid=p.ProductID
					WHERE  o.CustomerId= @CustomerId and   o.Order_ID=@Random 
			END
			IF (@Condition='GenQRcode')                                                
			BEGIN
				IF EXISTS(SELECT * FROM Ordermangement WHERE Order_ID=@Order_ID)
				BEGIN
					UPDATE Ordermangement SET Barcode=@QRcode WHERE Order_ID=@Order_ID
					SELECT 'True' Result, o.*,p.Height,p.[Weight],p.[Image] FROM Ordermangement o
					INNER JOIN Products p ON o.productid=p.ProductID
					WHERE  o.CustomerId= @CustomerId AND   o.Order_ID=@Order_ID
				END
				ELSE
					SELECT 'False' Result
   		    END
			IF (@Condition='UpdateOrders')                                                
			BEGIN
				IF EXISTS(SELECT * FROM Ordermangement WHERE Order_ID=@Order_ID AND ProductID=@ProductID)
				BEGIN
					UPDATE Ordermangement SET OrderStatus= @OrderStatus WHERE Order_ID=@Order_ID AND ProductID=@ProductID
					SELECT 'True' Result, o.*,p.Height,p.Weight,p.Image from Ordermangement o
					INNER JOIN Products p ON o.productid=p.ProductID
					WHERE o.Order_ID=@Order_ID AND o.ProductID=@ProductID
				END
				ELSE
				BEGIN
					SELECT 'False' Result, 'Updation Failed'[Outputmsg]
				END
		   END
		   IF (@Condition='DeleteOrders')                                                
		   BEGIN
				IF EXISTS(SELECT * FROM Ordermangement WHERE Order_ID=@Order_ID AND ProductID=@ProductID)
				BEGIN
					DELETE FROM Ordermangement WHERE Order_ID=@Order_ID AND ProductID=@ProductID
					SELECT 'True' Result,'Order Deleted sucessfully'[Outputmsg]
				END
				ELSE
				BEGIN
					SELECT 'False' Result, 'File Not found'[Outputmsg]
				END

		  END
END
-----------------
--usermanagement procedure---

--Based on login data shows like user and admin

CREATE PROCEDURE dbo.login_Master
(
@Loginid VARCHAR(20),
@Password VARCHAR(20),
@usertype VARCHAR(10)
)
AS
BEGIN
	IF EXISTS(SELECT * FROM user_info WHERE Userid=@Loginid and [Password]=@Password and 
	UserType=@usertype)
	BEGIN
		SELECT 'True' Result, o.*,p.Height,p.Weight,p.Image FROM Ordermangement o
		INNER JOIN Products p on o.productid=p.ProductID WHERE CustomerId= @Loginid ORDER BY 1 DESC
	END
	ELSE IF EXISTS(SELECT * FROM User_Administrator WHERE ID=@Loginid AND [Password]=@Password and 
	UserType=@usertype)
	BEGIN
		SELECT 'True' Result, o.*,p.Height,p.Weight,p.Image from Ordermangement o
		INNER JOIN Products p on o.productid=p.ProductID  ORDER BY 1 DESC
	END
	ELSE
		SELECT 'False' Result
END




