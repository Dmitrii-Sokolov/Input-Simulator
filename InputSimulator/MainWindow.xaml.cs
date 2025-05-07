using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace InputSimulator
{
  public partial class MainWindow : Window
  {
    private const int SecondsToRepeat = 10;
    int state = 0;
    char[] keys = new char[] {'a', 'b'};

    // Import functions from user32.dll
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    private readonly DispatcherTimer _timer;

    public MainWindow()
    {
      InitializeComponent();

      // Timer will tick every second
      _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(SecondsToRepeat) };
      _timer.Tick += UpdateActiveWindowInfo;
      _timer.Start();
    }

    private void UpdateActiveWindowInfo(object sender, EventArgs e)
    {
      // Get handle of the active window
      IntPtr hWnd = GetForegroundWindow();

      // Get the window title
      var sb = new StringBuilder(256);
      GetWindowText(hWnd, sb, sb.Capacity);
      string title = sb.ToString();

      // Get process ID (PID) and thread ID (TID)
      uint pid;
      uint tid = GetWindowThreadProcessId(hWnd, out pid);

      // Update the UI
      TitleText.Text = $"Title: {title}";
      HandleText.Text = $"HWND: 0x{hWnd.ToInt64():X}";
      ProcessText.Text = $"PID: {pid}, TID: {tid}";

      KeyboardEmulator.SendKey((ushort)(0x41 + keys[state] - 'a'));

      state = (state + 1) % keys.Length;
    }
  }
}