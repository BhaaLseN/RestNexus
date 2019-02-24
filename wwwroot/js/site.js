function prettyFullUrl(baseUrl, urlTemplate) {
    var cleanUrl = urlTemplate.replace(/^\/*/g, '').replace(/\/+/g, '/');

    var segments = cleanUrl
        .split('/')
        .map(function (s) {
            if (s.startsWith(':'))
                return '<i>' + s.substring(1) + '</i>';
            return s;
        });
    segments.unshift(baseUrl);

    var fullUrl = segments.join('/');
    return fullUrl;
}
