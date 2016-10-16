﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace UmiAoi.UWP.Controls
{
    public sealed class CircleMenu : ItemsControl
    {
        private static string CanvasName = "Canvas";
        public CircleMenu()
        {
            this.DefaultStyleKey = typeof(CircleMenu);
        }

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

        private Canvas canvas { get; set; }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
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
                }
            }
            canvas = GetTemplateChild(CanvasName) as Canvas;
            UpdateUI();

        }
                
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            var element = e as FrameworkElement;
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
            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (canvas != null)
            {
                canvas.Children.Clear();
                double theta = 0;
                double thetaRadians = 270;
                int count = Items.Count;
                foreach (var item in Items)
                {
                    var element = item as FrameworkElement;
                    if (element != null)
                    {
                        canvas.Children.Add(element);
                        //var diagonaphal = Math.Sqrt(Math.Pow(element.ActualWidth, 2) + Math.Pow(element.ActualHeight, 2)) / 2;
                        var x = (double)(CircleRadius * Math.Cos(thetaRadians) - element.Width / 2);
                        var y = (double)(CircleRadius * Math.Sin(thetaRadians) - element.Height / 2);
                        Canvas.SetLeft(element, x);
                        Canvas.SetTop(element, y);
                        theta += 360 / count;
                        thetaRadians = theta * Math.PI / 180F;
                    }
                }
            }
        }
    }
}