namespace Assets._Project.Develop.Runtime.Utilities.Text
{
    // Неразрывный пробел держит номер в конце текста на одной строке с предыдущим словом: «Лучники III» не оставит «III»
    // на отдельной строке. Номер — последнее слово из арабских цифр или римских (I, V, X, L, C, D, M).
    public static class NonBreakingSpace
    {
        public const char Character = ' ';

        private const char Space = ' ';
        private const string RomanDigits = "IVXLCDM";

        public static string BeforeTrailingNumber(string text)
        {
            int lastSpaceIndex = text.LastIndexOf(Space);

            if (lastSpaceIndex < 0 || IsNumber(text.Substring(lastSpaceIndex + 1)) == false)
                return text;

            return text.Remove(lastSpaceIndex, 1).Insert(lastSpaceIndex, Character.ToString());
        }

        private static bool IsNumber(string word)
        {
            if (word.Length == 0)
                return false;

            bool isArabic = true;
            bool isRoman = true;

            foreach (char symbol in word)
            {
                isArabic &= char.IsDigit(symbol);
                isRoman &= RomanDigits.IndexOf(symbol) >= 0;
            }

            return isArabic || isRoman;
        }
    }
}
