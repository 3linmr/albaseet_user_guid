using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.Dtos.ViewModels.Archive;

namespace Shared.CoreOne.Contracts.Archive
{
    public interface IBlobImageService
    {
	    BlobFileDto UploadImageAsync(IFormFile file);
	    string GetImage(byte[] image);
	}
}
