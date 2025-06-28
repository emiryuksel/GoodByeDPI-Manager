using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media; // ✨ EKLENDİ
using GoodByeDPI_Manager.Resources;

namespace GoodByeDPIManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;

            InitializeComponent();

            // Köşe yuvarlama olaylarını bağla
            this.Loaded += Window_Loaded;
            this.SizeChanged += Window_SizeChanged;

            // Dili başlatırken ComboBox'ta işaretle
            string currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            foreach (ComboBoxItem item in languageSelector.Items)
            {
                if (item.Tag?.ToString() == currentCulture)
                {
                    languageSelector.SelectedItem = item;
                    break;
                }
            }

            SetUIText();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void RunScript(string scriptName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinTools", scriptName);

            if (!File.Exists(path))
            {
                MessageBox.Show("Script not found: " + path);
                return;
            }

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);

                txtStatus.Text = Strings.StatusActive;
                txtStatus.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred:\n" + ex.Message);
            }
        }


        private void btnServiceInstall_Click(object sender, RoutedEventArgs e)
        {
            RunScript("service_install_dnsredir_turkey.cmd");
        }

        private void btnSingleRun_Click(object sender, RoutedEventArgs e)
        {
            RunScript("turkey_dnsredir.cmd");
        }

        private void btnServiceRemove_Click(object sender, RoutedEventArgs e)
        {
            RunScript("service_remove.cmd");
            txtStatus.Text = Strings.StatusRemoved;
            txtStatus.Foreground = Brushes.DarkRed;
        }

        private void languageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languageSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string? cultureCode = selectedItem.Tag?.ToString();
                if (!string.IsNullOrEmpty(cultureCode))
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
                    SetUIText();
                }
            }
        }

        private void SetUIText()
        {
            this.Title = Strings.AppTitle;
            txtAppTitle.Text = Strings.AppTitle;
            btnServiceInstall.Content = Strings.InstallButton;
            btnSingleRun.Content = Strings.RunOnceButton;
            btnServiceRemove.Content = Strings.RemoveServiceButton;
            txtStatus.Text = Strings.StatusInactive;
        }

        // ✨ EKLENEN KISIM: Yuvarlak köşe clip işlemi
        private void ApplyRoundedCorners()
        {
            double radius = 10.0; // Border'daki CornerRadius ile birebir olmalı
            if (this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                this.Clip = new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight), radius, radius);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyRoundedCorners();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyRoundedCorners();
        }


    }
}
