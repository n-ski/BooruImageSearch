using System;
using System.Net;
using System.Net.Http;
using System.Windows;
using DynamicData.Binding;
using ImageSearch.ViewModels;
using ImageSearch.WPF.Views;
using ReactiveUI;
using Splat;
using TomsToolbox.Essentials;
using TomsToolbox.Wpf.Styles;

namespace ImageSearch.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string[] _themes =
        {
            "Assets/Themes/LightTheme.xaml",
            "Assets/Themes/DarkTheme.xaml",
        };

        static App()
        {
            HttpClient = new HttpClient(new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            });

            ViewSettings.Default.WhenAnyPropertyChanged().WhereNotNull().Subscribe(s => s.Save());

            Locator.CurrentMutable
                .RegisterAnd<IViewFor<SearchResultViewModel>, SearchResultView>()
                .RegisterAnd<IViewFor<QueueItemViewModel>, QueueItemView>()
                .RegisterAnd<IViewFor<FileQueueItemViewModel>, FileQueueItemListView>(ViewContracts.QueueList)
                .RegisterAnd<IViewFor<UriQueueItemViewModel>, UriQueueItemListView>(ViewContracts.QueueList)
                .RegisterPlatformBitmapLoader();
        }

        internal static HttpClient HttpClient { get; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WpfStyles.RegisterDefaultStyles(Resources).RegisterDefaultWindowStyle();

            RegisterThemes();
        }

        private void RegisterThemes()
        {
            var themeDictionary = new ResourceDictionary();

            Current.Resources.MergedDictionaries.Insert(0, themeDictionary);

            var themeContainer = themeDictionary.MergedDictionaries;

            var themeDictionaries = Array.ConvertAll(_themes, theme => new Lazy<ResourceDictionary>(() =>
                new ResourceDictionary { Source = typeof(App).Assembly.GeneratePackUri(theme) }));

            ViewSettings.Default.WhenAnyValue(x => x.ColorThemeIndex)
                .Subscribe(colorIndex =>
                {
                    colorIndex = Math.Clamp(colorIndex, 0, themeDictionaries.Length - 1);

                    themeContainer.Clear();
                    themeContainer.Add(themeDictionaries[colorIndex].Value);
                });
        }
    }
}
