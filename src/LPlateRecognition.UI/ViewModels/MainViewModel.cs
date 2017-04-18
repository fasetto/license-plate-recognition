using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using LPlateRecognition.UI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LPlateRecognition.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LicensePlateDetector _licensePlateDetector;
        private string _licensePlate;
        private ImageSource _image;
        private ImageSource _imageOrigin;

        public MainViewModel()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _licensePlateDetector = new LicensePlateDetector(path + "\\..\\..\\..\\..\\tessdata");
        }

        public string LicensePlate
        {
            get => _licensePlate;
            set
            {
                _licensePlate = value;
                OnPropertyChanged();
            }
        }
        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }
        public ImageSource ImageOrigin
        {
            get => _imageOrigin;
            set
            {
                _imageOrigin = value;
                OnPropertyChanged();
            }
        }

        public void Init(string imageLocation)
        {
            Mat m = new Mat(imageLocation, LoadImageType.Unchanged);
            UMat um = m.ToUMat(AccessType.ReadWrite);

            ImageOrigin = BitmapToImageSource(m.Bitmap);

            ProcessImage(m);
        }

        private void ProcessImage(IInputOutputArray image)
        {
            List<IInputOutputArray> licensePlateImagesList         = new List<IInputOutputArray>();
            List<IInputOutputArray> filteredLicensePlateImagesList = new List<IInputOutputArray>();
            List<RotatedRect> licenseBoxList                       = new List<RotatedRect>();

            List<string> words = _licensePlateDetector.DetectLicensePlate(
            image,
            licensePlateImagesList,
            filteredLicensePlateImagesList,
            licenseBoxList
            );

            string plates = string.Empty;

            for (int i = 0; i < words.Count; i++)
            {
                Mat dest = new Mat();
                CvInvoke.VConcat(licensePlateImagesList[i], filteredLicensePlateImagesList[i], dest);

                Image = BitmapToImageSource(dest.Bitmap);
                plates += words[i] + "\n";

                PointF[] verticesF = licenseBoxList[i].GetVertices();
                System.Drawing.Point[] vertices = Array.ConvertAll(verticesF, System.Drawing.Point.Round);

                using (VectorOfPoint pts = new VectorOfPoint(vertices))
                    CvInvoke.Polylines(image, pts, true, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
            }

            LicensePlate = plates;

        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
