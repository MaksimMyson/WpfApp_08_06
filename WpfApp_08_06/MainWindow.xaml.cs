using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp_08_06
{
    public partial class MainWindow : Window
    {
        private List<Horse> horses = new List<Horse>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeHorses();
        }

        private void InitializeHorses()
        {
            horses.Clear();
            for (int i = 0; i < 5; i++)
            {
                horses.Add(new Horse { HorseName = $"Horse {i + 1}", Speed = 0 });
            }
            horseRaceContainer.ItemsSource = horses;
        }

        private async void StartRaceButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeHorses();
            StartRaceButton.IsEnabled = false;

            await Task.Run(() =>
            {
                foreach (var horse in horses)
                {
                    horse.Run();
                }

                while (!AreAllHorsesFinished())
                {
                    foreach (var horse in horses)
                    {
                        horse.Advance(new Random().Next(1, 11));
                    }

                    System.Threading.Thread.Sleep(100);
                }
            });

            StartRaceButton.IsEnabled = true;
            ShowResults();
        }

        private bool AreAllHorsesFinished()
        {
            foreach (var horse in horses)
            {
                if (!horse.Finished)
                    return false;
            }
            return true;
        }

        private void ShowResults()
        {
            resultsGrid.ItemsSource = horses;

            // Find winner
            Horse winner = null;
            foreach (var horse in horses)
            {
                if (winner == null || horse.FinishTime < winner.FinishTime)
                {
                    winner = horse;
                }
            }

            // Display winner
            WinnerTextBlock.Text = $"Winner: {winner.HorseName}, Finish Time: {winner.FinishTime}";
            WinnerTextBlock.Visibility = Visibility.Visible;
        }
    }

    public class Horse : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int speed;
        public string HorseName { get; set; }
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                OnPropertyChanged("Speed");
            }
        }

        public bool Finished { get; private set; }
        public DateTime FinishTime { get; private set; }

        public void Run()
        {
            Finished = false;
            Speed = 0;
        }

        public void Advance(int speedIncrease)
        {
            if (!Finished)
            {
                Speed += speedIncrease;
                if (Speed >= 100)
                {
                    Finished = true;
                    FinishTime = DateTime.Now;
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
