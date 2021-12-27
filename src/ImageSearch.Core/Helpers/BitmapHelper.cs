using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ImageSearch.Net;
using ReactiveUI;
using Splat;

namespace ImageSearch.Helpers
{
    internal static class BitmapHelper
    {
        private static readonly Lazy<Task<IBitmap>> _errorBitmap = new Lazy<Task<IBitmap>>(async () =>
        {
            IBitmap bitmap = await LoadBitmapAsync("ImageSearch.Resources.ImageError.png", typeof(BitmapHelper).Assembly, default, default);

            return bitmap;
        });

        private static readonly MemoizingMRUCache<Uri, Task<IBitmap>> _uriCache = new MemoizingMRUCache<Uri, Task<IBitmap>>(
            async (uri, _) =>
            {
                IBitmap? bitmap = null;

                try
                {
                    using Stream stream = await SingletonHttpClient.Current.GetStreamAsync(uri);

                    // Splat has a bug with unfreezable images so copy the stream to memory first.
                    Stream memory = new MemoryStream();
                    await stream.CopyToAsync(memory);
                    memory.Position = 0;

                    bitmap = await BitmapLoader.Current.Load(memory, default, default);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Could not load remote bitmap: {ex}");
                }

                return bitmap ?? await _errorBitmap.Value;
            },
            RxApp.BigCacheLimit);

        public static Task<IBitmap> LoadBitmapAsync(string resourceName, Assembly assembly, float? width, float? height)
        {
            Debug.Assert(string.IsNullOrEmpty(resourceName) is false);
            Debug.Assert(assembly is object);

            return LoadBitmapAsync(() => assembly.GetManifestResourceStream(resourceName)!, width, height);
        }

        public static Task<IBitmap> LoadBitmapAsync(FileInfo file, float? width, float? height)
        {
            Debug.Assert(file is object);
            Debug.Assert(file.Exists);

            return LoadBitmapAsync(() => file.OpenRead(), width, height);
        }

        public static Task<IBitmap> LoadBitmapAsync(Uri uri)
        {
            Debug.Assert(uri is object);

            return _uriCache.Get(uri);
        }

        private static async Task<IBitmap> LoadBitmapAsync(Func<Stream> streamFactory, float? width, float? height)
        {
            Debug.Assert(streamFactory is object);

            IBitmap? bitmap = null;

            try
            {
                using Stream stream = streamFactory();

                bitmap = await BitmapLoader.Current.Load(stream, width, height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not load local bitmap: {ex}");
            }

            return bitmap ?? await _errorBitmap.Value;
        }
    }
}
