using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AdinaCardGame
{
	public class Program
	{
		private static bool UseOverlay => bool.Parse(ConfigurationManager.AppSettings["OverlayTemplate"]);
        //TODO: UI?
		static void Main()
		{
			var imageCreator = new ImageCreator();
		    var promptCards = LoadPromptCards();
		    var answerCards = LoadAnswerCards();

		    var promptCardFrontImages = promptCards
		        .Select(
		            (promptCard, index) => new ImageToSave
		            {
		                Image = imageCreator.CreatePromptCardFront(promptCard),
		                Name = $"Prompt Card {index}"
		            })
		        .ToList();
		    var answerCardFrontImages = answerCards
		        .Select(
		            (answerCard, index) => new ImageToSave
		            {
		                Image = imageCreator.CreateAnswerCardFront(answerCard),
		                Name = $"Answer Card {index}"
		            })
		        .ToList();

            var allImages = promptCardFrontImages
                .Concat(answerCardFrontImages)
		        .ToList();

			if (UseOverlay)
			{
				var overlay = new Bitmap(ConfigurationManager.AppSettings["TemplatePath"]);
				overlay.SetResolution(300, 300);
				var matrix = new ColorMatrix {Matrix33 = .1f};
				var attributes = new ImageAttributes();
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				foreach (var image in allImages)
				{
					var graphics = Graphics.FromImage(image.Image);
					graphics.DrawImage(overlay, new Rectangle(0, 0, overlay.Width, overlay.Height), 0, 0, overlay.Width, overlay.Height,
						GraphicsUnit.Pixel, attributes);
				    
				}
			}
		    var outputPath = ConfigurationManager.AppSettings["OutputPath"];
		    var timeStampedFolder = Path.Combine(
		        outputPath,
		        "Images " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture));
		    Directory.CreateDirectory(timeStampedFolder);
            foreach (var image in allImages)
		        image.Image.Save($"{timeStampedFolder}\\{image.Name}.png", ImageFormat.Png);
		    
		}

	    private static IEnumerable<string> LoadAnswerCards()
	    {
	        var answerCardPath = ConfigurationManager.AppSettings["AnswerCardPath"];
	        return File.ReadAllLines(answerCardPath)
	            .Select(line => line.Trim())
	            .Where(line => !line.StartsWith("//") && !string.IsNullOrWhiteSpace(line))
	            .ToList();
	    }

	    private static IEnumerable<string> LoadPromptCards()
	    {
	        var promptCardPath = ConfigurationManager.AppSettings["PromptCardPath"];
	        return File.ReadAllLines(promptCardPath)
	            .Select(line => line.Trim())
	            .Where(line => !line.StartsWith("//") && !string.IsNullOrWhiteSpace(line))
	            .ToList();
	    }
	}
}
