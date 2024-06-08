using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace WpfApp_08_06
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private long _totalBytes;
        private long _copiedBytes;
        private bool _isPaused;
        private readonly object _pauseLock = new object();

        public MainWindow()
        {
            InitializeComponent();
            ThreadCountComboBox.SelectedIndex = 0; // Default to 1 thread
        }

        private void BrowseSourceFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SourceFilePath.Text = openFileDialog.FileName;
            }
        }

        private void BrowseDestinationFolder(object sender, RoutedEventArgs e)
        {
            var folderDialog = new Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() != Forms.DialogResult.OK)
            {
                return;
            }
            DestinationFolderPath.Text = folderDialog.SelectedPath;
        }

        private async void StartCopy(object sender, RoutedEventArgs e)
        {
            string sourceFile = SourceFilePath.Text;
            string destinationFolder = DestinationFolderPath.Text;

            if (string.IsNullOrEmpty(sourceFile) || string.IsNullOrEmpty(destinationFolder))
            {
                MessageBox.Show("Please select both source file and destination folder.");
                return;
            }

            if (ThreadCountComboBox.SelectedItem == null || !int.TryParse((ThreadCountComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(), out int threadCount))
            {
                MessageBox.Show("Please select a valid number of threads.");
                return;
            }

            string destinationFile = System.IO.Path.Combine(destinationFolder, System.IO.Path.GetFileName(sourceFile));

            _cancellationTokenSource = new CancellationTokenSource();
            _totalBytes = new FileInfo(sourceFile).Length;
            _copiedBytes = 0;
            _isPaused = false;
            CopyProgressBar.Value = 0;
            StatusTextBlock.Text = "Copying...";

            await Task.Run(() => CopyFile(sourceFile, destinationFile, threadCount, _cancellationTokenSource.Token));

            if (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                StatusTextBlock.Text = "Copy Completed.";
            }
        }

        private void PauseCopy(object sender, RoutedEventArgs e)
        {
            _isPaused = true;
            StatusTextBlock.Text = "Paused";
        }

        private void StopCopy(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            StatusTextBlock.Text = "Stopped";
        }

        private void CopyFile(string sourceFile, string destinationFile, int threadCount, CancellationToken cancellationToken)
        {
            const int bufferSize = 1024 * 1024; // 1MB buffer
            byte[] buffer = new byte[bufferSize];
            long fileSize = new FileInfo(sourceFile).Length;
            long partSize = fileSize / threadCount;

            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
            {
                ParallelOptions parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = threadCount,
                    CancellationToken = cancellationToken
                };

                Parallel.For(0, threadCount, parallelOptions, i =>
                {
                    long partStart = i * partSize;
                    long partEnd = (i == threadCount - 1) ? fileSize : partStart + partSize;

                    byte[] localBuffer = new byte[bufferSize];
                    sourceStream.Seek(partStart, SeekOrigin.Begin);

                    while (partStart < partEnd && !cancellationToken.IsCancellationRequested)
                    {
                        if (_isPaused)
                        {
                            lock (_pauseLock)
                            {
                                Monitor.Wait(_pauseLock);
                            }
                        }

                        int bytesToRead = (int)Math.Min(bufferSize, partEnd - partStart);
                        int bytesRead = sourceStream.Read(localBuffer, 0, bytesToRead);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        lock (destinationStream)
                        {
                            destinationStream.Seek(partStart, SeekOrigin.Begin);
                            destinationStream.Write(localBuffer, 0, bytesRead);
                        }

                        partStart += bytesRead;
                        Interlocked.Add(ref _copiedBytes, bytesRead);

                        Dispatcher.Invoke(() =>
                        {
                            CopyProgressBar.Value = (double)_copiedBytes / _totalBytes * 100;
                        });
                    }
                });
            }
        }
    }
}
