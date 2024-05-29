using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlayingCardDesigner.Models
{
    public class Back
    {
        public List<Element> Elemente { get; set; }

        public Back()
        {
            Elemente = new List<Element>();
        }
    }

    public class Color
    {
        public string Name { get; set; }
        public string Hex { get; set; }

        public Color() { }
    }

    public class Daten
    {
        public string FilePath { get; set; }
        public int ColumnCount { get; set; }
        public List<string> Columns { get; set; }
        public int RowCount { get; set; }
        public List<List<Cell>> Rows { get; set; }

        public Daten()
        {
            Columns = new List<string>();
            Rows = new List<List<Cell>>();
        }
    }

    public class Cell
    {
        public string Column { get; set; }
        public string Value { get; set; }

        public Cell() { }
    }

    public class Element
    {
        public string Name { get; set; }
        public string Shape { get; set; }
        public double Location_X { get; set; }
        public double Location_Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int CornerRadius { get; set; }
        public double BorderThickness { get; set; }
        public string BorderColor { get; set; }
        public string Fill { get; set; }
        public string ContentType { get; set; }
        public string FontColor { get; set; }
        public int Fontsize { get; set; }
        public string FontWeight { get; set; }
        public string VerticalAlignment { get; set; }
        public string HorizontalAlignment { get; set; }
        public int ContentRatio { get; set; }
        public string DataContext { get; set; }

        public Element()
        {
            
        }
    }

    public class Front
    {
        public List<Element> Elemente { get; set; }

        public Front()
        {
            Elemente = new List<Element>();
        }
    }

    public class Design
    {
        public string Name { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public int CornerRadius { get; set; }
        public string FontFamily { get; set; }
        public string ImageFilePath { get; set; }
        public List<Color> Colors { get; set; }
        public Daten Daten { get; set; }
        public Front Front { get; set; }
        public Back Back { get; set; }

        public Design(string fileName)
        {
            Name = fileName;
            Height = 100;
            Width = 100;
            CornerRadius = 0;
            FontFamily = "Book Antiqua";
            ImageFilePath = "D:\\Shared\\Dropbox\\PenAndPaper\\Dice Masters\\Spielkarten-Designs\\Images";
            Colors = new List<Color>() { new Color() {Name="Primary", Hex= "#FFFFFFFF" }, new Color() { Name = "Secondary", Hex = "#FFFFFFFF" } };

            Daten = new Daten();
            Front= new Front();
            Back= new Back();
        }
    }
}
