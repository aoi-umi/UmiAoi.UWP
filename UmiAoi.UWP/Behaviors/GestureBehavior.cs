using Microsoft.Xaml.Interactivity;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace UmiAoi.UWP.Behaviors
{
    public class GestureBehavior : DependencyObject, IBehavior
    {
        public DependencyObject AssociatedObject => _associatedObject;
        private DependencyObject _associatedObject { get; set; }
        private FrameworkElement _element { get; set; }
        
        private Storyboard _storyboard { get; set; }
        private DoubleAnimation _xAnim { get; set; }
        private DoubleAnimation _yAnim { get; set; }
        public void Attach(DependencyObject associatedObject)
        {
            _associatedObject = associatedObject;
            _element = _associatedObject as FrameworkElement;
            if (_element == null) return;

            _xAnim = new DoubleAnimation()
            {
                EnableDependentAnimation = true,
                EasingFunction = new ExponentialEase { Exponent = 4 },
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            _yAnim = new DoubleAnimation()
            {
                EnableDependentAnimation = true,
                EasingFunction = new ExponentialEase { Exponent = 4 },
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            Storyboard.SetTarget(_xAnim, _element);
            Storyboard.SetTarget(_yAnim, _element);
            Storyboard.SetTargetProperty(_xAnim, nameof(FrameworkElement.Width));
            Storyboard.SetTargetProperty(_yAnim, nameof(FrameworkElement.Height));
            _storyboard = new Storyboard();

            _element.ManipulationMode = ManipulationModes.System | ManipulationModes.Scale;
            _element.DoubleTapped += Element_DoubleTapped;
            _element.ManipulationDelta += _element_ManipulationDelta;
            _element.PointerWheelChanged += _element_PointerWheelChanged;
        }

        private void _element_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var delta = (double)e.GetCurrentPoint(_element).Properties.MouseWheelDelta / 1000;
            if (_element.ActualWidth < 50 && (1 + delta) <= 1) return;
            _element.Height = double.NaN;
            _element.Width = _element.ActualWidth * (1 + delta);
            e.Handled = true;
        }

        private void _element_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var delta = e.Delta.Scale;
            if (_element.ActualWidth < 50 && (delta) <= 0) return;
            _element.Height = double.NaN;
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
                    StartStoryboard(_element.ActualWidth, parent.ActualWidth, true);
                    _element.Height = double.NaN;
                }
                else
                {
                    StartStoryboard(_element.ActualHeight, parent.ActualHeight, false);
                    _element.Width = double.NaN;
                }
            }
        }

        private void StartStoryboard(double from, double to, bool isWidth)
        {
            _storyboard.Stop();
            _storyboard.Children.Clear();
            if (isWidth)
            {
                _xAnim.From = from;
                _xAnim.To = to;
                _storyboard.Children.Add(_xAnim);
            }
            else
            {
                _yAnim.From = from;
                _yAnim.To = to;
                _storyboard.Children.Add(_yAnim);
            }
            _storyboard.Begin();
        }

        public void Detach()
        {
            if (_element == null) return;
            _element.DoubleTapped -= Element_DoubleTapped;
            _element.ManipulationDelta -= _element_ManipulationDelta;
        }
    }
}
