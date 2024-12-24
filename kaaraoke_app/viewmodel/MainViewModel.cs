//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Windows.Input;
//using System.Windows.Threading;
//using NAudio.Wave;
//using System.IO;

//public class MainWindowViewModel : INotifyPropertyChanged
//{
//    private string _audioFilePath;
//    private string _lrcFilePath;
//    private IWavePlayer _waveOut;
//    private AudioFileReader _audioFileReader;
//    private ObservableCollection<Track> _tracks;
//    private Track _selectedTrack;
//    private List<LRCLine> _lrcLines;
//    private DispatcherTimer _timer;
//    private double _currentPosition;

//    public MainWindowViewModel()
//    {
//        LoadAudioCommand = new RelayCommand(LoadAudio);
//        LoadLRCCommand = new RelayCommand(LoadLRC);
//        PlayCommand = new RelayCommand(PlayAudio);
//        StopCommand = new RelayCommand(StopAudio);
//        PreviousCommand = new RelayCommand(PreviousTrack);
//        NextCommand = new RelayCommand(NextTrack);
//        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
//        _timer.Tick += Timer_Tick;
//        LoadTracks();
//    }

//    public ObservableCollection<Track> Tracks
//    {
//        get { return _tracks; }
//        set { _tracks = value; OnPropertyChanged("Tracks"); }
//    }

//    public Track SelectedTrack
//    {
//        get { return _selectedTrack; }
//        set
//        {
//            _selectedTrack = value;
//            if (value != null)
//            {
//                AudioFilePath = value.AudioFilePath;
//                LrcFilePath = value.LrcFilePath;
//                PlayAudio();
//            }
//            OnPropertyChanged("SelectedTrack");
//        }
//    }

//    public string AudioFilePath
//    {
//        get { return _audioFilePath; }
//        set { _audioFilePath = value; OnPropertyChanged("AudioFilePath"); }
//    }

//    public string LrcFilePath
//    {
//        get { return _lrcFilePath; }
//        set { _lrcFilePath = value; OnPropertyChanged("LrcFilePath"); }
//    }

//    public ICommand LoadAudioCommand { get; set; }
//    public ICommand LoadLRCCommand { get; set; }
//    public ICommand PlayCommand { get; set; }
//    public ICommand StopCommand { get; set; }
//    public ICommand PreviousCommand { get; set; }
//    public ICommand NextCommand { get; set; }

   

//    private void LoadTracks()
//    {
//        using (var context = new MusicDbContext())
//        {
//           // Tracks = new ObservableCollection<Track>(context.Tracks.Include(t => t.Album).ToList());
//        }
//    }

//    private void LoadAudio()
//    {
//        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
//        openFileDialog.Filter = "Audio files (*.mp3, *.wav)|*.mp3;*.wav";
//        if (openFileDialog.ShowDialog() == true)
//        {
//            AudioFilePath = openFileDialog.FileName;
//            PlayAudio();
//        }
//    }

//    private void LoadLRC()
//    {
//        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
//        openFileDialog.Filter = "LRC files (*.lrc)|*.lrc";
//        if (openFileDialog.ShowDialog() == true)
//        {
//            LrcFilePath = openFileDialog.FileName;
//            _lrcLines = ParseLRC(LrcFilePath);
//        }
//    }

//    private void PlayAudio()
//    {
//        if (_waveOut != null)
//        {
//            _waveOut.Stop();
//            _waveOut.Dispose();
//            _waveOut = null;
//            _audioFileReader.Dispose();
//            _audioFileReader = null;
//        }

//        _waveOut = new WaveOut();
//        _audioFileReader = new AudioFileReader(AudioFilePath);
//        _waveOut.Init(_audioFileReader);
//        _waveOut.Play();
//        _timer.Start();
//    }

//    private void StopAudio()
//    {
//        if (_waveOut != null)
//        {
//            _waveOut.Stop();
//            _waveOut.Dispose();
//            _waveOut = null;
//            _audioFileReader.Dispose();
//            _audioFileReader = null;
//            _timer.Stop();
//        }
//    }

//    private void PreviousTrack()
//    {
//        // Implement previous track logic
//    }

//    private void NextTrack()
//    {
//        // Implement next track logic
//    }

//    private void Timer_Tick(object sender, EventArgs e)
//    {
//        if (_audioFileReader != null)
//        {
//            _currentPosition = _audioFileReader.CurrentTime.TotalSeconds;
//            OnPropertyChanged("CurrentPosition");

//            foreach (var line in _lrcLines)
//            {
//                if (line.Time <= _currentPosition && _currentPosition < line.Time + 1)
//                {
//                    Lyrics = line.Text;
//                    break;
//                }
//            }

//            if (_audioFileReader.CurrentTime >= _audioFileReader.TotalTime)
//            {
//                _timer.Stop();
//                StopAudio();
//            }
//        }
//    }

//    public string Lyrics { get; set; }

//    public double CurrentPosition
//    {
//        get { return _currentPosition; }
//        set
//        {
//            _currentPosition = value;
//            if (_audioFileReader != null)
//            {
//                _audioFileReader.CurrentTime = TimeSpan.FromSeconds(value);
//            }
//            OnPropertyChanged("CurrentPosition");
//        }
//    }

//    public event PropertyChangedEventHandler PropertyChanged;

//    protected virtual void OnPropertyChanged(string propertyName)
//    {
//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }

//    private List<LRCLine> ParseLRC(string filePath)
//    {
//        List<LRCLine> lines = new List<LRCLine>();

//        string[] fileLines = File.ReadAllLines(filePath);
//        foreach (string line in fileLines)
//        {
//            if (line.StartsWith("["))
//            {
//                int startIndex = line.IndexOf('[');
//                int endIndex = line.IndexOf(']');
//                string timeTag = line.Substring(startIndex + 1, endIndex - startIndex - 1);
//                string[] timeParts = timeTag.Split(':');
//                string[] secondsParts = timeParts[1].Split('.');

//                int minutes = int.Parse(timeParts[0]);
//                int seconds = int.Parse(secondsParts[0]);
//                int milliseconds = int.Parse(secondsParts[1]);

//                int totalTime = minutes * 60 + seconds + milliseconds / 1000;

//                string text = line.Substring(endIndex + 1).Trim();

//                lines.Add(new LRCLine { Time = totalTime, Text = text });
//            }
//        }
//        return lines;
//    }
//}

//public class RelayCommand : ICommand
//{
//    private readonly Action _execute;
//    private readonly Func<bool> _canExecute;

//    public RelayCommand(Action execute, Func<bool> canExecute = null)
//    {
//        _execute = execute;
//        _canExecute = canExecute;
//    }

//    public bool CanExecute(object parameter)
//    {
//        return _canExecute?.Invoke() ?? true;
//    }

//    public void Execute(object parameter)
//    {
//        _execute();
//    }

//    public event EventHandler CanExecuteChanged
//    {
//        add { CommandManager.RequerySuggested += value; }
//        remove { CommandManager.RequerySuggested -= value; }
//    }
//}
