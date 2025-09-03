using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Items;
using Shared.Service.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Service.Services.Journal;
using Shared.CoreOne.Models.Domain.Journal;

namespace Shared.Service.Validators
{
	internal class JournalHeaderValidator : AbstractValidator<JournalHeader>
	{
		public JournalHeaderValidator(IStringLocalizer<JournalHeaderService> localizer)
		{
			RuleFor(x => x.JournalHeaderId)
				.NotEmpty()
				.NotNull()
				.NotEqual(0);

			RuleFor(x => x.TicketId)
				.NotEmpty()
				.NotNull()
				.NotEqual(0);

			RuleFor(x => x.JournalId)
				.NotEmpty()
				.NotNull()
				.NotEqual(0);	
			
			RuleFor(x => x.JournalCode)
				.NotEmpty()
				.NotNull()
				.GreaterThan(0);

			RuleFor(x => x.RemarksAr)
				.Length(0, 500);

			RuleFor(x => x.RemarksEn)
				.Length(0, 500);

			RuleFor(x => x.TicketDate)
				.NotEmpty()
				.NotNull();
		}
	}
}
