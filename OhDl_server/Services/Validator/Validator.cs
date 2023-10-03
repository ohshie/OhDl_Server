using System.Text.RegularExpressions;
using System.Web;

namespace OhDl_server.Validator;

public static class Validator
{
    public static bool ValidateUrl(string codedUrl, bool required, int minLength = 0, int maxLength = int.MaxValue)
    {
        var decodedUrl = HttpUtility.UrlDecode(codedUrl);
        
        decodedUrl = decodedUrl.Trim();
        if (required == false && decodedUrl == "") return true;
        if (required && decodedUrl == "") return false;

        Regex pattern = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
        Match match = pattern.Match(decodedUrl);
        if (match.Success == false) return false;
        return true;
    }
}