using Android.Graphics;
using FFImageLoading.Transformations;

namespace Ascetic.UI
{
    public class CropTransformation : TransformationBase, ICropTransformation
    {
        public double XOffsetFactor { get; set; }
        public double YOffsetFactor { get; set; }
        public double WidthFactor { get; set; }
        public double HeightFactor { get; set; }

        public override string Key
        {
            get
            {
                return string.Format("CropTransformation,xOffset={0},yOffset={1},width={2},height={3}",
                XOffsetFactor, YOffsetFactor, WidthFactor, HeightFactor);
            }
        }

        public CropTransformation()
        { }

        protected override Bitmap Transform(Bitmap sourceBitmap, string path, FFImageLoading.Work.ImageSource source, bool isPlaceholder, string key)
        {
            return ToCropped(sourceBitmap, XOffsetFactor, YOffsetFactor, WidthFactor, HeightFactor);
        }

        public static Bitmap ToCropped(Bitmap source, double xOffset, double yOffset, double Width, double Height)
        {
            var config = source.GetConfig();
            if (config == null)
                config = Bitmap.Config.Argb8888;    // This will support transparency

            Bitmap bitmap = Bitmap.CreateBitmap((int)(source.Width * Width), (int)(source.Height * Height), config);

            using (Canvas canvas = new Canvas(bitmap))
            using (Paint paint = new Paint())
            using (BitmapShader shader = new BitmapShader(source, Shader.TileMode.Clamp, Shader.TileMode.Clamp))
            using (Matrix matrix = new Matrix())
            {
                if (xOffset != 0 || yOffset != 0)
                {
                    matrix.SetTranslate((float)(-xOffset * source.Width), (float)(-yOffset * source.Height));
                    shader.SetLocalMatrix(matrix);
                }

                paint.SetShader(shader);
                paint.AntiAlias = false;

                RectF rectF = new RectF(0, 0, (float)(source.Width * Width), (float)(source.Height * Height));
                canvas.DrawRect(rectF, paint);

                return bitmap;
            }
        }
    }
}
