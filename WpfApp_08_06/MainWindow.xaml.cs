using System;
using System.IO;
using System.Text.RegularExpressions;
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
            string filePath = filePathTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(word) || string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("Please enter a word and file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int wordCount = await SearchWordAsync(word, filePath);
            resultTextBlock.Text = $"The word '{word}' was found {wordCount} times in the file.";
        }

        private async Task<int> SearchWordAsync(string word, string filePath)
        {
            int count = 0;
            await Task.Run(() =>
            {
                string content = File.ReadAllText(filePath);
                count = Regex.Matches(content, @"\b" + word + @"\b", RegexOptions.IgnoreCase).Count;
            });
            return count;
        }
    }
}