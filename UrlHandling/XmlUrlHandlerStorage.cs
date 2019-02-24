using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void SaveHandler(string urlTemplate, UrlHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (!(handler is JavaScriptUrlHandler jsHandler))
                throw new NotSupportedException("Only JavaScript handlers are supported at this point.");

            var existingHandler = _dataXml.Root.Elements(HandlerElementName).FirstOrDefault(e => e.Attribute(HandlerUrlTemplateAttributeName)?.Value == urlTemplate);
            if (existingHandler != null)
            {
                existingHandler.SetAttributeValue(HandlerFileNameAttributeName, WriteContent(existingHandler.Attribute(HandlerFileNameAttributeName)?.Value, jsHandler.Script));
                existingHandler.SetAttributeValue(HandlerUrlTemplateAttributeName, handler.UrlTemplate);
            }
            else
            {
                _dataXml.Root.Add(new XElement(HandlerElementName,
                    // we only support JavaScript at this point, so we can hardcode the type here.
                    new XAttribute(HandlerTypeAttributeName, "js"),
                    new XAttribute(HandlerFileNameAttributeName, WriteContent(null, jsHandler.Script)),
                    new XAttribute(HandlerUrlTemplateAttributeName, handler.UrlTemplate)));
            }

            _dataXml.Save(_dataXmlPath);
        }

        public bool DeleteHandler(string urlTemplate)
        {
            var existingHandler = _dataXml.Root.Elements(HandlerElementName).FirstOrDefault(e => e.Attribute(HandlerUrlTemplateAttributeName)?.Value == urlTemplate);
            if (existingHandler == null)
                return false;

            string fileName = existingHandler.Attribute(HandlerFileNameAttributeName)?.Value;
            TryDeleteContent(fileName);

            existingHandler.Remove();
            _dataXml.Save(_dataXmlPath);
            return true;
        }

        private void TryDeleteContent(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            string filePath = Path.Combine(_dataDirectory, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
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
        private string WriteContent(string fileName, string content)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = GenerateFileName();

            string filePath = Path.Combine(_dataDirectory, fileName);
            File.WriteAllText(filePath, content);

            return fileName;
        }

        private string GenerateFileName()
        {
            int fileCount = Directory.GetFiles(_dataDirectory, "*.js").Length;

            while (File.Exists(Path.Combine(_dataDirectory, fileCount + ".js")))
                fileCount++;

            return fileCount + ".js";
        }

        private static XDocument LoadXml(string dataXmlPath)
        {
            if (!File.Exists(dataXmlPath))
                return new XDocument(new XElement("handlers"));

            return XDocument.Load(dataXmlPath);
        }
    }
}
