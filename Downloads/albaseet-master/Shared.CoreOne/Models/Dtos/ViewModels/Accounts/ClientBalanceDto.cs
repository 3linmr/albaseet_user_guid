using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Accounts
{
    public class ClientBalanceDto
    {
        public int AccountId { get; set; }
        public int StoreId { get; set; }
        public decimal? Balance { get; set; }
    }
}
