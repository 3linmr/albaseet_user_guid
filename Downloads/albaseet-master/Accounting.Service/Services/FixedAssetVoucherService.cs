using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.FixedAssets;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.Service.Logic.Tree;
using Shared.Service.Services.Journal;
using Shared.Service.Services.Modules;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Accounting.Service.Services
{
    public class FixedAssetVoucherService : IFixedAssetVoucherService
    {
        private readonly IFixedAssetVoucherHeaderService _fixedAssetVoucherHeaderService;
        private readonly IFixedAssetVoucherDetailService _fixedAssetVoucherDetailService;
        private readonly IFixedAssetVoucherDetailPaymentService _fixedAssetVoucherDetailPaymentService;
        private readonly IFixedAssetMovementDetailService _fixedAssetMovementDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IStringLocalizer<FixedAssetVoucherService> _localizer;
        private readonly IFixedAssetService _fixedAssetService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJournalService _journalService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IStoreService _storeService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IAccountService _accountService;

        public FixedAssetVoucherService(
            IFixedAssetVoucherHeaderService fixedAssetVoucherHeaderService,
            IFixedAssetVoucherDetailService fixedAssetVoucherDetailService,
            IFixedAssetVoucherDetailPaymentService fixedAssetVoucherDetailPaymentService,
            IFixedAssetMovementDetailService fixedAssetMovementDetailService,
            IMenuNoteService menuNoteService,
            IStringLocalizer<FixedAssetVoucherService> localizer,
            IFixedAssetService fixedAssetService,
            IHttpContextAccessor httpContextAccessor, 
            IJournalService journalService,
            IJournalDetailService journalDetailService,
            IStoreService storeService,
            IPaymentMethodService paymentMethodService,
            IAccountService accountService)
        {
            _fixedAssetVoucherHeaderService = fixedAssetVoucherHeaderService;
            _fixedAssetVoucherDetailService = fixedAssetVoucherDetailService;
            _fixedAssetVoucherDetailPaymentService = fixedAssetVoucherDetailPaymentService;
            _menuNoteService = menuNoteService;
            _localizer = localizer;
            _fixedAssetService = fixedAssetService;
            _httpContextAccessor = httpContextAccessor;
            _journalService = journalService;
            _journalDetailService = journalDetailService;
            _storeService = storeService;
            _paymentMethodService = paymentMethodService;
            _accountService = accountService;
            _fixedAssetMovementDetailService = fixedAssetMovementDetailService;
        }
        public async Task<int> GetNextDepreciationCode(int storeId)
        {
            return await _fixedAssetVoucherHeaderService.GetAll().CountAsync(x => x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation && x.StoreId == storeId) + 1;           
        }

        public List<RequestChangesDto> GetFixedAssetVoucherRequestChanges(FixedAssetVoucherDto oldItem, FixedAssetVoucherDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.FixedAssetVoucherHeader, newItem.FixedAssetVoucherHeader);
            requestChanges.AddRange(items);

            if (oldItem.FixedAssetVoucherDetails!.Count != 0 && newItem.FixedAssetVoucherDetails!.Count != 0)
            {
                var oldCount = oldItem.FixedAssetVoucherDetails!.Count(x => x.DetailValue > 0);
                var newCount = newItem.FixedAssetVoucherDetails!.Count(x => x.DetailValue > 0);
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount;)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.FixedAssetVoucherDetails!.Where(x => x.DetailValue > 0).ToList()[i], newItem.FixedAssetVoucherDetails!.Where(x => x.DetailValue > 0).ToList()[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }
            if (oldItem.CostCenterJournalDetails!.Count != 0 && newItem.CostCenterJournalDetails!.Count != 0)
            {
                var oldCount = oldItem.CostCenterJournalDetails!.Count;
                var newCount = newItem.CostCenterJournalDetails!.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount;)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.CostCenterJournalDetails[i], newItem.CostCenterJournalDetails[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }
            if (oldItem.MenuNotes != null && oldItem.MenuNotes.Count != 0 && newItem.MenuNotes != null && newItem.MenuNotes.Count != 0)
            {
                var oldCount = oldItem.MenuNotes.Count;
                var newCount = newItem.MenuNotes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; )
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.MenuNotes[i], newItem.MenuNotes[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }
            return requestChanges;
        }

        public async Task<FixedAssetVoucherDto> GetFixedAssetVoucher(int fixedAssetVoucherHeaderId)
        {
            var header = await _fixedAssetVoucherHeaderService.GetFixedAssetVoucherHeaderById(fixedAssetVoucherHeaderId);
            var detail = await _fixedAssetVoucherDetailService.GetFixedAssetVoucherDetail(fixedAssetVoucherHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes((header.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation) ? MenuCodeData.FixedAssetDepreciation : (header.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Addition) ? MenuCodeData.FixedAssetAddition : MenuCodeData.FixedAssetExclusion, fixedAssetVoucherHeaderId).ToListAsync();
            return new FixedAssetVoucherDto() { FixedAssetVoucherHeader = header, FixedAssetVoucherDetails = detail, MenuNotes = menuNotes };
        }

        public async Task<List<FixedAssetVoucherDetailDto>> GetFixedAssetDepreciation(int fixedAssetId, DateTime? fromDate, DateTime? toDate)
        {
            var fixedAssetVoucherDetails = 
                await  _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail()
                .Where(x =>
                (x.FixedAssetId == fixedAssetId) &&
                (fromDate == null || x.DocumentDate >= fromDate) &&
                (toDate == null || x.DocumentDate <= toDate) &&
                (x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation)
                )
                .ToListAsync();
            return fixedAssetVoucherDetails;
        }
        public async Task<List<FixedAssetVoucherDetailDto>> GetAllFixedAssetsDepreciation(DateTime? fromDate, DateTime? toDate)
        {
            var fixedAssetVoucherDetails = await _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail()
                .Where(x => 
                (fromDate == null || x.DocumentDate >= fromDate) && 
                (toDate == null || x.DocumentDate <= toDate) &&
                (x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation)
                )
                .ToListAsync();
            return fixedAssetVoucherDetails;
        }
        public async Task<ResponseDto> SaveFixedAssetAdditionVoucher(
            FixedAssetAdditionDto fixedAssetAddition,
            bool hasApprove,
            bool approved,
            int? requestId)
        {
            var fixedAsset = await _fixedAssetService.GetFixedAssetByFixedAssetId(fixedAssetAddition.FixedAssetId??0);
            if (fixedAsset.FixedAssetId > 0)
            {
                var voucherDetailForIssuedFixedAsset = await _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail()
                .Where(x => x.FixedAssetId == fixedAsset.FixedAssetId)
                .ToListAsync();
                if (voucherDetailForIssuedFixedAsset.Any(x => 
                x.DocumentDate > fixedAssetAddition.AdditionDate && 
                (x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation || x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Exclusion)
                ))
                    return new ResponseDto() { Success = false, Message = _localizer["VoucherExistsAfterAdditionDate", fixedAssetAddition.AdditionDate??DateTime.UtcNow] };
                if (fixedAssetAddition.AdditionValue <= 0 || fixedAssetAddition.AdditionValue != fixedAssetAddition.FixedAssetVoucherDetailPayments!.Sum(x => x.CreditValue))
                    return new ResponseDto() { Success = false, Message = _localizer["AdditionValueNotCorrect"] };
                
                var isExclusionFixedAsset = voucherDetailForIssuedFixedAsset?
                         .Any(x => x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Exclusion);
                if (!(isExclusionFixedAsset ?? false))
                {
                    var paymentMethods = await _paymentMethodService.GetAllPaymentMethods().ToListAsync();
                    var journalVouchers = new List<FixedAssetJournalVoucherDto>()
                        {
                            new()
                            {
                                FixedAssetVoucherDetailId = 0,
                                FixedAssetVoucherHeaderId = 0,//saveFixedAssetVoucher.Id,
                                FixedAssetId = fixedAsset.FixedAssetId,
                                DocumentDate = fixedAssetAddition.AdditionDate ?? DateTime.UtcNow,
                                AccountId = fixedAsset.AccountId ?? 0,
                                CreditValue = 0,
                                DebitValue = Math.Round(fixedAssetAddition.AdditionValue ?? 0, 2),
                                CurrencyId = fixedAsset.CurrencyId
                            }
                        };
                    foreach (var payment in fixedAssetAddition.FixedAssetVoucherDetailPayments!)
                    {
                        var currentPaymentMethod = paymentMethods.FirstOrDefault(x => x.PaymentMethodId == payment.PaymentMethodId);
                        if (currentPaymentMethod != null)
                        {
                            journalVouchers.Add(
                                new FixedAssetJournalVoucherDto()
                                {
                                    FixedAssetVoucherDetailId = 0,
                                    FixedAssetVoucherHeaderId = 0,//saveFixedAssetVoucher.Id,
                                    FixedAssetId = fixedAsset.FixedAssetId,
                                    DocumentDate = fixedAssetAddition.AdditionDate ?? DateTime.UtcNow,
                                    AccountId = currentPaymentMethod.PaymentAccountId,
                                    CreditValue = Math.Round(payment.CreditValue, 2),
                                    DebitValue = 0,
                                    CurrencyId = currentPaymentMethod.CurrencyId
                                }
                                );
                        }
                    }

                    var saveFixedAssetVoucher = await SaveFixedAssetJournal(
                        new FixedAssetVoucherDto()
                        {
                            FixedAssetVoucherHeader = new FixedAssetVoucherHeaderDto()
                            {
                                FixedAssetVoucherHeaderId = 0,//saveFixedAssetVoucher.Id,
                                DocumentDate = fixedAssetAddition.AdditionDate ?? DateTime.UtcNow,
                                FixedAssetVoucherTypeId = FixedAssetVoucherTypeData.Addition,
                                StoreId = fixedAssetAddition.StoreId,
                                DocumentCode = fixedAssetAddition.DocumentCode
                            },
                            FixedAssetJournalVouchers = journalVouchers,
                            CostCenterJournalDetails = []
                        },
                        hasApprove,
                        approved,
                        requestId);
                    if (saveFixedAssetVoucher.Success)
                    {
                         saveFixedAssetVoucher = await SaveFixedAssetVoucher(
                            new FixedAssetVoucherDto()
                            {
                                FixedAssetVoucherHeader = new FixedAssetVoucherHeaderDto()
                                {
                                    FixedAssetVoucherHeaderId = 0,
                                    Prefix = "A",
                                    DocumentCode = fixedAssetAddition.DocumentCode,
                                    Suffix = fixedAssetAddition.Suffix,
                                    DocumentDate = fixedAssetAddition.AdditionDate ?? DateTime.UtcNow,
                                    FixedAssetVoucherTypeId = FixedAssetVoucherTypeData.Addition,
                                    JournalHeaderId = saveFixedAssetVoucher.Id,
                                    StoreId = fixedAssetAddition.StoreId
                                },
                                FixedAssetVoucherDetails =
                                [
                                    new()
                                {
                                    FixedAssetVoucherDetailId = 0,
                                    FixedAssetVoucherHeaderId = 0,
                                    FixedAssetId = fixedAsset.FixedAssetId,
                                    DocumentDate = fixedAssetAddition.AdditionDate??DateTime.UtcNow,
                                    DetailValue = Math.Round(fixedAssetAddition.AdditionValue??0, 2)
                                }
                                ],
                                CostCenterJournalDetails = []
                            },
                            hasApprove,
                            approved,
                            requestId
                            );
                    }
                    return saveFixedAssetVoucher;
                }
                else
                    return new ResponseDto() { Success = false, Message = _localizer["FixedAssetWasExcluded"] };
            }
            else
                return new ResponseDto() { Success = false, Message = _localizer["FixedAssetNotFound"] };
        }
        public async Task<ResponseDto> SaveAllFixedAssetDepreciationVoucher(
            DateTime depreciationDate, 
            int? storeId,
            bool hasApprove, 
            bool approved, 
            int? requestId)
        {
            try
            {
                var fixedAssets = await _fixedAssetService.GetAllFixedAssets().Where(x => x.IsMainFixedAsset == false).ToListAsync();
                foreach (var fixedAsset in fixedAssets)
                {
                    var fixedAssetVoucher = await SaveFixedAssetDepreciationVoucher(
                        fixedAsset,
                        depreciationDate,
                        storeId,
                        hasApprove,
                        approved,
                        requestId);
                    if (!fixedAssetVoucher.Success) return fixedAssetVoucher;
                }
            }
            catch (Exception ex) 
            {
                return new ResponseDto() { Success = false, Message = ex.Message };
            }
            return new ResponseDto() { Success = true, Message = _localizer["VoucherGeneratedSuccessfully", depreciationDate] };
        }
        public async Task<ResponseDto> SaveFixedAssetDepreciationVoucher(
            FixedAssetDto fixedAsset,
            DateTime depreciationDate,
            int? storeId,
            bool hasApprove,
            bool approved,
            int? requestId)
        {
            var voucherDetailForIssuedFixedAsset = await _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail()
                .Where(x => x.FixedAssetId == fixedAsset.FixedAssetId)
                .ToListAsync();
            var movementForIssuedFixedAsset = await _fixedAssetMovementDetailService.GetAllFixedAssetMovementDetails()
                .Where(x => x.FixedAssetId == fixedAsset.FixedAssetId)
                .ToListAsync();

            var fixedAssetVoucherMaxDate = voucherDetailForIssuedFixedAsset.Where(x => x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation).Max(x => x.DocumentDate) ?? voucherDetailForIssuedFixedAsset.Min(x => x.DocumentDate)??DateTime.UtcNow;
            var fixedAssetVoucherCount = (depreciationDate - fixedAssetVoucherMaxDate).TotalDays;
            if (fixedAssetVoucherCount > 0)
            {
                var voucherDetailForAllFixedAsset = await _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail()
                    .Where(x => x.DocumentDate > fixedAssetVoucherMaxDate && x.DocumentDate <= depreciationDate && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation)
                    .ToListAsync();

                var currentFixedAssetCreditValue = voucherDetailForIssuedFixedAsset
                    .Where(x => x.FixedAssetId == fixedAsset.FixedAssetId && x.DetailValue > 0 && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation)
                    .Sum(x => x.DetailValue);
                var currentFixedAssetDebitValue = voucherDetailForIssuedFixedAsset
                    .Where(x => x.FixedAssetId == fixedAsset.FixedAssetId && x.DetailValue > 0 && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Addition)
                    .Sum(x => x.DetailValue);
                while (fixedAssetVoucherMaxDate < depreciationDate)
                {
                    fixedAssetVoucherMaxDate = fixedAssetVoucherMaxDate.AddDays(1);
                    if ((currentFixedAssetDebitValue - currentFixedAssetCreditValue) > 1)
                    {
                        var isExclusionFixedAsset = voucherDetailForIssuedFixedAsset
                            .Any(x => x.DocumentDate < fixedAssetVoucherMaxDate && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Exclusion);
                        if (!isExclusionFixedAsset)
                        {
                            var currentFixedAssetMovement = movementForIssuedFixedAsset
                                .Where(x => x.MovementDate <= fixedAssetVoucherMaxDate)
                                .OrderByDescending(x => x.MovementDate)
                                .FirstOrDefault();
                            var currentFixedAssetValue = voucherDetailForIssuedFixedAsset
                                .Where(x => x.DocumentDate < fixedAssetVoucherMaxDate && x.FixedAssetId == fixedAsset.FixedAssetId && x.DetailValue > 0 && x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Addition)
                                .Sum(x => x.DetailValue);
                            var currentDepreciationValue = (currentFixedAssetValue / 365) * (fixedAsset.AnnualDepreciationPercent / 100);

                            var existsVoucherDetailForAllFixedAsset = voucherDetailForAllFixedAsset
                                .Where(x => x.DocumentDate == fixedAssetVoucherMaxDate)
                                .ToList();
                            int FixedAssetVoucherHeaderId = 0;
                            if (existsVoucherDetailForAllFixedAsset != null)
                            {
                                if (existsVoucherDetailForAllFixedAsset.Any(x => x.FixedAssetId == fixedAsset.FixedAssetId))
                                {
                                    break;
                                }
                                else
                                {
                                    FixedAssetVoucherHeaderId = existsVoucherDetailForAllFixedAsset.FirstOrDefault()?.FixedAssetVoucherHeaderId ?? 0;
                                }
                            };
                            currentFixedAssetCreditValue += currentDepreciationValue;
                            
                            var saveFixedAssetVoucher = await SaveFixedAssetVoucher(
                                new FixedAssetVoucherDto()
                                {
                                    FixedAssetVoucherHeader = new FixedAssetVoucherHeaderDto()
                                    {
                                        Prefix = "D",
                                        DocumentCode = await GetNextDepreciationCode(storeId??0),
                                        FixedAssetVoucherHeaderId = FixedAssetVoucherHeaderId,
                                        DocumentDate = fixedAssetVoucherMaxDate,
                                        FixedAssetVoucherTypeId = FixedAssetVoucherTypeData.Depreciation,
                                        StoreId = storeId
                                    },
                                    FixedAssetVoucherDetails =
                                    [
                                         new()
                                         {
                                             FixedAssetVoucherDetailId = 0,
                                             FixedAssetVoucherHeaderId = 0,
                                             FixedAssetId = fixedAsset.FixedAssetId,
                                             DocumentDate = fixedAssetVoucherMaxDate,
                                             DetailValue = Math.Round(currentDepreciationValue ?? 0, 2)
                                         }
                                    ],
                                    CostCenterJournalDetails = []
                                },
                                hasApprove,
                                approved,
                                requestId
                                );

                            if (saveFixedAssetVoucher.Success)
                            {
                                saveFixedAssetVoucher = await SaveFixedAssetJournal(
                                      new FixedAssetVoucherDto()
                                      {
                                          FixedAssetVoucherHeader = new FixedAssetVoucherHeaderDto()
                                          {
                                              FixedAssetVoucherHeaderId = saveFixedAssetVoucher.Id,
                                              DocumentDate = fixedAssetVoucherMaxDate,
                                              FixedAssetVoucherTypeId = FixedAssetVoucherTypeData.Depreciation,
                                              StoreId = storeId
                                          },
                                          FixedAssetJournalVouchers =
                                          [
                                              new()
                                              {
                                                  FixedAssetVoucherDetailId = 0,
                                                  FixedAssetVoucherHeaderId = 0,
                                                  FixedAssetId = fixedAsset.FixedAssetId,
                                                  AccountId = fixedAsset.DepreciationAccountId ?? 0,
                                                  CreditValue = 0,
                                                  DebitValue = Math.Round(currentDepreciationValue ?? 0, 2),
                                                  CurrencyId = fixedAsset.DepreciationCurrencyId
                                              },
                                              new()
                                              {
                                                  FixedAssetVoucherDetailId = 0,
                                                  FixedAssetVoucherHeaderId = 0,
                                                  FixedAssetId = fixedAsset.FixedAssetId,
                                                  AccountId = fixedAsset.CumulativeDepreciationAccountId ?? 0,
                                                  CreditValue = Math.Round(currentDepreciationValue ?? 0, 2),
                                                  DebitValue = 0,
                                                  CurrencyId = fixedAsset.CumulativeCurrencyId
                                              }
                                          ],
                                          CostCenterJournalDetails = currentFixedAssetMovement != null ? 
                                          [
                                               new CostCenterJournalDetailDto()
                                              {
                                                  CostCenterJournalDetailId = 0,
                                                  CostCenterId = currentFixedAssetMovement.CostCenterToId??0,
                                                  CreditValue = 0,
                                                  DebitValue = Math.Round(currentDepreciationValue ?? 0, 2),
                                                  JournalDetailId = 0
                                              } 
                                          ] : new List<CostCenterJournalDetailDto>()
                                      },
                                    hasApprove,
                                    approved,
                                    requestId);
                                if (!saveFixedAssetVoucher.Success)
                                    return saveFixedAssetVoucher;
                            }
                        }
                    }
                }
            }
            return new ResponseDto() { Success = true, Message = _localizer["VoucherGeneratedSuccessfully", depreciationDate] };
        }
        public async Task<ResponseDto> SaveFixedAssetExclusionVoucher(
            FixedAssetExclusionDto fixedAssetExclusion,
            bool hasApprove,
            bool approved,
            int? requestId)
        {
            var fixedAsset = await _fixedAssetService.GetFixedAssetByFixedAssetId(fixedAssetExclusion.FixedAssetId ?? 0);
            if (fixedAsset.FixedAssetId > 0)
            {
                var voucherDetailForIssuedFixedAsset = await _fixedAssetVoucherDetailService.GetAllFixedAssetVoucherDetail()
                    .Where(x => x.FixedAssetId == fixedAsset.FixedAssetId)
                    .ToListAsync();
                if (voucherDetailForIssuedFixedAsset.Any(x => x.DocumentDate > fixedAssetExclusion.ExclusionDate))
                    return new ResponseDto() { Success = false, Message = _localizer["VoucherExistsAfterExclusionDate", fixedAssetExclusion.ExclusionDate??DateTime.UtcNow] };
                var CumulativeDepreciationJournals = await _journalDetailService.GetAll().Where(x => x.AccountId == fixedAsset.CumulativeDepreciationAccountId).ToListAsync();
                var currentCumulativeDepreciationValue = (CumulativeDepreciationJournals?.Sum(x => x.CreditValue) ?? 0) - (CumulativeDepreciationJournals?.Sum(x => x.DebitValue) ?? 0);
                var fixedAssetJournals = await _journalDetailService.GetAll().Where(x => x.AccountId == fixedAsset.AccountId).ToListAsync();
                var fixedAssetValue = (fixedAssetJournals?.Sum(x => x.DebitValue) ?? 0);
                var differenceCumulativeDepreciationValue = fixedAssetValue - currentCumulativeDepreciationValue;
         

                if (differenceCumulativeDepreciationValue > 0)
                {
                    var isExclusionFixedAsset = voucherDetailForIssuedFixedAsset?
                        .Any(x => x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Exclusion);
                    if (!(isExclusionFixedAsset ?? false))
                    {
                        var saveFixedAssetVoucher = await SaveFixedAssetVoucher(
                            new FixedAssetVoucherDto()
                            {
                                FixedAssetVoucherHeader = new FixedAssetVoucherHeaderDto()
                                {
                                    Prefix = "E",
                                    DocumentCode = fixedAssetExclusion.DocumentCode,
                                    Suffix = fixedAssetExclusion.Suffix,
                                    FixedAssetVoucherHeaderId = 0,
                                    DocumentDate = fixedAssetExclusion.ExclusionDate ?? DateTime.UtcNow,
                                    FixedAssetVoucherTypeId = FixedAssetVoucherTypeData.Exclusion,
                                    StoreId = fixedAssetExclusion.StoreId
                                },
                                FixedAssetVoucherDetails =
                                [
                                    new()
                                    {
                                        FixedAssetVoucherDetailId = 0,
                                        FixedAssetVoucherHeaderId = 0,
                                        FixedAssetId = fixedAsset.FixedAssetId,
                                        DocumentDate = fixedAssetExclusion.ExclusionDate??DateTime.UtcNow,
                                        DetailValue = Math.Round(fixedAssetExclusion.ExclusionToValue??0, 2)
                                    }
                                ],
                                CostCenterJournalDetails = []
                            },
                            hasApprove,
                            approved,
                            requestId
                            );

                        if (saveFixedAssetVoucher.Success)
                        {
                            var paymentMethods = await _paymentMethodService.GetAllPaymentMethods().ToListAsync();
                            var profitsAndLossesVouchers = new List<FixedAssetJournalVoucherDto>()
                            {
                                new()
                                {
                                    FixedAssetVoucherDetailId = 0,
                                    FixedAssetVoucherHeaderId = 0,
                                    FixedAssetId = fixedAsset.FixedAssetId,
                                    AccountId = fixedAsset.CumulativeDepreciationAccountId ?? 0,
                                    CreditValue = 0,
                                    DebitValue = Math.Round(currentCumulativeDepreciationValue, 2),
                                    CurrencyId = fixedAsset.CumulativeCurrencyId
                                },
                                new()
                                {
                                    FixedAssetVoucherDetailId = 0,
                                    FixedAssetVoucherHeaderId = 0,
                                    FixedAssetId = fixedAsset.FixedAssetId,
                                    AccountId = fixedAsset.AccountId ?? 0,
                                    CreditValue = Math.Round(fixedAssetValue, 2),
                                    DebitValue = 0,
                                    CurrencyId = fixedAsset.CurrencyId
                                }
                            };

                            foreach (var payment in fixedAssetExclusion.FixedAssetVoucherDetailPayments!)
                            {
                                var currentPaymentMethod = paymentMethods.FirstOrDefault(x => x.PaymentMethodId == payment.PaymentMethodId);
                                if(currentPaymentMethod != null)
                                {
                                    profitsAndLossesVouchers.Add(
                                        new FixedAssetJournalVoucherDto()
                                        {
                                            FixedAssetVoucherDetailId = 0,
                                            FixedAssetVoucherHeaderId = 0,
                                            FixedAssetId = fixedAsset.FixedAssetId,
                                            DocumentDate = fixedAssetExclusion.ExclusionDate ?? DateTime.UtcNow,
                                            AccountId = currentPaymentMethod.PaymentAccountId,
                                            CreditValue = 0,
                                            DebitValue = Math.Round(payment.DebitValue, 2),
                                            CurrencyId = currentPaymentMethod.CurrencyId,
                                        }
                                        );
                                }
                            }
                            var profitOrLossValue = fixedAssetExclusion.FixedAssetVoucherDetailPayments!.Sum(x => x.DebitValue) - differenceCumulativeDepreciationValue;
                            var salesCurrencyId = await _accountService.GetAccountByAccountId(fixedAssetExclusion.SalesAccountId??0);
                            profitsAndLossesVouchers.Add(
                                new FixedAssetJournalVoucherDto()
                                {
                                    FixedAssetVoucherDetailId = 0,
                                    FixedAssetVoucherHeaderId = 0,
                                    FixedAssetId = fixedAsset.FixedAssetId,
                                    DocumentDate = fixedAssetExclusion.ExclusionDate ?? DateTime.UtcNow,
                                    AccountId = fixedAssetExclusion.SalesAccountId,
                                    CreditValue = (profitOrLossValue >= 0) ? Math.Round(profitOrLossValue, 2) : 0,
                                    DebitValue = (profitOrLossValue < 0) ? Math.Round(Math.Abs(profitOrLossValue), 2) : 0,
                                    CurrencyId = salesCurrencyId!.CurrencyId
                                }
                                );
                            saveFixedAssetVoucher = await SaveFixedAssetJournal(
                                new FixedAssetVoucherDto()
                                {
                                    FixedAssetVoucherHeader = new FixedAssetVoucherHeaderDto()
                                    {
                                        FixedAssetVoucherHeaderId = saveFixedAssetVoucher.Id,
                                        DocumentDate = fixedAssetExclusion.ExclusionDate ?? DateTime.UtcNow,
                                        FixedAssetVoucherTypeId = FixedAssetVoucherTypeData.Exclusion,
                                        StoreId = fixedAssetExclusion.StoreId
                                    },
                                    FixedAssetJournalVouchers = profitsAndLossesVouchers,
                                    CostCenterJournalDetails = []
                                },
                                hasApprove,
                                approved,
                                requestId);
                        }
                        return saveFixedAssetVoucher;
                    }
                    else
                        return new ResponseDto() { Success = false, Message = _localizer["ExclusionAlreadyExists"] };
                }
                else
                    return new ResponseDto() { Success = false, Message = _localizer["CumulativeGreaterThanFixedAsset"] };
            }
            else
                return new ResponseDto() { Success = false, Message = _localizer["FixedAssetNotFound"] };
        }
        public async Task<ResponseDto> SaveFixedAssetVoucher(FixedAssetVoucherDto fixedAssetVoucher, bool hasApprove, bool approved, int? requestId)
        {
            if (fixedAssetVoucher.FixedAssetVoucherHeader != null)
            {
                //var isUpdate = fixedAssetVoucher.FixedAssetVoucherHeader.FixedAssetVoucherHeaderId > 0;
                var saveHeader = await _fixedAssetVoucherHeaderService.SaveFixedAssetVoucherHeader(fixedAssetVoucher.FixedAssetVoucherHeader, hasApprove, approved, requestId);
                if (saveHeader.Success)
                {
                    var saveDetail = await _fixedAssetVoucherDetailService.SaveFixedAssetVoucherDetail(fixedAssetVoucher.FixedAssetVoucherDetails!, saveHeader.Id);                                                  
                    if (saveDetail.Response!.Success)
                    {
                        foreach(var detail in saveDetail.FixedAssetVoucherDetails!)
                        {
                            saveHeader.Success = await _fixedAssetVoucherDetailPaymentService.SaveFixedAssetVoucherDetailPayment(detail.FixedAssetVoucherDetailPayments, detail.FixedAssetVoucherDetailId);
                            if (!saveHeader.Success)
                                return saveHeader;
                        }
                        if (fixedAssetVoucher.MenuNotes != null)
                        {
                            await _menuNoteService.SaveMenuNotes(fixedAssetVoucher.MenuNotes, saveHeader.Id);
                        }
                    }
                }
                return saveHeader;
            }
            return new ResponseDto() { Success = false, Message = _localizer["FixedAssetVoucherHeaderIsNULL"]};
        }
        public async Task<ResponseDto> SaveFixedAssetJournal(FixedAssetVoucherDto fixedAssetVoucher, bool hasApprove, bool approved, int? requestId)
        {
            if (fixedAssetVoucher.FixedAssetVoucherHeader != null)
            {
                //var isUpdate = fixedAssetVoucher.FixedAssetVoucherHeader.FixedAssetVoucherHeaderId > 0;

                var journal = new JournalDto()
                {
                    JournalHeader = BuildJournalHeader(fixedAssetVoucher.FixedAssetVoucherHeader!, fixedAssetVoucher.FixedAssetVoucherHeader.FixedAssetVoucherHeaderId ?? 0, fixedAssetVoucher.FixedAssetVoucherHeader.JournalHeaderId.GetValueOrDefault()),
                    JournalDetails = BuildJournalDetail(fixedAssetVoucher.FixedAssetJournalVouchers!, fixedAssetVoucher.FixedAssetVoucherHeader.JournalHeaderId.GetValueOrDefault(), fixedAssetVoucher.CostCenterJournalDetails!.Count != 0),
                    CostCenterJournalDetails = BuildCostCenterJournalDetail(fixedAssetVoucher.CostCenterJournalDetails!)
                };
                var newJournal = await _journalService.SaveJournal(journal, hasApprove, approved, requestId);
                await _fixedAssetVoucherHeaderService.UpdateFixedAssetVoucherWithJournalHeaderId(fixedAssetVoucher.FixedAssetVoucherHeader.FixedAssetVoucherHeaderId ?? 0, newJournal.Id);
                return newJournal;

            }
            return new ResponseDto() { Success = false, Message = _localizer["FixedAssetVoucherHeaderIsNULL"] };
        }

        public static JournalHeaderDto BuildJournalHeader(FixedAssetVoucherHeaderDto header, int fixedAssetVoucherHeaderId, int journalHeaderId)
        {
            return new JournalHeaderDto()
            {
                MenuCode = (short)((header.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation) ? MenuCodeData.FixedAssetDepreciation : (header.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Addition) ? MenuCodeData.FixedAssetAddition : MenuCodeData.FixedAssetExclusion),
                MenuReferenceId = fixedAssetVoucherHeaderId,
                IsSystematic = true,
                JournalTypeId = JournalTypeData.FixedAssetVoucher,
                PeerReference = header.PeerReference,
                RemarksAr = header.RemarksAr,
                RemarksEn = header.RemarksEn,
                StoreId = header.StoreId ?? 0,
                TicketDate = header.DocumentDate ?? DateTime.UtcNow,
                JournalHeaderId = journalHeaderId
            };
        }

        public static List<JournalDetailDto> BuildJournalDetail(List<FixedAssetJournalVoucherDto> journals, int journalHeaderId, bool hasCostCenters)
        {
            var modelList = new List<JournalDetailDto>();
            foreach (var journal in journals)
            {
                var model = new JournalDetailDto()
                {
                    JournalHeaderId = journalHeaderId,
                    JournalDetailId = journal.DebitValue > 0 ? -1 : 0,
                    AccountId = journal.AccountId ?? 0,
                    CurrencyId = journal.CurrencyId ?? 0,
                    DebitValue = journal.DebitValue ?? 0,
                    CreditValue = journal.CreditValue ?? 0,
                    DebitValueAccount = journal.DebitValueAccount ?? 0,
                    CreditValueAccount = journal.CreditValueAccount ?? 0,
                    CurrencyRate = journal.CurrencyRate ?? 0,
                    IsLinkedToCostCenters = hasCostCenters,
                    RemarksAr = journal.RemarksAr,
                    RemarksEn = journal.RemarksEn
                };
                modelList.Add(model);
            }
            return modelList;
        }
                
        public static List<CostCenterJournalDetailDto> BuildCostCenterJournalDetail(List<CostCenterJournalDetailDto> costCenters)
        {
            foreach (var x in costCenters)
            {
                x.JournalDetailId = -1;
                x.CostCenterJournalDetailId = 0;
                x.DebitValue = x.DebitValue;
                x.CostCenterId = x.CostCenterId;
                x.RemarksAr = x.RemarksAr;
                x.RemarksEn = x.RemarksEn;
            }
            return costCenters;
        }
        public async Task<ResponseDto> DeleteFixedAssetVoucher(int fixedAssetVoucherHeaderId)
        {
            var fixedAssetHeader = await _fixedAssetVoucherHeaderService.GetFixedAssetVoucherHeaderById(fixedAssetVoucherHeaderId);
            if(fixedAssetHeader.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Addition || fixedAssetHeader.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation)
            {
                var notAllowDelete = await _fixedAssetVoucherHeaderService.GetAll().AnyAsync(x => 
                x.DocumentDate > fixedAssetHeader.DocumentDate && 
                (x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Depreciation || x.FixedAssetVoucherTypeId == FixedAssetVoucherTypeData.Exclusion)
                );
                if (notAllowDelete)
                    return new ResponseDto() { Success = false, Message = _localizer[""] };
            }
            var deleteDetailResult = await _fixedAssetVoucherDetailService.DeleteFixedAssetVoucherDetail(fixedAssetVoucherHeaderId);
            if(deleteDetailResult)
            {
                var result = await _fixedAssetVoucherHeaderService.DeleteFixedAssetVoucherHeader(fixedAssetVoucherHeaderId);
                if(result.Success)
                    result = await _journalService.DeleteJournal(fixedAssetHeader.JournalHeaderId.GetValueOrDefault());
                return result;
            }
            return new ResponseDto() { Success = false, Message = _localizer["CanNotDeleteFixedAssetVoucherDetail"] };
        }
    }
}