CREATE VIEW [dbo].[WeatherForecastView]
	AS 

WITH CTE AS (
	SELECT MAX(Id) Id, WeatherStationId
	FROM WeatherForecasts
	GROUP BY WeatherStationId
)
select WeatherForecasts.Id,
WeatherForecasts.TemperatureC,
WeatherForecasts.Summary,
WeatherForecasts.Timestamp,
Weatherstations.Address_City as WeatherStation_Address_City,
WeatherStations.Address_Street as WeatherStation_Address_Street,
WeatherStations.Id as WeatherStation_id,
WeatherStations.Name as WeatherStation_Name
FROM WeatherForecasts
INNER JOIN WeatherStations ON WeatherStations.Id = WeatherForecasts.WeatherStationId
INNER JOIN CTE ON CTE.Id = WeatherForecasts.Id
GO