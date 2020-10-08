using System;
using System.Collections.Generic;
using System.Linq;
using Jint;
using Jint.Native.Json;
using Jint.Native.Object;
using Jint.Parser;
using Newtonsoft.Json.Linq;
using RestNexus.JintInterop;

namespace RestNexus.UrlHandling
{
    public class JavaScriptUrlHandler : UrlHandler
    {
        private string _script;
        public string Script
        {
            get { return _script; }
            set
            {
                if (_script == value)
                    return;
                _script = value;
                _isPrepared = false;
            }
        }

        private readonly Dictionary<HttpVerb, JavaScriptHandleMethod> _handleMethods = new Dictionary<HttpVerb, JavaScriptHandleMethod>();
        private bool _isPrepared;

        private const string ParameterName = "request";

        public override object Handle(UrlRequest request)
        {
            PrepareScript();

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

            engine
                .SetValue("http", new HttpFunctions())
                .SetValue("globals", JsonString(engine, JavaScriptEnvironment.Instance.Globals))
                .Execute(Script)
                .SetValue(ParameterName, new
                {
                    url = url,
                    parameters = parameters,
                    headers = request.Headers,
                    body = bodyParam,
                });

            return engine;
        }

        private ObjectInstance JsonString(Engine engine, string jsonString)
        {
            var parser = new JsonParser(engine);
            var jsValue = parser.Parse(jsonString);
            var o = jsValue.TryCast<ObjectInstance>();
            // o could be null, when jsonString is not a valid JSON string for example.
            return o;
        }

        private void PrepareScript()
        {
            if (_isPrepared)
                return;

            var parser = new JavaScriptParser();
            var ast = parser.Parse(Script);
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

            _isPrepared = true;
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
