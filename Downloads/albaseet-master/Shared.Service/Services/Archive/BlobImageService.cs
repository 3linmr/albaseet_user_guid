using Shared.CoreOne.Contracts.Archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.Dtos.ViewModels.Archive;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shared.Service.Helpers.Algorithms;
using System.Buffers.Text;

namespace Shared.Service.Services.Archive
{
    public class BlobImageService : IBlobImageService
	{
		public BlobImageService()
		{
			
		}
		public BlobFileDto UploadImageAsync(IFormFile file)
		{
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
			var maxFileSize = 5 * 1024 * 1024; // 5MB


			if (file == null)
				throw new ArgumentNullException(nameof(file));

			if (file.Length > maxFileSize)
				throw new ArgumentException("File size exceeds 5MB limit.");

			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (!allowedExtensions.Contains(extension))
				throw new ArgumentException("Invalid file type. Only jpg, jpeg, and png are allowed.");

			// Generate a unique file name
			//var uniqueFileName = $"{Guid.NewGuid()}{extension}";
			//byte[] fileBytes;
			//using (var ms = new MemoryStream())
			//{
			//	await file.CopyToAsync(ms);
			//	fileBytes = ms.ToArray();
			//}
			var imageCompressed = CompressionAlgorithm.CompressImage(file);

			var image = new BlobFileDto
			{
				FileName = file.FileName,
				Content = imageCompressed,
				ContentType = file.ContentType,
			};
			return image;
		}

		public string GetImage(byte[] image)
		{
			return Convert.ToBase64String(image);
		}
	}
}
