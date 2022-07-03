CREATE TABLE [dbo].[WeatherForecasts]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1),
	[WeatherStationId] INT NOT NULL,
	TemperatureC int NOT NULL,
	Summary tinyint NOT NULL,
	[Timestamp] datetime2(2) NOT NULL DEFAULT(getutcdate()),
	CONSTRAINT PK_WeatherForecasts PRIMARY KEY ([Id]),
	CONSTRAINT FK_WeatherForecasts_WeatherStations FOREIGN KEY (WeatherStationId) REFERENCES WeatherStations([Id]) ON DELETE CASCADE,
)
