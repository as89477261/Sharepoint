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

namespace SilverlightApplication1
{
    public class Curriculum
    {
        public int MainID { get; set; }
        public string CourseID { get; set; }
        public string CourseNameThai { get; set; }
        public string CourseNameEnglish { get; set; }
        public int Credit { get; set; }
        public string CreditDesc { get; set; }
        public string Prerequisite { get; set; }
        public string CourseDescription { get; set; }
        public string CurriculumYear { get; set; }
        public string Group { get; set; }
        public string Course { get; set; }
    }
}
