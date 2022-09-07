namespace Dapper.DDD.Repository.Exceptions;

public class DapperRepositoryQueryException : Exception
{
	public DapperRepositoryQueryException(string query, Exception innerException) : base(
		$"Exception when executing query: {query}", innerException)
	{
	}
}