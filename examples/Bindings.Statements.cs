using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using service.tests.Extensions;
using service.tests.Net;
using service.tests.Net.Service;
using TechTalk.SpecFlow;

namespace service.tests
{
    [Binding]
    public partial class Bindings
    {
        [Given("webservice named (.*) with the wsdl url (.*)")]
        public void GivenWebService(string name, Uri wsdl)
        {
            WebServiceName = name;
            WebServiceWsdlUrl = wsdl;
        }

        [When("I authenticate using basic authentication with the username (.*) and password (.*) and the domain (.*)")]
        public void AuthenticateUsingBasicAuthentication(string username, string password, string domain)
        {
            Webservice = null;
            AuthenticationMethod = AuthenticationMethod.Basic;
            Username = username;
            Password = password;
            Domain = domain;
        }

        [When("I authenticate using forms authentication via the url (.*) with the username (.*) and the password (.*)")]
        public void AuthenticateUsingFormsAuthentication(Uri url, string username, string password)
        {
            Webservice = null;
            AuthenticationMethod = AuthenticationMethod.Forms;
            FormsAuthenticationUrl = url;
            Username = username;
            Password = password;
        }

        [When("I call the method (.*) with no parameters")]
        public void WhenICallTheMethodWithNoParameters(string method)
        {
            WebServiceMethodResult = Webservice.Call(method, new ArbitraryWebservice.MethodParamDescriptor[0]);
        }

        [When("I call the method (.*) with the parameter (.*)")]
        public void WhenICallTheMethodWithParameter(string method, ArbitraryWebservice.MethodParamDescriptor[] parameters)
        {
            WebServiceMethodResult = Webservice.Call(method, parameters );
        }

        [When("I call the method (.*) with the parameters (.*)")]
        public void WhenICallTheMethodWithParameters(string method, ArbitraryWebservice.MethodParamDescriptor[] parameters)
        {
            WebServiceMethodResult = Webservice.Call(method, parameters);
        }

        [Then("I expect it to return an object matching content (.*)")]
        public void ExpectItToReturnAnObjectMatchingContent(string notation)
        {
            var serializedResult = new StringBuilder();
            new JsonSerializer().Serialize(new StringWriter(serializedResult), WebServiceMethodResult);
            var left = new Babelfish.JSON.JSONDocument(string.Format("{{root:{0}}}", notation));
            var right = new Babelfish.JSON.JSONDocument(string.Format("{{root:{0}}}", serializedResult));
            left.AssertMemberEquality(right);
        }

        [Then("I expect it to return html containing (.*)")]
        public void ExpectItToReturnHtmlContaining(string notation)
        {
            var left = new Babelfish.HTML.HTMLDocument(notation);
            var right = new Babelfish.HTML.HTMLDocument(WebServiceMethodResult as string);
            left.AssertMemberEquality(right);
        }

        [Then("I expect it to return a string containing (.*)")]
        public void ExpectItToReturnAStringContaining(string content)
        {
            var result = WebServiceMethodResult as string ?? string.Empty;
            
            if (!result.Contains(content))
                Assert.Fail(string.Format("expected \"{0}\" but got \"{1}\"", content, result));
        }

        [Then("I expect it to return a string matching (.*)")]
        public void ExpectItToReturnAStringMatching(string left)
        {
            var right = WebServiceMethodResult.ToString();
            Assert.AreEqual(left, right);
        }

        [Then("I expect it to return the number (.*)")]
        public void ExpectItToReturnTheNumber(double number)
        {
            Assert.AreEqual(number, WebServiceMethodResult);
        }

        [Then("I expect it to return a base64 string matching (.*)")]
        public void ExpectItToReturnAStringInBase64Matching(string left)
        {
            var right = Convert.ToBase64String(Encoding.ASCII.GetBytes(WebServiceMethodResult.ToString()));

            if (WebServiceMethodResult.GetType() == typeof(string))
                right = Convert.ToBase64String(Encoding.ASCII.GetBytes((string)WebServiceMethodResult));
            else if (WebServiceMethodResult.GetType() == typeof(byte[]))
                right = (WebServiceMethodResult as byte[]).ToBase64String();

            Console.WriteLine("left='{0}', right='{1}'",left,right);

            Assert.AreEqual(left, right);
        }
    }
}
