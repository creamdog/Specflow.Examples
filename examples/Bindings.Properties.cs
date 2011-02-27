using System;
using service.tests.Net;
using service.tests.Net.Service;
using TechTalk.SpecFlow;

namespace service.tests
{
    public partial class Bindings
    {
        private static AuthenticationMethod AuthenticationMethod
        {
            get { return ScenarioContext.Current.ContainsKey("AuthenticationType") ? (AuthenticationMethod)ScenarioContext.Current["AuthenticationType"] : AuthenticationMethod.None; }
            set { ScenarioContext.Current["AuthenticationType"] = value; }
        }

        private static object WebServiceMethodResult
        {
            get { return ScenarioContext.Current.ContainsKey("WebServiceMethodResult") ? ScenarioContext.Current.Get<object>("WebServiceMethodResult") : null; }
            set { ScenarioContext.Current["WebServiceMethodResult"] = value; }
        }

        private static ArbitraryWebservice Webservice
        {
            get
            {
                if (ScenarioContext.Current.ContainsKey("Webservice"))
                    return ScenarioContext.Current.Get<ArbitraryWebservice>("Webservice");

                switch (AuthenticationMethod)
                {
                    case AuthenticationMethod.Forms:
                        ScenarioContext.Current["Webservice"] = ArbitraryWebservice.GetFormsAuthenticatedService(FormsAuthenticationUrl, Username, Password, WebServiceWsdlUrl, WebServiceName);
                        break;
                    case AuthenticationMethod.Basic:
                        ScenarioContext.Current["Webservice"] = ArbitraryWebservice.GetBasicAuthenticatedService(Username, Password, Domain, WebServiceWsdlUrl, WebServiceName);
                        break;
                    default:
                        ScenarioContext.Current["Webservice"] = ArbitraryWebservice.GetService(WebServiceWsdlUrl, WebServiceName);
                        break;
                }

                return ScenarioContext.Current.Get<ArbitraryWebservice>("Webservice");
            }
            set
            {
                if (value == null && ScenarioContext.Current.ContainsKey("Webservice"))
                {
                    Webservice.Dispose();
                    ScenarioContext.Current.Remove("Webservice");
                }

                if (value != null)
                    ScenarioContext.Current["Webservice"] = value;
            }
        }

        private static string WebServiceName
        {
            get { return ScenarioContext.Current.ContainsKey("WebServiceName") ? ScenarioContext.Current.Get<string>("WebServiceName") : null; }
            set { ScenarioContext.Current["WebServiceName"] = value; }
        }

        private static Uri WebServiceWsdlUrl
        {
            get { return ScenarioContext.Current.ContainsKey("WebServiceWsdlUrl") ? ScenarioContext.Current.Get<Uri>("WebServiceWsdlUrl") : null; }
            set { ScenarioContext.Current["WebServiceWsdlUrl"] = value; }
        }

        private static Uri FormsAuthenticationUrl
        {
            get { return ScenarioContext.Current.ContainsKey("FormsAuthenticationUrl") ? ScenarioContext.Current.Get<Uri>("FormsAuthenticationUrl") : null; }
            set { ScenarioContext.Current["FormsAuthenticationUrl"] = value; }
        }

        private static string Username
        {
            get { return ScenarioContext.Current.ContainsKey("Username") ? ScenarioContext.Current.Get<string>("Username") : null; }
            set { ScenarioContext.Current["Username"] = value; }
        }

        private static string Password
        {
            get { return ScenarioContext.Current.ContainsKey("Password") ? ScenarioContext.Current.Get<string>("Password") : null; }
            set { ScenarioContext.Current["Password"] = value; }
        }

        private static string Domain
        {
            get { return ScenarioContext.Current.ContainsKey("Domain") ? ScenarioContext.Current.Get<string>("Domain") : null; }
            set { ScenarioContext.Current["Domain"] = value; }
        }
    }
}
