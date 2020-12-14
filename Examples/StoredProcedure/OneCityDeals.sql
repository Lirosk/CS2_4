USE [Northwind]
GO
/****** Object:  StoredProcedure [dbo].[OneCityDeals]    Script Date: 14.12.2020 10:31:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[OneCityDeals]
@Beginning_Date datetime, @Ending_Date datetime
as
select
Customers.CustomerID as CustomerId,
Customers.ContactName as CustomerName,
Orders.EmployeeID as EmployeeId, 
Employees.FirstName + ' ' + Employees.LastName as EmployeeName,
Customers.City, 
Products.ProductName,
Products.SupplierID as SupplierId,
Suppliers.CompanyName
from Employees 
inner join Orders on Orders.EmployeeID = Employees.EmployeeID
inner join Customers on Customers.City = Employees.City
inner join Suppliers on Suppliers.City = Employees.City
inner join Products on Products.SupplierID = Suppliers.SupplierID
where Orders.ShippedDate Between @Beginning_Date And @Ending_Date
