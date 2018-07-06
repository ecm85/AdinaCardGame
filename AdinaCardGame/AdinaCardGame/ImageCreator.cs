using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Splendor
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
		private const int borderThickness = (int) (20 * dpiFactor);
		private const int borderPadding = (int) (25 * dpiFactor);
		private const int nestedBorderPadding = borderPadding + 25;

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

		private static int ArrowPadding => arrowImageSize / 2;

		public Image CreateQuestBack()
		{
			var cardBackBitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(cardBackBitmap);

			PrintCardBorder(graphics, null, ImageOrientation.Portrait, Color.DodgerBlue);
			DrawIconPentagon(graphics);
			PrintCardBackString(graphics, questBackText, questBackFontSize);
			PrintGameTitle(graphics);
			return cardBackBitmap;
		}

		public Image CreateQuestFront(Quest quest)
		{
			var bitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, ImageOrientation.Portrait, standardCardBackgroundColor);
			var headerFont = new Font(headerFontFamily, questHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
			var headerHeight = headerFont.Height;
			DrawString(
				graphics,
				quest.Name,
				headerFont,
				blackBrush,
				new RectangleF(borderPadding, borderPadding, cardShortSideInPixels - 2 * borderPadding, headerHeight),
				horizontalCenterAlignment);
			var questDescriptionFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);
			var questDescriptionHeight = questDescriptionFont.Height * 3;
			DrawString(
				graphics,
				quest.Description,
				questDescriptionFont,
				blackBrush,
				new RectangleF(
					borderPadding,
					headerHeight + borderPadding,
					cardShortSideInPixels - 2 * borderPadding,
					questDescriptionHeight),
				horizontalCenterAlignment);
			PrintCostsForQuest(graphics, quest, borderPadding, borderPadding + headerHeight + questDescriptionHeight);
			PrintPointsForQuest(graphics, quest, cardShortSideInPixels - (borderPadding + wreathImageWidth), borderPadding + headerHeight + questDescriptionHeight);
			var questImageY = borderPadding + headerHeight + questDescriptionHeight + cardFrontSmallImageSize;
			PrintScaledJpg(graphics, quest.Image, borderPadding, questImageY, cardShortSideInPixels - 2 * borderPadding, cardLongSideInPixels - (questImageY + borderPadding + questImageYBottomPadding));
			return bitmap;
		}

		private void PrintCostsForQuest(Graphics graphics, Quest quest, int initialX, int initialY)
		{
			for (var toolIndex = 0; toolIndex < quest.ToolRequirements.Count; toolIndex++)
			{
				PrintImageWithText(
					graphics,
					$"{quest.ToolRequirements[toolIndex]} BW",
					initialX + (questCostImageSize / 4) + toolIndex * (questCostImageSize + (questCostImageSize / 8) + questCostImageSize / 4),
					initialY,
					questCostImageSize,
					quest.ToolCountRequired.ToString(),
					(int)-(questCostImageSize / 2.6f),
					0);
			}
		}

		private void PrintPointsForQuest(Graphics graphics, Quest quest, int initialX, int initialY)
		{
			PrintPoints(graphics, quest.Points, initialX, initialY);
		}

		public Image CreateQuestAidFront()
		{
			var bitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(bitmap);

			PrintCardBorder(graphics, null, ImageOrientation.Portrait, standardCardBackgroundColor);

			var questAidTitle = "Completing Quests";
			var titleRectangle = new RectangleF(
				borderPadding,
				borderPadding,
				cardShortSideInPixels - 2 * borderPadding,
				cardLongSideInPixels - 2 * borderPadding);
			var titleFont = new Font(headerFontFamily, questHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
			DrawString(graphics, questAidTitle, titleFont, blackBrush, titleRectangle);

			var textRectangle = new RectangleF(
				borderPadding,
				borderPadding + titleFont.Height,
				cardShortSideInPixels - 2 * borderPadding,
				cardLongSideInPixels - (2 * borderPadding + titleFont.Height));
			var textFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);

			var questAidString = "After completing your action each day, check if you have the tools depicted on each quest." +
				"\r\n\r\nIf you do, equip your villagers with those tools and they will complete the quest for you. Take the quest card and place in front of you." +
				"\r\n\r\nDon't worry, they'll return the tools in the same condition (more or less). " +
				"\r\n\r\nYou only have enough villagers to complete one quest each day. If you gain the tool to complete two quests on the same day, one must be completed on the next day (or be stolen by someone else).";
			DrawString(graphics, questAidString, textFont, blackBrush, textRectangle);

			return bitmap;
		}

		public Image CreateSetupAidFront()
		{
			var bitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(bitmap);

			PrintCardBorder(graphics, null, ImageOrientation.Portrait, standardCardBackgroundColor);

			var setupAidTitle = "Game Setup";
			var titleRectangle = new RectangleF(
				borderPadding,
				borderPadding,
				cardShortSideInPixels - 2 * borderPadding,
				cardLongSideInPixels - 2 * borderPadding);
			var titleFont = new Font(headerFontFamily, questHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
			DrawString(graphics, setupAidTitle, titleFont, blackBrush, titleRectangle);

			var textRectangle = new RectangleF(
				borderPadding,
				borderPadding + titleFont.Height,
				cardShortSideInPixels - 2 * borderPadding,
				cardLongSideInPixels - (2 * borderPadding + titleFont.Height));
			var textFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);

			var setupAidString =
				"\r\n\r\nResources:\r\nGold: Always 5\r\n4 Players: 7 of each other resource\r\n3 Players: 5 of each other resource\r\n2 Players: 4 of each other resource" +
				"\r\n\r\n\r\nQuests:\r\n 1 + Player Count" +
				"\r\n\r\n\r\nTools:\r\n4 per Tier";
			DrawString(graphics, setupAidString, textFont, blackBrush, textRectangle);

			return bitmap;
		}

		public Image CreatePlayerAidFront()
		{
			var bitmap = CreateBitmap(ImageOrientation.Landscape);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, ImageOrientation.Landscape, standardCardBackgroundColor);
			var playerAidString = "Each day, you may take one of the following actions:" +
				"\r\n\u2022  Scavenge for Resources: gather 3 different resources" +
				"\r\n\u2022  Hunt for Resources: gather 2 of the same resource [as long as there is an abundance (4+)]" +
				"\r\n\u2022  Find Blueprint: Reserve a Tool from the display into your hand and take 1 gold [if available]" +
				"\r\n\u2022  Craft: Take a Tool from the display, place in front of you and return the depicted resources to the supply" +
				"\r\n\r\nAfter your action, check if you have the tools to complete any quests.";
			DrawString(
				graphics,
				playerAidString,
				new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel),
				blackBrush,
				new RectangleF(borderPadding, borderPadding, cardLongSideInPixels - 2 * borderPadding, cardShortSideInPixels - 2 * borderPadding));
			PrintLimitsReminder(graphics);
			return bitmap;
		}

		public Image CreatePlayerAidBack()
		{
			var bitmap = CreateBitmap(ImageOrientation.Landscape);
			var graphics = Graphics.FromImage(bitmap);
			PrintCardBorder(graphics, null, ImageOrientation.Landscape, standardCardBackgroundColor);
			PrintLimitsReminder(graphics);

			var columnWidth = resourceKeyImageSize * 2 + arrowImageSize + 2 * ArrowPadding;
			var columnPadding = (cardLongSideInPixels - 2*columnWidth)/3;
			var columnOffset = (cardLongSideInPixels - 2*columnWidth)/3;
			var firstColumnX = columnOffset;
			var secondColumnX = columnOffset + columnWidth + columnPadding;
			var rowWidth = resourceKeyImageSize;
			var rowPadding = (cardShortSideInPixels - 3*rowWidth)/5;
			var rowOffset = rowPadding;
			var firstRowY = rowOffset;
			var secondRowY = rowOffset + rowWidth + rowPadding;
			var thirdRowY = rowOffset + 2*(rowWidth + rowPadding);

			PrintImageMappingPng(graphics, "Axe BW", "Axe", "Wood", "Wood", firstColumnX, firstRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Sword BW", "Sword", "Dragonbone", "Dragonbone", secondColumnX, firstRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Staff BW", "Staff", "Magic", "Magic Shards", firstColumnX, secondRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Pick BW", "Pick", "Iron", "Iron Ore", secondColumnX, secondRowY, resourceKeyImageSize);
			PrintImageMappingPng(graphics, "Chisel BW", "Chisel", "Stone", "Stone", firstColumnX, thirdRowY, resourceKeyImageSize);
			using (var goldImage = Image.FromFile("Images\\Gold.png"))
				PrintImageMapping(graphics, CreateToolCardBack(3), "Reserve", goldImage, "Gold (wild)", secondColumnX, thirdRowY, resourceKeyImageSize);

			return bitmap;
		}

		private void PrintLimitsReminder(Graphics graphics)
		{
			var handLimitString = "Hand limit - 3";
			var resourceLimitString = "Resource limit - 10";
			var limitsReminderFont = new Font(bodyFontFamily, limitsFontSize, GraphicsUnit.Pixel);
			var textRectangle = new RectangleF(
				borderPadding,
				cardShortSideInPixels - (limitsReminderFont.Height + borderPadding),
				cardLongSideInPixels - (2 * borderPadding),
				limitsReminderFont.Height + borderPadding);
			DrawString(graphics, handLimitString, limitsReminderFont, blackBrush, textRectangle, horizontalNearAlignment);
			DrawString(graphics, resourceLimitString, limitsReminderFont, blackBrush, textRectangle, horizontalFarAlignment);
		}

		private void PrintImageMappingPng(Graphics graphics, string filename1, string label1, string filename2, string label2, int x, int y, int imageSize)
		{
			using (var image1 = Image.FromFile($"Images\\{filename1}.png"))
			using (var image2 = Image.FromFile($"Images\\{filename2}.png"))
			{
				PrintImageMapping(graphics, image1, label1, image2, label2, x, y, imageSize);
			}
		}

		private void PrintImageMapping(Graphics graphics, Image image1, string label1, Image image2, string label2, int x, int y, int imageSize)
		{
			var imageMappingFont = new Font(bodyFontFamily, bodyFontSize, GraphicsUnit.Pixel);
			var mappingTextWidth = imageSize + 2 * (arrowImageSize + 2 * ArrowPadding);
			var label1Rectangle = new RectangleF(x - (arrowImageSize + 2 * ArrowPadding), y + imageSize, mappingTextWidth, imageMappingFont.Height);
			var label2Rectangle = new RectangleF(x + imageSize + 2 * ArrowPadding + arrowImageSize - (arrowImageSize + 2 * ArrowPadding), y + imageSize, mappingTextWidth, imageMappingFont.Height);

			PrintScaledImage(graphics, image1, x, y, imageSize, imageSize);
			DrawString(graphics, label1, imageMappingFont, blackBrush, label1Rectangle, horizontalCenterAlignment);
			PrintScaledPng(graphics, "arrow", x + imageSize + ArrowPadding, y + (imageSize / 2), arrowImageSize, arrowImageSize);
			PrintScaledImage(graphics, image2, x + imageSize + ArrowPadding + arrowImageSize + ArrowPadding, y, imageSize, imageSize);
			DrawString(graphics, label2, imageMappingFont, blackBrush, label2Rectangle, horizontalCenterAlignment);
		}

		public Image CreateToolCardBack(int tier)
		{
			var cardBackBitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(cardBackBitmap);

			PrintCardBorder(graphics, null, ImageOrientation.Portrait, GetTierBackColor(tier));
			DrawIconPentagon(graphics);
			PrintTier(graphics, tier);
			PrintGameTitle(graphics);
			return cardBackBitmap;
		}

		private Color GetTierBackColor(int tier)
		{
			switch (tier)
			{
				case 1:
					return Color.FromArgb(255, 150, 90, 56);
				case 2:
					return Color.FromArgb(255, 168, 168, 168);
				case 3:
					return Color.FromArgb(255, 201, 137, 16);
				default:
					throw new InvalidOperationException("Invalid tier!");
			}
		}

		private void DrawIconPentagon(Graphics graphics)
		{
			var radius = ((cardShortSideInPixels - borderPadding)*(7.0/10))/1.902;

			var x = cardShortSideInPixels / 2;
			var y = cardLongSideInPixels / 2;
			var c1 = (int)(Math.Cos(2*Math.PI/5) * radius);
			var c2 = (int)(Math.Cos(Math.PI/5) * radius);
			var s1 = (int)(Math.Sin(2*Math.PI/5) * radius);
			var s2 = (int)(Math.Sin(4*Math.PI/5) * radius);

			var points = new[]
			{
				new Point(cardShortSideInPixels - x, (int)(cardLongSideInPixels - (y + radius))),
				new Point(cardShortSideInPixels - (x + s1), cardLongSideInPixels - (y + c1)),
				new Point(cardShortSideInPixels - (x + s2), cardLongSideInPixels - (y + -c2)),
				new Point(cardShortSideInPixels - (x + -s2), cardLongSideInPixels - (y + -c2)),
				new Point(cardShortSideInPixels - (x + -s1), cardLongSideInPixels - (y + c1))
			};

			var halfPentagonImageSize = pentagonImageSize / 2;
			PrintScaledPng(graphics, "Axe BW", points[0].X - halfPentagonImageSize, points[0].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
			PrintScaledPng(graphics, "Sword BW", points[1].X - halfPentagonImageSize, points[1].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
			PrintScaledPng(graphics, "Staff BW", points[2].X - halfPentagonImageSize, points[2].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
			PrintScaledPng(graphics, "Pick BW", points[3].X - halfPentagonImageSize, points[3].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
			PrintScaledPng(graphics, "Chisel BW", points[4].X - halfPentagonImageSize, points[4].Y - halfPentagonImageSize, pentagonImageSize, pentagonImageSize);
		}

		private void PrintTier(Graphics graphics, int tier)
		{
			PrintCardBackString(graphics, new string(Enumerable.Repeat('I', tier).ToArray()), tierTextFontSize);
		}

		private void PrintCardBackString(Graphics graphics, string text, int textSize)
		{
			DrawString(
				graphics,
				text,
				new Font(cardBackFontFamily, textSize, FontStyle.Bold, GraphicsUnit.Pixel),
				new SolidBrush(Color.Black),
				new RectangleF(borderPadding, borderPadding, cardShortSideInPixels - 2 * borderPadding, cardLongSideInPixels - 2 * borderPadding),
				fullCenterAlignment);
		}

		private void PrintGameTitle(Graphics graphics)
		{
			var titleFont = new Font(cardBackFontFamily, gameTitleFontSize, GraphicsUnit.Pixel);
			DrawString(
				graphics,
				"Splendor",
				titleFont,
				blackBrush,
				new RectangleF(borderPadding, borderPadding, cardShortSideInPixels - 2 * borderPadding, titleFont.Height),
				horizontalCenterAlignment);
			DrawString(
				graphics,
				"Forge",
				titleFont,
				blackBrush,
				new RectangleF(borderPadding, cardLongSideInPixels - (borderPadding + titleFont.Height), cardShortSideInPixels - 2 * borderPadding, titleFont.Height),
				horizontalCenterAlignment);
		}

		public Image CreateToolCardFront(NewCard newCard)
		{
			var cardBitmap = CreateBitmap(ImageOrientation.Portrait);
			var graphics = Graphics.FromImage(cardBitmap);
			PrintCardBorder(graphics, newCard.Color, ImageOrientation.Portrait, standardCardBackgroundColor);
			if (newCard.Points > 0)
				PrintPointsForTool(graphics, newCard);
			PrintToolIcon(newCard, graphics, (cardShortSideInPixels / 2) - (cardFrontSmallImageSize / 2), nestedBorderPadding);
			PrintResourceProduced(newCard, graphics, cardShortSideInPixels - (nestedBorderPadding + cardFrontSmallImageSize), nestedBorderPadding);
			PrintCardName(newCard, graphics);
			PrintToolImage(newCard, graphics);
			PrintCostsForTool(newCard, graphics);
			return cardBitmap;
		}

		private void PrintCardName(NewCard newCard, Graphics graphics)
		{
			var cardNameFont = new Font(headerFontFamily, toolHeaderFontSize, GraphicsUnit.Pixel);
			var topRectangle = new RectangleF(nestedBorderPadding, nestedBorderPadding + cardFrontSmallImageSize, cardShortSideInPixels - 2 * nestedBorderPadding, cardNameFont.Height);
			var bottomRectangle = new RectangleF(nestedBorderPadding, nestedBorderPadding + cardFrontSmallImageSize + cardNameFont.Height, cardShortSideInPixels - 2 * nestedBorderPadding, cardNameFont.Height);
			var nameParts = newCard.Name.Split(' ');
			var firstNamePart = nameParts.Take(nameParts.Length - 2).ToList();
			var lastNamePart = nameParts.Skip(firstNamePart.Count).ToList();
			DrawString(graphics, string.Join(" ", firstNamePart), cardNameFont, blackBrush, topRectangle, fullCenterAlignment);
			DrawString(graphics, string.Join(" ", lastNamePart), cardNameFont, blackBrush, bottomRectangle, fullCenterAlignment);
		}

		private void PrintToolImage(NewCard newCard, Graphics graphics)
		{
			const int cardFrontLargeImageSize = cardShortSideInPixels - (2 * nestedBorderPadding + 2 * cardFrontSmallImageSize);
			PrintScaledPng(
				graphics,
				newCard.Tool,
				cardShortSideInPixels / 2 - cardFrontLargeImageSize / 2,
				cardLongSideInPixels/2 - (cardFrontLargeImageSize/2),
				cardFrontLargeImageSize,
				cardFrontLargeImageSize);
		}

		private void PrintResourceProduced(NewCard newCard, Graphics graphics, int x, int y)
		{
			PrintImageWithText(graphics, newCard.ResourceProduced, x, y, cardFrontSmallImageSize, "+", 0, (int)(cardFrontSmallImageSize * (2.0/5)));
		}

		private void PrintToolIcon(NewCard newCard, Graphics graphics, int x, int y)
		{
			PrintScaledPng(graphics, $"{newCard.Tool} BW", x, y, cardFrontSmallImageSize, cardFrontSmallImageSize);
		}

		private void PrintCostsForTool(NewCard newCard, Graphics graphics)
		{
			var costList = newCard.Costs.ToList();
			for (var costIndex = 0; costIndex < costList.Count; costIndex++)
				PrintImageWithText(
					graphics,
					costList[costIndex].Key,
					nestedBorderPadding,
					cardLongSideInPixels - ((costIndex + 1)*cardFrontSmallImageSize + nestedBorderPadding),
					cardFrontSmallImageSize,
					costList[costIndex].Value.ToString(),
					0,
					(int)(cardFrontSmallImageSize * (2.0/5)));
		}

		private void PrintImageWithText(Graphics graphics, string fileName, int imageX, int imageY, int imageSide, string text, int textImageXOffset, int textImageYOffset)
		{
			PrintScaledPng(graphics, fileName, imageX, imageY, imageSide, imageSide);
			var textX = imageX + textImageXOffset;
			var textY = imageY + textImageYOffset;
			var font = new Font(bodyFontFamily, imageLabelFontSize);
			var path = new GraphicsPath();
			path.AddString(
				text,
				font.FontFamily,
				(int)font.Style,
				font.Size,
				new PointF(origin.X + textX, origin.Y + textY),
				new StringFormat());
			graphics.FillPath(Brushes.White, path);
			graphics.DrawPath(new Pen(Color.Black, textOutlineWidth), path);
		}

		private void PrintPointsForTool(Graphics graphics, NewCard newCard)
		{
			PrintPoints(
				graphics,
				newCard.Points,
				nestedBorderPadding,
				nestedBorderPadding);
		}

		private void PrintPoints(Graphics graphics, int points, int x, int y)
		{
			PrintScaledPng(graphics, "Wreath", x, y, wreathImageWidth, cardFrontSmallImageSize);
			var path = new GraphicsPath();
			var font = new Font(bodyFontFamily, imageLabelFontSize);
			path.AddString(
				points.ToString(),
				font.FontFamily,
				(int)font.Style,
				font.Size,
				new RectangleF(origin.X + x, origin.Y + y, wreathImageWidth, cardFrontSmallImageSize),
				horizontalCenterAlignment);
			graphics.FillPath(Brushes.White, path);
			graphics.DrawPath(new Pen(Color.Black, textOutlineWidth), path);
		}

		private void PrintScaledPng(Graphics graphics, string fileName, int x, int y, int width, int height)
		{
			using (var srcImage = Image.FromFile($"Images\\{fileName}.png"))
			{
				PrintScaledImage(graphics, srcImage, x, y, width, height);
			}
		}

		private void PrintScaledJpg(Graphics graphics, string fileName, int x, int y, int width, int height)
		{
			using (var srcImage = Image.FromFile($"Images\\{fileName}.jpg"))
			{
				PrintScaledImage(graphics, srcImage, x, y, width, height);
			}
		}

		private void PrintScaledImage(Graphics graphics, Image image, int x, int y, int width, int height)
		{
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphics.DrawImage(image, new Rectangle(origin.X + x, origin.Y + y, width, height));
		}

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

		private void DrawString(Graphics graphics, string text, Font font, Brush brush, RectangleF rectangle)
		{
			graphics.DrawString(text, font, brush, new RectangleF(origin.X + rectangle.X, origin.Y + rectangle.Y, rectangle.Width, rectangle.Height), horizontalCenterAlignment);
		}

		private void DrawString(Graphics graphics, string text, Font font, Brush brush, RectangleF rectangle, StringFormat stringFormat)
		{
			graphics.DrawString(text, font, brush, new RectangleF(origin.X + rectangle.X, origin.Y + rectangle.Y, rectangle.Width, rectangle.Height), stringFormat);
		}

		private void PrintCardBorder(Graphics graphics, Color? middleBorderColor, ImageOrientation orientation, Color backgroundColor)
		{
			var topSideInPixels = orientation == ImageOrientation.Landscape ? cardLongSideInPixels : cardShortSideInPixels;
			var leftSideInPixels = orientation == ImageOrientation.Portrait ? cardLongSideInPixels : cardShortSideInPixels;
			var topSideInPixelsWithBleed = orientation == ImageOrientation.Landscape ? cardLongSideInPixelsWithBleed : cardShortSideInPixelsWithBleed;
			var leftSideInPixelsWithBleed = orientation == ImageOrientation.Portrait ? cardLongSideInPixelsWithBleed : cardShortSideInPixelsWithBleed;

			graphics.FillRoundedRectangle(
				new SolidBrush(Color.Black),
				0,
				0,
				topSideInPixelsWithBleed,
				leftSideInPixelsWithBleed,
				borderRadius);
			graphics.FillRoundedRectangle(
				new SolidBrush(middleBorderColor ?? backgroundColor),
				origin.X + borderThickness,
				origin.Y + borderThickness,
				topSideInPixels - 2 * borderThickness,
				leftSideInPixels - 2 * borderThickness,
				borderRadius);
			if (middleBorderColor.HasValue)
				graphics.FillRoundedRectangle(
					new SolidBrush(backgroundColor),
					origin.X + (int)(borderThickness * 1.5),
					origin.Y + (int)(borderThickness * 1.5),
					topSideInPixels - 3 * borderThickness,
					leftSideInPixels - 3 * borderThickness,
					borderRadius);
		}
	}
}
