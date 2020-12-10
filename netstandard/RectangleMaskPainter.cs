using System;
using System.Collections.Generic;
using Ascetic.UI;
using NGraphics;
using Xamarin.Essentials;

namespace Ascetic.UI
{
    public class RectangleMaskPainter : MaskPainter
    {
        /// <summary>
        /// The border corners property.
        /// </summary>
        public static Xamarin.Forms.BindableProperty CornersProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(Corners), typeof(CornersEnum), typeof(RectangleMaskPainter), CornersEnum.All, Xamarin.Forms.BindingMode.OneWay);

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

        /// <summary>
        /// Gets or sets the rounded corners which will have the radius. This is a bindable property.
        /// </summary>
        /// <value>The width of the border.</value>
        public CornersEnum Corners
        {
            get { return (CornersEnum)GetValue(CornersProperty); }
            set { SetValue(CornersProperty, value); }
        }

        public override void Paint(CropperControl control, ICanvas canvas, Rect rect)
        {
            var maskHeight = FixPlatform(MaskHeight);
            var maskWidth = FixPlatform(MaskWidth);
            var x = FixPlatform(MaskX);
            var y = FixPlatform(MaskY);

            var maxside = Math.Max(rect.Width - maskWidth, rect.Height - maskHeight);
            maxside += FixPlatform(CornerRadius);
            Size size = new NGraphics.Size(maxside + maskWidth, maxside + maskHeight);

            var position = new NGraphics.Point(x, y);
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
            var square = new NGraphics.Rect(new NGraphics.Point(position.X - maskWidth / 2, position.Y - maskHeight / 2),
                new NGraphics.Size(maskWidth, maskHeight));

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

        protected virtual IEnumerable<PathOp> MakePathArray(Rect r, double radius)
        {
            var tlr = (Corners & CornersEnum.TopLeft) == CornersEnum.TopLeft ? radius : 0;
            var trr = (Corners & CornersEnum.TopRight) == CornersEnum.TopRight ? radius : 0;
            var brr = (Corners & CornersEnum.BottomRight) == CornersEnum.BottomRight ? radius : 0;
            var blr = (Corners & CornersEnum.BottomLeft) == CornersEnum.BottomLeft ? radius : 0;

            var path = new List<PathOp>{
                    new MoveTo(r.Left + tlr, r.Top),
                    new LineTo(r.Right - trr, r.Top),
            };

            if (trr > 0)
            {
                path.Add(new CurveTo(new Point(r.Right - radius, r.Top), r.TopRight, new Point(r.Right, r.Top + radius)));
            }

            path.Add(new LineTo(r.Right, r.Bottom - brr));

            if (brr > 0)
            {
                path.Add(new CurveTo(new Point(r.Right, r.Bottom - radius), r.BottomRight, new Point(r.Right - radius, r.Bottom)));
            }
            path.Add(new LineTo(r.Left + blr, r.Bottom));
            if (blr > 0)
            {
                path.Add(new CurveTo(new Point(r.Left + radius, r.Bottom), r.BottomLeft, new Point(r.Left, r.Bottom - radius)));
            }

            if (tlr > 0)
            {
                path.Add(new LineTo(r.Left, r.Top + tlr));

                path.Add(new CurveTo(new Point(r.Left, r.Top + radius), r.TopLeft, new Point(r.Left + radius, r.Top)));
                path.Add(new ClosePath());
            }
            else
            {
                path.Add(new ClosePath());
            }

            return path;
        }
    }
}
