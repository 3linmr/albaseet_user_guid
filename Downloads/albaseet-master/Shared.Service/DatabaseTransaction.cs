using System.Data;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Repository;
using Shared.Helper.Models.UserDetail;
using Shared.Repository.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace Shared.Service
{
	public class DatabaseTransaction : IDatabaseTransaction
	{
		private readonly ApplicationDbContext _context;
		private IDbContextTransaction? _transaction;

		public DatabaseTransaction(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> BeginTransaction(CancellationToken cancellationToken = default)
		{
			_transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);
			return true;
		}

		/// <summary>
		/// Begin transaction with timeout in minutes.
		/// </summary>
		/// <param name="timeoutInMinutes">The command timeout in minutes.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns></returns>
		public async Task<bool> BeginTransaction(int timeoutInMinutes, CancellationToken cancellationToken = default)
		{
			_context.Database.SetCommandTimeout(TimeSpan.FromMinutes(timeoutInMinutes));
			_transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
			return true;
		}

		public async Task<bool> Commit(CancellationToken cancellationToken = default)
		{
			if (_transaction != null)
			{
				await _transaction.CommitAsync(cancellationToken);
			}
			return true;
		}

		public async Task<bool> Rollback(CancellationToken cancellationToken = default)
		{
			if (_transaction != null)
			{
				await _transaction.RollbackAsync(cancellationToken);
			}
			return true;
		}

		public async ValueTask DisposeAsync()
		{
			if (_transaction != null)
			{
				await _transaction.DisposeAsync();
			}
		}
	}
}
