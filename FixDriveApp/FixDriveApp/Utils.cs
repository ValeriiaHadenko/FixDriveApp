using System.Text.RegularExpressions;

namespace FixDriveApp
{
    public static class Utils
    {
        public static bool IsValidPhoneNumber(string phoneNumber) =>
            Regex.IsMatch(phoneNumber, @"^\d{3}-\d{3}-\d{4}$");

        public static bool IsValidEmail(string email) =>
            Regex.IsMatch(email, @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");

        public static bool IsValidString(string str) => str != string.Empty;

        public static bool IsValidQuantity(string quantity) => int.Parse(quantity) >= 1;

        public static bool IsValidPrice(string price) => double.Parse(price) >= 1;

        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.Length >= 3 && phoneNumber.Length <= 6)
            {
                return $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3)}";
            }
            else if (phoneNumber.Length > 6)
            {
                return $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6)}";
            }
            else
            {
                return phoneNumber;
            }
        }

    }
}
