using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using Svg;

namespace AdinaCardGame
{
	public class ImageCreator
	{
	    public ImageCreator(
	        float cardWidthInInches,
	        float cardHeightInInches,
	        float bleedSizeInInches,
	        string promptFontFamily,
	        string answerFontFamily,
	        float borderRadius,
	        float borderPaddingInInches,
	        float maxPromptTextFontSize,
	        float maxAnswerTextFontSize,
	        Color promptCardFrontBackgroundColor,
	        Color promptCardFrontTextColor,
	        Color answerCardFrontBackgroundColor,
	        Color answerCardFrontTextColor)
	    {
	        CardWidthInInches = cardWidthInInches;
	        CardHeightInInches = cardHeightInInches;
	        BleedSizeInInches = bleedSizeInInches;
	        BorderRadius = borderRadius;
	        BorderPaddingInInches = borderPaddingInInches;
	        MaxPromptTextFontSize = maxPromptTextFontSize;
	        MaxAnswerTextFontSize = maxAnswerTextFontSize;
	        PromptCardFrontBackgroundColor = promptCardFrontBackgroundColor;
	        PromptCardFrontTextColor = promptCardFrontTextColor;
	        AnswerCardFrontBackgroundColor = answerCardFrontBackgroundColor;
	        AnswerCardFrontTextColor = answerCardFrontTextColor;
	        PromptFontFamily = new FontFamily(promptFontFamily);
	        AnswerFontFamily = new FontFamily(answerFontFamily);
	    }
		const float DpiFactor = 300.0f / 96;

	    private float CardWidthInInches { get; }
		private float CardHeightInInches { get; }

	    private float BleedSizeInInches { get; }

		private float CardWidthInInchesWithBleed => CardWidthInInches + BleedSizeInInches;
		private float CardHeightInInchesWithBleed => CardHeightInInches + BleedSizeInInches;

		private const int Dpi = (int)(96 * DpiFactor);
		private int CardWidthInPixels => (int)(Dpi * CardWidthInInches);
		private int CardHeightInPixels => (int)(Dpi * CardHeightInInches);

		private int CardWidthInPixelsWithBleed => (int)(Dpi * CardWidthInInchesWithBleed);
		private int CardHeightInPixelsWithBleed => (int)(Dpi * CardHeightInInchesWithBleed);

		private FontFamily PromptFontFamily { get; }
	    private FontFamily AnswerFontFamily { get; }

		private Point Origin => new Point((int) (BleedSizeInInches * Dpi), (int) (BleedSizeInInches * Dpi));

	    private float BorderRadius { get; }

        private int BorderPadding => (int)(BorderPaddingInInches * Dpi);

	    private float BorderPaddingInInches { get; }

	    private int TopBorderPadding => BorderPadding;
	    private int RightBorderPadding => BorderPadding;
	    private int LeftBorderPadding => BorderPadding;
	    private int BottomBorderPadding => BorderPadding;

        private int MaxPromptTextFontSizeInDpi => (int) (MaxPromptTextFontSize * DpiFactor);

	    private float MaxPromptTextFontSize { get; }

	    private int MaxAnswerTextFontSizeInDpi => (int)(MaxAnswerTextFontSize * DpiFactor);

	    private float MaxAnswerTextFontSize { get; }

	    //TODO: Potentially use these when making logo at bottom of cards
        //private const int resourceKeyImageSize = (int) (35 * DpiFactor);
        //private const int arrowImageSize = (int) (10 * DpiFactor);
        //private const int questCostImageSize = (int) (35 * DpiFactor);
        //private const int pentagonImageSize = (int) (25 * DpiFactor);
        //private const int wreathImageWidth = (int) (40 * DpiFactor);
        //private const int cardFrontSmallImageSize = (int) (35 * DpiFactor);
        //private const int questImageYBottomPadding = (int) (5 * DpiFactor);

        private Color PromptCardFrontBackgroundColor { get; }
	    private Color PromptCardFrontTextColor { get; }
	    private Color AnswerCardFrontBackgroundColor { get; }
        private Color AnswerCardFrontTextColor { get; }

        private Bitmap CreateBitmap(ImageOrientation orientation)
	    {
	        switch (orientation)
	        {
	            case ImageOrientation.Landscape:
	                return CreateBitmap(CardHeightInPixelsWithBleed, CardWidthInPixelsWithBleed);
	            case ImageOrientation.Portrait:
	                return CreateBitmap(CardWidthInPixelsWithBleed, CardHeightInPixelsWithBleed);
	        }
	        return null;
	    }

