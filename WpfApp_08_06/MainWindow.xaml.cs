using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp_08_06
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string word = wordTextBox.Text.Trim();
            string directoryPath = directoryPathTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(word) || string.IsNullOrWhiteSpace(directoryPath))
            {
                MessageBox.Show("Please enter a word and directory path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show("Directory does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            resultListBox.Items.Clear();
            await SearchWordInDirectoryAsync(word, directoryPath);
        }

        private async Task SearchWordInDirectoryAsync(string word, string directoryPath)
        {
            await Task.Run(() =>
            {
                string[] files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    try
                    {
                        string content = File.ReadAllText(file);
                        int count = content.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Where(w => string.Equals(w, word, StringComparison.OrdinalIgnoreCase)).Count();

                        if (count > 0)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                resultListBox.Items.Add($"File Name: {System.IO.Path.GetFileName(file)}");
                                resultListBox.Items.Add($"File Path: {file}");
                                resultListBox.Items.Add($"Word Count: {count}");
                                resultListBox.Items.Add(new string('-', 30));
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions while reading files
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            resultListBox.Items.Add($"Error reading file: {System.IO.Path.GetFileName(file)} - {ex.Message}");
                            resultListBox.Items.Add(new string('-', 30));
                        });
                    }
                }
            });
        }
    }
}
