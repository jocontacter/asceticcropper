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
        /// The painter property.
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
        /// Gets or sets the mask painter
        /// </summary>
        /// <value>mask painter.</value>
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
                0.0, Xamarin.Forms.BindingMode.OneWay, null, (bindable, oldValue, newValue) => (bindable as CropperControl).Invalidate());

        /// <summary>
        /// Gets or sets the width which will have the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The width of the border.</value>
        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
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
