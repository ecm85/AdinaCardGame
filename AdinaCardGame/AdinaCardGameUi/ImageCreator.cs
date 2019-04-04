using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using Svg;

namespace AdinaCardGame
{
	public class ImageCreator
	{
		const float DpiFactor = 300.0f / 96;

	    private static float CardWidthInInches => float.Parse(ConfigurationManager.AppSettings["CardWidthInInches"]);
		private static float CardHeightInInches => float.Parse(ConfigurationManager.AppSettings["CardHeightInInches"]);

		private static float BleedSizeInInches => float.Parse(ConfigurationManager.AppSettings["BleedSizeInInches"]);

		private static float CardWidthInInchesWithBleed => CardWidthInInches + BleedSizeInInches;
		private static float CardHeightInInchesWithBleed => CardHeightInInches + BleedSizeInInches;

		private const int Dpi = (int)(96 * DpiFactor);
		private static int CardWidthInPixels => (int)(Dpi * CardWidthInInches);
		private static int CardHeightInPixels => (int)(Dpi * CardHeightInInches);

		private static int CardWidthInPixelsWithBleed => (int)(Dpi * CardWidthInInchesWithBleed);
		private static int CardHeightInPixelsWithBleed => (int)(Dpi * CardHeightInInchesWithBleed);

		private readonly FontFamily promptFontFamily = new FontFamily(ConfigurationManager.AppSettings["PromptFontFamily"]);
	    private readonly FontFamily answerFontFamily = new FontFamily(ConfigurationManager.AppSettings["AnswerFontFamily"]);

		private readonly Point origin = new Point((int) (BleedSizeInInches * Dpi), (int) (BleedSizeInInches * Dpi));

	    private static int BorderRadius => int.Parse(ConfigurationManager.AppSettings["BorderRadius"]);

        private static int BorderPadding => (int)(float.Parse(ConfigurationManager.AppSettings["BorderPaddingInInches"]) * Dpi);

	    private static int TopBorderPadding => BorderPadding;
	    private static int RightBorderPadding => BorderPadding;
	    private static int LeftBorderPadding => BorderPadding;
	    private static int BottomBorderPadding => BorderPadding;

        private static int MaxPromptTextFontSize => (int) (int.Parse(ConfigurationManager.AppSettings["MaxPromptTextFontSize"]) * DpiFactor);
	    private static int MaxAnswerTextFontSize => (int)(int.Parse(ConfigurationManager.AppSettings["MaxAnswerTextFontSize"]) * DpiFactor);

        //TODO: Potentially use these when making logo at bottom of cards
        //private const int resourceKeyImageSize = (int) (35 * DpiFactor);
        //private const int arrowImageSize = (int) (10 * DpiFactor);
        //private const int questCostImageSize = (int) (35 * DpiFactor);
        //private const int pentagonImageSize = (int) (25 * DpiFactor);
        //private const int wreathImageWidth = (int) (40 * DpiFactor);
        //private const int cardFrontSmallImageSize = (int) (35 * DpiFactor);
        //private const int questImageYBottomPadding = (int) (5 * DpiFactor);

        private static string PromptCardFrontBackgroundColorText => ConfigurationManager.AppSettings["PromptCardFrontBackgroundColor"];
	    private static string PromptCardFrontTextColorText => ConfigurationManager.AppSettings["PromptCardFrontTextColor"];
	    private static string AnswerCardFrontBackgroundColorText => ConfigurationManager.AppSettings["AnswerCardFrontBackgroundColor"];
        private static string AnswerCardFrontTextColorText => ConfigurationManager.AppSettings["AnswerCardFrontTextColor"];

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
	        var bitmap = new Bitmap(width, height);
	        bitmap.SetResolution(Dpi, Dpi);
	        return bitmap;
	    }

	    public SvgDocument CreatePromptCardFront(string promptCardText)
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

	    public SvgDocument CreateAnswerCardFront(string answerCard)
	    {
	        var cardTokens = new[] { answerCard };
	        var cardFrontBackgroundColor = ParseColorText(AnswerCardFrontBackgroundColorText);
	        var cardFrontTextColor = ParseColorText(AnswerCardFrontTextColorText);
	        var maxFontSize = MaxAnswerTextFontSize;
	        var fontFamily = answerFontFamily;

	        return CreateCardFront(cardFrontBackgroundColor, maxFontSize, cardTokens, fontFamily, cardFrontTextColor);
	    }

        private SvgDocument CreateCardFront(Color cardFrontBackgroundColor, int maxFontSize, IList<string> cardTokens, FontFamily fontFamily,
	        Color cardFrontTextColor)
        {
            //return CreateBitmap(CardWidthInPixelsWithBleed, CardHeightInPixelsWithBleed);
            var bitmap = CreateBitmap(ImageOrientation.Portrait);
            var document = new SvgDocument
            {
                ViewBox = new SvgViewBox(0, 0, CardWidthInPixelsWithBleed, CardHeightInPixelsWithBleed)
            };
            var graphics = Graphics.FromImage(bitmap);
            PrintCardBackground(document, cardFrontBackgroundColor);

            var yOffset = (float)TopBorderPadding;
            var textFontSize = GetTextFontSize(maxFontSize, cardTokens, yOffset, graphics);
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

            ////TODO: Add logo

            return document;
	    }

	    private float GetTextFontSize(float maxFontSize, IList<string> promptCardTokens, float yOffset, Graphics graphics)
	    {
	        var availableHeight = CardHeightInPixels - (yOffset + BottomBorderPadding);
	        float heightAtNextAttempt;
	        var nextAttempt = maxFontSize;
	        do
            {
                heightAtNextAttempt = GetHeightForCardAtFontSize(promptCardTokens, graphics, nextAttempt);
                if (heightAtNextAttempt > availableHeight)
                    nextAttempt--;
            } while (heightAtNextAttempt > availableHeight);
	        return nextAttempt;
	    }

	    private float GetHeightForCardAtFontSize(IList<string> promptCardTokens, Graphics graphics, float fontSize)
	    {
	        var availableWidth = CardWidthInPixels - (LeftBorderPadding + RightBorderPadding);
	        var size = new SizeF(
	            availableWidth,
	            float.MaxValue);
	        var maxFont = new Font(promptFontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
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
	                X = new SvgUnitCollection {new SvgUnit(textRectangle.X + origin.X)},
	                Y = new SvgUnitCollection {new SvgUnit(textRectangle.Y + origin.Y + height) },

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
	                    X = new SvgUnitCollection {new SvgUnit(textRectangle.X + origin.X)},
	                    Y = new SvgUnitCollection {new SvgUnit(textRectangle.Y + origin.Y + height)},
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
	        var tokens = input.Split(new [] {" "}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var heightOfFirstToken = graphics.MeasureString(tokens[0], font, sizeForText, StringFormat.GenericDefault).Height;
            while (tokens.Any())
            {
                var nextToken = tokens[0];
                var lineWithNextToken = string.IsNullOrWhiteSpace(currentLine) ? nextToken : $"{currentLine} {nextToken}";
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

	    private static Color ParseColorText(string colorText)
	    {
	        var tokens = colorText.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
	        var color = Color.FromArgb(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
	        return color;
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
