using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlayingCardDesigner.Models
{
    public class Session
    {
        public string SessionFileName { get; set; }
        public string SessionFilePath { get; set; }
        public string SessionContent { get; set; }
        public Design SessionDesign { get; set; }

        public Session()
        {
        }

        public bool Serialize()
        {
            if (SessionDesign == null)
            {
                return true;
            }
            try
            {
                string jsonString = JsonConvert.SerializeObject(SessionDesign, Formatting.Indented);
                File.WriteAllText(SessionFilePath, jsonString);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });

                return false;
            }
        }

        public static Design Deserialize(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Design>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });
                return null;
            }
        }

        public void Reload()
        {
            Open(SessionFilePath);
        }

        public void Draw()
        {
            try
            {
                var canvas = MainWindow.Window.DrawingCanvas;
                canvas.Children.Clear();
                MainWindow.StartPointFront = new System.Windows.Point(canvas.ActualWidth/4- Helper.MillimetersToPixels(SessionDesign.Width)/2, canvas.ActualHeight/ 2 - Helper.MillimetersToPixels(SessionDesign.Height)/ 2);
                MainWindow.StartPointBack = new System.Windows.Point((canvas.ActualWidth / 4)*3 - Helper.MillimetersToPixels(SessionDesign.Width) / 2, MainWindow.StartPointFront.Y);

                //Bounds
                //var outerBoundsFront = new Border() {Height = Helper.MillimetersToPixels(SessionDesign.Height), Width = Helper.MillimetersToPixels(SessionDesign.Width), BorderBrush= System.Windows.Media.Brushes.Red, BorderThickness = new Thickness(3), CornerRadius = new CornerRadius(SessionDesign.CornerRadius) };

                //Canvas.SetTop(outerBoundsFront, MainWindow.StartPointFront.Y);
                //Canvas.SetLeft(outerBoundsFront, MainWindow.StartPointFront.X);
                //canvas.Children.Add(outerBoundsFront);

                //var outerBoundsBack = new Border() { Height = Helper.MillimetersToPixels(SessionDesign.Height), Width = Helper.MillimetersToPixels(SessionDesign.Width), BorderBrush = System.Windows.Media.Brushes.Red, BorderThickness = new Thickness(3), CornerRadius = new CornerRadius(SessionDesign.CornerRadius) };
                //Canvas.SetTop(outerBoundsBack, MainWindow.StartPointBack.Y);
                //Canvas.SetLeft(outerBoundsBack, MainWindow.StartPointBack.X);
                //canvas.Children.Add(outerBoundsBack);

                var sessionDirectory = new FileInfo(SessionFilePath).Directory;
                Renderer.ImagePath = sessionDirectory + SessionDesign.ImageFilePath;
                Renderer.Colors = SessionDesign.Colors;

                //Front
                foreach (var element in SessionDesign.Front.Elemente)
                {
                    Renderer.DrawElement(canvas, element, MainWindow.StartPointFront);
                }

                //Back
                foreach (var element in SessionDesign.Back.Elemente)
                {
                    Renderer.DrawElement(canvas, element, MainWindow.StartPointBack);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });
            }
        }

        public bool Export()
        {
            try
            {
                var currentSession = MainWindowViewModel.Main.Session;
                if (currentSession == null)
                    return false;

                if (currentSession.SessionDesign.Daten != null && currentSession.SessionDesign.Daten.Rows.Count > 0)
                {
                    MainWindow.Window.Dispatcher.Invoke(() =>
                    {
                        MainWindow.SetStatusBar("Export gestartet");
                    });

                    var exportWindow = new ExportWindow();
                    exportWindow.Show();
                    exportWindow.DrawImages(currentSession.SessionDesign);
                    var success = true;

                    if (success)
                    {
                        MainWindow.Window.Dispatcher.Invoke(() =>
                        {
                            MainWindow.SetStatusBar("SessionDesign exportiert");
                        });
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });

                return false;
            }
        }

        //public bool Collage()
        //{
        //    try
        //    {
        //        var currentSession = MainWindowViewModel.Main.Session;
        //        if (currentSession == null)
        //            return false;

        //        var answer = MessageBox.Show($"Möchten Sie einen Export durchführen?", "Export", MessageBoxButton.YesNo);
        //        if (answer == MessageBoxResult.Yes)
        //            Export();
        //        return Renderer.Collage(currentSession.SessionFileName, currentSession.SessionDesign.Daten, currentSession.SessionDesign.Height, currentSession.SessionDesign.Width);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.Message);
        //        MainWindow.Window.Dispatcher.Invoke(() =>
        //        {
        //            MainWindow.SetStatusBar("Error: " + ex.Message);
        //        });

        //        return false;
        //    }
        //}

        public static bool ImportDataContext()
        {
            try
            {
                var currentSession = MainWindowViewModel.Main.Session;
                if (currentSession == null)
                    return false;

                var csvFile = Helper.GetFileFromFileDialog("Datenkontext", MainWindowViewModel.SessionsDirectory + @"\Data", "CSV Files(*.csv) | *.csv");

                currentSession.SessionDesign.Daten = Helper.GetDataFromCSV(csvFile);
                currentSession.Serialize();
                Session.Open(currentSession.SessionFilePath);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });

                return false;
            }
        }

        public static bool New(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Info: Dateiname darf nicht leer sein.");
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Info: Dateiname darf nicht leer sein.");
                });
                return false;
            }

            var filePath = MainWindowViewModel.SessionsDirectory + @"\" + fileName+ ".json";
            if(File.Exists(filePath))
            {
                Console.WriteLine("Info: Datei existiert bereits.");
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Info: Datei existiert bereits.");
                });
                return false;
            }


            try
            {
                //var fileName = Helper.GetNextFileName(MainWindowViewModel.SessionsDirectory + @"\NewPlayingCardDesign.json");
                var templateText = File.ReadAllText(MainWindowViewModel.SessionsDirectory + @"\Template.txt");
                templateText = templateText.Replace("%DesignName%", fileName);
                var newSession = new Session()
                {
                    SessionDesign = new Design(fileName),
                    SessionFileName = fileName,
                    SessionFilePath = filePath,
                    SessionContent = templateText
                };

                newSession.Serialize();
                Open(newSession.SessionFilePath);

                MainWindowViewModel.Main.Session = newSession;
                MainWindow.Window.ShowSession();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });

                return false;
            }
        }

        public static bool Open(string path = "")
        {
            try
            {
                var designPath = path;
                if (string.IsNullOrEmpty(path))
                    designPath = Helper.GetFileFromFileDialog("Spielkarten -SessionDesign", MainWindowViewModel.SessionsDirectory, "JSON Files(*.json) | *.json");

                if (string.IsNullOrEmpty(designPath))
                    return true;

                var fileInfo = new FileInfo(designPath);
                var fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
                var openedSession = new Session()
                {
                    SessionDesign = Session.Deserialize(designPath),
                    SessionFileName = fileName,
                    SessionFilePath = designPath,
                    SessionContent = File.ReadAllText(designPath)
                };

                MainWindowViewModel.Main.Session = openedSession;
                //MainWindow.UpdateColors();
                //MainWindow.UpdateElements();
                //MainWindow.UpdateKontexte();
                //Renderer.Render(openedSession.SessionDesign.Background, openedSession.SessionDesign.Element.ToList(), MainWindow.Window.Canvas_Design);

                var lastOpenedFile = @"\LastOpened.txt";
                File.WriteAllText(lastOpenedFile, MainWindowViewModel.Main.Session.SessionFilePath);

                MainWindow.Window.ShowSession();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });

                return false;
            }
        }
    }
}
