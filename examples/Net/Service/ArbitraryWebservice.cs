using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.Linq;
using service.tests.Extensions;

namespace service.tests.Net.Service
{
    public class ArbitraryWebservice : IDisposable
    {
        private SoapHttpClientProtocol _service;

        private static SoapHttpClientProtocol GenerateService(Stream stream, string serviceName)
        {
            var serviceDescription = ServiceDescription.Read(stream);
            var serviceDescriptionImporter = new ServiceDescriptionImporter
            {
                ProtocolName = "Soap12",
                Style = ServiceDescriptionImportStyle.Client,
                CodeGenerationOptions = CodeGenerationOptions.GenerateProperties
            };

            serviceDescriptionImporter.AddServiceDescription(serviceDescription, null, null);

            var codeNamespace = new CodeNamespace();
            var codeCompileUnit = new CodeCompileUnit();

            codeCompileUnit.Namespaces.Add(codeNamespace);

            var warnings = serviceDescriptionImporter.Import(codeNamespace, codeCompileUnit);

            if (warnings > 0)
                throw new Exception(string.Format("{0}", warnings));

            using (var codeDomProvider = CodeDomProvider.CreateProvider("C#"))
            {
                var compilerParameters = new CompilerParameters(new[] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" }) { GenerateInMemory = true };

                var results = codeDomProvider.CompileAssemblyFromDom(compilerParameters, codeCompileUnit);

                if (results.Errors.Count > 0)
                    throw new Exception(string.Join(", ", results.Errors.Cast<CompilerError>().Select(e => e.ToString()).ToArray()));

                var service = results.CompiledAssembly.CreateInstance(serviceName);

                return service as SoapHttpClientProtocol;
            }
        }

        public static ArbitraryWebservice GetFormsAuthenticatedService(Uri loginUrl, string username, string password, Uri wsdlUrl, string serviceName)
        {
            using (var webClient = new WebClient())
            {
                webClient.FormsAuth(loginUrl, username, password);

                using (var stream = webClient.OpenRead(wsdlUrl))
                {
                    var service = GenerateService(stream, serviceName);
                    service.CookieContainer = webClient.CookieContainer;
                    return new ArbitraryWebservice { _service = service };
                }
            }
        }

        public static ArbitraryWebservice GetBasicAuthenticatedService(string username, string password, string domain, Uri wsdlUrl, string serviceName)
        {
            using (var webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(username, password, domain);

                using (var stream = webClient.OpenRead(wsdlUrl))
                {
                    var service = GenerateService(stream, serviceName);
                    service.Credentials = new NetworkCredential(username, password, domain);
                    return new ArbitraryWebservice { _service = service };
                }
            }
        }

        public static ArbitraryWebservice GetService(Uri wsdlUrl, string serviceName)
        {
            using (var webClient = new WebClient())
            {
                using (var stream = webClient.OpenRead(wsdlUrl))
                {
                    var service = GenerateService(stream, serviceName);
                    service.CookieContainer = webClient.CookieContainer;
                    return new ArbitraryWebservice { _service = service };
                }
            }
        }

        public object Call(string methodName, MethodParamDescriptor[] args)
        {
            var method = _service.GetType().GetMethod(methodName);

            if (method == null)
                throw new MissingMethodException(string.Format("this service contains no method named {0}", methodName));

            var methodParams = new List<object>();

            foreach (var p in method.GetParameters())
            {
                var param = p;

                if (!args.Any(a => a.Name == param.Name))
                    throw new ArgumentException("expected parameter not supplied", p.Name);

                methodParams.Add(args.Where(a => a.Name == param.Name)
                    .Select(a => a.Value.Convert(param.ParameterType))
                    .FirstOrDefault());
            }

            return method.Invoke(_service, methodParams.ToArray());
        }

        public void Dispose()
        {
            _service.Dispose();
        }

        public class MethodParamDescriptor
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }
    }
}
