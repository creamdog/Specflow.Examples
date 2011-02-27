using System;
using service.tests.Net.Service;
using TechTalk.SpecFlow;
using System.Linq;

namespace service.tests
{
    public partial class Bindings
    {
        [AfterScenario]
        public static void AfterScenario()
        {
            if (ScenarioContext.Current.ContainsKey("Webservice"))
                Webservice.Dispose();
            
            ScenarioContext.Current.Clear();
        }
    }

    [Binding]
    public class Converters
    {
        [StepArgumentTransformation]
        public double TransformDouble(string expr)
        {
            return Convert.ToDouble(expr);
        }

        [StepArgumentTransformation]
        public Uri TransformUri(string expr)
        {
            return new Uri(expr);
        }

        [StepArgumentTransformation]
        public ArbitraryWebservice.MethodParamDescriptor[] TransformParams(string expr)
        {
            var parameterList = expr.Split(new[] { " and " }, StringSplitOptions.None);
            return parameterList.Select(p => new ArbitraryWebservice.MethodParamDescriptor { Name = p.Split('=')[0].Trim(), Value = p.Split('=')[1].Trim() }).ToArray();
        }
    }
}
