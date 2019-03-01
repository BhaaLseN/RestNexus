declare class http {
    /** Performs a GET request to url
    * @param url The URL to request (must include scheme/host)
    * @param headers (Optional) List of headers to include with the request
    */
    static get(url: string, headers?): httpresponse;

    /** Performs a POST request to url passing body
    * @param url The URL to request (must include scheme/host)
    * @param body Any payload to be passed as JSON content (can be empty)
    * @param headers (Optional) List of headers to include with the request
    */
    static post(url: string, body, headers?): httpresponse;

    /** Performs a PUT request to url passing body
    * @param url The URL to request (must include scheme/host)
    * @param body Any payload to be passed as JSON content (can be empty)
    * @param headers (Optional) List of headers to include with the request
    */
    static put(url: string, body, headers?): httpresponse;

    /** Performs a web request (using the passed method) to url, optionally passing body
    * @param url The URL to request (must include scheme/host)
    * @param method A valid HTTP Verb (such as POST or PATCH)
    * @param body (Optional) Any payload to be passed as JSON content (can be empty)
    * @param headers (Optional) List of headers to include with the request
    */
    static put(url: string, method: string, body?, headers?): httpresponse;
}

declare class httpresponse {
    /** The HTTP response code of the request; or -1 in case of error */
    statusCode: number;
    /** The response content as text; or the exception text in case of error */
    response: string;
}
