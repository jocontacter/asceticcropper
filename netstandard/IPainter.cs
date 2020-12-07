using NGraphics;

namespace Ascetic.UI
{
    public interface IPainter
    {
        void Paint(CropperControl control, ICanvas canvas, Rect rect);
    }
}
