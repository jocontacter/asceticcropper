using System;
using System.Collections.Generic;
using NGraphics;

namespace Ascetic.UI
{
    public class GridMaskPainter : MaskPainter
    {
        public static Xamarin.Forms.BindableProperty CornerLengthProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(CornerLength), typeof(double), typeof(RectangleMaskPainter),
        0.0, Xamarin.Forms.BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the corner length which will have the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The length of the corners.</value>
        public double CornerLength
        {
            get { return (double)GetValue(CornerLengthProperty); }
            set { SetValue(CornerLengthProperty, value); }
        }

        public static Xamarin.Forms.BindableProperty CornerWidthProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(CornerWidth), typeof(double), typeof(RectangleMaskPainter),
        0.0, Xamarin.Forms.BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the corner length which will have the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The length of the corners.</value>
        public double CornerWidth
        {
            get { return (double)GetValue(CornerWidthProperty); }
            set { SetValue(CornerWidthProperty, value); }
        }

        public override void Paint(CropperControl control, ICanvas canvas, Rect rect)
        {
            var maskWidth = FixPlatform(MaskWidth);
            var maskHeight = FixPlatform(MaskHeight);
            var x = FixPlatform(MaskX);
            var y = FixPlatform(MaskY);

            var maxside = Math.Max(rect.Width - maskWidth, rect.Height - maskHeight);
            Size size = new NGraphics.Size(maxside + maskWidth, maxside + maskHeight);

            Pen borderPen = new Pen(control.BackgroundColor.AsNColor(), maxside);
            var bounds = new Rect(new NGraphics.Point(x - size.Width / 2, y - size.Height / 2), size);
            canvas.DrawRectangle(bounds, borderPen);


            borderPen = CreateBorderPen(control);
            var borderBounds = new Rect(x - maskWidth / 2, y - maskHeight / 2, maskWidth, maskHeight);
            canvas.DrawRectangle(borderBounds, borderPen);

            canvas.DrawRectangle(new Rect(
                new Point(borderBounds.X, borderBounds.Y + borderBounds.Height / 3),
                new Size(borderBounds.Width, borderBounds.Height / 3)), borderPen);

            canvas.DrawRectangle(new Rect(
                new Point(borderBounds.X + borderBounds.Width / 3, borderBounds.Y),
                new Size(borderBounds.Width / 3, borderBounds.Height)), borderPen);

            if (CornerLength > 0 && CornerWidth > 0)
            {
                canvas.DrawPath(MakeRecPathArray(borderBounds), new Pen(borderPen.Color, CornerWidth));
            }
        }

        protected virtual IEnumerable<PathOp> MakeRecPathArray(Rect r)
        {
            var length = CornerLength;
            var path = new List<PathOp>{
                new MoveTo(r.Left, r.Top + length),
                new LineTo(r.Left, r.Top),
                new LineTo(r.Left + length, r.Top),

                new MoveTo(r.Right - length, r.Top),
                new LineTo(r.Right, r.Top),
                new LineTo(r.Right, r.Top + length),

                new MoveTo(r.Right, r.Bottom - length),
                new LineTo(r.Right, r.Bottom),
                new LineTo(r.Right - length, r.Bottom),

                new MoveTo(r.Left + length, r.Bottom),
                new LineTo(r.Left, r.Bottom),
                new LineTo(r.Left, r.Bottom - length),
                new MoveTo(r.Left, r.Top + length),
                new ClosePath()
            };
            return path;
        }
    }
}
