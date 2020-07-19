using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace OnScreenOCR.PerformOCR
{
    public class OcrSpace : IOcrHandler
    {
        public async Task<string> RecognizeImage(byte[] imageArr)
        {
            try
            {
                var httpClient = new HttpClient { Timeout = new TimeSpan(0, 3, 0) };

                var content = new MultipartFormDataContent
                {
                    {new StringContent(AppSettings.Default.OcrSpaceApiKey), "apikey"},
                    {new StringContent(GetShortLanguageString()), "language"},
                    {new ByteArrayContent(imageArr, 0, imageArr.Length), "image", "image.bmp"}
                };


                var response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (responseContent == "\"The API key is invalid\"" || responseContent == "\"API Key is not specified. Please provide a valid API key.\"")
                    throw new Exception("Invalid API key");

                var ocrResult = JsonConvert.DeserializeObject<Rootobject>(responseContent);

                if (ocrResult.OCRExitCode == 1)
                {
                    return string.IsNullOrWhiteSpace(ocrResult.ParsedResults[0].ParsedText)
                        ? "No text found\n"
                        : ocrResult.ParsedResults[0].ParsedText;
                }

                throw new Exception("Error while preforming OCR");
            }

            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        private static string GetShortLanguageString()
        {
            return AppSettings.Default.OcrLanguage switch
            {
                0 => "ara",
                1 => "chs",
                2 => "cht",
                3 => "cze",
                4 => "dan",
                5 => "dut",
                6 => "eng",
                7 => "fin",
                8 => "fre",
                9 => "ger",
                10 => "gre",
                11 => "hun",
                12 => "jpn",
                13 => "kor",
                14 => "nor",
                15 => "pol",
                16 => "por",
                17 => "spa",
                18 => "swe",
                19 => "tur",
                _ => "eng"
            };
        }
    }

   
}