using System;
using System.Collections.Generic;
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
	    //TODO: Make configurable
	    private readonly FontFamily answerFontFamily = new FontFamily("Arial");

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

        //TODO: Make configurable
        private const int MaxPromptTextFontSize = (int) (20 * DpiFactor);
	    //TODO: Make configurable
	    private const int MaxAnswerTextFontSize = (int)(20 * DpiFactor);

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
	    //TODO: Make configurable
	    private const string AnswerCardFrontTextColorText = "0, 0, 0";

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
	        var cardTokens = promptCardText.Split(new[] { PromptBlankLine.PromptBlankIndicator }, StringSplitOptions.RemoveEmptyEntries)
	            .Select(token => token.Trim())
	            .ToList();
	        var cardFrontBackgroundColor = ParseColorText(PromptCardFrontBackgroundColorText);
	        var cardFrontTextColor = ParseColorText(PromptCardFrontTextColorText);
	        var maxFontSize = MaxPromptTextFontSize;
	        var fontFamily = promptFontFamily;

            return CreateCardFront(cardFrontBackgroundColor, maxFontSize, cardTokens, fontFamily, cardFrontTextColor);
	    }

	    public Image CreateAnswerCardFront(string answerCard)
	    {
	        var cardTokens = new[] { answerCard };
	        var cardFrontBackgroundColor = ParseColorText(AnswerCardFrontBackgroundColorText);
	        var cardFrontTextColor = ParseColorText(AnswerCardFrontTextColorText);
	        var maxFontSize = MaxAnswerTextFontSize;
	        var fontFamily = answerFontFamily;

	        return CreateCardFront(cardFrontBackgroundColor, maxFontSize, cardTokens, fontFamily, cardFrontTextColor);
	    }

        private Image CreateCardFront(Color cardFrontBackgroundColor, int maxFontSize, IList<string> cardTokens, FontFamily fontFamily,
	        Color cardFrontTextColor)
	    {
	        var bitmap = CreateBitmap(ImageOrientation.Portrait);
	        var graphics = Graphics.FromImage(bitmap);
	        PrintCardBackground(graphics, ImageOrientation.Portrait, cardFrontBackgroundColor);

	        var yOffset = (float) TopBorderPadding;
	        var textFontSize = GetTextFontSize(maxFontSize, cardTokens, yOffset, graphics);
	        var font = new Font(fontFamily, textFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
	        foreach (var cardToken in cardTokens)
	        {
	            yOffset = DrawNextStringToken(
	                yOffset,
	                cardToken,
	                graphics,
	                font,
	                cardFrontTextColor);
	        }

	        //TODO: Add logo

	        return bitmap;
	    }

	    private float GetTextFontSize(float maxFontSize, IList<string> promptCardTokens, float yOffset, Graphics graphics)
	    {
	        var availableHeight = CardLongSideInPixels - (yOffset + BottomBorderPadding);
	        float heightAtNextAttempt;
	        var nextAttempt = maxFontSize;
            do
            {
                heightAtNextAttempt = GetHeightForPromptCardAtFontSize(promptCardTokens, graphics, nextAttempt);
                if (heightAtNextAttempt > availableHeight)
                    nextAttempt--;
            } while (heightAtNextAttempt > availableHeight);

	        return nextAttempt;
	    }

	    private float GetHeightForPromptCardAtFontSize(IList<string> promptCardTokens, Graphics graphics, float fontSize)
	    {
	        var availableWidth = CardShortSideInPixels - (LeftBorderPadding + RightBorderPadding);
	        var size = new SizeF(
	            availableWidth,
	            float.MaxValue);
	        var maxFont = new Font(promptFontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
	        return promptCardTokens
	            .Sum(
	                promptCardToken =>
	                    graphics.MeasureString(promptCardToken, maxFont, size).Height);
	    }

	    private float DrawNextStringToken(
	        float yOffset,
	        string cardToken,
	        Graphics graphics,
	        Font font,
	        Color textColor)
	    {
	        var textRectangle = new RectangleF(
	            LeftBorderPadding,
	            yOffset,
	            CardShortSideInPixels - (LeftBorderPadding + RightBorderPadding),
	            CardLongSideInPixels - (yOffset + BottomBorderPadding));
	        var sizeForText = new SizeF(textRectangle.Width, textRectangle.Height);

	        var textToDraw = cardToken.Contains(PromptBlankLine.PromptBlankPlaceholder)
	            ? PadTextWithSpaces(graphics, cardToken, font, sizeForText)
	            : cardToken;
	        yOffset += graphics.MeasureString(textToDraw, font, sizeForText).Height;

	        graphics.DrawString(
	            origin,
	            textToDraw,
	            font,
	            new SolidBrush(textColor),
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
