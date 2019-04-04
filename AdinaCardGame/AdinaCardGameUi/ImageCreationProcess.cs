using System;
using System.Collections.Generic;
using System.Configuration;
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
                var document = card.CreateImage(card.Card);
                Directory.CreateDirectory(timeStampedFolder);
                var fileName = $"{card.FilePrefix} Card {card.Index}.svg";
                var filePath = $"{timeStampedFolder}\\{fileName}";
                document.Write(filePath);
                complete++;
                progress.Report(new ImageCreationProgress
                {
                    Complete = complete,
                    Total = allCards.Count,
                    MostRecentFileComplete = fileName
                });
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
