namespace Dapper.Repository.Configuration;

public abstract class DefaultConfiguration
{
	public IQueryGeneratorFactory? QueryGeneratorFactory { get; set; }
	public IConnectionFactory? ConnectionFactory { get; set; }
	public IDapperInjectionFactory? DapperInjectionFactory { get; set; }
}