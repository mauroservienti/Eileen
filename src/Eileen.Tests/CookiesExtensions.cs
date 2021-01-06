using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Eileen.Tests
{
    static class CookiesExtensions
    {
        private const string Domain = "localhost";
        public static void AddCookie(this CookieContainer container, string key, string value)
        {
            container.Add(new Cookie(key, value)
            {
                Domain = Domain
            });
        }

        public static void SetCookies(this HttpRequest request, CookieContainer container)
        {
            request.Headers.Add(HeaderNames.Cookie, container.GetCookieHeader(new Uri("http://" + Domain)));
        }
    }
}