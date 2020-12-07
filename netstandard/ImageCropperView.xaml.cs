using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FFImageLoading;
using FFImageLoading.Work;
using Xamarin.Forms;

namespace Ascetic.UI
{
    public partial class ImageCropperView : Grid
    {
        public static readonly BindableProperty MaskPainterProperty = BindableProperty.Create(nameof(MaskPainter), typeof(MaskPainter), typeof(ImageCropperView), new CircleMaskPainter() { IsDashed = true, MaskWidth = 100, MaskHeight = 100 }, BindingMode.OneWay);
        public static readonly BindableProperty PhotoSourceProperty = BindableProperty.Create(nameof(PhotoSource), typeof(Xamarin.Forms.ImageSource), typeof(ImageCropperView), null, BindingMode.OneWay);
        /// <summary>
        /// Background color property.
        /// </summary>
        public static new BindableProperty BackgroundColorProperty =
            BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(ImageCropperView), Color.Transparent, BindingMode.OneWay);

        /// <summary>
        /// The border width property.
        /// </summary>
        public static BindableProperty BorderWidthProperty =
            BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(ImageCropperView), 5.0, Xamarin.Forms.BindingMode.OneWay);

        /// <summary>
        /// The border color property.
        /// </summary>
        public static BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ImageCropperView), Color.CornflowerBlue, BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the color which will fill the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The color of the border.</value>
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the width which will have the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The width of the border.</value>
        public double BorderWidth
        {
            get => (double)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the color which will fill the background of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The color of the background.</value>
        public new Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public MaskPainter MaskPainter
        {
            get => (MaskPainter)GetValue(MaskPainterProperty);
            set => SetValue(MaskPainterProperty, value);
        }

        public Xamarin.Forms.ImageSource PhotoSource => (Xamarin.Forms.ImageSource)GetValue(PhotoSourceProperty);

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == MaskPainterProperty.PropertyName)
            {
                frame.CustomPainter = MaskPainter;
            }
            else if (propertyName == PhotoSourceProperty.PropertyName)
            {
                image.Source = PhotoSource;
            }
            else if (propertyName == BackgroundColorProperty.PropertyName)
            {
                frame.BackgroundColor = BackgroundColor;
            }
            else if (propertyName == BorderColorProperty.PropertyName)
            {
                frame.BorderColor = BorderColor;
            }
            else if (propertyName == BorderWidthProperty.PropertyName)
            {
                frame.BorderWidth = BorderWidth;
            }
        }

        ImageInformation ImageInformation { get; set; }

        double startX;
        double startY;
        double imageScale;
        double imageWidth;
        double imageHeight;
        double horizontalPadding;
        double verticalPadding;

        public ImageCropperView()
        {
            InitializeComponent();

            image.SizeChanged += Image_SizeChanged;
        }

        void PanGestureRecognizer_PanUpdated(System.Object sender, Xamarin.Forms.PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
            {
                startX = MaskPainter.MaskX;
                startY = MaskPainter.MaskY;
            }
            else if (e.StatusType == GestureStatus.Running)
            {
                var translationX = startX + e.TotalX;
                var translationY = startY + e.TotalY;

                translationX = Clamp(translationX
                    , horizontalPadding + MaskPainter.MaskWidth / 2
                    , imageWidth + horizontalPadding - MaskPainter.MaskWidth / 2);

                translationY = Clamp(translationY
                    , verticalPadding + MaskPainter.MaskHeight / 2
                    , imageHeight + verticalPadding - MaskPainter.MaskHeight / 2);

                MaskPainter.MaskX = translationX;
                MaskPainter.MaskY = translationY;
                MaskPainter.InvokeInvalidate();
            }
        }

        void PinchGestureRecognizer_PinchUpdated(System.Object sender, Xamarin.Forms.PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Running)
            {
                var w = MaskPainter.MaskWidth * e.Scale;
                var h = MaskPainter.MaskHeight * e.Scale;

                if (w >= 100 && h >= 100)// && IsFitsIn(w, h)
                {
                    var xminoffset = MaskPainter.MaskX - (horizontalPadding + w / 2);
                    var xmaxoffset = (horizontalPadding + imageWidth - w / 2) - MaskPainter.MaskX;
                    var yminoffset = MaskPainter.MaskY - (verticalPadding + h / 2);
                    var ymaxoffset = (verticalPadding + imageHeight - h / 2) - MaskPainter.MaskY;

                    if (xminoffset <= 0 && xmaxoffset <= 0)
                    {
                        return;
                    }
                    if (yminoffset <= 0 && ymaxoffset <= 0)
                    {
                        return;
                    }

                    if (xminoffset < 0)
                    {
                        MaskPainter.MaskX -= xminoffset;
                    }
                    if (xmaxoffset < 0 && xminoffset > 0)
                    {
                        MaskPainter.MaskX += xmaxoffset;
                    }
                    if (yminoffset < 0)
                    {
                        MaskPainter.MaskY -= yminoffset;
                    }
                    if (ymaxoffset < 0)
                    {
                        MaskPainter.MaskY += ymaxoffset;
                    }

                    MaskPainter.MaskWidth = w;
                    MaskPainter.MaskHeight = h;
                    MaskPainter.InvokeInvalidate();
                }
            }
        }

        T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
        {
            if (value.CompareTo(minimum) < 0)
                return minimum;
            if (value.CompareTo(maximum) > 0)
                return maximum;

            return value;
        }

        void CachedImage_Success(System.Object sender, FFImageLoading.Forms.CachedImageEvents.SuccessEventArgs e)
        {
            ImageInformation = e.ImageInformation;
            if (image.Height > 0)
            {
                CalculateScale(ImageInformation);
            }
        }

        private void Image_SizeChanged(object sender, EventArgs e)
        {
            if (ImageInformation != null)
            {
                CalculateScale(ImageInformation);
            }
        }

        void CalculateScale(ImageInformation info)
        {
            var viewRatio = image.Width / image.Height;
            var imageRatio = (double)info.CurrentWidth / info.CurrentHeight;

            if (viewRatio > imageRatio)
            {
                imageScale = image.Height / info.CurrentHeight;
            }
            else
            {
                imageScale = image.Width / info.CurrentWidth;
            }

            imageWidth = info.CurrentWidth * imageScale;
            imageHeight = info.CurrentHeight * imageScale;
            horizontalPadding = (image.Width - imageWidth) / 2;
            verticalPadding = (image.Height - imageHeight) / 2;

            Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(() => {
                MaskPainter.MaskX = image.Width / 2;
                MaskPainter.MaskY = image.Height / 2;
                var min = Math.Min(imageWidth, imageHeight);
                var maskScale = MaskPainter.MaskWidth / MaskPainter.MaskHeight;
                if (min == imageWidth)
                {
                    MaskPainter.MaskWidth = imageWidth;
                    MaskPainter.MaskHeight = MaskPainter.MaskWidth / maskScale;
                }
                else
                {
                    MaskPainter.MaskHeight = imageHeight;
                    MaskPainter.MaskWidth = MaskPainter.MaskHeight * maskScale;
                }

                MaskPainter.InvokeInvalidate();
            });
        }

        /// <summary>
        /// Gets the image as JPEG stream.
        /// </summary>
        /// <returns>The image as JPEG async.</returns>
        /// <param name="quality">Quality.</param>
        /// <param name="maxWidth">Max width.</param>
        /// <param name="maxHeight">Max height.</param>
        /// <param name="framePadding">Frame padding.</param>
        public Task<Stream> GetImageAsJpegAsync(int quality = 90, int maxWidth = 0, int maxHeight = 0)
        {
            TaskParameter task = null;

            task = ImageService.Instance.LoadFile((PhotoSource as FileImageSource).File);

            var transformations = new List<ITransformation>();

            //0.0->1.0
            var xPreOffset = (MaskPainter.MaskX - MaskPainter.MaskWidth / 2 - horizontalPadding) / imageWidth;
            var yPreOffset = (MaskPainter.MaskY - MaskPainter.MaskHeight / 2 - verticalPadding) / imageHeight;

            var cropTransformation = CrossCropTransformation.Current;
            cropTransformation.XOffsetFactor = xPreOffset;
            cropTransformation.YOffsetFactor = yPreOffset;
            cropTransformation.WidthFactor = MaskPainter.MaskWidth / imageWidth;
            cropTransformation.HeightFactor = MaskPainter.MaskHeight / imageHeight;

            transformations.Insert(0, cropTransformation);

            //if (ImageRotation != 0)
            //    transformations.Insert(0, new RotateTransformation(Math.Abs(ImageRotation), ImageRotation < 0) { Resize = true });

            return task
                .WithCache(FFImageLoading.Cache.CacheType.Disk)
                .Transform(transformations)
                .DownSample(maxWidth, maxHeight)
                .AsJPGStreamAsync(quality);
        }
    }
}
