using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Util;
using MathNet.Numerics;
using Microsoft.Graphics.Canvas;
using Windows.Storage;
using Windows.UI.Core;

namespace Mobile_ReadyExpressionAnalysisBusiness
{
    public class AnalysisModule
    {
        CascadeClassifier faceFinder;
        FacemarkLBF landmarkFinder;
        private EmotionModel model;
        readonly double gamma = 40000;

        public AnalysisModule(CascadeClassifier face, FacemarkLBF landmark)
        {
            
            TextReader reader = null;
            model = new EmotionModel();
            string filepath = "EmotionModel.xml";
            try
            {
                var serializer = new XmlSerializer(model.GetType());
                reader = new StreamReader(filepath);
                model = (EmotionModel)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            faceFinder = face;
            landmarkFinder = landmark;
        }

        public async void Analyze(CanvasBitmap bmp, int pixelWidth, int pixelHeight, int pixelStride, Action<int> analysisCallback)
        {
            byte[] pixelBytes = bmp.GetPixelBytes();
            //Windows.Storage.StorageFolder storageFolder = KnownFolders.SavedPictures;
            //var file = await storageFolder.CreateFileAsync("sampleImage.bmp", CreationCollisionOption.ReplaceExisting);
            //using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
            //{
            //    await bmp.SaveAsync(fs, CanvasBitmapFileFormat.Bmp);
            //}
            //Thread.Sleep(10000);
            // convert unmanaged data to an OpenCV Image
            //Mat test = CvInvoke.Imread(@"C:\Users\MrWai\Pictures\Saved Pictures\sampleImage.bmp");
            
            // Call unmanaged code
            Mat m = new Mat(pixelHeight, pixelWidth, Emgu.CV.CvEnum.DepthType.Cv8U, pixelStride);
            m.SetTo<byte>(pixelBytes);
            Mat gray = new Mat();
            CvInvoke.CvtColor(m, gray, Emgu.CV.CvEnum.ColorConversion.Bgra2Gray);
            

            //faceFinder = new CascadeClassifier("haarcascade_frontalface_alt.xml");
            Rectangle[] faces = faceFinder.DetectMultiScale(gray);

            if (faces.Length == 0)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    analysisCallback(0);
                });
                return; // no faces, abort analysis
            }

            // crop out the first face
            Mat cropped = new Mat(gray, faces[0]);
            Mat resized = new Mat();

            CvInvoke.Resize(cropped, resized, new System.Drawing.Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows));

            faces = faceFinder.DetectMultiScale(resized);

            if (faces.Length == 0)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    analysisCallback(0);
                });
                return; // no faces, abort analysis
            }

            // Use facemark to locate the facial landmarks
            //FacemarkLBF finder = new FacemarkLBF(new FacemarkLBFParams());
            //finder.LoadModel("lbfmodel.yaml"); // model sourced from https://raw.githubusercontent.com/kurnianggoro/GSOC2017/master/data/lbfmodel.yaml

            VectorOfRect regionOfInterest = new VectorOfRect();
            regionOfInterest.Push(faces);

            VectorOfVectorOfPointF facialLandmarks = new VectorOfVectorOfPointF();
            bool landmarkSuccess = landmarkFinder.Fit(resized, regionOfInterest, facialLandmarks);
            if (!landmarkSuccess)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    analysisCallback(0);
                });
                return; // algorithm failed, abort analysis
            }


            // convert the array of points to a 1d vector
            int landmarksCount = 68; // the LBF model used finds exactly 68 landmarks
            double[] landmarksVector = new double[landmarksCount];

            for (int i = 0; i < landmarksCount; i++)
            {
                landmarksVector[i] = facialLandmarks[0][i].X + (256 * facialLandmarks[0][i].Y);
            }

            double smallestDistance;
            double currentDistance;
            int analyzedEmotion = 0;
            smallestDistance = Math.Abs(Distance.Manhattan(landmarksVector, model.Happy));
            analyzedEmotion = 1;

            currentDistance = Math.Abs(Distance.Manhattan(landmarksVector, model.Sad));

            if (currentDistance < smallestDistance)
            {
                smallestDistance = currentDistance;
                analyzedEmotion = 2;
            }

            currentDistance = Math.Abs(Distance.Manhattan(landmarksVector, model.Anger));

            if (currentDistance < smallestDistance)
            {
                smallestDistance = currentDistance;
                analyzedEmotion = 3;
            }

            currentDistance = Math.Abs(Distance.Manhattan(landmarksVector, model.Surprise));

            if (currentDistance < smallestDistance)
            {
                smallestDistance = currentDistance;
                analyzedEmotion = 4;
            }

            currentDistance = Math.Abs(Distance.Manhattan(landmarksVector, model.Fear));

            if (currentDistance < smallestDistance)
            {
                smallestDistance = currentDistance;
                analyzedEmotion = 5;
            }

            currentDistance = Math.Abs(Distance.Manhattan(landmarksVector, model.Disgust));

            if (currentDistance < smallestDistance)
            {
                smallestDistance = currentDistance;
                analyzedEmotion = 6;
            }

            if (smallestDistance > gamma)
            {
                analyzedEmotion = 0;
            }

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                analysisCallback(analyzedEmotion);
            });

        }
    }
}
