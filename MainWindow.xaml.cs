using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GoodByeDPI_Manager.Resources;

namespace GoodByeDPIManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;

            InitializeComponent();

            // Dili başlatırken ComboBox'ta işaretle
            string currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            foreach (ComboBoxItem item in languageSelector.Items)
            {
                if (item.Tag.ToString() == currentCulture)
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
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptName);

            if (!File.Exists(path))
            {
                MessageBox.Show("Script not found: " + scriptName);
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
                txtStatus.Foreground = System.Windows.Media.Brushes.Green;
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
            txtStatus.Foreground = System.Windows.Media.Brushes.DarkRed;
        }

        private void languageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languageSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string cultureCode = selectedItem.Tag.ToString();
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
                SetUIText();
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
    }
}
