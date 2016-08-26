using Microsoft.Xaml.Interactivity;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UmiAoi.UWP.Behaviors
{
    public class DragBehavior : DependencyObject, IBehavior
    {
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
            }
        }

        private void Element_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            if (point.Properties.IsLeftButtonPressed)
            {
                var pos = point.Position;
                pos.X = pos.X - element.ActualWidth / 2.0;
                pos.Y = pos.Y - element.ActualHeight / 2.0;

                var left = (double)element.GetValue(Canvas.LeftProperty);
                var top = (double)element.GetValue(Canvas.TopProperty);
                element.SetValue(Canvas.LeftProperty, left + pos.X);
                element.SetValue(Canvas.TopProperty, top + pos.Y);
            }

        }

        public void Detach()
        {
            if (element != null)
            {
                element.PointerMoved -= Element_PointerMoved;
            }

        }
    }
}
