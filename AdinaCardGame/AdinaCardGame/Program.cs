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
            //TODO: Ignore comments in file
	        return new [] {"First Answer", "Second Answer"};
	    }

	    private static IEnumerable<string> LoadPromptCards()
	    {
            //TODO: Load words from file, one per line, from configurable location
            //TODO: Ignore comments in file
	        return new[]
            {
	            "In a world ravaged by @_@ our only solace is that we can use is a long line here that is multiple lines of lorem ipsum stuff @pre-_-post.@",
	            "@Step 1: _.@ @Step 2: _.@ Step 3: Profit",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            };
	    }
	}
}
