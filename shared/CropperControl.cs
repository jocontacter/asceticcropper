using NControl.Abstractions;
using NGraphics;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Ascetic.UI
{
    public class CropperControl : NControlView
    {
        /// <summary>
        /// The fill color property.
        /// </summary>
        public static Xamarin.Forms.BindableProperty CustomPainterProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(CustomPainter), typeof(IPainter), typeof(CropperControl), default,
                Xamarin.Forms.BindingMode.OneWay, null, (bindable, oldValue, newValue) =>
                {
                    var control = bindable as CropperControl;

                    if (oldValue is MaskPainter oldPainter)
                    {
                        oldPainter.MustInvalidateEvent -= control.OnMustInvalidate;
                    }

                    if (newValue is MaskPainter painter)
                    {
                        painter.MustInvalidateEvent += control.OnMustInvalidate;
                    }

                    control.Invalidate();
                });

        private void OnMustInvalidate(object sender, EventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Gets or sets the color which will fill the background of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The color of the background.</value>
        public IPainter CustomPainter
        {
            get { return (IPainter)GetValue(CustomPainterProperty); }
            set { SetValue(CustomPainterProperty, value); }
        }

        /// <summary>
        /// The fill color property.
        /// </summary>
        public static new Xamarin.Forms.BindableProperty BackgroundColorProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(BackgroundColor), typeof(Xamarin.Forms.Color), typeof(CropperControl),
                Xamarin.Forms.Color.Transparent, Xamarin.Forms.BindingMode.OneWay, null, (bindable, oldValue, newValue) => (bindable as CropperControl).Invalidate());

        /// <summary>
        /// Gets or sets the color which will fill the background of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The color of the background.</value>
        public new Xamarin.Forms.Color BackgroundColor
        {
            get { return (Xamarin.Forms.Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        /// <summary>
        /// The foreground color property.
        /// </summary>
        public static Xamarin.Forms.BindableProperty ForegroundColorProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(ForegroundColor), typeof(Xamarin.Forms.Color), typeof(CropperControl),
                Xamarin.Forms.Color.Black, Xamarin.Forms.BindingMode.OneWay, null, (bindable, oldValue, newValue) => (bindable as CropperControl).Invalidate());

        /// <summary>
        /// Gets or sets the color which will fill the foreground of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The color of the background.</value>
        public Xamarin.Forms.Color ForegroundColor
        {
            get { return (Xamarin.Forms.Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        /// <summary>
        /// The border color property.
        /// </summary>
        public static Xamarin.Forms.BindableProperty BorderColorProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(BorderColor), typeof(Xamarin.Forms.Color), typeof(CropperControl),
                Xamarin.Forms.Color.Black, Xamarin.Forms.BindingMode.OneWay, null, (bindable, oldValue, newValue) => (bindable as CropperControl).Invalidate());

        /// <summary>
        /// Gets or sets the color which will fill the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The color of the border.</value>
        public Xamarin.Forms.Color BorderColor
        {
            get { return (Xamarin.Forms.Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        /// <summary>
        /// The border width property.
        /// </summary>
        public static Xamarin.Forms.BindableProperty BorderWidthProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(CropperControl),
                0.0, Xamarin.Forms.BindingMode.OneWay, null, (bindable, oldValue, newValue) => (bindable as CropperControl).Invalidate(),
                coerceValue: (bindable, value) => {
                    if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                    {
                        //android bug, receives values in pixels
                        return (bindable as CropperControl).getInPixel((double)value);
                    }

                    return value;
                });

        /// <summary>
        /// Gets or sets the width which will have the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The width of the border.</value>
        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        //public double BorderWidthInPx { get; private set; } = 0;
        protected readonly double density;

        public double getInPixel(double dpSize)
        {
            return Math.Round(dpSize * density);
        }

        public float getInPixel(float dpSize)
        {
            return (float)Math.Round(dpSize * density);
        }

        public double getInDP(double pixelSize)
        {
            return Math.Round(pixelSize / density);
        }

        public NGraphics.Point getInDP(NGraphics.Point point)
        {
            return point / density;
        }

        public NGraphics.Rect getInDP(NGraphics.Rect rect)
        {
            return new NGraphics.Rect(rect.Position / density, rect.Size / density);
        }

        protected List<T> GetAllChildViews<T>(Layout<View> group) where T : View
        {
            var result = new List<T>();

            foreach (var child in group.Children)
            {
                if (child is T item)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public virtual CropperControl GetChildAtPoint(double x, double y)
        {
            if (Bounds.Contains(x, y))
            {
                var correctedX = x - Bounds.X;
                var correctedY = y - Bounds.Y;

                if (this.Content is CropperControl innerControl)
                {
                    return innerControl.GetChildAtPoint(correctedX, correctedY);
                }
                else if (this.Content is Layout<View> innerContainer)
                {
                    var innerControls = GetAllChildViews<CropperControl>(innerContainer);
                    //сначала верхние по Z-индексу!
                    innerControls.Reverse();

                    foreach (var c in innerControls)
                    {
                        if (correctedX >= 0 && correctedY > 0)
                        {
                            var res = c.GetChildAtPoint(correctedX, correctedY);

                            if (res != null)
                                return res;
                        }
                    }
                }

                return this;
            }

            return null;
        }

        public CropperControl()
        {
            density = DeviceDisplay.MainDisplayInfo.Density;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            Invalidate();
        }

        public override void Draw(NGraphics.ICanvas canvas, NGraphics.Rect rect)
        {
            if (BackgroundColor == Xamarin.Forms.Color.Transparent && BorderColor == Xamarin.Forms.Color.Transparent)
            {
                return;
            }

            CustomPainter.Paint(this, canvas, rect);
        }
    }
}
