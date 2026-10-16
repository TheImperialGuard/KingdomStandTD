using System;

namespace Assets._Project.Develop.Runtime.Utilities.Text
{
    // Форма слова после числа по правилам русского языка: 1 уровень, 2 уровня, 5 уровней, 11 уровней, 21 уровень.
    public static class RussianPlural
    {
        private const int Ten = 10;
        private const int Hundred = 100;
        private const int TeensFirst = 11;
        private const int TeensLast = 14;
        private const int FewFirst = 2;
        private const int FewLast = 4;

        public static string Select(int number, string one, string few, string many)
        {
            int lastTwoDigits = Math.Abs(number) % Hundred;

            if (lastTwoDigits >= TeensFirst && lastTwoDigits <= TeensLast)
                return many;

            int lastDigit = lastTwoDigits % Ten;

            if (lastDigit == 1)
                return one;

            if (lastDigit >= FewFirst && lastDigit <= FewLast)
                return few;

            return many;
        }
    }
}
