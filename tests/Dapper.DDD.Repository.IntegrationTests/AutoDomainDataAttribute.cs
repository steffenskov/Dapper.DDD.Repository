﻿using System.Runtime.CompilerServices;
using AutoFixture;
using AutoFixture.Xunit2;

namespace Dapper.DDD.Repository.IntegrationTests;

public class AutoDomainDataAttribute : AutoDataAttribute
{
	public AutoDomainDataAttribute([CallerMemberName] string callerMemberName = "") : base(() =>
	{
		var fixture = new Fixture();
		fixture.Customize<Category>(transform => transform
			.With(category => category.CategoryID, (CategoryId?)null)
			.With(category => category.Picture, (byte[]?)null)
			.With(category => category.CategoryName, Guid.NewGuid().ToString().Remove(15)));
		fixture.Customize<Customer>(transform => transform
			.With(customer => customer.Id, Guid.NewGuid())
			.With(customer => customer.InvoiceAddress,
				new Address("Streetname" + Guid.NewGuid(), new Zipcode(Random.Shared.Next(int.MaxValue))))
			.With(customer => customer.DeliveryAddress,
				new Address("Streetname" + Guid.NewGuid(), new Zipcode(Random.Shared.Next(int.MaxValue)))));

		fixture.Customize<CustomerWithNestedAddresses>(transform => transform
			.With(customer => customer.Id, Guid.NewGuid())
			.With(customer => customer.Addresses,
				new Addresses(
					new Address("Streetname" + Guid.NewGuid(), new Zipcode(Random.Shared.Next(int.MaxValue))),
					new Address("Streetname" + Guid.NewGuid(), new Zipcode(Random.Shared.Next(int.MaxValue))))));
		return fixture;
	})
	{
	}
}