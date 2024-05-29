using PlayingCardDesigner.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;

namespace PlayingCardDesigner
{
    public static class Renderer
    {
        public static string ImagePath { get; set; }
        public static List<Models.Color> Colors { get; set; }

        public static void DrawElement(Canvas canvas, Element element, Point StartPoint, int DataIndex = 0)
        {
            try
            {
                var design = MainWindowViewModel.Main.Session.SessionDesign;

                UIElement borderItem = null;
                UIElement item = null;

                if (element.Shape == "Rectangle")
                {
                    borderItem = new Border()
                    {
                        Height = Helper.MillimetersToPixels(element.Height),
                        Width = Helper.MillimetersToPixels(element.Width),
                        BorderThickness = new Thickness(element.BorderThickness),
                        CornerRadius = new CornerRadius(element.CornerRadius)
                    };

                    var targetBorderColor = Colors.Find(c => c.Name == element.BorderColor);
                    if (targetBorderColor != null)
                        ((Border)borderItem).BorderBrush = Helper.GetColorFromHex(targetBorderColor.Hex);

                    var targetFillColor = Colors.Find(c => c.Name == element.Fill);
                    if (targetFillColor != null)
                        ((Border)borderItem).Background = Helper.GetColorFromHex(targetFillColor.Hex);
                }
                else if (element.Shape == "Circle")
                {
                    borderItem = new Border()
                    {
                        Height = Helper.MillimetersToPixels(element.Height),
                        Width = Helper.MillimetersToPixels(element.Width),
                        BorderThickness = new Thickness(element.BorderThickness),
                        CornerRadius = new CornerRadius(100)
                    };

                    var targetBorderColor = Colors.Find(c => c.Name == element.BorderColor);
                    if (targetBorderColor != null)
                        ((Border)borderItem).BorderBrush = Helper.GetColorFromHex(targetBorderColor.Hex);

                    var targetFillColor = Colors.Find(c => c.Name == element.Fill);
                    if (targetFillColor != null)
                        ((Border)borderItem).Background = Helper.GetColorFromHex(targetFillColor.Hex);
                }

                StartPoint.Y += Helper.MillimetersToPixels(element.Location_Y);
                StartPoint.X += Helper.MillimetersToPixels(element.Location_X);

                if (borderItem != null)
                {
                    Canvas.SetTop(borderItem, StartPoint.Y);
                    Canvas.SetLeft(borderItem, StartPoint.X);
                    canvas.Children.Add(borderItem);
                }

                var calculatedHeight = Helper.MillimetersToPixels(element.Height) * (element.ContentRatio / 100);
                var calculatedWidth = Helper.MillimetersToPixels(element.Width) * (element.ContentRatio / 100);

                if (element.ContentType == "Image")
                {
                    var path = ImagePath + @"\" + element.DataContext;
                    if (design.Daten.Columns.Find(c => c == element.DataContext) != null)
                    {
                        var targetRow = design.Daten.Rows[DataIndex].Find(r => r.Column == element.DataContext);
                        if (targetRow != null)
                            path = ImagePath + @"\" + targetRow.Value;
                    }

                    if (!File.Exists(path))
                        path = ImagePath + @"\Empty.png";

                    var uriSource = new Uri(path);

                    item = new Image()
                    {
                        Source = new BitmapImage(uriSource),
                        Height = calculatedHeight,
                        Width = calculatedWidth
                    };
                }
                else if(element.ContentType == "Text")
                {
                    calculatedHeight = Helper.MillimetersToPixels(element.Height);
                    calculatedWidth = Helper.MillimetersToPixels(element.Width);

                    var text = "";
                    if (design.Daten.Columns.Find(c => c == element.DataContext) != null)
                    {
                        var targetRow = design.Daten.Rows[DataIndex].Find(r=> r.Column == element.DataContext);
                        if (targetRow != null)
                            text = targetRow.Value;
                    }
                    else text = element.DataContext;
                    
                    var textblock = new TextBlock()
                    {
                        Text = text,
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment= TextAlignment.Center,
                        Padding = new Thickness(0, 0, 5, 5),
                        FontFamily = new FontFamily(design.FontFamily),
                        FontWeight = element.FontWeight == "Normal" ? FontWeights.Regular : FontWeights.Bold,
                        FontSize = element.Fontsize,
                        HorizontalAlignment= HorizontalAlignment.Center,
                        VerticalAlignment= VerticalAlignment.Center,
                        Width=calculatedWidth*2
                    };

                    var targetFontColor = Colors.Find(c => c.Name == element.FontColor);
                    if (targetFontColor != null)
                        textblock.Foreground = Helper.GetColorFromHex(targetFontColor.Hex);

                    item = new Viewbox()
                    {
                        VerticalAlignment= VerticalAlignment.Stretch,
                        HorizontalAlignment= HorizontalAlignment.Stretch,
                        Height = calculatedHeight,
                        Width = calculatedWidth
                    };

                    //item = new Label()
                    //{
                    //    Content = textblock,
                    //    FontFamily = new FontFamily(design.FontFamily),
                    //    FontWeight = element.FontWeight == "Normal" ? FontWeights.Regular : FontWeights.Bold,
                    //    FontSize = fontSize,
                    //    VerticalAlignment = VerticalAlignment.Center,
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    HorizontalContentAlignment = HorizontalAlignment.Center,
                    //    VerticalContentAlignment = VerticalAlignment.Center,
                    //    Height = calculatedHeight,
                    //    Width = calculatedWidth,
                    //    Padding = new Thickness(0, 0, 0, 5)
                    //};

                    ((Viewbox)item).Child= textblock;
                }

                if(item != null)
                {
                    var top = StartPoint.Y;
                    switch (element.VerticalAlignment)
                    {
                        case "Center": top = StartPoint.Y + Helper.MillimetersToPixels(element.Height)/2 - calculatedHeight / 2; break;
                        case "Bottom": top = StartPoint.Y + Helper.MillimetersToPixels(element.Height) - calculatedHeight; break;
                    }

                    var left = StartPoint.X;
                    switch (element.HorizontalAlignment)
                    {
                        case "Center": left = StartPoint.X + Helper.MillimetersToPixels(element.Width)/2 - calculatedWidth / 2; break;
                        case "Right": left = StartPoint.X + Helper.MillimetersToPixels(element.Width) - calculatedWidth; break;
                    }

                    Canvas.SetTop(item, top);
                    Canvas.SetLeft(item, left);
                    canvas.Children.Add(item);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MainWindow.Window.Dispatcher.Invoke(() =>
                {
                    MainWindow.SetStatusBar("Error: " + ex.Message);
                });
            }

        }

        public static bool Export(Canvas canvas, string designName, string fileName, double width, double height)
        {
            try
            {
                var exportDirectory = MainWindowViewModel.SessionsDirectory + $@"\Export\{designName}";
                if (!Directory.Exists(exportDirectory))
                {
                    Directory.CreateDirectory(exportDirectory);
                }
                var filePath = exportDirectory + @"\" + fileName + ".png";

                var currentSession = MainWindowViewModel.Main.Session;
                Rect bounds = new Rect(new Size(width, height));
                double dpi = 192d;

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(canvas);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }

                rtb.Render(dv);

                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                MemoryStream ms = new MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();

                File.WriteAllBytes(filePath, ms.ToArray());

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
