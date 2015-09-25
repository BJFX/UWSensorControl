using System.Windows;
using System.Windows.Controls;

namespace USBLDC.AttachedProperty
{
    public class ListBoxSelectBringIntoView
    {

        public static bool GetIsBringSelectedItemIntoView(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsBringSelectedItemIntoViewProperty);
        }

        public static void SetIsBringSelectedItemIntoView(DependencyObject obj, bool value)
        {
            obj.SetValue(IsBringSelectedItemIntoViewProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsBringSelectedItemIntoView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBringSelectedItemIntoViewProperty =
            DependencyProperty.RegisterAttached("IsBringSelectedItemIntoView", typeof(bool), typeof(ListBoxSelectBringIntoView), new PropertyMetadata(false, OnIsBringSelectedItemIntoViewChganged));

        private static void OnIsBringSelectedItemIntoViewChganged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox listbox = d as ListBox;
            if (listbox != null)
            {
                listbox.SelectionChanged += (sender, args) => 
                {
                    listbox.ScrollIntoView(listbox.SelectedItem);
                };
            }
        }
    }
}
