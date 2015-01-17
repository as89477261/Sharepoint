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
using System.Collections.Generic;

namespace SLLecAssignment
{
    public class ACItemSemester
    {
        public string CourseID { get; set; }
        public string CourseNameThai { get; set; }
        public string Course { get; set; }
       // public string CourseNameEng { get; set; }
        public int Credit { get; set; }
        public string CreditDesc { get; set; }
        public string Curriculum { get; set; }
        public string Semester { get; set; }
        public string AcademicYear { get; set; }
        public string Lecturer { get; set; }
        public string keyId { get; set; }
    }
}
