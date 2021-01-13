using System;
using System.Collections.Generic;
using NGraphics;

namespace Ascetic.UI
{
    public class GridMaskPainter : MaskPainter
    {
        public static Xamarin.Forms.BindableProperty CornerLengthProperty =
            Xamarin.Forms.BindableProperty.Create(nameof(CornerLength), typeof(double), typeof(GridMaskPainter),
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
            Xamarin.Forms.BindableProperty.Create(nameof(CornerWidth), typeof(double), typeof(GridMaskPainter),
        0.0, Xamarin.Forms.BindingMode.OneWay);

        /// <summary>
        /// Gets or sets the corner weight which will have the border of a VisualElement. This is a bindable property.
        /// </summary>
        /// <value>The weight of the corners.</value>
        public double CornerWidth
        {
            get { return (double)GetValue(CornerWidthProperty); }
            set { SetValue(CornerWidthProperty, value); }
        }

        public override void Paint(CropperControl control, ICanvas canvas, Rect controlBounds)
        {
            var maskWidth = FixPlatform(MaskWidth);
            var maskHeight = FixPlatform(MaskHeight);
            var x = FixPlatform(MaskX);
            var y = FixPlatform(MaskY);

            var maskBounds = new Rect(x - maskWidth / 2, y - maskHeight / 2, maskWidth, maskHeight);
            var backgroundPath = MakeBackgroundPathArray(controlBounds, maskBounds);
            canvas.FillPath(backgroundPath, new SolidBrush(control.BackgroundColor.AsNColor()));

            var borderPen = CreateBorderPen(control);
            canvas.DrawRectangle(maskBounds, borderPen);

            canvas.DrawRectangle(new Rect(
                new Point(maskBounds.X, maskBounds.Y + maskBounds.Height / 3),
                new Size(maskBounds.Width, maskBounds.Height / 3)), borderPen);

            canvas.DrawRectangle(new Rect(
                new Point(maskBounds.X + maskBounds.Width / 3, maskBounds.Y),
                new Size(maskBounds.Width / 3, maskBounds.Height)), borderPen);

            if (CornerLength > 0 && CornerWidth > 0)
            {
                canvas.DrawPath(MakeGridPathArray(maskBounds), new Pen(borderPen.Color, FixPlatform(CornerWidth)));
            }
        }

        private IEnumerable<PathOp> MakeBackgroundPathArray(Rect fullBounds, Rect maskBounds)
        {
            return new List<PathOp>
            {
                new LineTo(maskBounds.TopLeft),
                new LineTo(maskBounds.TopRight),
                new LineTo(maskBounds.BottomRight),
                new LineTo(maskBounds.BottomLeft),
                new LineTo(maskBounds.TopLeft),
                new LineTo(fullBounds.TopLeft),
                new LineTo(fullBounds.BottomLeft),
                new LineTo(fullBounds.BottomRight),
                new LineTo(fullBounds.TopRight),
                new LineTo(fullBounds.TopLeft)
            };
        }

        protected virtual IEnumerable<PathOp> MakeGridPathArray(Rect maskBounds)
        {
            var length = FixPlatform(CornerLength);
            var path = new List<PathOp>{
                new MoveTo(maskBounds.Left, maskBounds.Top + length),
                new LineTo(maskBounds.Left, maskBounds.Top),
                new LineTo(maskBounds.Left + length, maskBounds.Top),

                new MoveTo(maskBounds.Right - length, maskBounds.Top),
                new LineTo(maskBounds.Right, maskBounds.Top),
                new LineTo(maskBounds.Right, maskBounds.Top + length),

                new MoveTo(maskBounds.Right, maskBounds.Bottom - length),
                new LineTo(maskBounds.Right, maskBounds.Bottom),
                new LineTo(maskBounds.Right - length, maskBounds.Bottom),

                new MoveTo(maskBounds.Left + length, maskBounds.Bottom),
                new LineTo(maskBounds.Left, maskBounds.Bottom),
                new LineTo(maskBounds.Left, maskBounds.Bottom - length),
                new MoveTo(maskBounds.Left, maskBounds.Top + length),
                new ClosePath()
            };

            return path;
        }
    }
}
