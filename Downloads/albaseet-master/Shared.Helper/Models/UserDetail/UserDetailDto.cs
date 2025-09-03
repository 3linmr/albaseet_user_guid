using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.Helper.Models.UserDetail
{
    public class UserDetailDto
    {

    }

    public class UserDataDto
    {
	    public string? UserId { get; set; }
	    public string? UserName { get; set; }
	    public string? NameAr { get; set; }
	    public string? NameEn { get; set; }
	    public string? Email { get; set; }
	    public string? PhoneNumber { get; set; }
	    public int UserSerial { get; set; }
	    public string? PinCode { get; set; }
	    public bool IsSystemAdmin { get; set; }
	    public int? CompanyId { get; set; }
	    public string? CompanyName { get; set; }
	    public int? EmployeeId { get; set; }
	    public string? LanguageCode { get; set; }
	    public DateTime? BirthDate { get; set; }
	    public bool Active { get; set; }
	    public List<MenuDto>? Menus { get; set; }
	    public List<MenuDto>? PlainMenus { get; set; }
	    public List<DocumentDto>? Documents { get; set; }
    }

	public class UserBasicInfoDto
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int UserSerial { get; set; }
        public string? PinCode { get; set; }
        public bool IsSystemAdmin { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int? EmployeeId { get; set; }
        public string? LanguageCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool Active { get; set; }
    }

    public class MenuDto
    {
        public int MenuId { get; set; }
        public int MenuCode { get; set; }
        public string? MenuName { get; set; }
        public string? MenuNameAr { get; set; }
        public string? MenuNameEn { get; set; }
        public int? MainMenuId { get; set; }
        public int? Section { get; set; }
        public bool? Show { get; set; }
        public int? ApplicationId { get; set; }
        public string? Icon { get; set; }
        public string? Image { get; set; }
        public bool IsFavorite { get; set; }
        public int MenuOrder { get; set; }
        public string? MenuUrl { get; set; }
        public bool IsMain { get; set; }
        public bool HasApprove { get; set; }

        public List<MenuDto> items { get; set; }
    }

    public class DocumentDto
    {
	    public int MenuCode { get; set; }
	    public string? MenuName { get; set; }
	    public string? MenuNameAr { get; set; }
	    public string? MenuNameEn { get; set; }
    }
	public class MenuVm
    {
        public int id { get; set; }
        public string text { get; set; }
        public int? MainMenu { get; set; }
        public bool? Show { get; set; }
        public int? ApplicationId { get; set; }
        public string? Icon { get; set; }
        public bool? IsFavorite { get; set; }
        public int? MenuOrder { get; set; }
        public string? MenuUrl { get; set; }
        public string? MenuText { get; set; }
        public bool? IsMain { get; set; }

        public bool IsGroup { get; set; }
        public bool selected { get; set; }
        public List<MenuVm> children { get; set; }
    }
    public class UserLanguageDto
    {
        public string? UserId { get; set; }
        public string? LanguageCode { get; set; }
    }

    public class MenuSearchDto
    {
	    public string? MenuName { get; set; }
	    public string? Label { get; set; }
	    public string? Route { get; set; }
    }

    public class UserFlagDto
    {
	    public int UserFlagId { get; set; }

	    public string? UserId { get; set; }

	    public short ApplicationId { get; set; }
	    public int? MenuCode { get; set; }

	    public short? ApplicationIdentifierId { get; set; } //Store

	    public int? ApplicationIdentifierValue { get; set; } //StoreId

	    public int FlagId { get; set; }

	    public string? FlagValue { get; set; }
    }

    public class UserRoleDto
    {
	    public string? UserId { get; set; }
	    public string? RoleId { get; set; }
	    public string? Name { get; set; }
	    public string? RoleName { get; set; }
    }

    public class SubscriptionBusinessCountDto
    {
	    public short BusinessCount { get; set; }

	    public short StoresCount { get; set; }

	    public bool HasApprovalSystem { get; set; }
    }

    public class UserValidDto
    {
	    public bool IsValid { get; set; }
	    public string? UserRole { get; set; }
	    public string? Message { get; set; }
    }
}