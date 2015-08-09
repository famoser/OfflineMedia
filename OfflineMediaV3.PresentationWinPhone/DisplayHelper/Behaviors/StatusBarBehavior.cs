using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace OfflineMediaV3.DisplayHelper.Behaviors
{
    public class StatusBarBehavior : DependencyObject, IBehavior
    {
        private const string IS_VISIBLE = "IsVisible";
        private const string FOREGROUND_COLOR = "ForegroundColor";
        private const string BACKGROUND_COLOR = "BackgroundColor";
        private const string BACKGROUND_OPACITY = "BackgroundOpacity";

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject) { }

        public void Detach() { }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register(IS_VISIBLE,
            typeof(bool),
            typeof(StatusBarBehavior),
            new PropertyMetadata(true, OnIsVisibleChanged));

        private static async void OnIsVisibleChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var statusBar = StatusBar.GetForCurrentView();
            if ((bool)e.NewValue)
                await statusBar.ShowAsync();
            else
                await statusBar.HideAsync();
        }

        public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(FOREGROUND_COLOR,
            typeof(Color),
            typeof(StatusBarBehavior),
            new PropertyMetadata(null, OnForegroundColorChanged));

        private static void OnForegroundColorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            StatusBar.GetForCurrentView().ForegroundColor = (Color)e.NewValue;
        }

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(BACKGROUND_COLOR,
            typeof(Color),
            typeof(StatusBarBehavior),
            new PropertyMetadata(null, OnBackgroundColorChanged));

        private static void OnBackgroundColorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as StatusBarBehavior;
            if (behavior.BackgroundOpacity == 0)
                behavior.BackgroundOpacity = 1;

            StatusBar.GetForCurrentView().BackgroundColor = behavior.BackgroundColor;
        }

        public double BackgroundOpacity
        {
            get { return (double)GetValue(BackgroundOpacityProperty); }
            set { SetValue(BackgroundOpacityProperty, value); }
        }

        public static readonly DependencyProperty BackgroundOpacityProperty =
            DependencyProperty.Register(BACKGROUND_OPACITY,
            typeof(double),
            typeof(StatusBarBehavior),
            new PropertyMetadata(null, OnBackgroundOpacityChanged));

        private static void OnBackgroundOpacityChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as StatusBarBehavior;
            StatusBar.GetForCurrentView().BackgroundOpacity = behavior.BackgroundOpacity;
        }
    }
}
