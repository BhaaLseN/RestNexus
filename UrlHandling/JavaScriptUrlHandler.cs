using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jint;
using Jint.Parser;
using Newtonsoft.Json.Linq;
using RestNexus.JintInterop;

namespace RestNexus.UrlHandling
{
    public class JavaScriptUrlHandler : UrlHandler
    {
        public string ScriptFile { get; set; }

        private readonly Dictionary<HttpVerb, JavaScriptHandleMethod> _handleMethods = new Dictionary<HttpVerb, JavaScriptHandleMethod>();
        private string _script;

        private const string ParameterName = "request";

        public override object Handle(UrlRequest request)
        {
            EnsureScript();

            if (!_handleMethods.TryGetValue(request.Method, out var handleMethod))
                return null;

            var engine = Prepare(request);
            string methodInvocation = handleMethod.GetInvocationString(ParameterName);
            var completionValue = engine
                .Execute(methodInvocation)
                .GetCompletionValue();

            return completionValue.ToObject();
        }

        private Engine Prepare(UrlRequest request)
        {
            var engine = new Engine();

            object bodyParam = request.Body;
            // Jint cannot natively use Newtonsoft objects, lets wrap them
            if (bodyParam is JObject jObject)
                bodyParam = new NewtonsoftObjectInstance(engine, jObject);

            string url = request.Url;
            var parameters = ExtractParameters(UrlTemplate, url);

            var completionValue = engine
                .Execute(_script)
                .SetValue("http", new HttpFunctions())
                .SetValue(ParameterName, new
                {
                    url = url,
                    parameters = parameters,
                    body = bodyParam,
                });

            return engine;
        }

        private void EnsureScript()
        {
            if (!string.IsNullOrWhiteSpace(_script))
                return;

            _script = File.ReadAllText(ScriptFile);
            var parser = new JavaScriptParser();
            var ast = parser.Parse(_script);
            foreach (var function in ast.FunctionDeclarations)
            {
                // try to find a function that matches the verb name
                string functionName = function.Id.Name;
                if (!Enum.TryParse<HttpVerb>(functionName, true, out var verb))
                    continue;

                _handleMethods[verb] = new JavaScriptHandleMethod(functionName)
                {
                    // assume if there is one parameter, we should pass our request object in there
                    WantsParameter = function.Parameters.Count() == 1,
                };
            }
        }

        private sealed class JavaScriptHandleMethod
        {
            public JavaScriptHandleMethod(string functionName)
            {
                Name = functionName;
            }

            public string Name { get; }
            public bool WantsParameter { get; set; }

            public string GetInvocationString(string parameterName)
            {
                if (WantsParameter)
                    return $"{Name}({parameterName});";

                return $"{Name}();";
            }
        };
    }
}
