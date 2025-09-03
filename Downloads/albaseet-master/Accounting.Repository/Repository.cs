using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Accounting.Repository;
using Accounting.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.Helper.Models.UserDetail;

namespace Accounting.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseObject
    {
        private readonly AccountingDbContext _context;
        private DbSet<T> _entities;
        string errorMessage = string.Empty;

        public Repository(AccountingDbContext context, ApplicationSettingDto application)
        {
	        DatabaseInitializer.ChangeDbContext(application, context).WaitAsync(TimeSpan.FromMinutes(1));
            this._context = context;
            _entities = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _entities.AsQueryable().Where(x=>x.Hide == false);
        }


        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _entities.Where(predicate);
            return query;
        }

        public async Task Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await _entities.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            await _entities.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _context.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            _entities.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            _context.Set<T>().RemoveRange(entities);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public DataTable SqlQuery(string query, string connectionString, params SqlParameter[] sqlParameters)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, connection);
            foreach (var param in sqlParameters)
            {
                cmd.Parameters.Add(param);
            }
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dataTable);
            connection.Close();
            da.Dispose();
            return dataTable;
        }

        public DataTable SqlQueryStoreProcedure(string query, string connectionString, params SqlParameter[] sqlParameters)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, connection);

            cmd.CommandType = CommandType.StoredProcedure;

            foreach (var param in sqlParameters)
            {
                cmd.Parameters.Add(param);
            }

            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd); da.Fill(dataTable);
            connection.Close();
            da.Dispose();
            return dataTable;
        }

        public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            throw new NotImplementedException();
            //return _entities.FromSqlRaw(query, parameters);
        }
    }
}
