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

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            string text = textInput.Text;
            try
            {
                TextAnalysisResult result = await AnalyzeTextAsync(text);
                vowelCountLabel.Content = result.VowelCount;
                consonantCountLabel.Content = result.ConsonantCount;
                totalCharacterCountLabel.Content = result.TotalCharacterCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Task<TextAnalysisResult> AnalyzeTextAsync(string text)
        {
            return Task.Run(() =>
            {
                int vowelCount = text.Count(c => "aeiouAEIOU".Contains(c));
                int consonantCount = text.Count(c => char.IsLetter(c) && !"aeiouAEIOU".Contains(c));
                int totalCharacterCount = text.Length;

                return new TextAnalysisResult
                {
                    VowelCount = vowelCount,
                    ConsonantCount = consonantCount,
                    TotalCharacterCount = totalCharacterCount
                };
            });
        }
    }

    public class TextAnalysisResult
    {
        public int VowelCount { get; set; }
        public int ConsonantCount { get; set; }
        public int TotalCharacterCount { get; set; }
    }
}