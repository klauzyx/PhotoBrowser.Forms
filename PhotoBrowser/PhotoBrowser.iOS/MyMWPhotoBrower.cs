using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Foundation;

using Ricardo.LibMWPhotoBrowser.iOS;

using UIKit;

using Xamarin.Forms.Platform.iOS;

namespace Stormlion.PhotoBrowser.iOS
{
    public class MyMWPhotoBrower : MWPhotoBrowserDelegate
    {
        protected PhotoBrowser _photoBrowser;

        protected List<MWPhoto> _photos = new List<MWPhoto>();

        public MyMWPhotoBrower(PhotoBrowser photoBrowser)
        {
            _photoBrowser = photoBrowser;
        }

        public void Show()
        {
            _photos = new List<MWPhoto>();

            foreach (Photo p in _photoBrowser.Photos)
            {
                MWPhoto mp;

                var regex = new Regex(@"data:(?<mime>[\w/\-\.]+);(?<encoding>\w+),(?<data>.*)", RegexOptions.Compiled);
                var match = regex.Match(p.URL);
                if (match.Success)
                {
                    var base64Data = match.Groups["data"].Value;
                    var encodedDataAsBytes = Convert.FromBase64String(base64Data);
                    var encodedData = NSData.FromArray(encodedDataAsBytes);
                    var image = UIImage.LoadFromData(encodedData);
                    mp = MWPhoto.FromImage(image);
                }
                else
                {
                    var url = new NSUrl(p.URL);
                    mp = MWPhoto.FromUrl(url);
                }

                if (!string.IsNullOrWhiteSpace(p.Title))
                    mp.Caption = p.Title;
                
                _photos.Add(mp);
            }

            MWPhotoBrowser browser = new MWPhotoBrowser(this)
            {
                EnableGrid = _photoBrowser.EnableGrid,

                BrowserBackgroundColor = _photoBrowser.BackgroundColor.ToUIColor(),
                
                DisplayActionButton = _photoBrowser.ActionButtonPressed != null,

                ZoomPhotosToFill = _photoBrowser.iOS_ZoomPhotosToFill

            };

            
            browser.SetCurrentPhoto((nuint)_photoBrowser.StartIndex);


            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            
            vc.PresentViewController(new UINavigationController(browser), true, null);
        }

        public override MWPhoto GetPhoto(MWPhotoBrowser photoBrowser, nuint index) => _photos[(int)index];

        public override nuint NumberOfPhotosInPhotoBrowser(MWPhotoBrowser photoBrowser) => (nuint)_photos.Count;


        public override void OnActionButtonPressed(MWPhotoBrowser photoBrowser, nuint index)
        {
            _photoBrowser.ActionButtonPressed?.Invoke((int)index);
        }

        public override void DidDisplayPhoto(MWPhotoBrowser photoBrowser, nuint index)
        {
            _photoBrowser.DidDisplayPhoto?.Invoke((int)index);
        }


        public void Close()
        {
            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
            _photoBrowser?.RaiseClosed();
        }
    }
}
