using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AdinaCardGame.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //TODO: Classes for all the input?
        //TODO: Validate font
        [HttpPost]
        public ActionResult GenerateImages(
            HttpPostedFileBase promptsInputFile, 
            HttpPostedFileBase answersInputFile,
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
            var promptCards = GetCardsFromStream(promptsInputFile);
            var answerCards = GetCardsFromStream(answersInputFile);

            var imageCreationProcess = new ImageCreationProcess();
            var dateStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture);
            var fileName = $"Card Images {dateStamp}.zip";

            var bytes = imageCreationProcess.Run(
                promptCards,
                answerCards,
                cardWidthInInches,
                cardHeightInInches,
                bleedSizeInInches,
                promptFontFamily,
                answerFontFamily,
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

        private static List<string> GetCardsFromStream(HttpPostedFileBase promptsInputFile)
        {
            var cards = new List<string>();
            var file = promptsInputFile;
            using (var fileStream = new StreamReader(file.InputStream, Encoding.Default))
            {
                while (!fileStream.EndOfStream)
                {
                    cards.Add(fileStream.ReadLine());
                }
            }

            return cards
                .Select(line => line.Trim())
                .Where(line => !line.StartsWith("//") && !string.IsNullOrWhiteSpace(line))
                .ToList();
        }
    }
}
