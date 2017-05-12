using System;

namespace Smellyriver.TankInspector.Design
{
	internal static class RomanNumberService
    {
        private const string RomanNumbers = " ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫ";    // remember to put a space before all the numbers

        public static string GetRomanNumber(int number)
        {
            if (number < 1 || number > 12)
                throw new NotSupportedException();

            return RomanNumbers[number].ToString();
        }
    }
}
