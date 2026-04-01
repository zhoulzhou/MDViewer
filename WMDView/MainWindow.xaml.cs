﻿using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WMDView;

public partial class MainWindow : Window
{
    private string? _currentFilePath;
    private readonly MarkdownRenderer _renderer;
    private FileSystemWatcher? _fileWatcher;
    private readonly DispatcherTimer _debounceTimer;
    private const int DebounceDelay = 100;

    public MainWindow()
    {
        InitializeComponent();
        _renderer = new MarkdownRenderer();
        _debounceTimer = new DispatcherTimer();
        _debounceTimer.Interval = TimeSpan.FromMilliseconds(DebounceDelay);
        _debounceTimer.Tick += OnDebounceTimerTick;
        Closed += OnWindowClosed;
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        ApplyInitialTheme();
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Markdown 文件 (*.md)|*.md|所有文件 (*.*)|*.*",
            Title = "打开 Markdown 文件"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            LoadMarkdownFile(openFileDialog.FileName);
        }
    }

    private void LoadMarkdownFile(string filePath)
    {
        try
        {
            var markdownContent = File.ReadAllText(filePath);
            var document = _renderer.Render(markdownContent);
            DocumentViewer.Document = document;
            SetCurrentFile(filePath);
            SetupFileWatcher(filePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"无法打开文件: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SetupFileWatcher(string filePath)
    {
        _fileWatcher?.Dispose();

        var directory = Path.GetDirectoryName(filePath);
        var fileName = Path.GetFileName(filePath);

        if (string.IsNullOrEmpty(directory)) return;

        _fileWatcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _fileWatcher.Changed += OnFileChanged;
        _fileWatcher.Renamed += OnFileRenamed;
        _fileWatcher.Deleted += OnFileDeleted;
        _fileWatcher.Created += OnFileCreated;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _debounceTimer.Stop();
        _debounceTimer.Start();
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        if (_currentFilePath != null && e.OldFullPath == _currentFilePath)
        {
            SetCurrentFile(e.FullPath);
            SetupFileWatcher(e.FullPath);
        }
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        _debounceTimer.Stop();
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        if (_currentFilePath != null && e.FullPath == _currentFilePath)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
    }

    private void OnDebounceTimerTick(object? sender, EventArgs e)
    {
        _debounceTimer.Stop();

        if (!string.IsNullOrEmpty(_currentFilePath) && File.Exists(_currentFilePath))
        {
            try
            {
                ReloadMarkdownFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法重新加载文件: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ReloadMarkdownFile()
    {
        if (string.IsNullOrEmpty(_currentFilePath) || !File.Exists(_currentFilePath))
            return;

        try
        {
            var markdownContent = File.ReadAllText(_currentFilePath);
            var document = _renderer.Render(markdownContent);
            DocumentViewer.Document = document;
        }
        catch (IOException)
        {
        }
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        _fileWatcher?.Dispose();
        _fileWatcher = null;
        SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category == UserPreferenceCategory.General)
        {
            ApplyTheme();
        }
    }

    private void ApplyInitialTheme()
    {
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        var isDark = IsSystemDarkTheme();
        _renderer.SetTheme(isDark);
        
        Background = isDark 
            ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30)) 
            : System.Windows.Media.Brushes.White;
        
        if (!string.IsNullOrEmpty(_currentFilePath) && File.Exists(_currentFilePath))
        {
            ReloadMarkdownFile();
        }
    }

    private bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key == null) return false;
            var value = key.GetValue("AppsUseLightTheme");
            if (value is int intValue)
            {
                return intValue == 0;
            }
        }
        catch
        {
        }
        return false;
    }

    public void SetDocument(FlowDocument document)
    {
        DocumentViewer.Document = document;
    }

    public void SetCurrentFile(string filePath)
    {
        _currentFilePath = filePath;
        Title = $"WMDView - {filePath}";
    }

    public string? GetCurrentFilePath()
    {
        return _currentFilePath;
    }
}
