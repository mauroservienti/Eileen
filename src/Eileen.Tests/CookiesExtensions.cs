using System;
using System.Collections.Generic;
using System.Linq;
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

        public static Dictionary<string, string> GetRawCookie(this HttpResponse response, string name)
        {
            var setCookieHeader = response.Headers["Set-Cookie"];
            var rawCookies = setCookieHeader.ToArray();

            foreach (string rawCookie in rawCookies)
            {
                var elements = rawCookie.Split(';');
                var cookieDictionary = elements.ToDictionary(e => e.Split('=').First().Trim(), e => e.Split('=').LastOrDefault()?.Trim());
                if (cookieDictionary.ContainsKey(name))
                {
                    return cookieDictionary;
                }
            }

            return new Dictionary<string, string>();
        }
    }
}