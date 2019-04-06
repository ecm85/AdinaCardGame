using System;
using System.Collections.Generic;
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

        //TODO: Client - get all numeric values
        //TODO: Client - make values required
        //TODO: Get colors via picker?
        //TODO: 
        //TODO: Classes for all the input?
        //TODO: Build pipeline
        [HttpPost]
        public ActionResult GenerateImages(HttpPostedFileBase promptsInputFile, HttpPostedFileBase answersInputFile)
        {
            var promptCards = GetCardsFromStream(promptsInputFile);
            var answerCards = GetCardsFromStream(answersInputFile);

            var imageCreationProcess = new ImageCreationProcess();
            var dateStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture);
            var fileName = $"Card Images {dateStamp}.zip";

            //TODO: Skip empty lines or lines that start with //, and maybe use the correct encoding?

            var cardWidthInInches = 1.75f;
            var cardHeightInInches = 3f;
            var bleedSizeInInches = .25f;
            var promptFontFamily = "Times New Roman";
            var answerFontFamily = "Arial";
            var borderRadius = 40;
            var borderPaddingInInches = .1f;
            var maxPromptTextFontSize = 15;
            var maxAnswerTextFontSize = 15;
            var promptCardFrontBackgroundColorText = "255, 255, 0";
            var promptCardFrontTextColorText = "0, 0, 0";
            var answerCardFrontBackgroundColorText = "0, 0, 0";
            var answerCardFrontTextColorText = "255, 255, 0";

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
                promptCardFrontBackgroundColorText,
                promptCardFrontTextColorText,
                answerCardFrontBackgroundColorText,
                answerCardFrontTextColorText);
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

        //private static IEnumerable<string> LoadCards(string path)
        //{
        //    return System.IO.File.ReadAllLines(path, Encoding.Default)
        //        ;
        //}
    }
}
