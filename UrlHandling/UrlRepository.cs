using System;
using System.Collections.Generic;
using System.Linq;

namespace RestNexus.UrlHandling
{
    public class UrlRepository
    {
        private readonly IUrlHandlerStorage _storage;
        private readonly Dictionary<UrlTemplate, UrlHandler> _handlers;

        public UrlRepository(IUrlHandlerStorage storage)
        {
            _storage = storage;

            var handlers = _storage.LoadHandlers();
            _handlers = handlers.ToDictionary(k => new UrlTemplate(k.UrlTemplate) { IsTemplate = true });
        }

        public UrlHandler Find(string url)
        {
            if (_handlers.TryGetValue(url, out var handler))
                return handler;

            return null;
        }
        public IEnumerable<UrlHandler> GetAll() => _handlers.Values;
        public UrlHandler Get(string urlTemplate) => _handlers.Values.FirstOrDefault(h => h.UrlTemplate == urlTemplate);

        public void Update(string urlTemplate, string newUrlTemplate, string newContent)
        {
            if (string.IsNullOrWhiteSpace(urlTemplate))
                throw new ArgumentNullException(nameof(urlTemplate));

            var templateToUpdate = new UrlTemplate(urlTemplate) { IsTemplate = true };
            if (_handlers.TryGetValue(templateToUpdate, out var handler))
            {
                if (!(handler is JavaScriptUrlHandler jsHandler))
                    throw new NotSupportedException("Only JavaScript handlers are supported at this point.");

                jsHandler.Script = newContent;
                if (!string.IsNullOrEmpty(newUrlTemplate) && newUrlTemplate != urlTemplate)
                {
                    var changedTemplateUrl = new UrlTemplate(newUrlTemplate) { IsTemplate = true };
                    if (_handlers.TryGetValue(changedTemplateUrl, out var theOtherHandler) && theOtherHandler != handler)
                        throw new InvalidOperationException($"A Handler for Url Template '{newUrlTemplate}' already exists.");
                    jsHandler.UrlTemplate = newUrlTemplate;
                    _handlers.Remove(templateToUpdate);
                    _handlers.Add(changedTemplateUrl, jsHandler);
                }
            }
            else
            {
                string newUrl = urlTemplate;
                if (!string.IsNullOrEmpty(newUrl))
                    newUrl = urlTemplate;
                handler = new JavaScriptUrlHandler
                {
                    UrlTemplate = newUrl,
                    Script = newContent,
                };
                var newTemplateUrl = new UrlTemplate(newUrl) { IsTemplate = true };
                if (!_handlers.TryAdd(newTemplateUrl, handler))
                    throw new InvalidOperationException($"A Handler for Url Template '{newUrl}' already exists.");
            }

            _storage.SaveHandler(urlTemplate, handler);
        }

        public bool Remove(string urlTemplate)
        {
            _handlers.Remove(new UrlTemplate(urlTemplate) { IsTemplate = true });
            return _storage.DeleteHandler(urlTemplate);
        }

        private sealed class UrlTemplate : IEquatable<UrlTemplate>
        {
            public bool IsTemplate { get; set; }

            private readonly string[] _segments;
            public UrlTemplate(string urlTemplate)
            {
                _segments = urlTemplate.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            // force the use of Equals
            public override int GetHashCode() => 0;
            public override bool Equals(object obj)
            {
                if (obj is UrlTemplate urlTemplate)
                    return Equals(urlTemplate);
                return false;
            }

            public bool Equals(UrlTemplate other)
            {
                if (other == null)
                    return false;
                if (_segments.Length != other._segments.Length)
                    return false;

                // assume "this" is the template
                if (!IsTemplate && other.IsTemplate)
                    return other.Equals(this);

                for (int i = 0; i < _segments.Length; i++)
                {
                    string mySegment = _segments[i];
                    string theirSegment = other._segments[i];

                    // skip template parameters, assume they match if nothing else fails
                    if (UrlHandler.IsParameter(mySegment))
                        continue;

                    if (mySegment != theirSegment)
                        return false;
                }
                return true;
            }

            public static implicit operator UrlTemplate(string urlTemplate) => new UrlTemplate(urlTemplate);
        }
    }
}
