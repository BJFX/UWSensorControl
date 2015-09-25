using System.Windows;

namespace USBLDC.AttachedProperty
{
    public class ScreenModelPropertys
    {
        public static bool GetIsLandScape(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLandScapeProperty);
        }

        public static void SetIsLandScape(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLandScapeProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsLandScape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLandScapeProperty =
            DependencyProperty.RegisterAttached("IsLandScape", typeof(bool), typeof(ScreenModelPropertys), new PropertyMetadata(true));  
    }
}
