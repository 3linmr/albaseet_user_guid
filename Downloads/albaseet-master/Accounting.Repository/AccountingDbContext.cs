using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Repository
{
	public class AccountingDbContext : DbContext
	{
		public AccountingDbContext(DbContextOptions<AccountingDbContext> options) : base(options)
		{

		}

	



		protected override void OnModelCreating(ModelBuilder builder)
		{
			//Make All Relations Restrict
			foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
			}



			
		}
	}
}