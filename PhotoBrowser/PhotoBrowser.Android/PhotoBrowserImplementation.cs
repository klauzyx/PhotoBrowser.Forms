using Com.Stfalcon.Frescoimageviewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace Stormlion.PhotoBrowser.Droid
{
    public class PhotoBrowserImplementation : IPhotoBrowser
    {
        private PhotoBrowser _photoBrowser;
        protected static ImageViewer _imageViewer;

        public void Show(PhotoBrowser photoBrowser)
        {
            _photoBrowser = photoBrowser;
            ImageViewer.Builder builder = new ImageViewer.Builder(Platform.Context, photoBrowser.Photos.Select(x => x.URL).ToArray());
            ImageOverlayView overlay = new ImageOverlayView(Platform.Context, photoBrowser);

            
            
            builder.SetBackgroundColor(ColorExtensions.ToAndroid(photoBrowser.BackgroundColor));

            builder.SetOverlayView(overlay);
            builder.SetContainerPaddingPx(photoBrowser.Android_ContainerPaddingPx);

            builder.SetImageChangeListener(overlay);
            builder.SetStartPosition(photoBrowser.StartIndex);
            builder.SetOnDismissListener(new DismissListener(_photoBrowser));

            _imageViewer = builder.Show();
        }

        public void Close()
        {
            if (_imageViewer != null)
            {
                _imageViewer.OnDismiss();
                _imageViewer = null;
            }
        }

        private class DismissListener : Java.Lang.Object, ImageViewer.IOnDismissListener
        {
            private readonly PhotoBrowser _photoBrowser;

            public DismissListener(PhotoBrowser photoBrowser)
            {
                _photoBrowser = photoBrowser;
            }

            public void OnDismiss()
            {
                _photoBrowser?.RaiseClosed();
            }
        }
    }
}