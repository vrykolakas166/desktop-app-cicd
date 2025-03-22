using Serilog;

namespace NETX.Helpers
{
    public class VersionHelper
    {
        public static bool IsValidUrl(string url)
        {
            Log.Verbose($"Url to check: {url}");
            // Check if the URL is well-formed and has the correct scheme
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool IsNewVersion(string? oldVer, string? newVer)
        {
            ArgumentNullException.ThrowIfNull(oldVer);
            ArgumentNullException.ThrowIfNull(newVer);

            Log.Verbose($"Old version: {oldVer}");
            Log.Verbose($"New version: {newVer}");

            return new Version(oldVer).CompareTo(new Version(newVer)) < 0;
        }
    }
}
