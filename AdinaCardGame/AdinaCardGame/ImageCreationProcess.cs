using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace AdinaCardGame
{
    public class ImageCreationProcess
    {
        public byte[] Run(
            IEnumerable<string> answerCards,
            IEnumerable<string> promptCards,
            float cardWidthInInches,
            float cardHeightInInches,
            float bleedSizeInInches,
            string promptFontFamily,
            string answerFontFamily,
            int borderRadius,
            float borderPaddingInInches,
            int maxPromptTextFontSize,
            int maxAnswerTextFontSize,
            string promptCardFrontBackgroundColorText,
            string promptCardFrontTextColorText,
            string answerCardFrontBackgroundColorText,
            string answerCardFrontTextColorText)
        {
            var imageCreator = new ImageCreator(
                cardWidthInInches,
                cardHeightInInches,
                bleedSizeInInches,
                promptFontFamily,
                answerFontFamily,
                borderRadius,
                borderPaddingInInches,
                maxPromptTextFontSize,
                maxAnswerTextFontSize,
                promptCardFrontBackgroundColorText,
                promptCardFrontTextColorText,
                answerCardFrontBackgroundColorText,
                answerCardFrontTextColorText);
            var promptCardsToGenerate = promptCards
                .Select((card, index) => new CardToGenerate
                {
                    Index = index,
                    Card = card,
                    CreateImage = imageCreator.CreatePromptCardFront,
                    FilePrefix = "Prompt"
                });
            var answerCardsToGenerate = answerCards
                .Select((card, index) => new CardToGenerate
                {
                    Index = index,
                    Card = card,
                    CreateImage = imageCreator.CreateAnswerCardFront,
                    FilePrefix = "Answer"
                });
            var allCards = promptCardsToGenerate
                .Concat(answerCardsToGenerate)
                .ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var card in allCards)
                    {
                        var fileName = $"{card.FilePrefix} Card {card.Index}.svg";
                        var entry = zipArchive.CreateEntry(fileName);
                        var document = card.CreateImage(card.Card);
                        using (var stream = entry.Open())
                            document.Write(stream);
                    }
                }
                return memoryStream.ToArray();
            }
        }
    }
}
