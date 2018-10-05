using System;
using System.Collections.Generic;
using System.Configuration;
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
using AdinaCardGame;

namespace AdinaCardGameUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            PromptPathLabel.Content = "Prompt file: " + ImageCreationProcess.PromptCardPath;
            AnswerPathLabel.Content = "Answer file: " + ImageCreationProcess.AnswerCardPath;
            ImageCreationProgressText.Content = "";
            ImageCreationProgressBar.Visibility = Visibility.Hidden;
            base.OnInitialized(e);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(ImageCreationProcess.PromptCardPath))
            {
                MessageBox.Show(
                    $"The prompt file is not available at {ImageCreationProcess.PromptCardPath}. Please make sure this file is available there, or change the configuration file to the correct path.",
                    "Prompt File Missing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(ImageCreationProcess.AnswerCardPath))
            {
                MessageBox.Show(
                    $"The answer file is not available at {ImageCreationProcess.AnswerCardPath}. Please make sure this file is available there, or change the configuration file to the correct path.",
                    "Prompt File Missing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }


            try
            {
                var progress = new Progress<ImageCreationProgress>();
                var imageCreationProcess = new ImageCreationProcess();
                progress.ProgressChanged += UpdateProgress;
                CreateImagesButton.IsEnabled = false;
                ImageCreationProgressBar.Visibility = Visibility.Visible;
                var timeStampedFolder = await Task.Run(() => imageCreationProcess.Run(progress));
                ImageCreationProgressBar.Visibility = Visibility.Hidden;
                CreateImagesButton.IsEnabled = true;
                ImageCreationProgressText.Content = "";
                MessageBox.Show($"Images have been created at : {timeStampedFolder}");
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error encountered. Please send Ezra the following:\n{exception}");
            }
        }

        public void UpdateProgress(object sender, ImageCreationProgress progress)
        {
            ImageCreationProgressBar.Maximum = progress.Total;
            ImageCreationProgressBar.Value = progress.Complete;
            ImageCreationProgressText.Content = progress.MostRecentFileComplete;
        }
    }
}
