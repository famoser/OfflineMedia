using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using OfflineMediaV3.Business.Enums;

namespace OfflineMediaV3.DisplayHelper
{
    public static class StateManager
    {
        #region Methods

        private static Dictionary<string, double> _state = new Dictionary<string, double>();

        public static void SaveScrollViewerOffset(PageKeys pageKey, DependencyObject dependencyObject)
        {
            try
            {
                ScrollViewer scrollViewer = GetScrollViewer(dependencyObject);

                if (scrollViewer != null)
                {
                    string key = GetUniqueKey(pageKey, dependencyObject);

                    if (_state.ContainsKey(key))
                        _state[key] = scrollViewer.VerticalOffset;
                    else
                        _state.Add(key, scrollViewer.VerticalOffset);
                }
            }
            catch
            {
            }
        }

        public static void RestoreScrollViewerOffset(PageKeys pageKey, DependencyObject dependencyObject)
        {
            try
            {
                ScrollViewer scrollViewer = GetScrollViewer(dependencyObject);

                if (scrollViewer != null)
                {
                    string key = GetUniqueKey(pageKey, dependencyObject);

                    if (_state.ContainsKey(key))
                    {
                        scrollViewer.ChangeView(_state[key], null, null);
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Private Methods

        private static ScrollViewer GetScrollViewer(DependencyObject dependencyObject)
        {
            ScrollViewer scrollViewer = null;

            if (dependencyObject is ScrollViewer)
            {
                scrollViewer = dependencyObject as ScrollViewer;
            }
            else
            {
                FrameworkElement frameworkElement = VisualTreeHelper.GetChild(dependencyObject, 0) as FrameworkElement;

                if (frameworkElement != null)
                {
                    scrollViewer = frameworkElement.FindName("ScrollViewer") as ScrollViewer;
                }
            }

            return scrollViewer;
        }

        private static string GetUniqueKey(PageKeys pagekey, DependencyObject dependencyObject)
        {
            string key = "ScrollOffset";

            FrameworkElement frameworkElement = dependencyObject as FrameworkElement;

            if (frameworkElement != null)
            {
                key =  pagekey +"_" + frameworkElement.Name + "_ScrollOffset";
            }

            return key;
        }

        #endregion
    }
}
