using PlayingCardDesigner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlayingCardDesigner
{
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        public List<Image> Exports { get; set; }

        public ExportWindow()
        {
            InitializeComponent();
            Exports = new List<Image>();
        }

        public void DrawImages(Design design)
        {
            
            ExportCanvas.Height = design.Height;
            ExportCanvas.Width = design.Width*2+2;

            this.Height= Helper.MillimetersToPixels(ExportCanvas.Height)+100;
            this.Width= Helper.MillimetersToPixels(ExportCanvas.Width)+50;

            var index = 0;
            foreach (var row in design.Daten.Rows)
            {
                ExportCanvas.Children.Clear();

                //Front
                foreach (var element in design.Front.Elemente)
                {
                    Renderer.DrawElement(ExportCanvas, element, new Point(0,0), index);
                }

                //Back
                foreach (var element in design.Back.Elemente)
                {
                    Renderer.DrawElement(ExportCanvas, element, new Point(Helper.MillimetersToPixels(design.Width)+2, 0), index);
                }

                var nameValue = design.Daten.Rows[index].Find(cell=> cell.Column == "Name").Value;

                try
                { 
                    MainWindow.Window.Dispatcher.Invoke(() =>
                    {
                        ExportCanvas.UpdateLayout();
                        if (ExportCanvas.Children.Count > 0)
                        {
                            var exportSuccess = Renderer.Export(ExportCanvas, design.Name, nameValue, Helper.MillimetersToPixels(ExportCanvas.Width), Helper.MillimetersToPixels(ExportCanvas.Height));
                            if (exportSuccess)
                            {
                                Helper.MessageBoxTimeout((System.IntPtr)0, $"{design.Name + "-" + nameValue} exportiert", "Export", 0, 0, 100);
                            }
                            else
                            {
                                MessageBox.Show($"{nameValue} konnte nicht exportiert werden");
                            }
                        }
                    });
                }
                catch(Exception ex)
                {
                    MainWindow.Window.Dispatcher.Invoke(() =>
                    {
                        MainWindow.SetStatusBar($"{nameValue} konnte nicht erstellt werden. Error: {ex.Message}");
                    });
                }

                index++;
            }

            //design.ForEach(element =>
            //{
            //    collage.UpdateLayout();
            //    if (collage.Children.Count > 0)
            //    {
            //        var fileName = "Page " + (CollageWindow.Collages.IndexOf(collage) + 1);
            //        //var exportSuccess = Renderer.Export(collage, currentSession.SessionFileName, fileName, collage.Width * 3.675, collage.Height * 3.675);
            //        //if (exportSuccess)
            //        //{
            //        //    MainWindow.SetStatusBar($"{currentSession.SessionFileName} - {fileName} exportiert");
            //        //    Helper.MessageBoxTimeout((System.IntPtr)0, $"{currentSession.SessionFileName} - {fileName} exportiert", "Export", 0, 0, 100);
            //        //}
            //        //else
            //        //{
            //        //    MainWindow.SetStatusBar($"{fileName} konnte nicht exportiert werden");
            //        //}
            //    }
            //});
        }
    }
}
