using Microsoft.Xaml.Interactivity;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace UmiAoi.UWP.Behaviors
{
    public sealed class FocusBehavior : DependencyObject, IBehavior
    {
        private DependencyObject _associatedObject;
        public DependencyObject AssociatedObject => _associatedObject;

        #region Property
        public CompositeTransformType TransformType
        {
            get { return (CompositeTransformType)GetValue(TransformTypeProperty); }
            set { SetValue(TransformTypeProperty, value); }
        }
        
        public static readonly DependencyProperty TransformTypeProperty =
            DependencyProperty.Register(nameof(TransformType), typeof(CompositeTransformType), typeof(FocusBehavior), new PropertyMetadata(CompositeTransformType.Center));

        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }
        
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(double), typeof(FocusBehavior), new PropertyMetadata(0.0));

        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }
        
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(double), typeof(FocusBehavior), new PropertyMetadata(0.0));



        public double DurationMilliseconds
        {
            get { return (double)GetValue(DurationMillisecondsProperty); }
            set { SetValue(DurationMillisecondsProperty, value); }
        }
        
        public static readonly DependencyProperty DurationMillisecondsProperty =
            DependencyProperty.Register(nameof(DurationMilliseconds), typeof(double), typeof(FocusBehavior), new PropertyMetadata(300.0));

        #endregion

        private bool IsBusy = false;
        private readonly Storyboard _storyboard = new Storyboard();
        private readonly DoubleAnimation _XAnim = new DoubleAnimation();
        private readonly DoubleAnimation _YAnim = new DoubleAnimation();
        public void Attach(DependencyObject associatedObject)
        {
            _associatedObject = associatedObject;
            var element = _associatedObject as FrameworkElement;
            if (element == null) return;
            var ct = new CompositeTransform();

            element.RenderTransform = ct;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            _YAnim.EasingFunction = _XAnim.EasingFunction = new ExponentialEase { Exponent = 4 };
            _YAnim.Duration = _XAnim.Duration = new Duration(TimeSpan.FromMilliseconds(DurationMilliseconds));
            Storyboard.SetTarget(_XAnim, element.RenderTransform);
            Storyboard.SetTarget(_YAnim, element.RenderTransform);

            switch (TransformType)
            {
                case CompositeTransformType.Center:
                    Storyboard.SetTargetProperty(_XAnim, nameof(CompositeTransform.CenterX));
                    Storyboard.SetTargetProperty(_YAnim, nameof(CompositeTransform.CenterY));
                    break;
                case CompositeTransformType.Rotation:
                    Storyboard.SetTargetProperty(_XAnim, nameof(CompositeTransform.Rotation));
                    break;
                case CompositeTransformType.Scale:
                    Storyboard.SetTargetProperty(_XAnim, nameof(CompositeTransform.ScaleX));
                    Storyboard.SetTargetProperty(_YAnim, nameof(CompositeTransform.ScaleY));
                    break;
                case CompositeTransformType.Skew:
                    Storyboard.SetTargetProperty(_XAnim, nameof(CompositeTransform.SkewX));
                    Storyboard.SetTargetProperty(_YAnim, nameof(CompositeTransform.SkewY));
                    break;
                case CompositeTransformType.Translate:
                    Storyboard.SetTargetProperty(_XAnim, nameof(CompositeTransform.TranslateX));
                    Storyboard.SetTargetProperty(_YAnim, nameof(CompositeTransform.TranslateY));
                    break;
            }
            if (TransformType == CompositeTransformType.Rotation)
            {
                _storyboard.Children.Add(_XAnim);
            }
            else
            {
                _storyboard.Children.Add(_XAnim);
                _storyboard.Children.Add(_YAnim);
            }
            element.PointerEntered += Element_PointerEntered;
            element.PointerExited += Element_PointerExited;
            _storyboard.Completed += _storyboard_Completed;
        }

        private void _storyboard_Completed(object sender, object e)
        {
            IsBusy = false;
        }

        private void Element_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!IsBusy)
            {
                StartScaleStoryboard(From, To);
                IsBusy = true;
            }
        }

        private void Element_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            StartScaleStoryboard(To, From);
        }

        private void StartScaleStoryboard(double from, double to)
        {
            _YAnim.From = _XAnim.From = from;
            _YAnim.To = _XAnim.To = to;
            _storyboard.Begin();
        }

        public void Detach()
        {
            _storyboard.Completed -= _storyboard_Completed;
            var obj = _associatedObject as FrameworkElement;
            if (obj == null) return;

            obj.PointerEntered -= Element_PointerEntered;
            obj.PointerExited -= Element_PointerExited;
        }
    }

    public enum CompositeTransformType
    {
        Center,
        Rotation,
        Scale,
        Skew,
        Translate
    }
}
