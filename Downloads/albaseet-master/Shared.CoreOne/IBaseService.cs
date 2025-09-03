using System.Data;
using System.Data.SqlClient;
using Shared.CoreOne.Models;

namespace Shared.CoreOne
{
    public interface IBaseService<T> where T : BaseObject
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Update(T entity);
        Task UpdateRange(IEnumerable<T> entities);
        Task Delete(T entity);
        Task RemoveRange(IEnumerable<T> entities);
        DataTable SqlQuery(string query, string connectionString);
        DataTable SqlQueryWithParameters(string query, string connectionString, params SqlParameter[] parameters);
    }
}
