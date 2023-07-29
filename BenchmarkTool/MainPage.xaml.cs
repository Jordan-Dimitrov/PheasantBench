using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace BenchmarkTool
{
    public partial class MainPage : ContentPage
    {
        private int _Score;
        private List<Task> _Tasks;
        private bool _StopCheck;
        private int _NumCores;

        public MainPage()
        {
            InitializeComponent();
            _Score = 0;
            _NumCores = Environment.ProcessorCount;
            CpuCoresLabel.Text = _NumCores.ToString();
            BenchmarkScoreLabel.Text = _Score.ToString();

        }

        private async void OnStartBenchmarkClicked(object sender, EventArgs e)
        {
            StartBenchmarkButton.IsEnabled = false;
            ActivityIndicator.IsRunning = true;
            ActivityIndicator.IsVisible = true;

            _Score = 0;
            _Tasks = new List<Task>();
            _StopCheck = false;

            for (int i = 0; i < _NumCores; i++)
            {
                _Tasks.Add(StressCoreAsync());
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            while (!_StopCheck && stopwatch.Elapsed < TimeSpan.FromMinutes(1))
            {
                await Task.Delay(100);
            }

            _StopCheck = true;

            await Task.WhenAll(_Tasks);

            BenchmarkScoreLabel.Text = _Score.ToString();

            StartBenchmarkButton.IsEnabled = true;
            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
        }

        private async Task StressCoreAsync()
        {
            await Task.Run(() =>
            {
                while (!_StopCheck)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        double result = Math.Sqrt(i) * Math.Sin(i);
                    }
                    _Score++;
                }
            });
        }
    }
}
