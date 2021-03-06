﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AdinaCardGame
{
	public static class GraphicsExtension
	{
		private static readonly StringFormat HorizontalCenterAlignment = new StringFormat { Alignment = StringAlignment.Center};

	    private static GraphicsPath GenerateRoundedRectangle(
			this Graphics graphics,
			RectangleF rectangle,
			float radius)
		{
			var path = new GraphicsPath();
			if (radius <= 0.0F)
			{
				path.AddRectangle(rectangle);
				path.CloseFigure();
				return path;
			}
			else
			{
				if (radius >= (Math.Min(rectangle.Width, rectangle.Height)) / 2.0)
					return graphics.GenerateCapsule(rectangle);
				var diameter = radius * 2.0F;
				var sizeF = new SizeF(diameter, diameter);
				var arc = new RectangleF(rectangle.Location, sizeF);
				path.AddArc(arc, 180, 90);
				arc.X = rectangle.Right - diameter;
				path.AddArc(arc, 270, 90);
				arc.Y = rectangle.Bottom - diameter;
				path.AddArc(arc, 0, 90);
				arc.X = rectangle.Left;
				path.AddArc(arc, 90, 90);
				path.CloseFigure();
			}
			return path;
		}

		private static GraphicsPath GenerateCapsule(
			this Graphics graphics,
			RectangleF baseRect)
		{
			if (graphics == null) throw new ArgumentNullException(nameof(graphics));
			var path = new GraphicsPath();
			try
			{
				float diameter;
				RectangleF arc;
				if (baseRect.Width > baseRect.Height)
				{
					diameter = baseRect.Height;
					var sizeF = new SizeF(diameter, diameter);
					arc = new RectangleF(baseRect.Location, sizeF);
					path.AddArc(arc, 90, 180);
					arc.X = baseRect.Right - diameter;
					path.AddArc(arc, 270, 180);
				}
				else if (baseRect.Width < baseRect.Height)
				{
					diameter = baseRect.Width;
					var sizeF = new SizeF(diameter, diameter);
					arc = new RectangleF(baseRect.Location, sizeF);
					path.AddArc(arc, 180, 180);
					arc.Y = baseRect.Bottom - diameter;
					path.AddArc(arc, 0, 180);
				}
				else path.AddEllipse(baseRect);
			}
			catch { path.AddEllipse(baseRect); }
			finally { path.CloseFigure(); }
			return path;
		}

		/// <summary>
		/// Draws a rounded rectangle specified by a pair of coordinates, a width, a height and the radius
		/// for the arcs that make the rounded edges.
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.</param>
		/// <param name="width">Width of the rectangle to draw.</param>
		/// <param name="height">Height of the rectangle to draw.</param>
		/// <param name="radius">The radius of the arc used for the rounded edges.</param>
		/// <param name="graphics"></param>
		public static void DrawRoundedRectangle(
			this Graphics graphics,
			Pen pen,
			float x,
			float y,
			float width,
			float height,
			float radius)
		{
			var rectangle = new RectangleF(x, y, width, height);
			var path = graphics.GenerateRoundedRectangle(rectangle, radius);
			var old = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.DrawPath(pen, path);
			graphics.SmoothingMode = old;
		}

		/// <summary>
		/// Draws a rounded rectangle specified by a pair of coordinates, a width, a height and the radius
		/// for the arcs that make the rounded edges.
		/// </summary>
		/// <param name="pen"></param>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.</param>
		/// <param name="width">Width of the rectangle to draw.</param>
		/// <param name="height">Height of the rectangle to draw.</param>
		/// <param name="radius">The radius of the arc used for the rounded edges.</param>
		/// <param name="graphics"></param>
		public static void DrawRoundedRectangle(
			this Graphics graphics,
			Pen pen,
			int x,
			int y,
			int width,
			int height,
			int radius)
		{
			graphics.DrawRoundedRectangle(
				pen,
				Convert.ToSingle(x),
				Convert.ToSingle(y),
				Convert.ToSingle(width),
				Convert.ToSingle(height),
				Convert.ToSingle(radius));
		}

		/// <summary>
		/// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
		/// and the radius for the arcs that make the rounded edges.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
		/// <param name="width">Width of the rectangle to fill.</param>
		/// <param name="height">Height of the rectangle to fill.</param>
		/// <param name="radius">The radius of the arc used for the rounded edges.</param>
		public static void FillRoundedRectangle(
			this Graphics graphics,
			Brush brush,
			float x,
			float y,
			float width,
			float height,
			float radius)
		{
			var rectangle = new RectangleF(x, y, width, height);
			var path = graphics.GenerateRoundedRectangle(rectangle, radius);
			var old = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.FillPath(brush, path);
			graphics.SmoothingMode = old;
		}

		/// <summary>
		/// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
		/// and the radius for the arcs that make the rounded edges.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
		/// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
		/// <param name="width">Width of the rectangle to fill.</param>
		/// <param name="height">Height of the rectangle to fill.</param>
		/// <param name="radius">The radius of the arc used for the rounded edges.</param>
		public static void FillRoundedRectangle(
			this Graphics graphics,
			Brush brush,
			int x,
			int y,
			int width,
			int height,
			int radius)
		{
			graphics.FillRoundedRectangle(
				brush,
				Convert.ToSingle(x),
				Convert.ToSingle(y),
				Convert.ToSingle(width),
				Convert.ToSingle(height),
				Convert.ToSingle(radius));
		}

        //public static void PrintImageWithText(this Graphics graphics, string fileName, int imageX, int imageY, int imageSide, string text, int textImageXOffset, int textImageYOffset)
        //{
        //    PrintScaledPng(graphics, fileName, imageX, imageY, imageSide, imageSide);
        //    var textX = imageX + textImageXOffset;
        //    var textY = imageY + textImageYOffset;
        //    var font = new Font(bodyFontFamily, imageLabelFontSize);
        //    var path = new GraphicsPath();
        //    path.AddString(
        //        text,
        //        font.FontFamily,
        //        (int)font.Style,
        //        font.Size,
        //        new PointF(origin.X + textX, origin.Y + textY),
        //        new StringFormat());
        //    graphics.FillPath(Brushes.White, path);
        //    graphics.DrawPath(new Pen(Color.Black, textOutlineWidth), path);
        //}



        //public static void PrintScaledPng(this Graphics graphics, string fileName, int x, int y, int width, int height)
        //{
        //    using (var srcImage = Image.FromFile($"Images\\{fileName}.png"))
        //    {
        //        PrintScaledImage(graphics, srcImage, x, y, width, height);
        //    }
        //}

        //public static void PrintScaledJpg(this Graphics graphics, string fileName, int x, int y, int width, int height)
        //{
        //    using (var srcImage = Image.FromFile($"Images\\{fileName}.jpg"))
        //    {
        //        PrintScaledImage(graphics, srcImage, x, y, width, height);
        //    }
        //}

        //public static void PrintScaledImage(this Graphics graphics, Image image, int x, int y, int width, int height)
        //{
        //    graphics.SmoothingMode = SmoothingMode.AntiAlias;
        //    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //    graphics.DrawImage(image, new Rectangle(origin.X + x, origin.Y + y, width, height));
        //}

        public static void DrawString(this Graphics graphics, Point origin, string text, Font font, Brush brush, RectangleF rectangle)
        {
            graphics.DrawString(text, font, brush, new RectangleF(origin.X + rectangle.X, origin.Y + rectangle.Y, rectangle.Width, rectangle.Height), HorizontalCenterAlignment);
        }

        public static void DrawString(this Graphics graphics, Point origin, string text, Font font, Brush brush, RectangleF rectangle, StringFormat stringFormat)
        {
            graphics.DrawString(text, font, brush, new RectangleF(origin.X + rectangle.X, origin.Y + rectangle.Y, rectangle.Width, rectangle.Height), stringFormat);
        }
    }
}