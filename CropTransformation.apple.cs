using System;
using Ascetic.UI;
using CoreGraphics;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Plugin.AsceticCropper.CropTransformation))]
namespace Plugin.AsceticCropper
{
    public class CropTransformation : TransformationBase, ICropTransformation
    {
        public double XOffsetFactor { get; set; }
        public double YOffsetFactor { get; set; }
        public double WidthFactor { get; set; }
        public double HeightFactor { get; set; }

        public CropTransformation()
        { }


        public override string Key
        {
            get
            {
                return string.Format("CropTransformation,XOffsetFactor={0},YOffsetFactor={1},width={2},height={3}",
                XOffsetFactor, YOffsetFactor, WidthFactor, HeightFactor);
            }
        }

        protected override UIImage Transform(UIImage sourceBitmap, string path, ImageSource source, bool isPlaceholder, string key)
        {
            return ToCropped(sourceBitmap, XOffsetFactor, YOffsetFactor, WidthFactor, HeightFactor);
        }

        public static UIImage ToCropped(UIImage image, double XOffsetFactor, double YOffsetFactor, double WidthFactor, double HeightFactor)
        {
            var imgSize = image.Size;
            UIGraphics.BeginImageContextWithOptions(new CGSize(WidthFactor * imgSize.Width, HeightFactor * imgSize.Height), false, (nfloat)0.0);

            try
            {
                using (var context = UIGraphics.GetCurrentContext())
                {
                    var clippedRect = new CGRect(0, 0, WidthFactor * imgSize.Width, HeightFactor * imgSize.Height);
                    context.ClipToRect(clippedRect);
                    var x = -XOffsetFactor * imgSize.Width;
                    var y = -YOffsetFactor * imgSize.Height;
                    var drawRect = new CGRect(x, y, imgSize.Width, imgSize.Height);
                    image.Draw(drawRect);
                    var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
                    return modifiedImage;
                }
            }
            finally
            {
                UIGraphics.EndImageContext();
            }
        }
    }
}
