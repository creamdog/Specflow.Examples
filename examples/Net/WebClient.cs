using System;
using System.Net;
using System.Text;
using System.Web;
using Babelfish;
using Babelfish.Extensions;
using System.Linq;

namespace service.tests.Net
{
    public class WebClient : System.Net.WebClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        public CookieContainer CookieContainer
        {
            get
            {
                return _cookieContainer;
            }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);

            if (request is HttpWebRequest)
                (request as HttpWebRequest).CookieContainer = _cookieContainer;

            return request;
        }

        public string FormsAuth(Uri url, string username, string password)
        {
            var response = DownloadData(url);

            INode document = new Babelfish.HTML.HTMLDocument(Encoding.ASCII.GetString(response));

            var postData = string.Empty;

            var formElements = document.Find(n => n.Name.ToLower() == "input" && new[] { "hidden", "button", "submit", "checkbox", "radio", "text", "password" }
            .Contains(n.Attributes["type"]));

            foreach (var formElement in formElements)
            {
                postData += postData.Length > 0 ? "&" : string.Empty;

                var name = formElement.Attributes["name"];
                var value = formElement.Attributes["value"];

                switch (formElement.Attributes["type"])
                {
                    case "text":
                        postData += string.Format("{0}={1}", name, HttpUtility.UrlEncode(username));
                        break;
                    case "password":
                        postData += string.Format("{0}={1}", name, HttpUtility.UrlEncode(password));
                        break;
                    default:
                        if (!string.IsNullOrEmpty(value))
                            postData += string.Format("{0}={1}", name, HttpUtility.UrlEncode(value));
                        break;
                }
            }

            Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string result;
            try
            {
                result = Encoding.ASCII.GetString(UploadData(url, "POST", Encoding.ASCII.GetBytes(postData)));
            }
            catch (WebException ex)
            {
                result = ex.ToString();
            }
            return result;
        }

    }
}
