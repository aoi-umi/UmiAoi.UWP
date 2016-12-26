using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
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
            isDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
            //Loaded += CircleMenu_Loaded;
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
            if (e.OldValue != e.NewValue)
            {
                var circleMenu = (d as CircleMenu);
                circleMenu.UpdateUI();
                circleMenu.BeginAnimate();
            }
        }

        public IconElement MenuIcon
        {
            get { return (IconElement)GetValue(MenuIconProperty); }
            set { SetValue(MenuIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuIconProperty =
            DependencyProperty.Register(nameof(MenuIcon), typeof(IconElement), typeof(CircleMenu), new PropertyMetadata(new FontIcon() { Glyph = "\ue700" }));

        #endregion

        public delegate void ItemsTappedEventHandler(Object sender, TappedRoutedEventArgs e);
        public event ItemsTappedEventHandler ItemsTapped;

        private Canvas canvas { get; set; }
        private FrameworkElement menu { get; set; }
        private Storyboard storyboard { get; set; }
        //private bool isInited { get; set; }
        private bool isDesignMode {get;set;}
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            canvas = GetTemplateChild(CanvasName) as Canvas;
            menu = GetTemplateChild(MenuName) as FrameworkElement;
            storyboard = new Storyboard();
            AddEvent();
            menu.AllowFocusOnInteraction = true;
            AllowFocusOnInteraction = true;
            UpdateUI();
        }

        private void AddEvent()
        {
            storyboard.Completed += Storyboard_Completed;
            menu.Tapped += Menu_Tapped;
            menu.LostFocus += Menu_LostFocus;
        }

        private void RemoveEvent()
        {
            Loaded -= CircleMenu_Loaded;
            menu.Tapped -= Menu_Tapped;
            menu.LostFocus += Menu_LostFocus;
            storyboard.Completed -= Storyboard_Completed;
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            UpdateAfterItemsChanged();
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
            //isInited = true;
            UpdatePositionAndStoryBoard();
        }

        private void UpdateUI()
        {
            if (canvas != null)
            {
                if (IsOpen)
                {
                    UpdateAfterItemsChanged();
                }
                else
                {
                    UpdatePositionAndStoryBoard();
                }
            }
        }

        private void UpdatePositionAndStoryBoard()
        {
            double theta = OffsetAngle;
            double thetaRadians = OffsetAngle * Math.PI / 180F;
            int count = Items.Count;
            foreach (var item in Items)
            {
                var element = item as FrameworkElement;
                if (element != null)
                {
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

        private void UpdateStoryBoard()
        {
            if (!isDesignMode && storyboard != null && canvas != null && storyboard.Children.Count != canvas.Children.Count * 2)
            {
                storyboard.Stop();
                storyboard.Children.Clear();
                foreach (var item in Items)
                {
                    var element = item as FrameworkElement;
                    if (element != null)
                    {
                        double x = Canvas.GetLeft(element);
                        double y = Canvas.GetTop(element);
                        if (!IsOpen) SetStoryBoard(storyboard, element, x, 0, y, 0);
                        else SetStoryBoard(storyboard, element, 0, x, 0, y);
                    }
                }
            }
        }

        private void UpdateAfterItemsChanged()
        {
            if (canvas == null) return;
            bool remove = false;
            IEnumerable<UIElement> changedItems = Items.Where(x => (x as UIElement) != null && !canvas.Children.Contains(x)).Select(x => x as UIElement);
            //int removeCount = changedItems.Count();
            //int addCount = Items.Where(x => (x as UIElement) != null && !canvas.Children.Contains(x)).Select(x => x as UIElement).Count();
            if (changedItems.Count() == 0)
            {
                changedItems = canvas.Children.Where(x => !Items.Contains(x));
                remove = true;
            }
            if (changedItems.Count() == 0) return;
            //System.Diagnostics.Debug.WriteLine("{0},{1}", removeCount, addCount);
            if (!IsOpen || !isDesignMode)
            {
                foreach (var item in changedItems)
                {
                    var element = item as FrameworkElement;
                    if (element != null)
                    {

                        if (!remove)
                        {
                            if (!IsOpen) element.Visibility = Visibility.Collapsed;
                            if (!canvas.Children.Contains(element)) AddElementToCanvas(element);
                        }
                        else
                        {
                            element.Tapped -= Items_Tapped;
                            canvas.Children.Remove(element);
                        }
                    }
                }
                UpdatePositionAndStoryBoard();
                return;
            }
            UpdateItemsChangedStoryBoard(remove, changedItems);
        }

        private void AddElementToCanvas(FrameworkElement element)
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
            //element.AllowFocusOnInteraction = true;
            element.Tapped += Items_Tapped;
            canvas.Children.Add(element);
        }

        private void UpdateItemsChangedStoryBoard(bool remove, IEnumerable<UIElement> changedItems)
        {
            Storyboard itemsChangedStoryBoard = new Storyboard();
            foreach (var item in changedItems)
            {
                var element = item as FrameworkElement;
                double from = 1, to = 0;
                if (!remove)
                {
                    from = 0;
                    to = 1;
                }
                if (element != null)
                {
                    var animateX = new DoubleAnimation()
                    {
                        EnableDependentAnimation = true,
                        EasingFunction = new ExponentialEase { Exponent = 4, EasingMode = EasingMode.EaseOut },
                        Duration = new Duration(TimeSpan.FromMilliseconds(millSeconds)),
                        From = from,
                        To = to
                    };
                    var animateY = new DoubleAnimation
                    {
                        EnableDependentAnimation = true,
                        EasingFunction = new ExponentialEase { Exponent = 4, EasingMode = EasingMode.EaseOut },
                        Duration = new Duration(TimeSpan.FromMilliseconds(millSeconds)),
                        From = from,
                        To = to,
                    };
                    var ct = new CompositeTransform();
                    element.RenderTransform = ct;
                    element.RenderTransformOrigin = new Point(0.5, 0.5);
                    Storyboard.SetTarget(animateX, element.RenderTransform);
                    Storyboard.SetTarget(animateY, element.RenderTransform);
                    Storyboard.SetTargetProperty(animateX, nameof(CompositeTransform.ScaleX));
                    Storyboard.SetTargetProperty(animateY, nameof(CompositeTransform.ScaleY));
                    itemsChangedStoryBoard.Children.Add(animateX);
                    itemsChangedStoryBoard.Children.Add(animateY);
                }
            }
            double theta = OffsetAngle;
            double thetaRadians = OffsetAngle * Math.PI / 180F;
            int count = Items.Count;
            foreach (var item in Items)
            {
                var element = item as FrameworkElement;
                if (element != null)
                {
                    var x = (double)(CircleRadius * Math.Sin(thetaRadians));
                    var y = (double)(-CircleRadius * Math.Cos(thetaRadians));
                    if (!remove)
                    {
                        if (!canvas.Children.Contains(element))
                        {
                            AddElementToCanvas(element);
                            Canvas.SetLeft(element, x);
                            Canvas.SetTop(element, y);
                        }
                    }
                    SetStoryBoard(itemsChangedStoryBoard, element, Canvas.GetLeft(element), x, Canvas.GetTop(element), y);
                    if (double.IsNaN(ThetaAngle)) theta += 360F / count;
                    else theta += ThetaAngle;
                    thetaRadians = theta * Math.PI / 180F;
                }
            }
            itemsChangedStoryBoard.Begin();
            itemsChangedStoryBoard.Completed += (sender, e) =>
            {
                var storyboard = (sender as Storyboard);
                if (storyboard != null)
                {
                    storyboard.Stop();
                    storyboard.Children.Clear();
                }
                if (remove)
                {
                    foreach (var item in changedItems)
                    {
                        var element = item as FrameworkElement;
                        if (element != null && !Items.Contains(element)) canvas.Children.Remove(element);
                    }
                }
                UpdatePositionAndStoryBoard();
            };
        }

        private void SetStoryBoard(Storyboard storyboard, FrameworkElement element, double xFrom, double xTo, double yFrom, double yTo)
        {
            var animateX = new DoubleAnimation()
            {
                EnableDependentAnimation = true,
                EasingFunction = new ExponentialEase { Exponent = 4, EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(millSeconds)),
                From = xFrom,
                To = xTo,
            };
            var animateY = new DoubleAnimation
            {
                EnableDependentAnimation = true,
                EasingFunction = new ExponentialEase { Exponent = 4, EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromMilliseconds(millSeconds)),
                From = yFrom,
                To = yTo,
            };
            Storyboard.SetTarget(animateX, element);
            Storyboard.SetTarget(animateY, element);
            Storyboard.SetTargetProperty(animateX, "(Canvas.Left)");
            Storyboard.SetTargetProperty(animateY, "(Canvas.Top)");
            storyboard.Children.Add(animateX);
            storyboard.Children.Add(animateY);
        }

        //private bool isStoryCompleted = true;
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

        private void CircleMenu_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Menu_LostFocus(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        private void Menu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsOpen = !IsOpen;
        }

        private void Items_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsOpen = false;
            ItemsTapped?.Invoke(sender, e);
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
            RemoveEvent();
        }
    }
}
