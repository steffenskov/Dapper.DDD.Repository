using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using benchmark.Aggregates;
using Dapper.Repository.Configuration;
using Dapper.Repository.Interfaces;
using Dapper.Repository.Repositories;
using Microsoft.Extensions.Options;

namespace benchmark.Repositories;
public class CustomerRepository : TableRepository<Customer, string>, ITableRepository<Customer, string>
{
	public CustomerRepository(IOptions<TableAggregateConfiguration<Customer>> options, IOptions<DefaultConfiguration> defaultOptions) : base(options, defaultOptions)
	{
	}
}
