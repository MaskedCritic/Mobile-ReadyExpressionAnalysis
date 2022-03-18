using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Util;
using MathNet.Numerics;

namespace Mobile_ReadyExpressionAnalysisBusiness
{
    public class AnalysisModule
    {
        CascadeClassifier faceFinder;
        private EmotionModel model;
        readonly double gamma = 27;

        public AnalysisModule()
        {
            faceFinder = new CascadeClassifier("haarcascade_frontalface_default.xml");
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
        }

        public void Analyze(byte[] pixelBytes, int pixelWidth, int pixelHeight, int pixelStride, Action<int> analysisCallback)
        {
            // convert unmanaged data to an OpenCV Image
            
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(pixelBytes.Length);
            Marshal.Copy(pixelBytes, 0, unmanagedPointer, pixelBytes.Length);
            // Call unmanaged code
            
            Image<Emgu.CV.Structure.Bgra, Byte> toProcess = new Image<Emgu.CV.Structure.Bgra, byte>(pixelWidth, pixelHeight, pixelStride, unmanagedPointer);

            Mat m = toProcess.Mat;
            Mat gray = new Mat();
            CvInvoke.CvtColor(m, gray, Emgu.CV.CvEnum.ColorConversion.Bgra2Gray);

            Rectangle[] faces = faceFinder.DetectMultiScale(gray);
            
            if (faces.Length == 0)
            {
                StateModel.State = 0; // set emotion state to default
                Marshal.FreeHGlobal(unmanagedPointer);
                return; // no faces, abort analysis
            }

            // crop out the first face
            Mat cropped = new Mat(gray, faces[0]);
            Mat resized = new Mat();

            CvInvoke.Resize(cropped, resized, new System.Drawing.Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows));

            faces = faceFinder.DetectMultiScale(resized);

            if (faces.Length == 0)
            {
                StateModel.State = 0; // set emotion state to default
                Marshal.FreeHGlobal(unmanagedPointer);
                return; // no faces, abort analysis
            }

            // Use facemark to locate the facial landmarks
            FacemarkLBF finder = new FacemarkLBF(new FacemarkLBFParams());
            finder.LoadModel("lbfmodel.yaml"); // model sourced from https://raw.githubusercontent.com/kurnianggoro/GSOC2017/master/data/lbfmodel.yaml

            VectorOfRect regionOfInterest = new VectorOfRect();
            regionOfInterest.Push(faces);

            VectorOfVectorOfPointF facialLandmarks = new VectorOfVectorOfPointF();
            bool landmarkSuccess = finder.Fit(resized, regionOfInterest, facialLandmarks);
            if (!landmarkSuccess)
            {
                StateModel.State = 0; // set emotion state to default
                Marshal.FreeHGlobal(unmanagedPointer);
                return; // algorithm failed, abort analysis
            }

            Marshal.FreeHGlobal(unmanagedPointer);

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

            analysisCallback(analyzedEmotion);
        }
    }
}
