using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using LFRun.Utilities;
using LFRun.ViewModels;
using Microsoft.Win32;

namespace LFRun
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const uint WS_EX_CONTEXTHELP = 0x00000400;
        private const uint WS_MINIMIZEBOX = 0x00020000;
        private const uint WS_MAXIMIZEBOX = 0x00010000;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_CONTEXTHELP = 0xF180;

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        public MainWindow()
        {
            // Ensure data contracts
            RegistryUtilities.EnsureRegistryContract("SaveHistory", 1, RegistryValueKind.DWord);
            RegistryUtilities.EnsureRegistryContract("History", "[]", RegistryValueKind.String);

            DataContext = new MainWindowViewModel();
            Loaded += MainWindow_Loaded;
            InitializeComponent();
        }

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Left = 0;
            Top = SystemParameters.WorkArea.Height - ActualHeight;
            InputComboBox.Focus(); 
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            uint styles = GetWindowLong(hwnd, GWL_STYLE);
            styles &= 0xFFFFFFFF ^ (WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
            SetWindowLong(hwnd, GWL_STYLE, styles);
            styles = GetWindowLong(hwnd, GWL_EXSTYLE);
            styles |= WS_EX_CONTEXTHELP;
            SetWindowLong(hwnd, GWL_EXSTYLE, styles);
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HelpHook);
        }

        private IntPtr HelpHook(IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            if (msg == WM_SYSCOMMAND &&
                ((int)wParam & 0xFFF0) == SC_CONTEXTHELP)
            {
                // MessageBox.Show("help");
                MainWindowViewModel mwvm = (MainWindowViewModel)DataContext;
                ICommand helpCommand = mwvm.AboutCommand;
                if (helpCommand.CanExecute(null))
                    helpCommand.Execute(null);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt))
            {
                MainWindowViewModel mwvm = (MainWindowViewModel)DataContext;

                mwvm.ShowMenu = !mwvm.ShowMenu;
            }
        }

<<<<<<< HEAD
        private void MainMenu_Closed(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel mwvm = (MainWindowViewModel)DataContext;

            mwvm.ShowMenu = !mwvm.ShowMenu;
=======
        private void MenuItem_Hide(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel mwvm = (MainWindowViewModel)DataContext;

            mwvm.ShowMenu = false;
>>>>>>> 176c763e35ce54a153b9c7b16dbf2de56569321a
        }
    }
}
