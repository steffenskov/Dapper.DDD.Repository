/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
INSERT INTO WeatherStations(Name, Address_City, Address_Street)
values ('New York station', 'New York', 'Some road #123'),
('London station', 'London', 'Another road #42'),
('Big Apple weather', 'Manhattan', 'Third road')

GO

declare @counter int = 0
while @counter < 5 
begin

INSERT INTO WeatherForecasts(WeatherStationId, TemperatureC, Summary)
SELECT Id, 
CAST(RAND()*50 AS int),
Id % 3
FROM WeatherStations
SET @counter = @counter +1
waitfor delay '00:00:02'
END
GO