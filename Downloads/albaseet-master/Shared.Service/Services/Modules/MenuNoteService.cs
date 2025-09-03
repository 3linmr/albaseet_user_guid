using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class MenuNoteService : BaseService<MenuNote>, IMenuNoteService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<MenuNoteService> _localizer;
		private readonly IMenuNoteIdentifierService _menuNoteIdentifierService;
		private readonly IColumnIdentifierService _columnIdentifierService;

		public MenuNoteService(IRepository<MenuNote> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<MenuNoteService> localizer, IMenuNoteIdentifierService menuNoteIdentifierService, IColumnIdentifierService columnIdentifierService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_menuNoteIdentifierService = menuNoteIdentifierService;
			_columnIdentifierService = columnIdentifierService;
		}

		public IQueryable<MenuNoteDto> GetMenuNotes(int menuCode, int referenceId)
		{
			var trueValue = _localizer["True"].Value;
			var falseValue = _localizer["False"].Value;
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return
				from menuNote in _repository.GetAll().Where(x => x.MenuCode == menuCode && x.ReferenceId == referenceId)
				from menuNoteIdentifier in _menuNoteIdentifierService.GetAll().Where(x => x.MenuNoteIdentifierId == menuNote.MenuNoteIdentifierId)
				from columnIdentifier in _columnIdentifierService.GetColumnIdentifiersDropDown().Where(x => x.ColumnIdentifierId == menuNoteIdentifier.ColumnIdentifierId)
				select new MenuNoteDto()
				{
					MenuNoteId = menuNote.MenuNoteId,
					MenuCode = menuNote.MenuCode,
					ReferenceId = menuNote.ReferenceId,
					Note = menuNote.Note,
					NoteValue = menuNote.NoteValue,
					ShowInReports = menuNote.ShowInReports,
					ShowOnPrint = menuNote.ShowOnPrint,
					ShowOnSelection = menuNote.ShowOnSelection,
					MenuNoteIdentifierId = menuNote.MenuNoteIdentifierId,
					MenuNoteIdentifierName = language == LanguageCode.Arabic ? menuNoteIdentifier.MenuNoteIdentifierNameAr : menuNoteIdentifier.MenuNoteIdentifierNameEn,
					ColumnIdentifierName = columnIdentifier.ColumnIdentifierName,
					ColumnIdentifierId = columnIdentifier.ColumnIdentifierId,
					NoteValueReadable = MenuNoteLogic.GetMenuNoteReadableValue(columnIdentifier.ColumnIdentifierId, menuNote.NoteValue,trueValue,falseValue)
				};
		}

		public async Task<ResponseDto> SaveMenuNotes(List<MenuNoteDto> notes, int referenceId)
		{
			if (notes.Any())
			{
				await DeleteNotes(notes, referenceId);
				await AddNotes(notes, referenceId);
				await EditNotes(notes, referenceId);
				return new ResponseDto() { Success = true, Message = _localizer["NotesSavedSuccessfully"] };
			}
			return new ResponseDto() { Success = false, Message = _localizer["NothingToBeSaved"] };
		}

		public async Task<ResponseDto> DeleteMenuNotes(int menuCode, int referenceId)
		{
			var data = await _repository.GetAll().Where(x => x.ReferenceId == referenceId && x.MenuCode == menuCode).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return new ResponseDto() { Success = true, Message = _localizer["NotesDeletedSuccessfully"] };
			}
			return new ResponseDto() { Success = false, Message = _localizer["NothingToBeDeleted"] };
		}

		public async Task<bool> DeleteNotes(List<MenuNoteDto> notes, int referenceId)
		{
			if (notes.Any())
			{
				var menuCode = notes[0].MenuCode;
				var currentNotes = _repository.GetAll().Where(x => x.ReferenceId == referenceId && x.MenuCode == menuCode).AsNoTracking().ToList();
				var notesToBeDeleted = currentNotes.Where(p => notes.All(p2 => p2.MenuNoteId != p.MenuNoteId)).ToList();
				if (notesToBeDeleted.Any())
				{
					_repository.RemoveRange(notesToBeDeleted);
					await _repository.SaveChanges();
					return true;
				}
			}
			return false;
		}

		public async Task<bool> AddNotes(List<MenuNoteDto> notes, int referenceId)
		{
			var currentNotes = notes.Where(x => x.MenuNoteId <= 0).ToList();
			var notesList = new List<MenuNote>();
			var newId = await GetNextId();
			foreach (var note in currentNotes)
			{
				var newNote = new MenuNote()
				{
					MenuNoteId = newId,
					ReferenceId = referenceId,
					MenuCode = note.MenuCode,
					NoteValue = note.NoteValue,
					Note = note.Note,
					MenuNoteIdentifierId = note.MenuNoteIdentifierId,
					ShowInReports = note.ShowInReports,
					ShowOnPrint = note.ShowOnPrint,
					ShowOnSelection = note.ShowOnSelection,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
					Hide = false
				};
				notesList.Add(newNote);
				newId++;
			}

			if (notesList.Any())
			{
				await _repository.InsertRange(notesList);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		public async Task<bool> EditNotes(List<MenuNoteDto> notes, int referenceId)
		{
			var currentNotes = notes.Where(x => x.MenuNoteId > 0).ToList();
			var notesList = new List<MenuNote>();
			foreach (var note in currentNotes)
			{
				var newNote = new MenuNote()
				{
					MenuNoteId = note.MenuNoteId,
					ReferenceId = referenceId,
					MenuCode = note.MenuCode,
					NoteValue = note.NoteValue,
					Note = note.Note,
					MenuNoteIdentifierId = note.MenuNoteIdentifierId,
					ShowInReports = note.ShowInReports,
					ShowOnPrint = note.ShowOnPrint,
					ShowOnSelection = note.ShowOnSelection,
					ModifiedAt = DateHelper.GetDateTimeNow(),
					UserNameModified = await _httpContextAccessor!.GetUserName(),
					IpAddressModified = _httpContextAccessor?.GetIpAddress(),
					Hide = false
				};
				notesList.Add(newNote);
			}

			if (notesList.Any())
			{
				_repository.UpdateRange(notesList);
				await _repository.SaveChanges();
				return true;
			}
			return false;

		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.MenuNoteId) + 1; } catch { id = 1; }
			return id;
		}
	}

	public static class MenuNoteLogic
	{
		//public static object? GetMenuNoteValueFromDb(byte columnIdentifierId, string? menuNote)
		//{
		//	if (columnIdentifierId == ColumnIdentifierData.Boolean)
		//	{
		//		return menuNote?.ToString() == "1" ? true : false;

		//	}
		//	return menuNote?.ToString();
		//}
		//public static string? GetMenuNoteValueFromView(byte columnIdentifierId, object? menuNote)
		//{
		//	if (columnIdentifierId == ColumnIdentifierData.Boolean)
		//	{
		//		if (menuNote?.ToString() == "true" || menuNote?.ToString() == "True")
		//		{
		//			return "1";
		//		}
		//		else
		//		{
		//			return "0";
		//		}

		//	}
		//	return menuNote?.ToString();
		//}
		
		public static string? GetMenuNoteReadableValue(byte columnIdentifierId, object? menuNote,string trueValue,string falseValue)
		{
			if (columnIdentifierId == ColumnIdentifierData.Boolean)
			{
				return menuNote?.ToString() == "1" ? trueValue : falseValue;

			}
			return menuNote?.ToString();
		}
	}
}
