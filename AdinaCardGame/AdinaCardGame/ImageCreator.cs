using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace AdinaCardGame
{
	public class ImageCreator
	{
		const float dpiFactor = 300.0f / 96;


		private const float cardShortSideInInches = 2.5f;
		private const float cardLongSideInInches = 3.5f;

		private const float bleedSizeInInches = .25f;

		private const float cardShortSideInInchesWithBleed = cardShortSideInInches + bleedSizeInInches;
		private const float cardLongSideInInchesWithBleed = cardLongSideInInches + bleedSizeInInches;

		//card: 2.5x3.5 = 240 * 336
		private const int dpi = (int)(96 * dpiFactor);
		private const int cardShortSideInPixels = (int)(dpi * cardShortSideInInches);
		private const int cardLongSideInPixels = (int)(dpi * cardLongSideInInches);

		private const int cardShortSideInPixelsWithBleed = (int)(dpi * cardShortSideInInchesWithBleed);
		private const int cardLongSideInPixelsWithBleed = (int)(dpi * cardLongSideInInchesWithBleed);

		private readonly Color standardCardBackgroundColor = Color.BurlyWood;
		private const string questBackText = "Quest";
		private readonly FontFamily headerFontFamily = new FontFamily("Tempus Sans ITC");
		private static readonly FontFamily bodyFontFamily = new FontFamily("Calibri");
		private static readonly FontFamily cardBackFontFamily = new FontFamily("Cambria");

		private readonly StringFormat horizontalCenterAlignment = new StringFormat { Alignment = StringAlignment.Center};
		private readonly StringFormat fullCenterAlignment = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
		private readonly StringFormat horizontalNearAlignment = new StringFormat {Alignment = StringAlignment.Near};
		private readonly StringFormat horizontalFarAlignment = new StringFormat {Alignment = StringAlignment.Far};
		private readonly SolidBrush blackBrush = new SolidBrush(Color.Black);

		private Point origin = new Point((int) (bleedSizeInInches * dpi / 2), (int) (bleedSizeInInches * dpi / 2));

		private const int borderRadius = 40;

		private const float textOutlineWidth = .5f * dpiFactor;

		private const int limitsFontSize = (int) (10 * dpiFactor);
		private const int bodyFontSize = (int) (11 * dpiFactor);
		private const int questHeaderFontSize = (int) (13 * dpiFactor);
		private const int toolHeaderFontSize = (int) (20 * dpiFactor);
		private const int imageLabelFontSize = (int) (20 * dpiFactor);
		private const int gameTitleFontSize = (int) (38 * dpiFactor);
		private const int questBackFontSize = (int) (45 * dpiFactor);
		private const int tierTextFontSize = (int) (80 * dpiFactor);

		private const int resourceKeyImageSize = (int) (35 * dpiFactor);
		private const int arrowImageSize = (int) (10 * dpiFactor);
		private const int questCostImageSize = (int) (35 * dpiFactor);
		private const int pentagonImageSize = (int) (25 * dpiFactor);
		private const int wreathImageWidth = (int) (40 * dpiFactor);
		private const int cardFrontSmallImageSize = (int) (35 * dpiFactor);
		private const int questImageYBottomPadding = (int) (5 * dpiFactor);
	    private const string PromptCardFrontBackgroundColorText = "0, 0, 0";
	    private const string AnswerCardFrontBackgroundColorText = "255, 255, 255";

	    private static int ArrowPadding => arrowImageSize / 2;

	    private Bitmap CreateBitmap(ImageOrientation orientation)
	    {
	        switch (orientation)
	        {
	            case ImageOrientation.Landscape:
	                return CreateBitmap(cardLongSideInPixelsWithBleed, cardShortSideInPixelsWithBleed);
	            case ImageOrientation.Portrait:
	                return CreateBitmap(cardShortSideInPixelsWithBleed, cardLongSideInPixelsWithBleed);
	        }
	        return null;
	    }

	    private Bitmap CreateBitmap(int width, int height)
	    {
	        var bitmap = new Bitmap(width, height);
	        bitmap.SetResolution(dpi, dpi);
	        return bitmap;
	    }

	    public Image CreatePromptCardFront(string promptCard)
	    {
			var bitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(bitmap);
	        var promptCardFrontBackgroundColor = ParseColorText(PromptCardFrontBackgroundColorText);
	        PrintCardBack(graphics, ImageOrientation.Portrait, promptCardFrontBackgroundColor);
            //TODO: Print promptCard
	        return bitmap;
	    }

	    public Image CreateAnswerCardFront(string answerCard)
	    {
	        var bitmap = CreateBitmap(ImageOrientation.Portrait);
	        var graphics = Graphics.FromImage(bitmap);
	        var answerCardFrontBackgroundColor = ParseColorText(AnswerCardFrontBackgroundColorText);
	        PrintCardBack(graphics, ImageOrientation.Portrait, answerCardFrontBackgroundColor);
            //TODO: Print answerCard
            return bitmap;
	    }

	    private static Color ParseColorText(string colorText)
	    {
	        var tokens = colorText.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
	        var color = Color.FromArgb(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
	        return color;
	    }

        private void PrintCardBack(Graphics graphics, ImageOrientation orientation, Color backgroundColor)
	    {
	        var topSideInPixelsWithBleed = orientation == ImageOrientation.Landscape ? cardLongSideInPixelsWithBleed : cardShortSideInPixelsWithBleed;
	        var leftSideInPixelsWithBleed = orientation == ImageOrientation.Portrait ? cardLongSideInPixelsWithBleed : cardShortSideInPixelsWithBleed;

	        graphics.FillRoundedRectangle(
	            new SolidBrush(backgroundColor),
	            0,
	            0,
	            topSideInPixelsWithBleed,
	            leftSideInPixelsWithBleed,
	            borderRadius);
	    }
    }
}
