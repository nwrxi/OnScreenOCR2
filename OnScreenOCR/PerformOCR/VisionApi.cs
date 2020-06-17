using System;
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
            var client = ImageAnnotatorClient.Create();

            var textAnnotations = await client.DetectTextAsync(image);
            return textAnnotations.ToString() == "[ ]" ? "No text found" : textAnnotations[0].Description;
        }
    }
}