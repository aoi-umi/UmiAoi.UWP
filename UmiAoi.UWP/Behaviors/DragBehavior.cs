using Microsoft.Xaml.Interactivity;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UmiAoi.UWP.Behaviors
{
    public class DragBehavior : DependencyObject, IBehavior
    {


        public bool IsKeptMovePointCenter
        {
            get { return (bool)GetValue(IsKeepedMovePointCenterProperty); }
            set { SetValue(IsKeepedMovePointCenterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsKeepedMovePointCenter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsKeepedMovePointCenterProperty =
            DependencyProperty.Register(nameof(IsKeptMovePointCenter), typeof(bool), typeof(DragBehavior), new PropertyMetadata(true));


        public DependencyObject AssociatedObject => _associatedObject;
        private DependencyObject _associatedObject;
        private FrameworkElement element;

        public void Attach(DependencyObject associatedObject)
        {
            _associatedObject = associatedObject;
            element = associatedObject as FrameworkElement;
            if (element != null)
            {
                element.PointerMoved += Element_PointerMoved;
                element.PointerExited += Element_PointerExited;
            }
        }

        private Point PressedPoint { get; set; }

        private void Element_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            if (point.Properties.IsLeftButtonPressed)
            {
                var pos = point.Position;
                if (PressedPoint.X == 0 && PressedPoint.Y == 0)
                {
                    PressedPoint = new Point(pos.X, pos.Y);
                }
                if (IsKeptMovePointCenter)
                {
                    pos.X = pos.X - element.ActualWidth / 2.0;
                    pos.Y = pos.Y - element.ActualHeight / 2.0;
                }
                else if (PressedPoint != null)
                {
                    pos.X = pos.X - PressedPoint.X;
                    pos.Y = pos.Y - PressedPoint.Y;
                }

                var left = (double)element.GetValue(Canvas.LeftProperty);
                var top = (double)element.GetValue(Canvas.TopProperty);
                element.SetValue(Canvas.LeftProperty, left + pos.X);
                element.SetValue(Canvas.TopProperty, top + pos.Y);
            }
        }

        private void Element_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (PressedPoint.X != 0 && PressedPoint.Y != 0)
            {
                PressedPoint = new Point(0, 0);
            }
        }

        public void Detach()
        {
            if (element != null)
            {
                element.PointerMoved -= Element_PointerMoved;
                element.PointerExited -= Element_PointerExited;
            }

        }
    }
}
