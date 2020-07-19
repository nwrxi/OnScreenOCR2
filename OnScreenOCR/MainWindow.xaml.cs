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
using Google.Rpc;
using MaterialDesignThemes.Wpf;
using OnScreenOCR.Helpers;
using OnScreenOCR.PerformOCR;

namespace OnScreenOCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool OcrInProgress { get; set; }
        private HotkeyManager _hotkeyManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsBtn.IsChecked = true;
            _hotkeyManager = new HotkeyManager();
            _hotkeyManager.CreateHotkey(Modifiers.ModControl, 0x14); //caps-lock
            _hotkeyManager.HotKeyPressed += HotkeyOnHotKeyPressed;
        }

        private async void PerformBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OcrInProgress)
            {
                Dialog.IsOpen = true;
                return;
            }

            OcrInProgress = true;
            if (WindowState != WindowState.Minimized)
                Hide();

            await Task.Delay(180);
            var image = Screenshot.CaptureRegion();

            if (image == null)
            {
                OcrInProgress = false;
                ShowWindow();
                return;
            }

            var bitImage = await ConvertImage(image);

            ShowWindow();

            string recognizedText;
            try
            {
                IOcrHandler ocrHandler = AppSettings.Default.OcrEngine switch
                {
                    0 => new VisionApi(),
                    1 => new OcrSpace(),
                    _ => new VisionApi()
                };

                recognizedText = await RecognizeText(ocrHandler, bitImage);
            }
            catch (Exception exception)
            {
                DialogText.Text = exception.Message;
                AppSettings.Default.GOOGLE_APPLICATION_CREDENTIALS = "";
                OcrInProgress = false;
                ShowWindow();
                Dialog.IsOpen = true;

                return;
            }

            OpenNewWindow(recognizedText);

            if(AppSettings.Default.CopyToClipboard)
                Clipboard.SetDataObject(recognizedText);

            OcrInProgress = false;
        }

        private void ShowWindow()
        {
            if (Visibility == Visibility.Hidden)
                Show();
        }

        private static void OpenNewWindow(string recognizedText)
        {
            if (AppSettings.Default.OpenNewWindow)
            {
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
            }
        }

        private static async Task<string> RecognizeText(IOcrHandler ocrHandler, byte[] bitImage)
        {
            return await ocrHandler.RecognizeImage(bitImage);
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

        private void HotkeyOnHotKeyPressed(object sender, EventArgs e)
        {
            PerformBtn_Click(this, null);
        }

        private void ExitBtn_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SettingsBtn_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
        }

        private void AboutBtn_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AboutPage());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _hotkeyManager.DeleteHotkeys();
        }
    }
}
