using Xamarin.Forms;

namespace Ascetic.UI
{
    public static class Mixins
    {
        public static NGraphics.Color ToNColor(this Color color)
        {
            return new NGraphics.Color(color.R, color.G, color.B, color.A);
        }
    }
}
