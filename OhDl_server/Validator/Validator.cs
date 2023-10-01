using System.Text.RegularExpressions;

namespace OhDl_server.Validator;

public static class Validator
{
    public static bool ValidateUrl(string value, bool required, int minLength = 0, int maxLength = int.MaxValue)
    {
        value = value.Trim();
        if (required == false && value == "") return true;
        if (required && value == "") return false;

        Regex pattern = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
        Match match = pattern.Match(value);
        if (match.Success == false) return false;
        return true;
    }
}