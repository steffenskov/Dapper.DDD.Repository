using System.Runtime.CompilerServices;
using AutoFixture;
using AutoFixture.Xunit2;

namespace Dapper.Repository.IntegrationTests;
public class AutoDomainDataAttribute : AutoDataAttribute
{
	public AutoDomainDataAttribute([CallerMemberName] string callerMemberName = "") : base(() =>
	  {
		  var fixture = new Fixture();
		  fixture.Customize<Category>(transform => transform
													  .With(category => category.CategoryID, 0)
													  .With(category => category.Picture, (byte[]?)null)
													.With(category => category.CategoryName, Guid.NewGuid().ToString().Remove(15)));

		  return fixture;
	  })
	{
	}
}
