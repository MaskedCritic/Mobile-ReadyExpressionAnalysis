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
    /// <summary>
    /// Analyzes the emotion shown in image data that contains a human face
    /// </summary>
    public class AnalysisModule
    {
        CascadeClassifier faceFinder;
        FacemarkLBF landmarkFinder;
        private EmotionModel model;
        
        // Threshold value
        readonly double gamma = 100000;

        /// <summary>
        /// Creates a new AnalysisModule object
        /// Requires an EmotionModel.xml file exists in the same directory
        /// </summary>
        /// <param name="face">The Emgu.CV.CascadeClassifier used for Haar Cascades face detection</param>
        /// <param name="landmark">The Emgu.CV.Face.FacemarkLBF object used for locating facial landmarks</param>
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

        /// <summary>
        /// Analyzes the given CanvasBitmap object for emotion
        /// </summary>
        /// <param name="bmp">The CanvasBitmap that contains facial data</param>
        /// <param name="pixelWidth">The width of bmp in pixels</param>
        /// <param name="pixelHeight">The height of bmp in pixels</param>
        /// <param name="pixelStride">The number of bytes per pixel in bmp</param>
        /// <param name="analysisCallback">The UI method to be called for displaying the analyzed emotion results to the user</param>
        public async void Analyze(CanvasBitmap bmp, int pixelWidth, int pixelHeight, int pixelStride, Action<int> analysisCallback)
        {
            // get the bytes from the bitmap and convert them to an Emgu.CV.Mat object
            byte[] pixelBytes = bmp.GetPixelBytes();
            Mat m = new Mat(pixelHeight, pixelWidth, Emgu.CV.CvEnum.DepthType.Cv8U, pixelStride);
            m.SetTo<byte>(pixelBytes);

            // convert the Mat to grayscale
            Mat gray = new Mat();
            CvInvoke.CvtColor(m, gray, Emgu.CV.CvEnum.ColorConversion.Bgra2Gray);
            
            // locate the faces in the image
            Rectangle[] faces = faceFinder.DetectMultiScale(gray);

            // If no faces are found, abort analysis
            if (faces.Length == 0)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    analysisCallback(0);
                });
                return;
            }

            // crop out the first face in the image
            Mat cropped = new Mat(gray, faces[0]);

            // resize the cropped image to 256 x 256 pixels
            Mat resized = new Mat();
            CvInvoke.Resize(cropped, resized, new System.Drawing.Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows));

            // locate the face in the resized image, for use by the FacemarkLBF algorithm
            faces = faceFinder.DetectMultiScale(resized);

            // If something went wrong and a face can no longer be located, abort analysis
            if (faces.Length == 0)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    analysisCallback(0);
                });
                return;
            }

            // Use facemark to locate the facial landmarks
            // model sourced from https://raw.githubusercontent.com/kurnianggoro/GSOC2017/master/data/lbfmodel.yaml

            VectorOfRect regionOfInterest = new VectorOfRect();
            regionOfInterest.Push(faces);

            VectorOfVectorOfPointF facialLandmarks = new VectorOfVectorOfPointF();
            bool landmarkSuccess = landmarkFinder.Fit(resized, regionOfInterest, facialLandmarks);
            
            // if something went wrong and facial landmarks could not be found, abort analysis
            if (!landmarkSuccess)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    analysisCallback(0);
                });
                return;
            }

            // convert the array of points to a 1d vector
            int landmarksCount = 68; // the LBF model used finds exactly 68 landmarks
            double[] landmarksVector = new double[landmarksCount];

            for (int i = 0; i < landmarksCount; i++)
            {
                // treat the landmark coordinates as an address in a 2d array, and convert that to an address for a 1d array
                landmarksVector[i] = facialLandmarks[0][i].X + (256 * facialLandmarks[0][i].Y);
            }

            // compare the distances between the model and the facial landmarks array
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

            // the smallest distance is too large to be considered conclusive, set result to neutral/no result
            if (smallestDistance > gamma)
            {
                analyzedEmotion = 0;
            }

            // perform the callback method on a UI thread
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                analysisCallback(analyzedEmotion);
            });
        }
    }
}
