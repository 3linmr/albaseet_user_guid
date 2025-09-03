using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using ExifTag = SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag;
using IExifValue = SixLabors.ImageSharp.Metadata.Profiles.Exif.IExifValue;
using Size = System.Drawing.Size;

namespace Shared.Service.Helpers.Algorithms
{
	public static class CompressionAlgorithm
	{
		public static byte[] CompressImage(IFormFile inputImage)
		{
			byte[] result = null;

			var memoryStream = new MemoryStream();

			using (var image = Image.Load(inputImage.OpenReadStream()))
			{
				var before = memoryStream.Length;

				var beforeMutations = image.Size;

				var size = ResizeKeepAspect(beforeMutations.Width, beforeMutations.Height, 1300, 1300);

				IResampler sampler = KnownResamplers.Lanczos3;
				ResizeMode mode = ResizeMode.Max;

				var resizeOptions = new ResizeOptions
				{
					Size = new SixLabors.ImageSharp.Size(size.Width, size.Height),
					Sampler = sampler,
					Compand = true,
					Mode = mode
				};

				image.Mutate(x => x.Resize(resizeOptions));
				TransformImage(image);

				var afterMutations = image.Size;

				var encoder = new JpegEncoder() { Quality = 70 };

				image.Save(memoryStream, encoder);
				memoryStream.Position = 0;

				result = memoryStream.ToArray();

				var after = memoryStream.Length;
			}
			result = memoryStream.ToArray();

			return result;
		}

		private static void TransformImage(Image image)
		{
			if (image.Metadata?.ExifProfile is null)
				return;

			// Use TryGetValue to safely get the orientation value.
			if (!image.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out IExifValue<ushort>? exifOrientation))
				return;

			SetRotateFlipMode(exifOrientation, out var rotateMode, out var flipMode);

			image.Mutate(x => x.RotateFlip(rotateMode, flipMode));
			// Reset the orientation tag to 1 after transformation.
			image.Metadata.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
		}

		private static void SetRotateFlipMode(IExifValue exifOrientation, out RotateMode rotateMode, out FlipMode flipMode)
		{
			var orientation = (ushort)exifOrientation.GetValue();

			switch (orientation)
			{
				case 2:
					rotateMode = RotateMode.None;
					flipMode = FlipMode.Horizontal;
					break;
				case 3:
					rotateMode = RotateMode.Rotate180;
					flipMode = FlipMode.None;
					break;
				case 4:
					rotateMode = RotateMode.Rotate180;
					flipMode = FlipMode.Horizontal;
					break;
				case 5:
					rotateMode = RotateMode.Rotate90;
					flipMode = FlipMode.Horizontal;
					break;
				case 6:
					rotateMode = RotateMode.Rotate90;
					flipMode = FlipMode.None;
					break;
				case 7:
					rotateMode = RotateMode.Rotate90;
					flipMode = FlipMode.Vertical;
					break;
				case 8:
					rotateMode = RotateMode.Rotate270;
					flipMode = FlipMode.None;
					break;
				default:
					rotateMode = RotateMode.None;
					flipMode = FlipMode.None;
					break;
			}
		}

		private static Size ResizeKeepAspect(int width, int height, int maxWidth, int maxHeight)
		{
			double dbl = (double)width / (double)height;
			var newWidth = (int)((double)maxWidth * dbl);
			var newHeight = (int)((double)maxHeight / dbl);
			return new Size(newWidth, newHeight);
		}
	}
}
