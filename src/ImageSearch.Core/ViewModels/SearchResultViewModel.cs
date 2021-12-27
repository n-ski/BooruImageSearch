using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using BooruDotNet.Search.Results;
using ImageSearch.Helpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Validation;

namespace ImageSearch.ViewModels
{
    public class SearchResultViewModel : ReactiveObject
    {
        private readonly IResult _result;

        public SearchResultViewModel(IResult result)
        {
            _result = Requires.NotNull(result, nameof(result));

            ImageSize = result.Width.HasValue && result.Height.HasValue
                ? new Size(result.Width.Value, result.Height.Value) : Size.Empty;

            Observable.Return(SourceUri)
                .Where(uri => uri is object && uri.IsAbsoluteUri)
                .Select(uri =>
                {
                    var builder = new UriBuilder("https://www.google.com/s2/favicons") { Query = $"domain={uri.Host}" };

                    return builder.Uri;
                })
                .Select(uri => Observable.FromAsync(() => BitmapHelper.LoadBitmapAsync(uri, default, default)))
                .Switch()
                .ToPropertyEx(this, x => x.SiteIconImage, null, true, RxApp.MainThreadScheduler);

            Observable.Return(ImageUri)
                .WhereNotNull()
                .Select(uri => Observable.FromAsync(() => BitmapHelper.LoadBitmapAsync(uri, default, default)))
                .Switch()
                .ToPropertyEx(this, x => x.PreviewImage, null, true, RxApp.MainThreadScheduler);

            OpenSource = ReactiveCommand.Create(MethodHelper.DoNothing);
            CopySource = ReactiveCommand.Create(MethodHelper.DoNothing);
            SearchForSimilar = ReactiveCommand.Create(MethodHelper.DoNothing);
        }

        public Uri ImageUri => _result.PreviewImageUri;
        public double Similarity => _result.Similarity;
        public Uri SourceUri => _result.Source;
        public Size ImageSize { get; }
        public IBitmap? SiteIconImage { [ObservableAsProperty] get; }
        public IBitmap? PreviewImage { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, Unit> OpenSource { get; }
        public ReactiveCommand<Unit, Unit> CopySource { get; }
        public ReactiveCommand<Unit, Unit> SearchForSimilar { get; }
    }
}
