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
