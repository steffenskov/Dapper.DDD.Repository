namespace Dapper.Repository
{
	/// <summary>
	/// Marker record to ensure only records are being used, as this enables { get; init; } properties
	/// </summary>
	public abstract record DbEntity
	{
	}
}
