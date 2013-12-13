using System;
using System.Net;
using System.Windows.Navigation;

namespace PlaystationApp.Tools
{
    internal class AssociationUriMapper : UriMapperBase
    {
        private string _tempUri;

        public override Uri MapUri(Uri uri)
        {
            _tempUri = HttpUtility.UrlDecode(uri.ToString());
            if (!_tempUri.Contains("?authCode=")) return uri;
            int authCodeIndex = _tempUri.IndexOf("authCode=", StringComparison.Ordinal) + 9;
            string authCode = _tempUri.Substring(authCodeIndex);
            return new Uri("/LaunchPage.xaml?authCode=" + authCode, UriKind.Relative);
        }
    }
}