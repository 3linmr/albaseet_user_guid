using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Shared.CoreOne;
using Shared.CoreOne.Models;

namespace Shared.Service
{
	public class BaseService<T> : IBaseService<T> where T : BaseObject
	{
		public IRepository<T> _repository;

		public BaseService(IRepository<T> repository)
		{
			_repository = repository;
		}

		public IQueryable<T> GetAll()
		{
			return _repository.GetAll();
		}

		public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
		{
			return _repository.FindBy(predicate);
		}

		public async Task Insert(T entity)
		{
			await _repository.Insert(entity);
			await _repository.SaveChanges();
		}

		public async Task InsertRange(IEnumerable<T> entities)
		{
			await _repository.InsertRange(entities);
			await _repository.SaveChanges();
		}

		public async Task Update(T entity)
		{
			_repository.Update(entity);
			await _repository.SaveChanges();
		}

		public async Task UpdateRange(IEnumerable<T> entities)
		{
			_repository.UpdateRange(entities);
			await _repository.SaveChanges();
		}

		public async Task Delete(T entity)
		{
			_repository.Delete(entity);
			await _repository.SaveChanges();
		}

		public async Task RemoveRange(IEnumerable<T> entities)
		{
			_repository.RemoveRange(entities);
			await _repository.SaveChanges();
		}

		public DataTable SqlQuery(string query, string connectionString)
		{
			return _repository.SqlQuery(query, connectionString);
		}
		public DataTable SqlQueryWithParameters(string query, string connectionString, params SqlParameter[] parameters)
		{
			return _repository.SqlQuery(query, connectionString, parameters);
		}
	}
}
