﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Serialization;
using Google.Cloud.Vision.V1;

namespace OnScreenOCR.PerformOCR
{
    public class VisionApi : IOcrHandler
    {
        public async Task<string> RecognizeImage(byte[] imageArr)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AppSettings.Default.GOOGLE_APPLICATION_CREDENTIALS);

            var image = await Task.Run(() => Image.FromBytes(imageArr));
            var client = await ImageAnnotatorClient.CreateAsync();


            //var imageContext = new ImageContext();
            //imageContext.LanguageHints.Add("ja");
            //var request = new AnnotateImageRequest { ImageContext = imageContext, Image = image, Features = { new Feature { Type = Feature.Types.Type.TextDetection } } };
            //var response = await client.AnnotateAsync(request);

            //return response.TextAnnotations.ToString() == "[ ]" ? "No text found" : response.TextAnnotations[0].Description; ;


            var textAnnotations = await client.DetectTextAsync(image);
            return textAnnotations.ToString() == "[ ]" ? "No text found" : textAnnotations[0].Description;
        }
    }
}