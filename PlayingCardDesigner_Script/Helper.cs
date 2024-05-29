using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using PlayingCardDesigner.Models;

namespace PlayingCardDesigner
{
    public static class Helper
    {
        public static double MillimetersToPixels(double value)
        {
            return value / 2;
        }

        public static double PixelsToMillimeters(double value)
        {
            return value * 2;
        }

        public static SolidColorBrush GetColorFromHex(string hex)
        {
            try
            {
                if (string.IsNullOrEmpty(hex))
                    return System.Windows.Media.Brushes.White;
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
                //return (SolidColorBrush)new BrushConverter().ConvertFromString(hex);
                return new SolidColorBrush(color);
            }
            catch(Exception)
            {
                return System.Windows.Media.Brushes.White;
            }
        }

        public static string GetFileFromFileDialog(string title, string initialDirectory, string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = initialDirectory,
                Filter = filter
            };
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;
            else return "";
        }

        public static string FindDropboxFolder()
        {
            var dropboxDirectory = "";
            if (Directory.Exists(@"D:\Shared\Dropbox"))
                dropboxDirectory = @"D:\Shared\Dropbox";
            else if (Directory.Exists(@"C:\Users\Leo\Dropbox"))
                dropboxDirectory = @"C:\Users\Leo\Dropbox";
            else if (Directory.Exists(@"C:\Users\glanzer\Dropbox"))
                dropboxDirectory = @"C:\Users\glanzer\Dropbox";

            return dropboxDirectory;
        }

        public static string GetNextFileName(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var fileName = fileInfo.Name.Replace(fileInfo.Extension,"");
            if (!File.Exists(filePath))
                return fileName;
            else
            {
                var counter = 1;
                while (true)
                {
                    fileInfo = new FileInfo(filePath);
                    var newFileName = fileInfo.Name.Replace(fileInfo.Extension,"") + "-" + counter;
                    var newPath = fileInfo.Directory + @"\" + newFileName + fileInfo.Extension;
                    if (!File.Exists(newPath))
                    {
                        return newFileName;
                    }
                    else counter++;
                }
            }
        }

        //public static string GetNextElementName(Session session, string currentName)
        //{
        //    var newName = currentName;
        //    if(!session.SessionDesign.Element.Any())
        //        return newName;

        //    var counter = 1;
        //    while (true)
        //    {
        //        var foundElement = session.SessionDesign.Element.ToList().Find(element => element.Name == newName);
        //        if(foundElement == null)
        //            return newName;
        //        else
        //        {
        //            newName = currentName + "-" + counter;
        //            counter++;
        //        }
        //    }
        //}

        public static ObservableCollection<string> GetHeaderFromCSV(string csvFile)
        {
            var headers = new ObservableCollection<string>();

            if (string.IsNullOrEmpty(csvFile))
                return headers;

            var lines = File.ReadAllLines(csvFile).ToList();
            if(lines.Any())
            {
                var firstLine = lines.FirstOrDefault().Replace("\"", "");
                headers = new ObservableCollection<string>(firstLine.Split(new char[] { ';' }).ToList());
            }

            return headers;
        }

        public static Daten GetDataFromCSV(string csvFile)
        {
            if (string.IsNullOrEmpty(csvFile))
                return null;

            var data = new Daten() { FilePath = csvFile };

            var lines = File.ReadAllLines(csvFile);
            string[] headers = lines[0].Split(';');
            foreach (string header in headers)
            {
                data.Columns.Add(header);
            }

            data.Rows = new List<List<Cell>>();

            for(int i = 1; i < lines.Count(); i++)
            {
                string[] cells = lines[i].Split(';');
                var row = new List<Cell>();
                for (int j = 0; j < headers.Length; j++)
                {
                    var cell = new Cell() {Column = headers[j], Value = cells[j] };
                    row.Add(cell);
                }
                data.Rows.Add(row);
            }

            data.ColumnCount = data.Columns.Count;
            data.RowCount = data.Rows.Count;

            return data;
        }

        //public static bool IsDarkColor(SolidColorBrush color)
        //{
        //    var isDark = false;

        //    if (color == null)
        //        return isDark;

        //    MainWindow.Window.Dispatcher.Invoke(() =>
        //    {
        //        var col = ColorTranslator.FromHtml(color.ToString());
        //        if (col.R * 0.2126 + col.G * 0.7152 + col.B * 0.0722 < 255 / 2)
        //        {
        //            isDark =  true;
        //        }
        //        else
        //        {
        //            isDark = false;
        //        }
        //    });

        //    return isDark;
        //}

        public static string TransformMathFormular(string formular)
        {
            var result = new List<string>();

            if(formular.Contains("->+"))
            {
                var parts = formular.Split(new[] { "->+" }, StringSplitOptions.RemoveEmptyEntries);
                var basis = Convert.ToDouble(parts[0]);
                var addition = Convert.ToDouble(parts[1]);

                result.Add(basis.ToString());
                var temp = basis;
                for (int i = 0; i < 5; i++)
                {
                    temp += basis;
                    result.Add(temp.ToString());
                }

                return string.Join("\n", result);
            }
            else return formular;
        }

        //public static List<Element> GetListAsValue(ObservableCollection<Element> elements)
        //{
        //    var newElements = new List<Element>();
        //    foreach (var element in elements)
        //    {
        //        newElements.Add(GetElementAsValue(element));
        //    }
        //    return newElements;
        //}

        //public static Element GetElementAsValue(Element element)
        //{
        //    var newElement = new Element();

        //    var props = typeof(Element).GetProperties().ToList();
        //    foreach (var prop in props)
        //    {
        //        try
        //        {
        //            prop.SetValue(newElement, prop.GetValue(element));
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error: " + ex.Message);
        //        }
        //    }

        //    return newElement;
        //}

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern int MessageBoxTimeout(IntPtr hwnd, String text, String title, uint type, Int16 wLanguageId, Int32 milliseconds);
    }
}
