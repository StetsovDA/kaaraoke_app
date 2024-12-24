using NAudio.Wave;
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
using System.Windows.Threading;
using NAudio.FileFormats;
using NAudio.CoreAudioApi;
using NAudio;
using System.Media;
using NAudio.Wave.Compression;
using System.ComponentModel;
using System;

namespace kaaraoke_app
{
    public partial class MainWindow : Window
    {
        private string audioFilePath;
        private string lrcFilePath;
        private DispatcherTimer timer;
        private List<LRCLine> lrcLines;
        private IWavePlayer waveOut;
        private AudioFileReader audioFileReader;
        private List<string> audioFiles = new List<string>();
        private int currentAudioIndex = 0;
        public virtual AudioFile AudioFile { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            timer.Tick += Timer_Tick;
        }

        private void loadAudioButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.mp3, *.wav)|*.mp3;*.wav";
            if (openFileDialog.ShowDialog() == true)
            {
                audioFilePath = openFileDialog.FileName;
                using (var context = new KaraokeContext())
                {
                    var audioFile = new AudioFile { FilePath = audioFilePath, Title = System.IO.Path.GetFileName(audioFilePath) };
                    context.AudioFiles.Add(audioFile);
                    context.SaveChanges();
                }
                audioFiles.Add(audioFilePath);
            }
        }
        private void LoadAudioFiles()
        {
            using (var context = new KaraokeContext())
            {
                audioFiles = context.AudioFiles.Select(a => a.FilePath).ToList();
                
            }
        }
        private void loadLRCButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "LRC files (*.lrc)|*.lrc";
            if (openFileDialog.ShowDialog() == true)
            {
                lrcFilePath = openFileDialog.FileName;
                lrcLines = ParseLRC(lrcFilePath);
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {

            if (audioFilePath != null && lrcFilePath != null)
            {
                if (waveOut != null && waveOut.PlaybackState == PlaybackState.Paused)
                {
                    waveOut.Play();
                }
                else if (waveOut != null && waveOut.PlaybackState == PlaybackState.Stopped)
                {
                    PlayAudioFromCurrentPosition();
                }
                else
                {
                    PlayAudio();
                }
                timer.Start();
            }
        }

        private void PlayAudioFromCurrentPosition()
        {
            if (waveOut != null && waveOut.PlaybackState != PlaybackState.Stopped)
            {
                waveOut.Stop();
            }
            else
            {
                if (waveOut != null)
                {
                    waveOut.Dispose();
                    waveOut = null;
                }
                if (audioFileReader != null)
                {
                    audioFileReader.Dispose();
                    audioFileReader = null;
                }
            }

            waveOut = new WaveOut();
            audioFileReader = new AudioFileReader(audioFilePath);
            waveOut.Init(audioFileReader);
            waveOut.Play();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            StopAudio();
        }

        private void PlayAudio()
        {
            if (waveOut != null && waveOut.PlaybackState != PlaybackState.Stopped)
            {
                waveOut.Stop();
            }
            else
            {
                if (waveOut != null)
                {
                    waveOut.Dispose();
                    waveOut = null;
                }
                if (audioFileReader != null)
                {
                    audioFileReader.Dispose();
                    audioFileReader = null;
                }
            }

            waveOut = new WaveOut();
            audioFileReader = new AudioFileReader(audioFilePath);
            waveOut.Init(audioFileReader);
            waveOut.Play();
            lyricsTextBox.Text = "";

            audioSlider.Maximum = audioFileReader.TotalTime.TotalSeconds;
            timeTextBlock.Text = $"0:00 / {audioFileReader.TotalTime}";
        }

        private void StopAudio()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
                audioFileReader.Dispose();
                audioFileReader = null;
                timer.Stop();
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentAudioIndex > 0)
            {
                currentAudioIndex--;
                audioFilePath = audioFiles[currentAudioIndex];
                PlayAudio();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentAudioIndex < audioFiles.Count - 1)
            {
                currentAudioIndex++;
                audioFilePath = audioFiles[currentAudioIndex];
                PlayAudio();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (audioFileReader != null)
            {
                double currentPosition = audioFileReader.CurrentTime.TotalSeconds;
                if (audioSlider.Value != currentPosition)
                {
                    audioSlider.Value = currentPosition;
                }

                foreach (var line in lrcLines)
                {
                    if (line.Time <= currentPosition && currentPosition < line.Time + 1)
                    {
                        lyricsTextBox.Text = line.Text;
                        break;
                    }
                }

                timeTextBlock.Text = $"{audioFileReader.CurrentTime:mm\\:ss} / {audioFileReader.TotalTime:mm\\:ss}";

                if (audioFileReader.CurrentTime >= audioFileReader.TotalTime)
                {
                    timer.Stop();
                    StopAudio();
                }
            }
        }

        private void audioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audioFileReader != null && e.NewValue >= 0 && e.NewValue <= audioSlider.Maximum)
            {
                audioFileReader.CurrentTime = TimeSpan.FromSeconds(audioSlider.Value);
            }
        }

        private List<LRCLine> ParseLRC(string filePath)
        {
            List<LRCLine> lines = new List<LRCLine>();

            string[] fileLines = File.ReadAllLines(filePath);
            foreach (string line in fileLines)
            {
                if (line.StartsWith("["))
                {
                    int startIndex = line.IndexOf('[');
                    int endIndex = line.IndexOf(']');
                    string timeTag = line.Substring(startIndex + 1, endIndex - startIndex - 1);
                    string[] timeParts = timeTag.Split(':');
                    string[] secondsParts = timeParts[1].Split('.');

                    int minutes = int.Parse(timeParts[0]);
                    int seconds = int.Parse(secondsParts[0]);
                    int milliseconds = int.Parse(secondsParts[1]);

                    int totalTime = minutes * 60 + seconds + milliseconds / 1000;

                    string text = line.Substring(endIndex + 1).Trim();

                    lines.Add(new LRCLine { Time = totalTime, Text = text });
                }
            }
            return lines;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadMediaFromDatabase(1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadMediaFromDatabase(2);
        }

        private void LoadMediaFromDatabase(int buttonId)
        {
            using (var context = new KaraokeContext())
            {
                var audioFile = context.AudioFiles.FirstOrDefault(a => a.Id == buttonId);
                var lrcFile = context.LRCFiles.FirstOrDefault(l => l.AudioFileId == buttonId); 

                if (audioFile != null)
                {
                    audioFilePath = audioFile.FilePath;
                }

                if (lrcFile != null)
                {
                    lrcFilePath = lrcFile.FilePath;
                }

                MessageBox.Show($"Loaded Audio: {audioFilePath}\nLoaded LRC: {lrcFilePath}");
            }
        }
    }

    public class LRCLine
    {
        public int Time { get; set; }
        public string Text { get; set; }
    }
} 
