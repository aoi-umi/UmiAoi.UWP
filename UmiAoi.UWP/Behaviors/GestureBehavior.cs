using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace UmiAoi.UWP.Behaviors
{
    public class GestureBehavior : DependencyObject, IBehavior
    {
        public DependencyObject AssociatedObject => _associatedObject;
        private DependencyObject _associatedObject { get; set; }     
        private FrameworkElement _element { get; set; }

        //private bool IsBusy = false;
        //private Storyboard _storyboard { get; set; }
        //private DoubleAnimation _XAnim { get; set; }
        //private DoubleAnimation _YAnim { get; set; }
        public void Attach(DependencyObject associatedObject)
        {
            _associatedObject = associatedObject;
            _element = _associatedObject as FrameworkElement;
            if (_element == null) return;

            //_XAnim = new DoubleAnimation();
            //_XAnim.EasingFunction = new ExponentialEase { Exponent = 4 };
            //_XAnim.Duration = new Duration(TimeSpan.FromMilliseconds(100));
            //Storyboard.SetTarget(_XAnim, _element);
            //Storyboard.SetTargetProperty(_XAnim, nameof(FrameworkElement.Width));

            //_storyboard = new Storyboard();
            //_storyboard.Children.Add(_XAnim);

            _element.ManipulationMode = ManipulationModes.System | ManipulationModes.Scale;
            _element.DoubleTapped += Element_DoubleTapped;
            _element.ManipulationDelta += _element_ManipulationDelta;
            _element.PointerWheelChanged += _element_PointerWheelChanged;
        }



        private void _element_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            _element.Height = double.NaN;
            _element.Width = double.NaN;
            var delta = (double)e.GetCurrentPoint(_element).Properties.MouseWheelDelta / 1000;
            if (_element.ActualWidth < 50 && (1 + delta) <= 0) return;
            _element.Width = _element.ActualWidth * (1 + delta);
            e.Handled = true;
        }

        private void _element_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _element.Height = double.NaN;
            _element.Width = double.NaN;
            var delta = e.Delta.Scale;
            if (_element.ActualWidth < 50 && (delta) <= 0) return;
            _element.Width = _element.ActualWidth * (delta);
            e.Handled = true;
        }

        private void Element_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var parent = _element.Parent as FrameworkElement;
            if (parent != null)
            {
                if (_element.Width != parent.ActualWidth)
                {
                    _element.Width = parent.ActualWidth;
                    _element.Height = double.NaN;
                }
                else
                {
                    _element.Height = parent.ActualHeight;
                    _element.Width = double.NaN;
                }
            }
        }

        //private void StartStoryboard(double from, double to)
        //{
        //    _storyboard.Stop();
        //    _XAnim.From = from;
        //    _XAnim.To = to;
        //    _storyboard.Begin();
        //}

        public void Detach()
        {
            if (_element == null) return;
            _element.DoubleTapped -= Element_DoubleTapped;
            _element.ManipulationDelta -= _element_ManipulationDelta;
        }
    }
}
