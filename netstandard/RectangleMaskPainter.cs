using System;
using Ascetic.UI;
using NGraphics;
using Xamarin.Essentials;

namespace Ascetic.UI
{
    public class RectangleMaskPainter : MaskPainter
    {
        /// <summary>
        /// The corners radius property.
        /// </summary>
        public static Xamarin.Forms.BindableProperty CornerRadiusProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(RectangleMaskPainter),
                0.0, Xamarin.Forms.BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the corner radius which will have the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The radius of the corners.</value>
        public double CornerRadius
        {
            get { return (double)FixPlatform((double)GetValue(CornerRadiusProperty)); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public override void Paint(CropperControl control, ICanvas canvas, Rect rect)
        {
            var maxside = Math.Max(rect.Width - MaskWidth, rect.Height - MaskHeight);
            Size size = new NGraphics.Size(maxside + MaskWidth, maxside + MaskHeight);

            var position = new NGraphics.Point(MaskX, MaskY);
            var bounds = new Rect(new NGraphics.Point(position.X - size.Width / 2, position.Y - size.Height / 2), size);
            Pen borderPen = new Pen(control.BackgroundColor.AsNColor(), maxside);

            if (CornerRadius > 0)
            {
                var pathArray = MakePathArray(bounds, FixPlatform(CornerRadius));
                canvas.DrawPath(pathArray, borderPen);
            }
            else
            {
                canvas.DrawRectangle(bounds, borderPen);
            }

            borderPen = CreateBorderPen(control);
            var square = new NGraphics.Rect(new NGraphics.Point(position.X - MaskWidth / 2, position.Y - MaskHeight / 2),
                new NGraphics.Size(MaskWidth, MaskHeight));

            if (CornerRadius > 0)
            {
                var pathArray = MakePathArray(square, FixPlatform(CornerRadius));
                canvas.DrawPath(pathArray, borderPen);
            }
            else
            {
                canvas.DrawRectangle(square, borderPen);
            }
        }
    }
}
