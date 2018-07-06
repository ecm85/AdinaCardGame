using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace AdinaCardGame
{
	class Program
	{
		private static bool useOverlay = false;
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

			if (useOverlay)
			{
                //TODO: Make this path configurable, and figure out what to use for it
				var overlay = new Bitmap("c:\\delete\\poker-card.png");
				overlay.SetResolution(300, 300);
				var matrix = new ColorMatrix {Matrix33 = .5f};
				var attributes = new ImageAttributes();
				attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				foreach (var image in allImages)
				{
					var graphics = Graphics.FromImage(image.Image);
					graphics.DrawImage(overlay, new Rectangle(0, 0, overlay.Width, overlay.Height), 0, 0, overlay.Width, overlay.Height,
						GraphicsUnit.Pixel, attributes);
                //TODO: Make this path configurable anbd alsways put images in a timestamped subfolder 
					image.Image.Save($"c:\\delete\\images\\{image.Name}.png", ImageFormat.Png);
				}
			}
			else
			{
				foreach (var image in allImages)
                //TODO: Make this path configurable anbd alsways put images in a timestamped subfolder 
					image.Image.Save($"c:\\delete\\images\\{image.Name}.png", ImageFormat.Png);
			}
		}

	    private static IEnumerable<string> LoadAnswerCards()
	    {
            //TODO: Load words from file, one per line, from configurable location
	        return new [] {"First Answer", "Second Answer"};
	    }

	    private static IEnumerable<string> LoadPromptCards()
	    {
            //TODO: Load words from file, one per line, from configurable location
	        return new [] { "In a world ravaged by __________ our only solace is __________.", "Second Prompt" };
	    }
	}
}
