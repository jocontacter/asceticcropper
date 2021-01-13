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
            Xamarin.Forms.BindableProperty.Create(nameof(Corners), typeof(CornersEnum), typeof(RectangleMaskPainter), CornersEnum.None, Xamarin.Forms.BindingMode.OneWay);

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
            get { return (double)GetValue(CornerRadiusProperty); }
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

        public override void Paint(CropperControl control, ICanvas canvas, Rect controlBounds)
        {
            var maskWidth = FixPlatform(MaskWidth);
            var maskHeight = FixPlatform(MaskHeight);
            var x = FixPlatform(MaskX);
            var y = FixPlatform(MaskY);
            var cornerRadius = FixPlatform(CornerRadius);

            var maskBounds = new Rect(x - maskWidth / 2, y - maskHeight / 2, maskWidth, maskHeight);
            var backgroundPath = MakePathArray(controlBounds, maskBounds, cornerRadius, invert: true);

            canvas.FillPath(backgroundPath, new SolidBrush(control.BackgroundColor.AsNColor()));

            var borderPen = CreateBorderPen(control);
            
            if (CornerRadius > 0)
            {
                var maskPath = MakePathArray(controlBounds, maskBounds, cornerRadius);
                canvas.DrawPath(maskPath, borderPen);
            }
            else
            {
                canvas.DrawRectangle(maskBounds, borderPen);
            }
        }

        protected virtual IEnumerable<PathOp> MakePathArray(Rect fullBounds, Rect maskBounds, double radius, bool invert = false)
        {
            var tlr = (Corners & CornersEnum.TopLeft) == CornersEnum.TopLeft ? radius : 0;
            var trr = (Corners & CornersEnum.TopRight) == CornersEnum.TopRight ? radius : 0;
            var brr = (Corners & CornersEnum.BottomRight) == CornersEnum.BottomRight ? radius : 0;
            var blr = (Corners & CornersEnum.BottomLeft) == CornersEnum.BottomLeft ? radius : 0;

            var path = new List<PathOp>{
                    new MoveTo(maskBounds.Left + tlr, maskBounds.Top),
                    new LineTo(maskBounds.Right - trr, maskBounds.Top),
            };

            if (trr > 0)
            {
                //    path.Add(new ArcTo(new Size(trr), false, true, new Point(r.Right, r.Top + trr)));
                path.Add(new CurveTo(new Point(maskBounds.Right - radius, maskBounds.Top), maskBounds.TopRight, new Point(maskBounds.Right, maskBounds.Top + radius)));
            }

            path.Add(new LineTo(maskBounds.Right, maskBounds.Bottom - brr));

            if (brr > 0)
            {
                //    path.Add(new ArcTo(new Size(brr), false, true, new Point(r.Right - brr, r.Bottom)));
                path.Add(new CurveTo(new Point(maskBounds.Right, maskBounds.Bottom - radius), maskBounds.BottomRight, new Point(maskBounds.Right - radius, maskBounds.Bottom)));
            }

            path.Add(new LineTo(maskBounds.Left + blr, maskBounds.Bottom));

            if (blr > 0)
            {
                //    path.Add(new ArcTo(new Size(blr), false, true, new Point(r.Left, r.Bottom - blr)));
                path.Add(new CurveTo(new Point(maskBounds.Left + radius, maskBounds.Bottom), maskBounds.BottomLeft, new Point(maskBounds.Left, maskBounds.Bottom - radius)));
            }

            path.Add(new LineTo(maskBounds.Left, maskBounds.Top + tlr));

            if (tlr > 0)
            {
                //    path.Add(new ArcTo(new Size(tlr), false, true, new Point(r.Left + tlr, r.Top)));
                path.Add(new CurveTo(new Point(maskBounds.Left, maskBounds.Top + radius), maskBounds.TopLeft, new Point(maskBounds.Left + radius, maskBounds.Top)));
            }

            if(invert)
            {
                path.Add(new LineTo(fullBounds.TopLeft));
                path.Add(new LineTo(fullBounds.BottomLeft));
                path.Add(new LineTo(fullBounds.BottomRight));
                path.Add(new LineTo(fullBounds.TopRight));
                path.Add(new LineTo(fullBounds.TopLeft));
            }
            else
            {
                path.Add(new ClosePath());
            }
            
            return path;
        }
    }
}
