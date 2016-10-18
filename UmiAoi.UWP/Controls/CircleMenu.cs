using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace UmiAoi.UWP.Controls
{
    public sealed class CircleMenu : ItemsControl
    {
        private static string CanvasName = "Canvas";
        private static string MenuName = "Menu";

        public CircleMenu()
        {
            this.DefaultStyleKey = typeof(CircleMenu);
            Loaded += CircleMenu_Loaded;
        }

        #region DependencyProperty
        public double CircleRadius
        {
            get { return (double)GetValue(CircleRadiusProperty); }
            set { SetValue(CircleRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircleRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CircleRadiusProperty =
            DependencyProperty.Register(nameof(CircleRadius), typeof(double), typeof(CircleMenu), new PropertyMetadata(100.0, new PropertyChangedCallback(CircleRadiusChanged)));

        private static void CircleRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CircleMenu).UpdateUI();
        }

        public double ThetaAngle
        {
            get { return (double)GetValue(ThetaAngleProperty); }
            set { SetValue(ThetaAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThetaRadians.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThetaAngleProperty =
            DependencyProperty.Register(nameof(ThetaAngle), typeof(double), typeof(CircleMenu), new PropertyMetadata(double.NaN, new PropertyChangedCallback(ThetaAngleChanged)));

        private static void ThetaAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CircleMenu).UpdateUI();
        }

        public double OffsetAngle
        {
            get { return (double)GetValue(OffsetAngleProperty); }
            set { SetValue(OffsetAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffsetAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetAngleProperty =
            DependencyProperty.Register(nameof(OffsetAngle), typeof(double), typeof(CircleMenu), new PropertyMetadata(0.0, new PropertyChangedCallback(OffsetAngleChanged)));

        private static void OffsetAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CircleMenu).UpdateUI();
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(CircleMenu), new PropertyMetadata(true, new PropertyChangedCallback(IsOpenChanged)));

        private static void IsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var circleMenu = (d as CircleMenu);
            circleMenu.UpdateUI();
            circleMenu.BeginAnimate();
        }

        #endregion

        public delegate void ItemsTappedEventHandler(Object sender, TappedRoutedEventArgs e);
        public event ItemsTappedEventHandler ItemsTapped;

        private Canvas canvas { get; set; }
        private FrameworkElement menu { get; set; }
        private Storyboard storyboard { get; set; }
        private ItemCollection OldItems { get; set; }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            canvas = GetTemplateChild(CanvasName) as Canvas;
            menu = GetTemplateChild(MenuName) as FrameworkElement;
            menu.Tapped += Menu_Tapped;
            UpdateUI();
        }

        private void CircleMenu_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Menu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsOpen = !IsOpen;
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            UpdateUI();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        private double millSeconds = 500;
        private void Init()
        {
            storyboard = new Storyboard();
            storyboard.Completed += Storyboard_Completed;
            UpdateStoryBoard();
        }

        private void UpdateStoryBoard()
        {
            if (storyboard != null && canvas != null && storyboard.Children.Count != canvas.Children.Count * 2)
            {
                storyboard.Stop();
                storyboard.Children.Clear();
                foreach (var item in Items)
                {
                    var element = item as FrameworkElement;
                    if (element != null)
                    {
                        element.Tapped += Items_Tapped;
                        var animateX = new DoubleAnimation()
                        {
                            EnableDependentAnimation = true,
                            EasingFunction = new ExponentialEase { Exponent = 4, EasingMode = EasingMode.EaseOut },
                            Duration = new Duration(TimeSpan.FromMilliseconds(millSeconds)),
                            From = 0,
                            To = Canvas.GetLeft(element),
                        };
                        var animateY = new DoubleAnimation
                        {
                            EnableDependentAnimation = true,
                            EasingFunction = new ExponentialEase { Exponent = 4, EasingMode = EasingMode.EaseOut },
                            Duration = new Duration(TimeSpan.FromMilliseconds(millSeconds)),
                            From = 0,
                            To = Canvas.GetTop(element),
                        };
                        if (!IsOpen)
                        {
                            element.Visibility = Visibility.Collapsed;
                            animateX.From = animateX.To;
                            animateX.To = 0;
                            animateY.From = animateY.To;
                            animateY.To = 0;
                        }
                        Storyboard.SetTarget(animateX, element);
                        Storyboard.SetTarget(animateY, element);
                        Storyboard.SetTargetProperty(animateX, "(Canvas.Left)");
                        Storyboard.SetTargetProperty(animateY, "(Canvas.Top)");
                        storyboard.Children.Add(animateX);
                        storyboard.Children.Add(animateY);
                    }
                }
            }
        }

        private void UpdateUI()
        {
            if (canvas != null)
            {               
                canvas.Children.Clear();
                double theta = OffsetAngle;
                double thetaRadians = OffsetAngle * Math.PI / 180F;
                int count = Items.Count;
                foreach (var item in Items)
                {
                    var element = item as FrameworkElement;
                    if (element != null)
                    {
                        var bindingModel = new BindingModel()
                        {
                            Source = this,
                            Path = nameof(Width),
                            BindingElement = element,
                            Property = FrameworkElement.WidthProperty,
                            BindingMode = BindingMode.OneWay
                        };
                        Helper.BindingHelper(bindingModel);
                        bindingModel.Path = nameof(Height);
                        bindingModel.Property = FrameworkElement.HeightProperty;
                        Helper.BindingHelper(bindingModel);

                        if (!canvas.Children.Contains(element)) canvas.Children.Add(element);
                        var x = (double)(CircleRadius * Math.Sin(thetaRadians));
                        var y = (double)(-CircleRadius * Math.Cos(thetaRadians));
                        Canvas.SetLeft(element, x);
                        Canvas.SetTop(element, y);
                        if (double.IsNaN(ThetaAngle)) theta += 360F / count;
                        else theta += ThetaAngle;
                        thetaRadians = theta * Math.PI / 180F;                       
                    }
                }
                UpdateStoryBoard();
            }
        }

        private void Items_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsOpen = false;
            ItemsTapped?.Invoke(sender, e);
        }

        private void BeginAnimate()
        {
            if (storyboard == null) return;
            storyboard.Stop();
            if (IsOpen)
            {
                UpdateItemsVisibility(Visibility.Visible);

                foreach (var child in storyboard.Children)
                {
                    var animate = child as DoubleAnimation;
                    if (animate != null)
                    {
                        animate.EasingFunction.EasingMode = EasingMode.EaseOut;
                        if (animate.To == 0)
                        {
                            animate.To = animate.From;
                            animate.From = 0;
                        }
                    }
                }
            }
            else
            {
                foreach (var child in storyboard.Children)
                {
                    var animate = child as DoubleAnimation;
                    if (animate != null)
                    {
                        animate.EasingFunction.EasingMode = EasingMode.EaseIn;
                        if (animate.From == 0)
                        {
                            animate.From = animate.To;
                            animate.To = 0;
                        }
                    }
                }
            }
            storyboard.Begin();
        }

        private void Storyboard_Completed(object sender, object e)
        {
            if (!IsOpen)
            {
                UpdateItemsVisibility(Visibility.Collapsed);
            }
        }

        private void UpdateItemsVisibility(Visibility visibility)
        {
            foreach (var child in canvas.Children)
            {
                var element = child as FrameworkElement;
                if (element != null)
                    element.Visibility = visibility;
            }
        }

        protected override void OnDisconnectVisualChildren()
        {
            base.OnDisconnectVisualChildren();
            Loaded -= CircleMenu_Loaded;
            menu.Tapped -= Menu_Tapped;
        }
    }
}
