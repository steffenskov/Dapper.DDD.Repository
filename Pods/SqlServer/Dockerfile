FROM mcr.microsoft.com/mssql/server:2019-CU10-ubuntu-20.04

ENV ACCEPT_EULA="Y"
ENV MSSQL_SA_PASSWORD=SqlServer2019
ENV MSSQL_PID="Express"

ENV PATH=$PATH:/opt/mssql-tools/bin


########################
# Setup Northwind Data #
########################

COPY ./instnwnd.sql /usr/share/instnwnd.sql
RUN (/opt/mssql/bin/sqlservr --accept-eula & ) \
	&& (/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SqlServer2019 -i /usr/share/instnwnd.sql) \
	&& (/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SqlServer2019 -q "use Northwind; CREATE TABLE CompositeUsers ( Id_Username VARCHAR(50) NOT NULL, Id_Password VARCHAR(50) NOT NULL, Age INT NULL, DateCreated DATETIME NOT NULL DEFAULT(GETDATE()) ); ALTER TABLE CompositeUsers ADD CONSTRAINT PK_CompositeUsers PRIMARY KEY (Id_Username, Id_Password);") \
	&& (/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SqlServer2019 -q "use Northwind; CREATE TABLE CustomersWithValueObject ( Id UNIQUEIDENTIFIER NOT NULL, Name VARCHAR(100) NOT NULL, InvoiceAddress_Zipcode INT NOT NULL, InvoiceAddress_Street VARCHAR(200) NOT NULL, DeliveryAddress_Zipcode INT NOT NULL, DeliveryAddress_Street VARCHAR(200) NOT NULL ); ALTER TABLE CustomersWithValueObject ADD CONSTRAINT PK_CustomersWithValueObject PRIMARY KEY (Id);") 
