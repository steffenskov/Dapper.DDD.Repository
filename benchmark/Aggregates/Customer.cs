using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace benchmark.Aggregates;
public class Customer
{
	public string CustomerID { get; set; } = default!;
	public string CompanyName { get; set; } = default!;
	public string ContactName { get; set; } = default!;
	public string ContactTitle { get; set; } = default!;
	public string Address { get; set; } = default!;
	public string City { get; set; } = default!;
	public string? Region { get; set; }
	public string PostalCode { get; set; } = default!;
	public string Country { get; set; } = default!;
	public string Phone { get; set; } = default!;
	public string? Fax { get; set; }
}
