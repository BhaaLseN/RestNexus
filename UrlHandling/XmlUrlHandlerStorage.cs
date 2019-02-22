using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace RestNexus.UrlHandling
{
    internal sealed class XmlUrlHandlerStorage : IUrlHandlerStorage
    {
        private static readonly XName HandlerElementName = "handler";
        private static readonly XName HandlerTypeAttributeName = "type";
        private static readonly XName HandlerFileNameAttributeName = "file";
        private static readonly XName HandlerUrlTemplateAttributeName = "url";

        private readonly string _dataDirectory;
        private readonly string _dataXmlPath;
        private readonly XDocument _dataXml;

        public XmlUrlHandlerStorage(IConfiguration configuration)
        {
            _dataDirectory = configuration.GetValue<string>("Directories:Data");
            Directory.CreateDirectory(_dataDirectory);

            _dataXmlPath = Path.Combine(_dataDirectory, "handlers.xml");
            _dataXml = LoadXml(_dataXmlPath);
        }

        public IEnumerable<UrlHandler> LoadHandlers()
        {
            int handlerNumber = 0;
            foreach (var handlerElement in _dataXml.Root.Elements(HandlerElementName))
            {
                handlerNumber++;

                string type = handlerElement.Attribute(HandlerTypeAttributeName)?.Value;
                if (type != "js")
                    throw new NotSupportedException($"Unsupported Url Handler type '{type}' for handler #{handlerNumber}. Supported types are: js");

                yield return new JavaScriptUrlHandler()
                {
                    UrlTemplate = handlerElement.Attribute(HandlerUrlTemplateAttributeName)?.Value,
                    Script = ReadContent(handlerElement.Attribute(HandlerFileNameAttributeName)?.Value),
                };
            }
        }

        private string ReadContent(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            string filePath = Path.Combine(_dataDirectory, fileName);
            if (!File.Exists(filePath))
                return null;

            return File.ReadAllText(filePath);
        }

        private static XDocument LoadXml(string dataXmlPath)
        {
            if (!File.Exists(dataXmlPath))
                return new XDocument(new XElement("handlers"));

            return XDocument.Load(dataXmlPath);
        }
    }
}
