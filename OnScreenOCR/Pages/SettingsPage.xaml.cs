using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Media.Ocr;
using Google.Apis.Auth.OAuth2;
using Microsoft.Win32;

namespace OnScreenOCR
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            PopulateApiList();

            SetChosenSettings();
        }

        private void SetChosenSettings()
        {
            if (AppSettings.Default.OcrEngine >= 0)
                ApiList.SelectedIndex = AppSettings.Default.OcrEngine;

            //if (AppSettings.Default.OcrLanguage >= 0)
            //    LanguageList.SelectedIndex = AppSettings.Default.OcrLanguage;

            if (!string.IsNullOrWhiteSpace(AppSettings.Default.GOOGLE_APPLICATION_CREDENTIALS))
            {
                GoogleKey.Margin = new Thickness(0, 11, 219, 0);
                GoogleKey.Text = "Google API key: Found";
            }

            if (!string.IsNullOrWhiteSpace(AppSettings.Default.OcrSpaceApiKey))
            {
                SpaceText.Text = "OCR.Space API key: Found";
                SpaceText.Margin = new Thickness(0, 79, 195, 0);
                SpaceKey.Text = AppSettings.Default.OcrSpaceApiKey;
            }
        }

        private void PopulateApiList()
        {
            var ocrEngines = new List<string> {"Google Vision", "OCR.Space", "Azure OCR"};

            if (OcrEngine.AvailableRecognizerLanguages.Count > 0)
            {
                ocrEngines.Add("Windows/Not Implemented");
            }

            ApiList.ItemsSource = ocrEngines;
        }


        private void ApiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppSettings.Default.OcrEngine = ApiList.SelectedIndex;
            PopulateLanguageList();

            AppSettings.Default.Save();
        }
        private void PopulateLanguageList()
        {
            
            switch (ApiList.SelectedIndex)
            {
                case 0:
                    LanguageList.ItemsSource = new List<string> { "Automatic" };
                    LanguageList.SelectedIndex = 0;
                    break;
                case 1:
                    LanguageList.ItemsSource = new List<string>
                        { "Arabic", "Chinese Simplified", "Chinese Traditional", 
                          "Czech", "Danish", "Dutch", "English", "Finnish", "French",
                          "German", "Greek", "Hungarian", "Japanese", "Korean", "Norwegian",
                          "Polish", "Portuguese", "Spanish", "Swedish", "Turkish"
                        };

                    LanguageList.SelectedIndex = AppSettings.Default.OcrLanguage < 0 ? 6 : AppSettings.Default.OcrLanguage;
                    break;
                case 2:
                    break;
                case 3:
                    LanguageList.ItemsSource = OcrEngine.AvailableRecognizerLanguages.Select(l => l.DisplayName);
                    break;
            }

        }

        private void LanguageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(AppSettings.Default.OcrEngine != 0 && LanguageList.SelectedIndex != -1)
                AppSettings.Default.OcrLanguage = LanguageList.SelectedIndex;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDlg = new OpenFileDialog {DefaultExt = ".json", Filter = "JSON files (.json)|*.json"};

            var result = openFileDlg.ShowDialog();
            if (result == true)
            {
                AppSettings.Default.GOOGLE_APPLICATION_CREDENTIALS = openFileDlg.FileName;
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AppSettings.Default.GOOGLE_APPLICATION_CREDENTIALS);

                GoogleKey.Margin = new Thickness(0, 11, 219, 0);
                GoogleKey.Text = "Google API key: Found";
            }

            AppSettings.Default.Save();

            Dialog.IsOpen = true;
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            AppSettings.Default.Save();
        }

        private void SpaceKey_LostFocus(object sender, RoutedEventArgs e)
        {
            AppSettings.Default.OcrSpaceApiKey = SpaceKey.Text;
        }
    }
}
