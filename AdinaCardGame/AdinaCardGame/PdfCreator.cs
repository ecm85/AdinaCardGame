using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Splendor
{
	public static class PdfCreator
	{
		//page: 8.5x11 = 816 * 1056
		public static void AddPageToPdf(PdfDocument document, IList<Image> images, PageOrientation pageOrientation)
		{
			var firstXImage = XImage.FromGdiPlusImage(images.First());
			var page = document.AddPage();
			page.Size = PageSize.Letter;
			page.Orientation = pageOrientation;
			var pageWidth = page.Width;
			var pageHeight = page.Height;
			const int padding = 7;
			var horizontalWhiteSpace = (pageWidth - (3*firstXImage.PointWidth))/(padding * 2 + 2);
			var verticalWhiteSpace = (pageHeight - (3*firstXImage.PointHeight))/(padding * 2 + 2);
			var xGraphics = XGraphics.FromPdfPage(page);

			for (var index = 0; index < images.Count; index++)
			{
				var xImage = XImage.FromGdiPlusImage(images[index]);

				var row = index % 3;
				var column = index / 3;

				var x = (row * (xImage.PointWidth + horizontalWhiteSpace)) + padding * horizontalWhiteSpace;
				var y = (column * (xImage.PointHeight + verticalWhiteSpace)) + padding * verticalWhiteSpace;

				xGraphics.DrawImage(xImage, x, y);
			}
		}
	}
}