	    private Bitmap CreateBitmap(int width, int height)
	    {
	        var bitmap = new Bitmap(width, height, PixelFormat.Format16bppRgb555);
	        bitmap.SetResolution(Dpi, Dpi);
	        return bitmap;
	    }

	    public SvgDocument CreatePromptCardFront(string promptCardText)
	    {
	        var cardTokens = promptCardText.Split(new[] { PromptBlankLine.PromptBlankIndicator }, StringSplitOptions.RemoveEmptyEntries)
	            .Select(token => token.Trim())
	            .ToList();
	        var maxFontSize = MaxPromptTextFontSizeInDpi;
	        var fontFamily = PromptFontFamily;

            return CreateCardFront(PromptCardFrontBackgroundColor, maxFontSize, cardTokens, fontFamily, PromptCardFrontTextColor);
	    }

	    public SvgDocument CreateAnswerCardFront(string answerCard)
	    {
	        var cardTokens = new[] { answerCard };
	        var maxFontSize = MaxAnswerTextFontSizeInDpi;
	        var fontFamily = AnswerFontFamily;

	        return CreateCardFront(AnswerCardFrontBackgroundColor, maxFontSize, cardTokens, fontFamily, AnswerCardFrontTextColor);
	    }

	    private SvgDocument CreateCardFront(
	        Color cardFrontBackgroundColor,
	        int maxFontSize,
	        IList<string> cardTokens,
	        FontFamily fontFamily,
	        Color cardFrontTextColor)
	    {
	        using (var bitmap = CreateBitmap(ImageOrientation.Portrait))
	        {
	            var document = new SvgDocument
	            {
	                ViewBox = new SvgViewBox(0, 0, CardWidthInPixelsWithBleed, CardHeightInPixelsWithBleed)
	            };
	            using (var graphics = Graphics.FromImage(bitmap))
	            {
	                PrintCardBackground(document, cardFrontBackgroundColor);

	                var yOffset = (float) TopBorderPadding;
	                var textFontSize = GetTextFontSize(maxFontSize, cardTokens, yOffset, graphics, fontFamily);
	                foreach (var cardToken in cardTokens)
	                {
	                    yOffset = DrawNextStringToken(
	                        yOffset,
	                        cardToken,
	                        document,
	                        graphics,
	                        fontFamily,
	                        textFontSize,
	                        cardFrontTextColor);
	                }
	            }

	            ////TODO: Add logo

	            return document;
	        }
	    }

	    private float GetTextFontSize(
	        float maxFontSize,
	        IList<string> promptCardTokens,
	        float yOffset,
	        Graphics graphics,
	        FontFamily fontFamily)
	    {
	        var availableHeight = CardHeightInPixels - (yOffset + BottomBorderPadding);
	        float heightAtNextAttempt;
	        var nextAttempt = maxFontSize;
	        do
            {
                heightAtNextAttempt = GetHeightForCardAtFontSize(
                    promptCardTokens,
                    graphics,
                    nextAttempt,
                    fontFamily);
                if (heightAtNextAttempt > availableHeight)
                    nextAttempt--;
            } while (heightAtNextAttempt > availableHeight);
	        return nextAttempt;
	    }

