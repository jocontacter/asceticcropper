using System;
using System.Collections.Generic;
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

            var maskBounds = new Rect(x - maskWidth / 2, y - maskHeight / 2, maskWidth, maskHeight);
            var backgroundPathArray = MakePathArray(rect, maskBounds, invert: true);
            var maskPathArray = MakePathArray(rect, maskBounds);
            Pen borderPen = CreateBorderPen(control);

            canvas.FillPath(backgroundPathArray, new SolidBrush(control.BackgroundColor.AsNColor()));
            canvas.DrawPath(maskPathArray, borderPen);
            //canvas.DrawEllipse(maskBounds, borderPen);//figure not the same as path
        }

        protected virtual IEnumerable<PathOp> MakePathArray(Rect fullBounds, Rect maskBounds, bool invert = false)
        {
            var path = new List<PathOp>{
                    new MoveTo(maskBounds.Center.X, maskBounds.Top),
                    new CurveTo(new Point(maskBounds.Center.X, maskBounds.Top), maskBounds.TopRight, new Point(maskBounds.Right, maskBounds.Center.Y)),
                    new CurveTo(new Point(maskBounds.Right, maskBounds.Center.Y), maskBounds.BottomRight, new Point(maskBounds.Center.X, maskBounds.Bottom)),
                    new CurveTo(new Point(maskBounds.Center.X, maskBounds.Bottom), maskBounds.BottomLeft, new Point(maskBounds.Left, maskBounds.Center.Y)),
                    new CurveTo(new Point(maskBounds.Left, maskBounds.Center.Y), maskBounds.TopLeft, new Point(maskBounds.Center.X, maskBounds.Top))
            };

            if (invert)
            {
                path.Add(new LineTo(fullBounds.Center.X, fullBounds.Top));
                path.Add(new LineTo(fullBounds.TopLeft));
                path.Add(new LineTo(fullBounds.BottomLeft));
                path.Add(new LineTo(fullBounds.BottomRight));
                path.Add(new LineTo(fullBounds.TopRight));
                path.Add(new LineTo(fullBounds.Center.X, fullBounds.Top));
            }
            else
            {
                path.Add(new ClosePath());
            }

            return path;
        }
    }
}