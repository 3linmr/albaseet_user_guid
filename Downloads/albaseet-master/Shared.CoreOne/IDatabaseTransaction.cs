using Shared.Helper.Models.UserDetail;

namespace Shared.CoreOne
{
	public interface IDatabaseTransaction : IAsyncDisposable
	{
		Task<bool> BeginTransaction(CancellationToken cancellationToken = default);
		Task<bool> BeginTransaction(int timeoutInMinutes, CancellationToken cancellationToken = default);
		Task<bool> Commit(CancellationToken cancellationToken = default);
		Task<bool> Rollback(CancellationToken cancellationToken = default);
	}
}
