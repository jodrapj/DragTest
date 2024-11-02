using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace DragTest
{
    public class DraggableElement : FrameworkElement
    {
        private Pen mainPen = new Pen(Brushes.Black, 1.0);
        Rect inner = Rect.Empty;
        Rect outer = Rect.Empty;

        Vector lastOffset = new(0,0);
        private bool isinitd = false;
        public float borderSize { get; set; }
        public Brush Background { get; set; } = Brushes.White;

        private double pos1 = 0,
                       pos2 = 0,
                       pos3 = 0,
                       pos4 = 0;

        private List<DraggableElement> otherElements = new List<DraggableElement>();

        protected override void OnRender(DrawingContext drawingContext)
        {

            if (!isinitd)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(Parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(Parent, i);
                    if (child != this && child is DraggableElement)
                    {
                        otherElements.Add(child as DraggableElement);
                    }
                }

                isinitd = true;

            }


            if (borderSize > 0)
            {
                inner = new Rect(0 + borderSize, lastOffset.Y + borderSize, ActualWidth - borderSize * 2, ActualHeight - borderSize * 2);
                outer = new Rect(lastOffset.X, lastOffset.Y, this.ActualWidth, this.ActualHeight);
            }
            else
            {
                outer = new Rect(lastOffset.X, lastOffset.Y, this.ActualWidth, this.ActualHeight);
            }

            drawingContext.DrawRectangle(Background, mainPen, outer);
            drawingContext.DrawRectangle(Background, mainPen, inner);

            Console.WriteLine(this.Name);
            for (int i = 0; i < otherElements.Count; i++)
            {
                Console.WriteLine("OtherElement: " + otherElements[i].Name);
            }

            base.OnRender(drawingContext);

        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) => InvalidateVisual();

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MakeTopmost();
                DragElement(); 
            } else
            {
                this.MouseMove -= CalculatePosition;
            }
        }

        private void CalculatePosition(object sender, MouseEventArgs e)
        {
            pos1 = pos3 - e.GetPosition(MainWindow.instance).X;
            pos2 = pos4 - e.GetPosition(MainWindow.instance).Y;
            pos3 = e.GetPosition(MainWindow.instance).X;
            pos4 = e.GetPosition(MainWindow.instance).Y;

            Console.WriteLine(pos1 + "\n" + pos2 + "\n" + pos3 + "\n" + pos4 + "\n");

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MakeTopmost();
                this.VisualOffset = new Vector(this.VisualOffset.X - pos1, this.VisualOffset.Y - pos2);
                this.lastOffset = this.VisualOffset;
            } else
            {
                this.MouseMove -= CalculatePosition;
            }
        }

        private void DragElement()
        {
            pos3 = this.VisualOffset.X + this.ActualWidth / 2;
            pos4 = this.VisualOffset.Y + this.ActualHeight / 2;

            this.MouseMove += CalculatePosition;
        }


        private void MakeTopmost()
        {
            Panel.SetZIndex(this, 1);
            foreach(var element in otherElements)
            {
                Panel.SetZIndex(element, 0);
            }
        }
    }
}
