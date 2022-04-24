using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Util;
using MathNet.Numerics;

namespace Mobile_ReadyExpressionAnalysisTraining
{
    class Program
    {
        static void Main(string[] args)
        {
            //Train();
            Test();
        }

        static void Train()
        {
            string pathPrefix = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\Cohn-Kanade Database\\Cohn-Kanade Database\\CK+\\Labeled\\";
            string pathExtension = ".png";
            
            // Read in images
            VectorOfMat happy = new VectorOfMat();
            VectorOfMat sad = new VectorOfMat();
            VectorOfMat anger = new VectorOfMat();
            VectorOfMat surprise = new VectorOfMat();
            VectorOfMat fear = new VectorOfMat();
            VectorOfMat disgust = new VectorOfMat();

            for (int i = 1; i < 69; i++)
            {
                happy.Push(CvInvoke.Imread(pathPrefix + "happy" + i + pathExtension));
            }

            for (int i = 1; i < 24; i++)
            {
                sad.Push(CvInvoke.Imread(pathPrefix + "sad" + i + pathExtension));
            }

            for (int i = 1; i < 39; i++)
            {
                anger.Push(CvInvoke.Imread(pathPrefix + "anger" + i + pathExtension));
            }

            for (int i = 1; i < 83; i++)
            {
                surprise.Push(CvInvoke.Imread(pathPrefix + "surprise" + i + pathExtension));
            }

            for (int i = 1; i < 21; i++)
            {
                fear.Push(CvInvoke.Imread(pathPrefix + "fear" + i + pathExtension));
            }

            for (int i = 1; i < 59; i++)
            {
                disgust.Push(CvInvoke.Imread(pathPrefix + "disgust" + i + pathExtension));
            }

            // For each image:
            // Use haar cascades to find the faces
            CascadeClassifier faceClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");

            VectorOfRect happyROI = new VectorOfRect();
            for (int i = 0; i < happy.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(happy[i]); // Find the face
                if (faces.Length > 0)
                {
                    happyROI.Push(faces);
                }
            }

            VectorOfRect sadROI = new VectorOfRect();
            for (int i = 0; i < sad.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(sad[i]); // Find the face
                if (faces.Length > 0)
                {
                    sadROI.Push(faces);
                }
            }

            VectorOfRect angerROI = new VectorOfRect();
            for (int i = 0; i < anger.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(anger[i]); // Find the face
                if (faces.Length > 0)
                {
                    angerROI.Push(faces);
                }
            }

            VectorOfRect surpriseROI = new VectorOfRect();
            for (int i = 0; i < surprise.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(surprise[i]); // Find the face
                if (faces.Length > 0)
                {
                    surpriseROI.Push(faces);
                }
            }

            VectorOfRect fearROI = new VectorOfRect();
            for (int i = 0; i < fear.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(fear[i]); // Find the face
                if (faces.Length > 0)
                {
                    fearROI.Push(faces);
                }
            }

            VectorOfRect disgustROI = new VectorOfRect();
            for (int i = 0; i < disgust.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(disgust[i]); // Find the face
                if (faces.Length > 0)
                {
                    disgustROI.Push(faces);
                }
            }

            // Crop and resize the faces to 256 X 256
            VectorOfMat happycrop = new VectorOfMat();
            VectorOfMat sadcrop = new VectorOfMat();
            VectorOfMat angercrop = new VectorOfMat();
            VectorOfMat surprisecrop = new VectorOfMat();
            VectorOfMat fearcrop = new VectorOfMat();
            VectorOfMat disgustcrop = new VectorOfMat();
            
            for (int i = 0; i < happy.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(happy[i], happyROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                happycrop.Push(resized);
            }

            for (int i = 0; i < sad.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(sad[i], sadROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                sadcrop.Push(resized);
            }

            for (int i = 0; i < anger.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(anger[i], angerROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                angercrop.Push(resized);
            }

            for (int i = 0; i < surprise.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(surprise[i], surpriseROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                surprisecrop.Push(resized);
            }

            for (int i = 0; i < fear.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(fear[i], fearROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                fearcrop.Push(resized);
            }

            for (int i = 0; i < disgust.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(disgust[i], disgustROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                disgustcrop.Push(resized);
            }

            // Use haar cascades to find the faces
            happyROI = new VectorOfRect();
            for (int i = 0; i < happycrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(happycrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    happyROI.Push(faces);
                }
            }

            sadROI = new VectorOfRect();
            for (int i = 0; i < sadcrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(sadcrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    sadROI.Push(faces);
                }
            }

            angerROI = new VectorOfRect();
            for (int i = 0; i < angercrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(angercrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    angerROI.Push(faces);
                }
            }

            surpriseROI = new VectorOfRect();
            for (int i = 0; i < surprisecrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(surprisecrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    surpriseROI.Push(faces);
                }
            }

            fearROI = new VectorOfRect();
            for (int i = 0; i < fearcrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(fearcrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    fearROI.Push(faces);
                }
            }

            disgustROI = new VectorOfRect();
            for (int i = 0; i < disgustcrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(disgustcrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    disgustROI.Push(faces);
                }
            }

            // Use facemark to locate the facial landmarks
            FacemarkLBF finder = new FacemarkLBF(new FacemarkLBFParams());
            finder.LoadModel("lbfmodel.yaml"); // model sourced from https://raw.githubusercontent.com/kurnianggoro/GSOC2017/master/data/lbfmodel.yaml

            List<VectorOfVectorOfPointF> happyLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < happycrop.Size; i++)
            {
                VectorOfVectorOfPointF happyLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect happyROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = happyROI[i];
                happyROIi.Push(ROIi);
                bool happySuccess = finder.Fit(happycrop[i], happyROIi, happyLandmarks);
                if (happySuccess)
                {
                    happyLandmarksCollection.Add(happyLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> sadLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < sadcrop.Size; i++)
            {
                VectorOfVectorOfPointF sadLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect sadROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = sadROI[i];
                sadROIi.Push(ROIi);
                bool sadSuccess = finder.Fit(sadcrop[i], sadROIi, sadLandmarks);
                if (sadSuccess)
                {
                    sadLandmarksCollection.Add(sadLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> angerLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < angercrop.Size; i++)
            {
                VectorOfVectorOfPointF angerLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect angerROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = angerROI[i];
                angerROIi.Push(ROIi);
                bool angerSuccess = finder.Fit(angercrop[i], angerROIi, angerLandmarks);
                if (angerSuccess)
                {
                    angerLandmarksCollection.Add(angerLandmarks);
                }
            }
            
            List<VectorOfVectorOfPointF> surpriseLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < surprisecrop.Size; i++)
            {
                VectorOfVectorOfPointF surpriseLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect surpriseROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = surpriseROI[i];
                surpriseROIi.Push(ROIi);
                bool surpriseSuccess = finder.Fit(surprisecrop[i], surpriseROIi, surpriseLandmarks);
                if (surpriseSuccess)
                {
                    surpriseLandmarksCollection.Add(surpriseLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> fearLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < fearcrop.Size; i++)
            {
                VectorOfVectorOfPointF fearLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect fearROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = fearROI[i];
                fearROIi.Push(ROIi);
                bool fearSuccess = finder.Fit(fearcrop[i], fearROIi, fearLandmarks);
                if (fearSuccess)
                {
                    fearLandmarksCollection.Add(fearLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> disgustLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < disgustcrop.Size; i++)
            {
                VectorOfVectorOfPointF disgustLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect disgustROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = disgustROI[i];
                disgustROIi.Push(ROIi);
                bool disgustSuccess = finder.Fit(disgustcrop[i], disgustROIi, disgustLandmarks);
                if (disgustSuccess)
                {
                    disgustLandmarksCollection.Add(disgustLandmarks);
                }
            }

            // Composite the landmarks from each image for each emotion
            int landmarksCount = 68;

            double[] happyLandmarksCompositeX = new double[landmarksCount];
            double[] happyLandmarksCompositeY = new double[landmarksCount];
            for (int i = 0; i < happyLandmarksCollection.Count; i++)
            {
                for (int j = 0; j < landmarksCount; j++)
                {
                    happyLandmarksCompositeX[j] += happyLandmarksCollection[i][0][j].X;
                    happyLandmarksCompositeY[j] += happyLandmarksCollection[i][0][j].Y;
                }
            }

            for (int i = 0; i < landmarksCount; i++)
            {
                happyLandmarksCompositeX[i] = happyLandmarksCompositeX[i] / happyLandmarksCollection.Count;
                happyLandmarksCompositeY[i] = happyLandmarksCompositeY[i] / happyLandmarksCollection.Count;
            }

            double[] sadLandmarksCompositeX = new double[landmarksCount];
            double[] sadLandmarksCompositeY = new double[landmarksCount];
            for (int i = 0; i < sadLandmarksCollection.Count; i++)
            {
                for (int j = 0; j < landmarksCount; j++)
                {
                    sadLandmarksCompositeX[j] += sadLandmarksCollection[i][0][j].X;
                    sadLandmarksCompositeY[j] += sadLandmarksCollection[i][0][j].Y;
                }
            }

            for (int i = 0; i < landmarksCount; i++)
            {
                sadLandmarksCompositeX[i] = sadLandmarksCompositeX[i] / sadLandmarksCollection.Count;
                sadLandmarksCompositeY[i] = sadLandmarksCompositeY[i] / sadLandmarksCollection.Count;
            }

            double[] angerLandmarksCompositeX = new double[landmarksCount];
            double[] angerLandmarksCompositeY = new double[landmarksCount];
            for (int i = 0; i < angerLandmarksCollection.Count; i++)
            {
                for (int j = 0; j < landmarksCount; j++)
                {
                    angerLandmarksCompositeX[j] += angerLandmarksCollection[i][0][j].X;
                    angerLandmarksCompositeY[j] += angerLandmarksCollection[i][0][j].Y;
                }
            }

            for (int i = 0; i < landmarksCount; i++)
            {
                angerLandmarksCompositeX[i] = angerLandmarksCompositeX[i] / angerLandmarksCollection.Count;
                angerLandmarksCompositeY[i] = angerLandmarksCompositeY[i] / angerLandmarksCollection.Count;
            }

            double[] surpriseLandmarksCompositeX = new double[landmarksCount];
            double[] surpriseLandmarksCompositeY = new double[landmarksCount];
            for (int i = 0; i < surpriseLandmarksCollection.Count; i++)
            {
                for (int j = 0; j < landmarksCount; j++)
                {
                    surpriseLandmarksCompositeX[j] += surpriseLandmarksCollection[i][0][j].X;
                    surpriseLandmarksCompositeY[j] += surpriseLandmarksCollection[i][0][j].Y;
                }
            }

            for (int i = 0; i < landmarksCount; i++)
            {
                surpriseLandmarksCompositeX[i] = surpriseLandmarksCompositeX[i] / surpriseLandmarksCollection.Count;
                surpriseLandmarksCompositeY[i] = surpriseLandmarksCompositeY[i] / surpriseLandmarksCollection.Count;
            }

            double[] fearLandmarksCompositeX = new double[landmarksCount];
            double[] fearLandmarksCompositeY = new double[landmarksCount];
            for (int i = 0; i < fearLandmarksCollection.Count; i++)
            {
                for (int j = 0; j < landmarksCount; j++)
                {
                    fearLandmarksCompositeX[j] += fearLandmarksCollection[i][0][j].X;
                    fearLandmarksCompositeY[j] += fearLandmarksCollection[i][0][j].Y;
                }
            }

            for (int i = 0; i < landmarksCount; i++)
            {
                fearLandmarksCompositeX[i] = fearLandmarksCompositeX[i] / fearLandmarksCollection.Count;
                fearLandmarksCompositeY[i] = fearLandmarksCompositeY[i] / fearLandmarksCollection.Count;
            }

            double[] disgustLandmarksCompositeX = new double[landmarksCount];
            double[] disgustLandmarksCompositeY = new double[landmarksCount];
            for (int i = 0; i < disgustLandmarksCollection.Count; i++)
            {
                for (int j = 0; j < landmarksCount; j++)
                {
                    disgustLandmarksCompositeX[j] += disgustLandmarksCollection[i][0][j].X;
                    disgustLandmarksCompositeY[j] += disgustLandmarksCollection[i][0][j].Y;
                }
            }

            for (int i = 0; i < landmarksCount; i++)
            {
                disgustLandmarksCompositeX[i] = disgustLandmarksCompositeX[i] / disgustLandmarksCollection.Count;
                disgustLandmarksCompositeY[i] = disgustLandmarksCompositeY[i] / disgustLandmarksCollection.Count;
            }

            // Convert the lists of facial landmark points to 1d vectors

            double[] happyLandmarksVector = new double[landmarksCount];
            for (int i = 0; i < landmarksCount; i++)
            {
                happyLandmarksVector[i] = happyLandmarksCompositeX[i] + (happyLandmarksCompositeY[i] * 256);
            }

            double[] sadLandmarksVector = new double[landmarksCount];
            for (int i = 0; i < landmarksCount; i++)
            {
                sadLandmarksVector[i] = sadLandmarksCompositeX[i] + (sadLandmarksCompositeY[i] * 256);
            }

            double[] angerLandmarksVector = new double[landmarksCount];
            for (int i = 0; i < landmarksCount; i++)
            {
                angerLandmarksVector[i] = angerLandmarksCompositeX[i] + (angerLandmarksCompositeY[i] * 256);
            }

            double[] surpriseLandmarksVector = new double[landmarksCount];
            for (int i = 0; i < landmarksCount; i++)
            {
                surpriseLandmarksVector[i] = surpriseLandmarksCompositeX[i] + (surpriseLandmarksCompositeY[i] * 256);
            }

            double[] fearLandmarksVector = new double[landmarksCount];
            for (int i = 0; i < landmarksCount; i++)
            {
                fearLandmarksVector[i] = fearLandmarksCompositeX[i] + (fearLandmarksCompositeY[i] * 256);
            }

            double[] disgustLandmarksVector = new double[landmarksCount];
            for (int i = 0; i < landmarksCount; i++)
            {
                disgustLandmarksVector[i] = disgustLandmarksCompositeX[i] + (disgustLandmarksCompositeY[i] * 256);
            }

            // save the models

            EmotionModel toSave = new EmotionModel();
            toSave.Happy = happyLandmarksVector;
            toSave.Sad = sadLandmarksVector;
            toSave.Anger = angerLandmarksVector;
            toSave.Surprise = surpriseLandmarksVector;
            toSave.Fear = fearLandmarksVector;
            toSave.Disgust = disgustLandmarksVector;

            string savePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\EmotionModel.xml";
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(toSave.GetType());
                writer = new StreamWriter(savePath);
                serializer.Serialize(writer, toSave);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        static void Test()
        {
            TextReader reader = null;
            EmotionModel model = new EmotionModel();
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

            string pathPrefix = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\Cohn-Kanade Database\\Cohn-Kanade Database\\CK+\\Labeled\\";
            string pathExtension = ".png";

            // Read in images
            VectorOfMat happy = new VectorOfMat();
            VectorOfMat sad = new VectorOfMat();
            VectorOfMat anger = new VectorOfMat();
            VectorOfMat surprise = new VectorOfMat();
            VectorOfMat fear = new VectorOfMat();
            VectorOfMat disgust = new VectorOfMat();

            for (int i = 1; i < 69; i++)
            {
                happy.Push(CvInvoke.Imread(pathPrefix + "happy" + i + pathExtension));
            }

            for (int i = 1; i < 24; i++)
            {
                sad.Push(CvInvoke.Imread(pathPrefix + "sad" + i + pathExtension));
            }

            for (int i = 1; i < 39; i++)
            {
                anger.Push(CvInvoke.Imread(pathPrefix + "anger" + i + pathExtension));
            }

            for (int i = 1; i < 83; i++)
            {
                surprise.Push(CvInvoke.Imread(pathPrefix + "surprise" + i + pathExtension));
            }

            for (int i = 1; i < 21; i++)
            {
                fear.Push(CvInvoke.Imread(pathPrefix + "fear" + i + pathExtension));
            }

            for (int i = 1; i < 59; i++)
            {
                disgust.Push(CvInvoke.Imread(pathPrefix + "disgust" + i + pathExtension));
            }

            // Begin stopwatch
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // For each image:
            // Use haar cascades to find the faces
            CascadeClassifier faceClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");

            VectorOfRect happyROI = new VectorOfRect();
            for (int i = 0; i < happy.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(happy[i]); // Find the face
                if (faces.Length > 0)
                {
                    happyROI.Push(faces);
                }
            }

            VectorOfRect sadROI = new VectorOfRect();
            for (int i = 0; i < sad.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(sad[i]); // Find the face
                if (faces.Length > 0)
                {
                    sadROI.Push(faces);
                }
            }

            VectorOfRect angerROI = new VectorOfRect();
            for (int i = 0; i < anger.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(anger[i]); // Find the face
                if (faces.Length > 0)
                {
                    angerROI.Push(faces);
                }
            }

            VectorOfRect surpriseROI = new VectorOfRect();
            for (int i = 0; i < surprise.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(surprise[i]); // Find the face
                if (faces.Length > 0)
                {
                    surpriseROI.Push(faces);
                }
            }

            VectorOfRect fearROI = new VectorOfRect();
            for (int i = 0; i < fear.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(fear[i]); // Find the face
                if (faces.Length > 0)
                {
                    fearROI.Push(faces);
                }
            }

            VectorOfRect disgustROI = new VectorOfRect();
            for (int i = 0; i < disgust.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(disgust[i]); // Find the face
                if (faces.Length > 0)
                {
                    disgustROI.Push(faces);
                }
            }

            // Crop and resize the faces to 256 X 256
            VectorOfMat happycrop = new VectorOfMat();
            VectorOfMat sadcrop = new VectorOfMat();
            VectorOfMat angercrop = new VectorOfMat();
            VectorOfMat surprisecrop = new VectorOfMat();
            VectorOfMat fearcrop = new VectorOfMat();
            VectorOfMat disgustcrop = new VectorOfMat();

            for (int i = 0; i < happy.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(happy[i], happyROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                happycrop.Push(resized);
            }

            for (int i = 0; i < sad.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(sad[i], sadROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                sadcrop.Push(resized);
            }

            for (int i = 0; i < anger.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(anger[i], angerROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                angercrop.Push(resized);
            }

            for (int i = 0; i < surprise.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(surprise[i], surpriseROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                surprisecrop.Push(resized);
            }

            for (int i = 0; i < fear.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(fear[i], fearROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                fearcrop.Push(resized);
            }

            for (int i = 0; i < disgust.Size; i++)
            {
                Mat resized = new Mat();
                Mat cropped = new Mat(disgust[i], disgustROI[i]);
                CvInvoke.Resize(cropped, resized, new Size(0, 0), (256f / cropped.Cols), (256f / cropped.Rows)); // resize the image to 256 x 256 pixels
                disgustcrop.Push(resized);
            }

            // Use haar cascades to find the faces
            happyROI = new VectorOfRect();
            for (int i = 0; i < happycrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(happycrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    happyROI.Push(faces);
                }
            }

            sadROI = new VectorOfRect();
            for (int i = 0; i < sadcrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(sadcrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    sadROI.Push(faces);
                }
            }

            angerROI = new VectorOfRect();
            for (int i = 0; i < angercrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(angercrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    angerROI.Push(faces);
                }
            }

            surpriseROI = new VectorOfRect();
            for (int i = 0; i < surprisecrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(surprisecrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    surpriseROI.Push(faces);
                }
            }

            fearROI = new VectorOfRect();
            for (int i = 0; i < fearcrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(fearcrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    fearROI.Push(faces);
                }
            }

            disgustROI = new VectorOfRect();
            for (int i = 0; i < disgustcrop.Size; i++)
            {
                var faces = faceClassifier.DetectMultiScale(disgustcrop[i]); // Find the face
                if (faces.Length > 0)
                {
                    disgustROI.Push(faces);
                }
            }

            // Use facemark to locate the facial landmarks
            FacemarkLBF finder = new FacemarkLBF(new FacemarkLBFParams());
            finder.LoadModel("lbfmodel.yaml"); // model sourced from https://raw.githubusercontent.com/kurnianggoro/GSOC2017/master/data/lbfmodel.yaml

            List<VectorOfVectorOfPointF> happyLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < happycrop.Size; i++)
            {
                VectorOfVectorOfPointF happyLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect happyROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = happyROI[i];
                happyROIi.Push(ROIi);
                bool happySuccess = finder.Fit(happycrop[i], happyROIi, happyLandmarks);
                if (happySuccess)
                {
                    happyLandmarksCollection.Add(happyLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> sadLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < sadcrop.Size; i++)
            {
                VectorOfVectorOfPointF sadLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect sadROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = sadROI[i];
                sadROIi.Push(ROIi);
                bool sadSuccess = finder.Fit(sadcrop[i], sadROIi, sadLandmarks);
                if (sadSuccess)
                {
                    sadLandmarksCollection.Add(sadLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> angerLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < angercrop.Size; i++)
            {
                VectorOfVectorOfPointF angerLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect angerROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = angerROI[i];
                angerROIi.Push(ROIi);
                bool angerSuccess = finder.Fit(angercrop[i], angerROIi, angerLandmarks);
                if (angerSuccess)
                {
                    angerLandmarksCollection.Add(angerLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> surpriseLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < surprisecrop.Size; i++)
            {
                VectorOfVectorOfPointF surpriseLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect surpriseROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = surpriseROI[i];
                surpriseROIi.Push(ROIi);
                bool surpriseSuccess = finder.Fit(surprisecrop[i], surpriseROIi, surpriseLandmarks);
                if (surpriseSuccess)
                {
                    surpriseLandmarksCollection.Add(surpriseLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> fearLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < fearcrop.Size; i++)
            {
                VectorOfVectorOfPointF fearLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect fearROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = fearROI[i];
                fearROIi.Push(ROIi);
                bool fearSuccess = finder.Fit(fearcrop[i], fearROIi, fearLandmarks);
                if (fearSuccess)
                {
                    fearLandmarksCollection.Add(fearLandmarks);
                }
            }

            List<VectorOfVectorOfPointF> disgustLandmarksCollection = new List<VectorOfVectorOfPointF>();
            for (int i = 0; i < disgustcrop.Size; i++)
            {
                VectorOfVectorOfPointF disgustLandmarks = new VectorOfVectorOfPointF();
                VectorOfRect disgustROIi = new VectorOfRect();
                Rectangle[] ROIi = new Rectangle[1];
                ROIi[0] = disgustROI[i];
                disgustROIi.Push(ROIi);
                bool disgustSuccess = finder.Fit(disgustcrop[i], disgustROIi, disgustLandmarks);
                if (disgustSuccess)
                {
                    disgustLandmarksCollection.Add(disgustLandmarks);
                }
            }

            double gamma = 100000f;
            
            int[] happyResults = new int[happyLandmarksCollection.Count];
            for (int j = 0; j < happyLandmarksCollection.Count; j++)
            {
                // convert the array of points to a 1d vector
                int landmarksCount = 68; // the LBF model used finds exactly 68 landmarks
                double[] landmarksVector = new double[landmarksCount];

                for (int i = 0; i < landmarksCount; i++)
                {
                    // treat the landmark coordinates as an address in a 2d array, and convert that to an address for a 1d array
                    landmarksVector[i] = happyLandmarksCollection[j][0][i].X + (256 * happyLandmarksCollection[j][0][i].Y);
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
                happyResults[j] = analyzedEmotion;
            }

            int[] sadResults = new int[sadLandmarksCollection.Count];
            for (int j = 0; j < sadLandmarksCollection.Count; j++)
            {
                // convert the array of points to a 1d vector
                int landmarksCount = 68; // the LBF model used finds exactly 68 landmarks
                double[] landmarksVector = new double[landmarksCount];

                for (int i = 0; i < landmarksCount; i++)
                {
                    // treat the landmark coordinates as an address in a 2d array, and convert that to an address for a 1d array
                    landmarksVector[i] = sadLandmarksCollection[j][0][i].X + (256 * sadLandmarksCollection[j][0][i].Y);
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
                sadResults[j] = analyzedEmotion;
            }

            int[] angerResults = new int[angerLandmarksCollection.Count];
            for (int j = 0; j < angerLandmarksCollection.Count; j++)
            {
                // convert the array of points to a 1d vector
                int landmarksCount = 68; // the LBF model used finds exactly 68 landmarks
                double[] landmarksVector = new double[landmarksCount];

                for (int i = 0; i < landmarksCount; i++)
                {
                    // treat the landmark coordinates as an address in a 2d array, and convert that to an address for a 1d array
                    landmarksVector[i] = angerLandmarksCollection[j][0][i].X + (256 * angerLandmarksCollection[j][0][i].Y);
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
                angerResults[j] = analyzedEmotion;
            }

            int[] surpriseResults = new int[surpriseLandmarksCollection.Count];
            for (int j = 0; j < surpriseLandmarksCollection.Count; j++)
            {
                // convert the array of points to a 1d vector
                int landmarksCount = 68; // the LBF model used finds exactly 68 landmarks
                double[] landmarksVector = new double[landmarksCount];

                for (int i = 0; i < landmarksCount; i++)
                {
                    // treat the landmark coordinates as an address in a 2d array, and convert that to an address for a 1d array
                    landmarksVector[i] = surpriseLandmarksCollection[j][0][i].X + (256 * surpriseLandmarksCollection[j][0][i].Y);
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
                surpriseResults[j] = analyzedEmotion;
            }

            int[] fearResults = new int[fearLandmarksCollection.Count];
            for (int j = 0; j < fearLandmarksCollection.Count; j++)
            {
                // convert the array of points to a 1d vector
                int landmarksCount = 68; // the LBF model used finds exactly 68 landmarks
                double[] landmarksVector = new double[landmarksCount];

                for (int i = 0; i < landmarksCount; i++)
                {
                    // treat the landmark coordinates as an address in a 2d array, and convert that to an address for a 1d array
                    landmarksVector[i] = fearLandmarksCollection[j][0][i].X + (256 * fearLandmarksCollection[j][0][i].Y);
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
                fearResults[j] = analyzedEmotion;
            }

            int[] disgustResults = new int[disgustLandmarksCollection.Count];
            for (int j = 0; j < disgustLandmarksCollection.Count; j++)
            {
                // convert the array of points to a 1d vector
                int landmarksCount = 68; // the LBF model used finds exactly 68 landmarks
                double[] landmarksVector = new double[landmarksCount];

                for (int i = 0; i < landmarksCount; i++)
                {
                    // treat the landmark coordinates as an address in a 2d array, and convert that to an address for a 1d array
                    landmarksVector[i] = disgustLandmarksCollection[j][0][i].X + (256 * disgustLandmarksCollection[j][0][i].Y);
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
                disgustResults[j] = analyzedEmotion;
            }

            // stop stopwatch
            watch.Stop();

            int[] happyGuesses = new int[7];
            for (int i = 0; i < 7; i++)
                happyGuesses[i] = 0;

            for (int i = 0; i < happyResults.Length; i++)
            {
                happyGuesses[happyResults[i]]++;
            }

            int[] sadGuesses = new int[7];
            for (int i = 0; i < 7; i++)
                sadGuesses[i] = 0;

            for (int i = 0; i < sadResults.Length; i++)
            {
                sadGuesses[sadResults[i]]++;
            }

            int[] angerGuesses = new int[7];
            for (int i = 0; i < 7; i++)
                angerGuesses[i] = 0;

            for (int i = 0; i < angerResults.Length; i++)
            {
                angerGuesses[angerResults[i]]++;
            }

            int[] surpriseGuesses = new int[7];
            for (int i = 0; i < 7; i++)
                surpriseGuesses[i] = 0;

            for (int i = 0; i < surpriseResults.Length; i++)
            {
                surpriseGuesses[surpriseResults[i]]++;
            }

            int[] fearGuesses = new int[7];
            for (int i = 0; i < 7; i++)
                fearGuesses[i] = 0;

            for (int i = 0; i < fearResults.Length; i++)
            {
                fearGuesses[fearResults[i]]++;
            }

            int[] disgustGuesses = new int[7];
            for (int i = 0; i < 7; i++)
                disgustGuesses[i] = 0;

            for (int i = 0; i < disgustResults.Length; i++)
            {
                disgustGuesses[disgustResults[i]]++;
            }

            Console.WriteLine(happyGuesses[0] + " " + happyGuesses[1] + " " + happyGuesses[2] + " " + happyGuesses[3] + " " + happyGuesses[4] + " " + happyGuesses[5] + " " + happyGuesses[6]);

            Console.WriteLine(sadGuesses[0] + " " + sadGuesses[1] + " " + sadGuesses[2] + " " + sadGuesses[3] + " " + sadGuesses[4] + " " + sadGuesses[5] + " " + sadGuesses[6]);

            Console.WriteLine(angerGuesses[0] + " " + angerGuesses[1] + " " + angerGuesses[2] + " " + angerGuesses[3] + " " + angerGuesses[4] + " " + angerGuesses[5] + " " + angerGuesses[6]);

            Console.WriteLine(surpriseGuesses[0] + " " + surpriseGuesses[1] + " " + surpriseGuesses[2] + " " + surpriseGuesses[3] + " " + surpriseGuesses[4] + " " + surpriseGuesses[5] + " " + surpriseGuesses[6]);

            Console.WriteLine(fearGuesses[0] + " " + fearGuesses[1] + " " + fearGuesses[2] + " " + fearGuesses[3] + " " + fearGuesses[4] + " " + fearGuesses[5] + " " + fearGuesses[6]);

            Console.WriteLine(disgustGuesses[0] + " " + disgustGuesses[1] + " " + disgustGuesses[2] + " " + disgustGuesses[3] + " " + disgustGuesses[4] + " " + disgustGuesses[5] + " " + disgustGuesses[6]);

            Console.WriteLine((watch.ElapsedMilliseconds / 289) + " milliseconds per image");
        }
    }
}
