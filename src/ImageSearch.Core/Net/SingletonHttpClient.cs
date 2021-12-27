using System;
using System.Net.Http;
using Splat;

namespace ImageSearch.Net
{
    public static class SingletonHttpClient
    {
        private const string _contract = "ImageSearchHttpClient";

        public static void Register(Func<HttpClient> valueFactory)
        {
            Locator.CurrentMutable.RegisterLazySingleton(valueFactory, _contract);
        }

        internal static HttpClient Current => Locator.Current.GetService<HttpClient>(_contract) ?? throw new InvalidOperationException("HttpClient isn't registered.");
    }
}
