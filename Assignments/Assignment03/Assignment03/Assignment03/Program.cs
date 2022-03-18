using System;
using System.Collections.Generic;
using System.Numerics.Tensors;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualBasic.FileIO;

namespace Assignment03
{
    class Program
    {
        static void Main(string[] args)
        {
            TextFieldParser parse = new TextFieldParser("D:/source/repos/581TermProject/581TermProject/fer2013.csv");
            parse.Delimiters = new string[] { "," };
            List<List<string>> dataCollection = new List<List<string>>();
            while (!parse.EndOfData)
            {
                List<string> line = new List<string>();
                string[] temp;
                try
                {
                    temp = parse.ReadFields();
                    for (int i = 0; i < temp.Length; i++)
                    {
                        line.Add(temp[i]);
                    }
                    dataCollection.Add(line);
                }
                catch (MalformedLineException e)
                {
                    Console.WriteLine("Line " + e.Message + "is invalid. Skipping");
                }

                
            }
        }

        public int Process(byte[] imageData)
        {
            // attempt to detect a face
            Image<Bgra, byte> cvImage = new Image<Bgra, byte>(48, 48);
            cvImage.Bytes = imageData;
            Mat cvMat = cvImage.Mat;
            CascadeClassifier faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            var faces = faceCascade.DetectMultiScale(cvMat);

            // no face detected, return without processing
            if (faces.Length < 1)
            {
                return 0;
            }

            // face found, process emotion
            
        }

    }
}