	    private float GetHeightForCardAtFontSize(
	        IList<string> promptCardTokens,
	        Graphics graphics,
	        float fontSize,
	        FontFamily fontFamily)
	    {
	        var availableWidth = CardWidthInPixels - (LeftBorderPadding + RightBorderPadding);
	        var size = new SizeF(
	            availableWidth,
	            float.MaxValue);
	        var maxFont = new Font(fontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
	        return promptCardTokens
	            .Sum(
	                promptCardToken =>
	                    graphics.MeasureString(promptCardToken, maxFont, size, StringFormat.GenericDefault).Height);
	    }

	    private float DrawNextStringToken(
	        float yOffset,
	        string cardToken,
	        SvgElement container,
            Graphics graphics,
	        FontFamily fontFamily,
            float textFontSize,
	        Color textColor)
	    {
	        
	        if (cardToken.Contains(PromptBlankLine.PromptBlankPlaceholder))
	        {
	            var textRectangle = new RectangleF(
	                LeftBorderPadding,
	                yOffset,
	                CardWidthInPixels - (LeftBorderPadding + RightBorderPadding),
	                CardHeightInPixels - (yOffset + BottomBorderPadding));
	            var sizeForText = new SizeF(textRectangle.Width, textRectangle.Height);

	            var font = new Font(fontFamily, textFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                var textToDraw = PadTextWithSpaces(graphics, cardToken, font, sizeForText);

	            var height = graphics.MeasureString(textToDraw, font, sizeForText, StringFormat.GenericDefault).Height;
	            yOffset += height;

	            var textElement = new SvgText(textToDraw)
	            {
	                Fill = new SvgColourServer(textColor),
	                FontFamily = fontFamily.Name,
	                FontSize = new SvgUnit(textFontSize),
	                FontStyle = SvgFontStyle.Normal,
	                X = new SvgUnitCollection {new SvgUnit(textRectangle.X + Origin.X)},
	                Y = new SvgUnitCollection {new SvgUnit(textRectangle.Y + Origin.Y + height) },

	            };
	            container.Children.Add(textElement);

	            return yOffset;
	        }
	        else
	        {
	            var textElement = new SvgText
	            {
	                Fill = new SvgColourServer(textColor),
	                FontFamily = fontFamily.Name,
	                FontSize = new SvgUnit(textFontSize),
	                FontStyle = SvgFontStyle.Normal
                };
	            container.Children.Add(textElement);
	            var textRectangle = new RectangleF(
	                LeftBorderPadding,
	                yOffset,
	                CardWidthInPixels - (LeftBorderPadding + RightBorderPadding),
	                CardHeightInPixels - (yOffset + BottomBorderPadding));
	            var sizeForText = new SizeF(textRectangle.Width, textRectangle.Height);
	            var font = new Font(fontFamily, textFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                foreach (var lineToken in GetLineTokens(cardToken, graphics, font, sizeForText))
	            {
	                textRectangle = new RectangleF(
	                    LeftBorderPadding,
	                    yOffset,
	                    CardWidthInPixels - (LeftBorderPadding + RightBorderPadding),
	                    CardHeightInPixels - (yOffset + BottomBorderPadding));
	                sizeForText = new SizeF(textRectangle.Width, textRectangle.Height);

	                font = new Font(fontFamily, textFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
	                var height = graphics.MeasureString(lineToken, font, sizeForText, StringFormat.GenericDefault).Height;
	                yOffset += height;

	                var textSpanElement = new SvgTextSpan
	                {
	                    X = new SvgUnitCollection {new SvgUnit(textRectangle.X + Origin.X)},
	                    Y = new SvgUnitCollection {new SvgUnit(textRectangle.Y + Origin.Y + height)},
	                    Text = lineToken
	                };
	                textElement.Children.Add(textSpanElement);
	            }

	            return yOffset;
	        }
	    }

	    private IEnumerable<string> GetLineTokens(string input, Graphics graphics, Font font, SizeF sizeForText)
	    {

            var currentLine = "";
	        var tokens = Regex.Split(input, @"(?<=[ -])").ToList();
            var heightOfFirstToken = graphics.MeasureString(tokens[0], font, sizeForText, StringFormat.GenericDefault).Height;
            while (tokens.Any())
            {
                var nextToken = tokens[0];
                var lineWithNextToken = string.IsNullOrWhiteSpace(currentLine) ? nextToken : $"{currentLine}{nextToken}";
                var heightWithNextToken = graphics.MeasureString(lineWithNextToken, font, sizeForText, StringFormat.GenericDefault).Height;
                if (heightWithNextToken - heightOfFirstToken > 0.0)
                {
                    yield return currentLine;
                    currentLine = "";
                }
                else
                {
                    currentLine = lineWithNextToken;
                    tokens.Remove(nextToken);
                }
            }
	        yield return currentLine;
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
	                out var charactersFitted,
	                out var linesFitted);
	            fitsInOneLine = charactersFitted == promptBlankLine.FullLineText.Length && linesFitted == 1;
	        } while (fitsInOneLine);
	        promptBlankLine.BlankLength--;
            return promptBlankLine.FullLineText;
	    }

        private void PrintCardBackground(SvgDocument document, Color backgroundColor)
	    {
	        var topSideInPixelsWithBleed = CardHeightInPixelsWithBleed;
	        var leftSideInPixelsWithBleed = CardWidthInPixelsWithBleed;
	        var rectangle = new SvgRectangle
	        {
	            Fill = new SvgColourServer(backgroundColor),
	            Width = leftSideInPixelsWithBleed,
	            Height = topSideInPixelsWithBleed,
	            CornerRadiusX = BorderRadius,
	            CornerRadiusY = BorderRadius
	        };
	        document.Children.Add(rectangle);
	    }
    }
}
