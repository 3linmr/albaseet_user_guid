using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
	public class ItemDivisionDto
	{

	}

	public class ItemCategoryDto
	{
		public int ItemCategoryId { get; set; }
		public int ItemCategoryCode { get; set; }
		public string? CategoryNameAr { get; set; }
		public string? CategoryNameEn { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}

	public class ItemCategoryDropDownDto
	{
		public int ItemCategoryId { get; set; }
		public string? CategoryName { get; set; }
	}

	public class ItemSubCategoryDto
	{
		public int ItemSubCategoryId { get; set; }
		public int ItemSubCategoryCode { get; set; }
		public string? SubCategoryNameAr { get; set; }
		public string? SubCategoryNameEn { get; set; }
		public string? SubCategoryName { get; set; }
		public int ItemCategoryId { get; set; }
		public string? ItemCategoryName { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}
	public class ItemSubCategoryDropDownDto
	{
		public int ItemCategoryId { get; set; }
		public int ItemSubCategoryId { get; set; }
		public string? SubCategoryName { get; set; }
	}
	public class ItemSectionDto
	{
		public int ItemSectionId { get; set; }
		public int ItemSectionCode { get; set; }
		public string? SectionNameAr { get; set; }
		public string? SectionNameEn { get; set; }
		public string? SectionName { get; set; }
		public int ItemSubCategoryId { get; set; }
		public string? ItemSubCategoryName { get; set; }
		public int ItemCategoryId { get; set; }
		public string? ItemCategoryName { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}
	public class ItemSectionDropDownDto
	{
		public int ItemSubCategoryId { get; set; }
		public int ItemSectionId { get; set; }
		public string? ItemSectionName { get; set; }
	}
	public class ItemSubSectionDto
	{
		public int ItemSubSectionId { get; set; }
		public int ItemSubSectionCode { get; set; }
		public string? SubSectionNameAr { get; set; }
		public string? SubSectionNameEn { get; set; }
		public string? ItemSubSectionName { get; set; }
		public int ItemSectionId { get; set; }
		public string? ItemSectionName { get; set; }
		public int ItemSubCategoryId { get; set; }
		public string? ItemSubCategoryName { get; set; }
		public int ItemCategoryId { get; set; }
		public string? ItemCategoryName { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}
	public class ItemSubSectionDropDownDto
	{
		public int ItemSectionId { get; set; }
		public int ItemSubSectionId { get; set; }
		public string? ItemSubSectionName { get; set; }
	}
	public class MainItemDto
	{
		public int MainItemId { get; set; }
		public int MainItemCode { get; set; }
		public string? MainItemNameAr { get; set; }
		public string? MainItemNameEn { get; set; }
		public string? MainItemName { get; set; }
		public int ItemSubSectionId { get; set; }
		public string? ItemSubSectionName { get; set; }
		public int ItemSectionId { get; set; }
		public string? ItemSectionName { get; set; }
		public int ItemSubCategoryId { get; set; }
		public string? ItemSubCategoryName { get; set; }
		public int ItemCategoryId { get; set; }
		public string? ItemCategoryName { get; set; }
        public int CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}
	public class MainItemDropDownDto
	{
		public int ItemSubSectionId { get; set; }
		public int MainItemId { get; set; }
		public string? MainItemName { get; set; }
	}
	public class ItemDivisionNamesDto
	{
		public string? ItemCategoryName { get; set; }
		public string? ItemCategoryNameAr { get; set; }
		public string? ItemCategoryNameEn { get; set; }
		public string? ItemCategoryNameSelect { get; set; }
		public string? ItemCategoryNameSelectAr { get; set; }
		public string? ItemCategoryNameSelectEn { get; set; }
		public string? ItemCategoryNameFirst { get; set; }
		public string? ItemCategoryNameFirstAr { get; set; }
		public string? ItemCategoryNameFirstEn { get; set; }
		public string? ItemSubCategoryName { get; set; }
		public string? ItemSubCategoryNameAr { get; set; }
		public string? ItemSubCategoryNameEn { get; set; }
		public string? ItemSubCategoryNameSelect { get; set; }
		public string? ItemSubCategoryNameSelectAr { get; set; }
		public string? ItemSubCategoryNameSelectEn { get; set; }
		public string? ItemSubCategoryNameFirst { get; set; }
		public string? ItemSubCategoryNameFirstAr { get; set; }
		public string? ItemSubCategoryNameFirstEn { get; set; }
		public string? ItemSectionName { get; set; }
		public string? ItemSectionNameAr { get; set; }
		public string? ItemSectionNameEn { get; set; }
		public string? ItemSectionNameSelect { get; set; }
		public string? ItemSectionNameSelectAr { get; set; }
		public string? ItemSectionNameSelectEn { get; set; }
		public string? ItemSectionNameFirst { get; set; }
		public string? ItemSectionNameFirstAr { get; set; }
		public string? ItemSectionNameFirstEn { get; set; }
		public string? ItemSubSectionName { get; set; }
		public string? ItemSubSectionNameAr { get; set; }
		public string? ItemSubSectionNameEn { get; set; }
		public string? ItemSubSectionNameSelect { get; set; }
		public string? ItemSubSectionNameSelectAr { get; set; }
		public string? ItemSubSectionNameSelectEn { get; set; }
		public string? ItemSubSectionNameFirst { get; set; }
		public string? ItemSubSectionNameFirstAr { get; set; }
		public string? ItemSubSectionNameFirstEn { get; set; }
		public string? MainItemName { get; set; }
		public string? MainItemNameAr { get; set; }
		public string? MainItemNameEn { get; set; }
		public string? MainItemNameSelect { get; set; }
		public string? MainItemNameSelectAr { get; set; }
		public string? MainItemNameSelectEn { get; set; }
	}
}
