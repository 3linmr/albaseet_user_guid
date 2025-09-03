using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Domain.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Services.Basics
{
    public class DocumentStatusService: BaseService<DocumentStatus>, IDocumentStatusService
    {
        public DocumentStatusService(IRepository<DocumentStatus> repository): base(repository) { }
    }
}
