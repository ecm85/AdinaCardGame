using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AdinaCardGame
{
    public class ImageCreationProcess
    {
        private static bool UseOverlay => bool.Parse(ConfigurationManager.AppSettings["OverlayTemplate"]);

        public string Run(IProgress<ImageCreationProgress> progress)
        {
            var imageCreator = new ImageCreator();
            var promptCards = LoadPromptCards()
                .Select((card, index) => new CardToGenerate { Index = index, Card = card, CreateImage = imageCreator.CreatePromptCardFront, FilePrefix = "Prompt"});
            var answerCards = LoadAnswerCards()
                .Select((card, index) => new CardToGenerate { Index = index, Card = card, CreateImage = imageCreator.CreateAnswerCardFront, FilePrefix = "Answer"});
            var allCards = promptCards
                .Concat(answerCards)
                .ToList();
            var outputPath = ConfigurationManager.AppSettings["OutputPath"];
            var timeStampedFolder = Path.Combine(
                outputPath,
                "Images " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture));
            var complete = 0;

            foreach (var card in allCards)
            {
                using (var image = card.CreateImage(card.Card))
                {
                    var imageToSave = new ImageToSave
                    {
                        Image = image,
                        Name = $"{card.FilePrefix} Card {card.Index}"
                    };

                    if (UseOverlay)
                    {
                        var overlay = new Bitmap(ConfigurationManager.AppSettings["TemplatePath"]);
                        overlay.SetResolution(300, 300);
                        var matrix = new ColorMatrix {Matrix33 = .1f};
                        var attributes = new ImageAttributes();
                        attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                        var graphics = Graphics.FromImage(imageToSave.Image);
                        graphics.DrawImage(overlay, new Rectangle(0, 0, overlay.Width, overlay.Height), 0, 0,
                            overlay.Width, overlay.Height,
                            GraphicsUnit.Pixel, attributes);
                    }

                    Directory.CreateDirectory(timeStampedFolder);
                    var fileName = $"{imageToSave.Name}.png";
                    var filePath = $"{timeStampedFolder}\\{fileName}";
                    imageToSave.Image.Save(filePath, ImageFormat.Png);
                    complete++;
                    progress.Report(new ImageCreationProgress
                    {
                        Complete = complete,
                        Total = allCards.Count,
                        MostRecentFileComplete = fileName
                    });
                }
            }
            return timeStampedFolder;
        }

        private static IEnumerable<string> LoadAnswerCards()
        {
            return LoadCards(AnswerCardPath);
        }

        public static string AnswerCardPath => ConfigurationManager.AppSettings["AnswerCardPath"];

        private IEnumerable<string> LoadPromptCards()
        {
            return LoadCards(PromptCardPath);
        }

        public static string PromptCardPath => ConfigurationManager.AppSettings["PromptCardPath"];

        private static IEnumerable<string> LoadCards(string path)
        {
            return File.ReadAllLines(path, Encoding.Default)
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("//") && !string.IsNullOrWhiteSpace(line))
                .ToList();
        }
    }
}
