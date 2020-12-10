using System;
namespace Ascetic.UI
{
    [Flags]
    public enum CornersEnum
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomRight = 4,
        BottomLeft = 8,
        All = 15
    }
}
