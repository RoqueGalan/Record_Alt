using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace RecordFCS_Alt.Helpers
{
    public class Thumbnail
    {
        public string OrigenSrc { get; set; }
        public string DestinoSrc { get; set; }
        public double LimiteAnchoAlto { get; set; }

        private Image Imagen { get; set; }
        private int AnchoThumb { get; set; }
        private int AltoThumb { get; set; }


        public void GuardarThumbnail()
        {
            Imagen = Image.FromFile(OrigenSrc);

            getDimensionSinDistorsionar();

            generateThumnail();
        }

        private void getDimensionSinDistorsionar()
        {
            //Identificar Valor Maximo
            double maximoValor;
            if (Imagen.Width > Imagen.Height)
                maximoValor = Convert.ToDouble(Imagen.Width);
            else
                maximoValor = Convert.ToDouble(Imagen.Height);

            //regla de tres con la dimension del thumbnail y el maximo valor
            double porcentajeImagenReduccion = LimiteAnchoAlto / maximoValor;

            //asignar valores para thumbnail
            AnchoThumb = Convert.ToInt32(Convert.ToDouble(Imagen.Width) * porcentajeImagenReduccion);
            AltoThumb = Convert.ToInt32(Convert.ToDouble(Imagen.Height) * porcentajeImagenReduccion);


        }

        private void generateThumnail()
        {
            Image thumbnail = new Bitmap(AnchoThumb, AltoThumb);
            Graphics graphic = Graphics.FromImage(thumbnail);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.DrawImage(Imagen, 0, 0, AnchoThumb, AltoThumb);
            ImageCodecInfo[] info;
            info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 50L);
            thumbnail.Save(DestinoSrc, info[1], encoderParameters);
        }

    }
}