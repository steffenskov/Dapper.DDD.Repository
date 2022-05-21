using System;
using System.Runtime.CompilerServices;
using AutoFixture;
using AutoFixture.Xunit2;
using Dapper.Repository.IntegrationTests.Entities;

namespace Dapper.Repository.IntegrationTests
{
	public class AutoDomainDataAttribute : AutoDataAttribute
	{
		public AutoDomainDataAttribute([CallerMemberName] string callerMemberName = "") : base(() =>
		  {
			  var fixture = new Fixture();
			  fixture.Customize<CategoryEntity>(transform => transform
			  											.With(category => category.CategoryId, 0)
			  											.With(category => category.Picture, (byte[]?)null)
														.With(category => category.Name, Guid.NewGuid().ToString().Remove(15)));

			  return fixture;
		  })
		{
		}
	}
}
