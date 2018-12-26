using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App2
{
    public static class ListBoxSelectionBehavior
    {
        public static readonly DependencyProperty ClickSelectionProperty =
        DependencyProperty.RegisterAttached("ClickSelection",
                                            typeof(bool),
                                            typeof(ListBoxSelectionBehavior),
                                            new PropertyMetadata(false, OnClickSelectionChanged));

        public static bool GetClickSelection(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClickSelectionProperty);
        }
        public static void SetClickSelection(DependencyObject obj, bool value)
        {
            obj.SetValue(ClickSelectionProperty, value);
        }
        private static void OnClickSelectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            if (dpo is ListBox listBox)
            {
                if ((bool)e.NewValue == true)
                {
                    listBox.SelectionMode = SelectionMode.Multiple;
                    listBox.SelectionChanged += OnSelectionChanged;
                }
                else
                {
                    listBox.SelectionChanged -= OnSelectionChanged;
                }
            }
        }
        static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListBox listBox = sender as ListBox;
                var valid = e.AddedItems[0];
                foreach (var item in new ArrayList(listBox.SelectedItems.ToArray()))
                {
                    if (item != valid)
                    {
                        listBox.SelectedItems.Remove(item);
                    }
                }
            }
        }
    }
}
