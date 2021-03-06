﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UmiAoi.UWP.Controls
{
    public sealed partial class MaskView : UserControl
    {
        public MaskView()
        {
            InitializeComponent();
        }

        public FrameworkElement MaskContent
        {
            get { return (FrameworkElement)GetValue(MaskContentProperty); }
            set { SetValue(MaskContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaskContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaskContentProperty =
            DependencyProperty.Register(nameof(MaskContent), typeof(FrameworkElement), typeof(MaskView), new PropertyMetadata(null));        
    }
}
