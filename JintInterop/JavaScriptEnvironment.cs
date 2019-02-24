using System.IO;
using Microsoft.Extensions.Configuration;

namespace RestNexus.JintInterop
{
    public class JavaScriptEnvironment
    {
        public static JavaScriptEnvironment Instance { get; internal set; }

        private readonly string _dataDirectory;
        private readonly string _globalsFilePath;

        public JavaScriptEnvironment(IConfiguration configuration)
        {
            _dataDirectory = Path.Combine(configuration.GetValue<string>("Directories:Data"), "environment");
            Directory.CreateDirectory(_dataDirectory);

            _globalsFilePath = Path.Combine(_dataDirectory, "globals.js");
            if (File.Exists(_globalsFilePath))
            {
                Globals = File.ReadAllText(_globalsFilePath);
            }
            else
            {
                Globals = @"{
    // add your settings here.
}";
            }
        }

        public string Globals { get; private set; }

        public void UpdateGlobals(string newGlobals)
        {
            Globals = newGlobals;
            File.WriteAllText(_globalsFilePath, newGlobals);
        }
    }
}
