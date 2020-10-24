using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AdinaCardGame.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //TODO: Classes for all the input?
        //TODO: Validate font
        [HttpPost]
        public async Task<ActionResult> GenerateImages(
            IFormFile promptsInputFile,
            IFormFile answersInputFile,
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
            var promptCards = await GetCardsFromStream(promptsInputFile);
            var answerCards = await GetCardsFromStream(answersInputFile);

            var imageCreationProcess = new ImageCreationProcess();
            var dateStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture);
            var fileName = $"Card Images {dateStamp}.zip";

            var bytes = imageCreationProcess.Run(
                promptCards,
                answerCards,
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
            return File(bytes, "application/zip", fileName);
        }

        private static async Task<List<string>> GetCardsFromStream(IFormFile file)
        {
            var cards = new List<string>();
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                using (var streamReader = new StreamReader(memoryStream, Encoding.Default))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        cards.Add(line);
                    }
                }
            }

            return cards
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("//") && !string.IsNullOrWhiteSpace(line))
                .ToList();
        }
    }
}
