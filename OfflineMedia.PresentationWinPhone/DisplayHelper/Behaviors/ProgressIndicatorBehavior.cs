using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace OfflineMedia.DisplayHelper.Behaviors
{
    public class ProgressIndicatorBehavior : DependencyObject, IBehavior
    {
        private const string IS_VISIBLE = "IsVisible";
        private const string TEXT = "Text";
        private const string VALUE = "Value";
        private const string IS_INDETERMINATE = "IsIndeterminate";

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
            typeof(ProgressIndicatorBehavior),
            new PropertyMetadata(false, OnIsVisibleChanged));

        private static async void OnIsVisibleChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var progressIndicator = StatusBar.GetForCurrentView().ProgressIndicator;
            if ((bool)e.NewValue)
            {
                progressIndicator.ProgressValue = 0;
                await progressIndicator.ShowAsync();
            }
            else
            {
                await progressIndicator.HideAsync();
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(TEXT,
            typeof(string),
            typeof(ProgressIndicatorBehavior),
            new PropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ProgressIndicatorBehavior behavior = (ProgressIndicatorBehavior)d;
            StatusBar.GetForCurrentView().ProgressIndicator.Text = behavior.Text;
        }

        public object Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
             DependencyProperty.Register(VALUE,
             typeof(object),
             typeof(ProgressIndicatorBehavior),
             new PropertyMetadata(null, OnValueChanged));

        private static void OnValueChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            double? val = null;
            if (e.NewValue != null)
                val = (double?)Convert.ToDouble(e.NewValue);

            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = val;
        }

        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        public static readonly DependencyProperty IsIndeterminateProperty =
             DependencyProperty.Register(IS_INDETERMINATE,
             typeof(bool),
             typeof(ProgressIndicatorBehavior),
             new PropertyMetadata(false, OnIsIndeterminateChanged));

        private static void OnIsIndeterminateChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var progressIndicator = StatusBar.GetForCurrentView().ProgressIndicator;
            if ((bool)e.NewValue)
                progressIndicator.ProgressValue = null;
            else
                progressIndicator.ProgressValue = 0;
        }
    }
}
