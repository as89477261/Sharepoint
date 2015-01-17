using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace SilverlightApplication1.Converter
{
    public class MajorCourseConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value.ToString() == "Major Course")
                {
                    //return "http://dms/eng/ac_admin/images/CorCourse.png";
                    return "Red";
                }
                else if (value.ToString() == "Core Course")
                {
                    return "Blue";
                }
                else if (value.ToString().StartsWith("Se"))
                {
                    return "Orange";
                }
            }
            
            //return "http://dms/eng/ac_admin/images/Select.png";
            return "Green";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
