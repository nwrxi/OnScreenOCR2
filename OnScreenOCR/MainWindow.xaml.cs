using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GI.Screenshot;
using OnScreenOCR.PerformOCR;

namespace OnScreenOCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool OcrInProgress { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void PerformBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OcrInProgress)
            {
                MessageBox.Show("OCR is already in progress");
                return;
            }

            OcrInProgress = true;
            Hide();
            await Task.Delay(180);
            var image = Screenshot.CaptureRegion();

            var bitImage = await ConvertImage(image);

            Show();

            var recognizedText = await new VisionApi().RecognizeImage(bitImage);


            if (Application.Current.Windows.OfType<ResultWindow>().Any())
            {
                var resWindow = Application.Current.Windows.OfType<ResultWindow>().ElementAt(0);
                resWindow.ResultText.Text = recognizedText;
            }
            else
            {
                var resWindow = new ResultWindow {ResultText = {Text = recognizedText}};
                resWindow.Show();
            }
            //MessageBox.Show(recognizedText);
            Clipboard.SetText(recognizedText);

            OcrInProgress = false;
        }

        private static async Task<byte[]> ConvertImage(BitmapSource image)
        {
            BitmapEncoder encoder = new BmpBitmapEncoder();

            byte[] bitImage;
            await using (var stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                bitImage = stream.ToArray();
                stream.Close();
            }

            return bitImage;
        }
    }
}
