using System;
using Ascetic.UI;
using NGraphics;

namespace Ascetic.UI
{
    public class CircleMaskPainter : MaskPainter
    {
        public override void Paint(CropperControl control, ICanvas canvas, Rect rect)
        {
            var maskHeight = FixPlatform(MaskHeight);
            var maskWidth = FixPlatform(MaskWidth);
            var x = FixPlatform(MaskX);
            var y = FixPlatform(MaskY);

            var maxside = Math.Max(rect.Width - maskWidth, rect.Height - maskHeight);
            maxside *= 2.5;
            var size = new NGraphics.Size(maxside + maskWidth, maxside + maskHeight);
            var position = new NGraphics.Point(x, y);

            canvas.DrawEllipse(
                new Rect(new NGraphics.Point(position.X - size.Width / 2, position.Y - size.Height / 2), size),
                color: control.BackgroundColor.AsNColor(),
                width: maxside);

            Pen borderPen = CreateBorderPen(control);
            var square = new NGraphics.Rect(position - maskHeight / 2, new NGraphics.Size(maskHeight));
            canvas.DrawEllipse(square, borderPen);
        }
    }
}