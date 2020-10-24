using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace AdinaCardGame
{
    public class ImageCreationProcess
    {
        public byte[] Run(
            IEnumerable<string> promptCards,
            IEnumerable<string> answerCards,
            float cardWidthInInches,
            float cardHeightInInches,
            float bleedSizeInInches,
            float borderRadius,
            float borderPaddingInInches,
            float maxPromptTextFontSize,
            float maxAnswerTextFontSize,
            Color promptCardFrontBackgroundColor,
            Color promptCardFrontTextColor,
            Color answerCardFrontBackgroundColor,
            Color answerCardFrontTextColor)
        {
            var imageCreator = new ImageCreator(
                cardWidthInInches,
                cardHeightInInches,
                bleedSizeInInches,
                borderRadius,
                borderPaddingInInches,
                maxPromptTextFontSize,
                maxAnswerTextFontSize,
                promptCardFrontBackgroundColor,
                promptCardFrontTextColor,
                answerCardFrontBackgroundColor,
                answerCardFrontTextColor);
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
                        using (var singleFileStream = new MemoryStream())
                        {
                            document.Write(singleFileStream);
                            using (var zipStream = entry.Open())
                                zipStream.Write(singleFileStream.ToArray());
                        }
                    }
                }
                return memoryStream.ToArray();
            }
        }
    }
}
