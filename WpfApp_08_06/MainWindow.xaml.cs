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

        private void StartThreads(object sender, RoutedEventArgs e)
        {
            Thread numberThread = new Thread(GenerateNumbers);
            Thread letterThread = new Thread(GenerateLetters);
            Thread symbolThread = new Thread(GenerateSymbols);

            numberThread.Priority = GetSelectedPriority(NumberThreadPriority);
            letterThread.Priority = GetSelectedPriority(LetterThreadPriority);
            symbolThread.Priority = GetSelectedPriority(SymbolThreadPriority);

            numberThread.Start();
            letterThread.Start();
            symbolThread.Start();
        }

        private ThreadPriority GetSelectedPriority(System.Windows.Controls.ComboBox comboBox)
        {
            switch (comboBox.SelectedIndex)
            {
                case 0: return ThreadPriority.Lowest;
                case 1: return ThreadPriority.BelowNormal;
                case 2: return ThreadPriority.Normal;
                case 3: return ThreadPriority.AboveNormal;
                case 4: return ThreadPriority.Highest;
                default: return ThreadPriority.Normal;
            }
        }

        private void GenerateNumbers()
        {
            for (int i = 0; i < 100; i++)
            {
                AppendText($"Number: {i}");
                Thread.Sleep(100);
            }
        }

        private void GenerateLetters()
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                AppendText($"Letter: {c}");
                Thread.Sleep(100);
            }
        }

        private void GenerateSymbols()
        {
            string symbols = "!@#$%^&*()";
            foreach (char c in symbols)
            {
                AppendText($"Symbol: {c}");
                Thread.Sleep(100);
            }
        }

        private void AppendText(string text)
        {
            Dispatcher.Invoke(() =>
            {
                OutputTextBox.AppendText(text + Environment.NewLine);
                OutputTextBox.ScrollToEnd();
            });
        }
    }
}