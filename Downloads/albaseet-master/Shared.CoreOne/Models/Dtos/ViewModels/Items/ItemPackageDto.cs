using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
    public class ItemPackageDto
    {
        public int ItemPackageId { get; set; }
        public int ItemPackageCode { get; set; }
        public string? PackageNameAr { get; set; }
        public string? PackageNameEn { get; set; }
        public string? PackageCode { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public string? UserNameModified { get; set; }
    }

    public class ItemPackageDropDownDto
    {
        public int ItemPackageId { get; set; }
        public string? ItemPackageName { get; set; }
    }

    public class ItemPackageInfoDto
    {
	    public int ItemPackageId { get; set; }
	    public int? MainPackageId { get; set; }
	    public bool IsFirstNode { get; set; }
	    public bool IsLastNode { get; set; }
	    public bool IsParent { get; set; }
	}
    public class PackageTreeDto
    {
	    public int PackageId { get; set; }
	    public int MainPackageId { get; set; }
	    public decimal Packing { get; set; }
	    public List<PackageTreeDto>? Items { get; set; }
    }

    public class ItemPackageVm
    {
	    public int ItemPackageId { get; set; }
	    public string? ItemPackageName { get; set; }
	    public decimal Packing { get; set; }
	}

    public class ItemPackageLevelDto
    {
	    public int ItemPackageId { get; set; }
	    public int? MainItemPackageId { get; set; }
	    public string? ItemPackageName { get; set; }
	    public int Level { get; set; }
	    public bool IsFirstLevel { get; set; }
	    public bool IsSecondLevel { get; set; }
	    public bool IsLastLevel { get; set; }
    }

	public class ItemPackageSiblingDto
    {
	    public int FromPackageId { get; set; }
	    public string? FromPackageName { get; set; }
	    public int ToPackageId { get; set; }
	    public string? ToPackageName { get; set; }
		public decimal Packing { get; set; }
    }
}
