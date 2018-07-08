namespace AdinaCardGame
{
    public class PromptBlankLine
    {
        public PromptBlankLine(string originalText)
        {
            var indexOfPlaceholder = originalText.IndexOf(PromptBlankPlaceholder);
            CharactersBeforeBlank = originalText.Substring(0, indexOfPlaceholder);
            CharactersAfterBlank = indexOfPlaceholder + 1 < originalText.Length ?
                originalText.Substring(indexOfPlaceholder + 1, originalText.Length - (indexOfPlaceholder + 1)) :
                "";
        }

        private string CharactersBeforeBlank { get; }
        private string CharactersAfterBlank { get; }

        public const char PromptBlankIndicator = '@';
        public const char PromptBlankPlaceholder = '_';

        public int BlankLength { get; set; }

        public string FullLineText
        {
            get
            {
	            var dashesToInsert = new string(PromptBlankPlaceholder, BlankLength);
	            return $"{CharactersBeforeBlank}{dashesToInsert}{CharactersAfterBlank}";
            }
        }
    }
}
