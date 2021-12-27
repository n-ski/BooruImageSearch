using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using BooruDotNet.Search.Services;
using ImageSearch.Net;
using ImageSearch.ViewModels;

namespace ImageSearch.Helpers
{
    internal static class SearchServiceHelper
    {
        internal static ReadOnlyCollection<SearchServiceViewModel> GetServices()
        {
            HttpClient httpClient = SingletonHttpClient.Current;

            SearchServiceViewModel[] services =
            {
                new SearchServiceViewModel(
                    new DanbooruService(httpClient),
                    "Danbooru",
                    "ImageSearch.Resources.ServiceIcons.Danbooru.png"),
                new SearchServiceViewModel(
                    new IqdbService(httpClient),
                    "IQDB (multi-service)",
                    "ImageSearch.Resources.ServiceIcons.IQDB.png"),
                new SearchServiceViewModel(
                    new IqdbService(httpClient, "gelbooru"),
                    "Gelbooru",
                    "ImageSearch.Resources.ServiceIcons.Gelbooru.png"),
            };

            return Array.AsReadOnly(services);
        }
    }
}
