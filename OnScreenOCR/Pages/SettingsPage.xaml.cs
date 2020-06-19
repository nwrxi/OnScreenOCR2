﻿using System;
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

            var ocrEngines = new List<string> {"Google Vision", "OCR.Space"};

            if (OcrEngine.AvailableRecognizerLanguages.Count > 0)
            {
                ocrEngines.Add("Windows 10");
            }

            ApiList.ItemsSource = ocrEngines;

            if (AppSettings.Default.OcrEngine >= 0)
                ApiList.SelectedIndex = AppSettings.Default.OcrEngine;

            if (AppSettings.Default.OcrLanguage >= 0)
                LanguageList.SelectedIndex = AppSettings.Default.OcrLanguage;

        }


        private void ApiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppSettings.Default.OcrEngine = ApiList.SelectedIndex;
            PopulateLanguageList();
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
                    LanguageList.ItemsSource = new List<string> { "TODO" };
                    break;
                case 2:
                    LanguageList.ItemsSource = OcrEngine.AvailableRecognizerLanguages.Select(l => l.DisplayName);
                    break;
            }

        }

        private void LanguageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppSettings.Default.OcrLanguage = LanguageList.SelectedIndex;
        }
    }
}