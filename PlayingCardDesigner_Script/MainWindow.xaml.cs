using Newtonsoft.Json;
using PlayingCardDesigner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace PlayingCardDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Window { get; set; }

        public static SolidColorBrush White { get; set; } = (SolidColorBrush) new BrushConverter().ConvertFrom("#FFFFFF");
        public static SolidColorBrush Black { get; set; } = (SolidColorBrush) new BrushConverter().ConvertFrom("#000000");

        public static Point StartPointFront { get; set; }
        public static Point StartPointBack { get; set; }

        public static string LastOpened { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            Window = this;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;


            var lastOpenedFile = @"\LastOpened.txt";
            if(File.Exists(lastOpenedFile))
                LastOpened = File.ReadAllText(lastOpenedFile);
            if (!string.IsNullOrEmpty(LastOpened))
            {
                Session.Open(LastOpened);
            }
        }

        public void ShowSession()
        {
            this.BTN_Export.IsEnabled = true;
            this.Grid_Session.Visibility = Visibility.Visible;
        }

        public static void SetStatusBar(string message)
        {
            Window.LB_Statusbar.Content = message;
        }

        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            Session.New(TB_NewItem.Text);
            TB_NewItem.Text = "";
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            Session.Open();
        }

        //private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        //{
        //    var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
        //    if(currentSession != null)
        //        currentSession.Save();

        //    Application.Current.Shutdown();
        //}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.R)
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                currentSession.Reload();

                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    SetStatusBar("Änderungen gespeichert");
                });
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.E)
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                //currentSession.Export();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
                //currentSession.Collage();
            }
        }

        private void BTN_Export_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Export();
        }

        private void BTN_Collage_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            //currentSession.Collage();
        }

        private void BTN_Import_Click(object sender, RoutedEventArgs e)
        {
            Session.ImportDataContext();
        }

        private void BTN_Draw_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Draw();
        }

        private void TB_NewItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(TB_NewItem.Text))
                BTN_Menu_New.IsEnabled = false;
            else BTN_Menu_New.IsEnabled=true;
        }

        private void BTN_Reload_Click(object sender, RoutedEventArgs e)
        {
            var currentSession = ((MainWindowViewModel)(this.DataContext)).Session;
            currentSession.Reload();
        }
    }
}
