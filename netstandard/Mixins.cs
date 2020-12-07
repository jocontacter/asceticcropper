using Xamarin.Forms;

namespace Ascetic.UI
{
    public static class Mixins
    {
        public static NGraphics.Color AsNColor(this Color color)
        {
            return new NGraphics.Color(color.R, color.G, color.B, color.A);
        }
    }
}
