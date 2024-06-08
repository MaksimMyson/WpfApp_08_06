using System.IO;
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

namespace WpfApp_08_06
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string sourceDir = sourceTextBox.Text;
            string destinationDir = destinationTextBox.Text;
            int numThreads = int.Parse(threadsTextBox.Text);

            if (!Directory.Exists(sourceDir))
            {
                MessageBox.Show("Source directory does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            try
            {
                await CopyDirectoryAsync(sourceDir, destinationDir, numThreads);
                MessageBox.Show("Directory copied successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Task CopyDirectoryAsync(string sourceDir, string destinationDir, int numThreads)
        {
            return Task.Run(() =>
            {
                string[] files = Directory.GetFiles(sourceDir);
                Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = numThreads }, (file) =>
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(destinationDir, fileName);
                    File.Copy(file, destFile, true);
                });

                string[] dirs = Directory.GetDirectories(sourceDir);
                foreach (string dir in dirs)
                {
                    string dirName = Path.GetFileName(dir);
                    string destDir = Path.Combine(destinationDir, dirName);
                    Directory.CreateDirectory(destDir);
                    CopyDirectoryAsync(dir, destDir, numThreads).Wait();
                }
            });
        }
    }
}