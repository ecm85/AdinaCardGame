using System;
using System.Drawing;
using System.Linq;

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

	    private const int TopBorderPadding = BorderPadding;
	    private const int RightBorderPadding = BorderPadding;
	    private const int LeftBorderPadding = BorderPadding;
	    private const int BottomBorderPadding = BorderPadding;

        private const int PromptTextFontSize = (int) (20 * DpiFactor);

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

	    public Image CreatePromptCardFront(string promptCardText)
	    {
			var bitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(bitmap);
	        var promptCardFrontBackgroundColor = ParseColorText(PromptCardFrontBackgroundColorText);
	        var promptCardFrontTextColor = ParseColorText(PromptCardFrontTextColorText);
	        PrintCardBackground(graphics, ImageOrientation.Portrait, promptCardFrontBackgroundColor);
	        var promptFont = new Font(promptFontFamily, PromptTextFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
	        var promptCardTokens = promptCardText.Split(new[] {PromptBlankLine.PromptBlankIndicator}, StringSplitOptions.RemoveEmptyEntries)
	            .Select(token => token.Trim())
	            .ToList();

	        var yOffset = (float)TopBorderPadding;
	        foreach (var promptCardToken in promptCardTokens)
	        {
	            yOffset = DrawNextPromptToken(
	                yOffset,
	                promptCardToken,
	                graphics,
	                promptFont,
	                promptCardFrontTextColor);
	        }
	        
            //TODO: Handle new lines for ___'s
            //TODO: Find max font size that fits, with a cap
            //TODO: Add logo

            return bitmap;
	    }

	    private float DrawNextPromptToken(
	        float yOffset,
	        string promptCardToken,
	        Graphics graphics,
	        Font promptFont,
	        Color promptCardFrontTextColor)
	    {
	        var textRectangle = new RectangleF(
	            LeftBorderPadding,
	            yOffset,
	            CardShortSideInPixels - (LeftBorderPadding + RightBorderPadding),
	            CardLongSideInPixels - (yOffset + BottomBorderPadding));
	        var sizeForText = new SizeF(textRectangle.Width, textRectangle.Height);

	        var textToDraw = (promptCardToken.Contains(PromptBlankLine.PromptBlankPlaceholder))
	            ? PadTextWithSpaces(graphics, promptCardToken, promptFont, sizeForText)
	            : promptCardToken;
	        yOffset += graphics.MeasureString(textToDraw, promptFont, sizeForText).Height;

	        graphics.DrawString(
	            origin,
	            textToDraw,
	            promptFont,
	            new SolidBrush(promptCardFrontTextColor),
	            textRectangle,
	            horizontalNearAlignment);
	        return yOffset;
	    }

	    private string PadTextWithSpaces(Graphics graphics, string textToPad, Font font, SizeF sizeForText)
	    {
            if (textToPad.Count(character => character == PromptBlankLine.PromptBlankPlaceholder) > 1)
                throw new InvalidOperationException("Multiple blank placeholders in one line.");
            var promptBlankLine = new PromptBlankLine(textToPad);
	        bool fitsInOneLine;
	        do
	        {
	            promptBlankLine.BlankLength++;
	            graphics.MeasureString(
	                promptBlankLine.FullLineText,
	                font,
	                sizeForText,
	                StringFormat.GenericDefault,
	                out _,
	                out var linesFitted);
	            fitsInOneLine = linesFitted == 1;
	        } while (fitsInOneLine);
	        promptBlankLine.BlankLength--;
            return promptBlankLine.FullLineText;
	    }

	    public Image CreateAnswerCardFront(string answerCard)
	    {
	        var bitmap = CreateBitmap(ImageOrientation.Portrait);
	        var graphics = Graphics.FromImage(bitmap);
	        var answerCardFrontBackgroundColor = ParseColorText(AnswerCardFrontBackgroundColorText);
	        PrintCardBackground(graphics, ImageOrientation.Portrait, answerCardFrontBackgroundColor);
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

        private void PrintCardBackground(Graphics graphics, ImageOrientation orientation, Color backgroundColor)
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
