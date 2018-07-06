using System;
using System.Drawing;

namespace AdinaCardGame
{
	public class ImageCreator
	{
		const float DpiFactor = 300.0f / 96;

        //TODO: Make configurable
		private const float CardShortSideInInches = 2.5f;
        //TODO: Make configurable
		private const float CardLongSideInInches = 3.5f;

        //TODO: Make configurable
		private const float BleedSizeInInches = .25f;

		private const float CardShortSideInInchesWithBleed = CardShortSideInInches + BleedSizeInInches;
		private const float CardLongSideInInchesWithBleed = CardLongSideInInches + BleedSizeInInches;

		//card: 2.5x3.5 = 240 * 336
		private const int Dpi = (int)(96 * DpiFactor);
		private const int CardShortSideInPixels = (int)(Dpi * CardShortSideInInches);
		private const int CardLongSideInPixels = (int)(Dpi * CardLongSideInInches);

		private const int CardShortSideInPixelsWithBleed = (int)(Dpi * CardShortSideInInchesWithBleed);
		private const int CardLongSideInPixelsWithBleed = (int)(Dpi * CardLongSideInInchesWithBleed);

        //TODO: Make configurable
		private readonly FontFamily promptFontFamily = new FontFamily("Arial");

		private readonly StringFormat horizontalNearAlignment = new StringFormat {Alignment = StringAlignment.Near};

		private readonly Point origin = new Point((int) (BleedSizeInInches * Dpi / 2), (int) (BleedSizeInInches * Dpi / 2));

        //TODO: Make configurable
		private const int BorderRadius = 40;

        //TODO: Make configurable
	    private const int BorderPadding = (int)(25 * DpiFactor);

		private const int PromptTextFontSize = (int) (13 * DpiFactor);

        //TODO: Potentially use these when making logo at bottom of cards
		//private const int resourceKeyImageSize = (int) (35 * DpiFactor);
		//private const int arrowImageSize = (int) (10 * DpiFactor);
		//private const int questCostImageSize = (int) (35 * DpiFactor);
		//private const int pentagonImageSize = (int) (25 * DpiFactor);
		//private const int wreathImageWidth = (int) (40 * DpiFactor);
		//private const int cardFrontSmallImageSize = (int) (35 * DpiFactor);
		//private const int questImageYBottomPadding = (int) (5 * DpiFactor);

        //TODO: Make configurable
	    private const string PromptCardFrontBackgroundColorText = "0, 0, 0";
        //TODO: Make configurable
	    private const string PromptCardFrontTextColorText = "255, 255, 255";
        //TODO: Make configurable
	    private const string AnswerCardFrontBackgroundColorText = "255, 255, 255";


	    private Bitmap CreateBitmap(ImageOrientation orientation)
	    {
	        switch (orientation)
	        {
	            case ImageOrientation.Landscape:
	                return CreateBitmap(CardLongSideInPixelsWithBleed, CardShortSideInPixelsWithBleed);
	            case ImageOrientation.Portrait:
	                return CreateBitmap(CardShortSideInPixelsWithBleed, CardLongSideInPixelsWithBleed);
	        }
	        return null;
	    }

	    private Bitmap CreateBitmap(int width, int height)
	    {
	        var bitmap = new Bitmap(width, height);
	        bitmap.SetResolution(Dpi, Dpi);
	        return bitmap;
	    }

	    public Image CreatePromptCardFront(string promptCard)
	    {
			var bitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(bitmap);
	        var promptCardFrontBackgroundColor = ParseColorText(PromptCardFrontBackgroundColorText);
	        var promptCardFrontTextColor = ParseColorText(PromptCardFrontTextColorText);
	        PrintCardBack(graphics, ImageOrientation.Portrait, promptCardFrontBackgroundColor);
	        var promptFont = new Font(promptFontFamily, PromptTextFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            graphics.DrawString(
                origin,
	            promptCard,
                promptFont,
                new SolidBrush(promptCardFrontTextColor),
	            new RectangleF(BorderPadding, BorderPadding, CardShortSideInPixels - 2 * BorderPadding, CardLongSideInPixels - 2 * BorderPadding),
                horizontalNearAlignment);
            //TODO: Handle new lines for ___'s
            //TODO: Find max font size that fits
            //TODO: Add logo

            return bitmap;
	    }

	    public Image CreateAnswerCardFront(string answerCard)
	    {
	        var bitmap = CreateBitmap(ImageOrientation.Portrait);
	        var graphics = Graphics.FromImage(bitmap);
	        var answerCardFrontBackgroundColor = ParseColorText(AnswerCardFrontBackgroundColorText);
	        PrintCardBack(graphics, ImageOrientation.Portrait, answerCardFrontBackgroundColor);
            //TODO: Print answerCard
            //TODO: Find max font size that fits
            //TODO: Add logo
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
	        var topSideInPixelsWithBleed = orientation == ImageOrientation.Landscape ? CardLongSideInPixelsWithBleed : CardShortSideInPixelsWithBleed;
	        var leftSideInPixelsWithBleed = orientation == ImageOrientation.Portrait ? CardLongSideInPixelsWithBleed : CardShortSideInPixelsWithBleed;

	        graphics.FillRoundedRectangle(
	            new SolidBrush(backgroundColor),
	            0,
	            0,
	            topSideInPixelsWithBleed,
	            leftSideInPixelsWithBleed,
	            BorderRadius);
	    }
    }
}
