using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Util;

namespace Mobile_ReadyExpressionAnalysisTraining
{
    class Program
    {
        static void Main(string[] args)
        {
            Train();
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
    }
}
