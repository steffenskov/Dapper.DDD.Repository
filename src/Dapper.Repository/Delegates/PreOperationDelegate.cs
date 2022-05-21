using System.ComponentModel;

namespace Dapper.Repository.Delegates
{
	public delegate void PreOperationDelegate<T>(T inputEntity, CancelEventArgs e);
}
