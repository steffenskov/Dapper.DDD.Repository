CREATE TABLE [dbo].[WeatherStations]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[Name] VARCHAR(50) NOT NULL,
	[Address_City] VARCHAR(50) NOT NULL,
	[Address_Street] VARCHAR(100) NOT NULL,
	CONSTRAINT PK_WeatherStations PRIMARY KEY ([Id])
);
GO
CREATE UNIQUE INDEX UQ_WeatherStations_Name ON WeatherStations(Name);