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
        
        if (decodedUrl.Length < minLength || decodedUrl.Length > maxLength) return false;
        
        return Uri.TryCreate(decodedUrl, UriKind.Absolute, out Uri uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}