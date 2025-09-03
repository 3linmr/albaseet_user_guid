using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Approval;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Journal
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class JournalEntriesController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IJournalHeaderService _journalHeaderService;
		private readonly ITransactionTypeService _transactionTypeService;
		private readonly IAccountTaxService _accountTaxService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IJournalService _journalService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public JournalEntriesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer,IJournalHeaderService journalHeaderService,ITransactionTypeService transactionTypeService,IAccountTaxService accountTaxService,IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService,IJournalService journalService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_journalHeaderService = journalHeaderService;
			_transactionTypeService = transactionTypeService;
			_accountTaxService = accountTaxService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_approvalSystemService = approvalSystemService;
			_journalService = journalService;
			_httpContextAccessor = httpContextAccessor;
		}


		[HttpGet]
		[Route("ReadJournalHeaders")]
		public async Task<IActionResult> ReadJournalHeaders(DataSourceLoadOptions loadOptions)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_journalHeaderService.GetUserJournalHeaders(), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = await _journalHeaderService.GetUserJournalHeaders().ToListAsync();
				var data = DataSourceLoader.Load(model, loadOptions);
				return Ok(data);
			}
		}
		
		[HttpGet]
		[Route("GetJournalCode")]
		public async Task<IActionResult> GetJournalCode(int storeId, DateTime ticketDate)
		{
			var data = await _journalHeaderService.GetJournalCode(storeId, ticketDate);
			return Ok(data);
		}

		[HttpGet]
		[Route("CheckExistingTaxReference")]
		public async Task<IActionResult> CheckExistingTaxReference(string taxReference, DateTime ticketDate)
		{
			var data = await _journalService.CheckExistingTaxReference(taxReference, ticketDate);
			return Ok(data);
		}

		[HttpPost]
		[Route("CheckExistingTaxReferences")]
		public async Task<IActionResult> CheckExistingTaxReferences([FromBody] JournalDetailTaxReferenceDto model)
		{
			var data = await _journalService.CheckExistingTaxReferences(model.TaxReferences, model.TicketDate);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetTransactionTypesDropDown")]
		public async Task<IActionResult> GetTransactionTypesDropDown()
		{
			var data = await _transactionTypeService.GetTransactionTypesDropDown().ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAccountTax")]
		public async Task<IActionResult> GetAccountTax(int storeId, int taxId, bool isDebit)
		{
			var data = await _accountTaxService.GetAccountTax(storeId, taxId,isDebit);
			return Ok(data);
		}


		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null)
			{
				if (data.CanBeConverted<JournalDto>())
				{
				    var journal = data.ConvertToType<JournalDto>();
				    var userCanLook = await _httpContextAccessor.UserCanLook(0, journal?.JournalHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? journal : new JournalDto());
				}
			}
			return Ok(new JournalDto());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetJournal(int id)
		{
			var journal = await _journalService.GetJournal(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, journal.JournalHeader?.StoreId ?? 0);
			return Ok(userCanLook ? journal : new JournalDto());
		}

		[HttpGet]
		[Route("GetJournalDetail")]
		public async Task<IActionResult> GetJournalDetail(int id)
		{
			var journal = await _journalService.GetJournal(id);
			return Ok(journal.JournalDetails);
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] JournalDto model, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.JournalEntry);
				if (hasApprove.HasApprove)
				{
					if (hasApprove.OnAdd && model.JournalHeader!.JournalHeaderId == 0)
					{
						response = await SendApproveRequest(model, null, requestId, 0, false);
					}
					else if (hasApprove.OnEdit && model.JournalHeader!.JournalHeaderId > 0)
					{
						var oldValue = await _journalService.GetJournal(model.JournalHeader!.JournalHeaderId);
						response = await SendApproveRequest(model, oldValue, requestId, model.JournalHeader!.JournalHeaderId, false);
					}
					else
					{
						response = await SaveJournal(model, hasApprove.HasApprove,false, requestId);
					}
				}
				else
				{
					response = await SaveJournal(model, hasApprove.HasApprove,false, requestId);
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpPost]
		[Route("SaveJournal")]
		public async Task<ResponseDto> SaveJournal(JournalDto model,bool hasApprove,bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _journalService.SaveJournal(model, hasApprove,approved, requestId);
			if (response.Success)
			{
				await _databaseTransaction.Commit();
			}
			else
			{
				await _databaseTransaction.Rollback();
			}
			return response;
		}

		[HttpDelete]
		[Route("DeleteJournal")]
		public async Task<IActionResult> DeleteJournal(int journalHeaderId, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.JournalEntry);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					var model = await _journalService.GetJournal(journalHeaderId);
					response = await SendApproveRequest(null, model, requestId, journalHeaderId, true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _journalService.DeleteJournal(journalHeaderId);
					if (response.Success)
					{
						await _databaseTransaction.Commit();
					}
					else
					{
						await _databaseTransaction.Rollback();
					}
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpPost]
		[Route("SendApproveRequest")]
		public async Task<ResponseDto> SendApproveRequest([FromQuery] JournalDto? newModel, [FromQuery] JournalDto? oldModel, int requestId, int journalHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (journalHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit)
			{
				changes = _journalService.GetJournalEntryRequestChanges(oldModel!, newModel!);
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = MenuCodeData.JournalEntry,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = journalHeaderId,
				ReferenceCode = journalHeaderId.ToString(),
				OldValue = oldModel,
				NewValue = newModel,
				Changes = changes
			};
			await _databaseTransaction.BeginTransaction();

			var response = await _handleApprovalRequestService.SaveRequest(request);
			if (response.Success)
			{
				await _databaseTransaction.Commit();
			}
			else
			{
				await _databaseTransaction.Rollback();
			}
			return response;
		}
	}

}
