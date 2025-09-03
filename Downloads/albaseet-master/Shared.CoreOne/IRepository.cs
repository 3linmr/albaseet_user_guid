using System.Data;
using System.Data.SqlClient;
using Shared.CoreOne.Models;

namespace Shared.CoreOne
{
    public interface IRepository<T> where T : BaseObject
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task SaveChanges();
        DataTable SqlQuery(string query, string connectionString, params SqlParameter[] sqlParameters);
        DataTable SqlQueryStoreProcedure(string query, string connectionString, params SqlParameter[] sqlParameters);
        IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);
    }
}
