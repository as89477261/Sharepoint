using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.SharePoint.Client;
using System.Collections.ObjectModel;

namespace SilverlightApplication1
{
    public partial class MainPage : UserControl
    {
        int sumCourse;
        string HostName;
        ClientContext CCAcAdmin;
        ListItemCollection listItemAC;
        ListItemCollection listItemTQF2008;

        List<SubjectBySemester> listCurriculum;
        List<SubjectBySemester> listCurriculumDuplicate;
        List<SubjectBySemester> listSubSemester;
       
        public MainPage()
        {
            InitializeComponent();
            HostName = Application.Current.Host.Source.Host;
            StartUp();
            ComboBinding();
            StartupControl();
        }

        #region QueryData
        private void StartUp()
        {
            CCAcAdmin = new ClientContext("http://" + HostName + "/en/tqf");
            List listTQF = CCAcAdmin.Web.Lists.GetByTitle("Curriculum");
            CamlQuery camlTQF = new CamlQuery();
            camlTQF.ViewXml = "";// "<View><Query><Where><Eq><FieldRef Name='Curriculum'/><Value Type='Text'>2008</Value></Eq></Where></Query></View>";
            listItemTQF2008 = listTQF.GetItems(camlTQF);
            CCAcAdmin.Load(listItemTQF2008);
            CCAcAdmin.ExecuteQueryAsync(new ClientRequestSucceededEventHandler(DispatchCreateCurriculumObj), null);
        }
        List listAcAdmin;
        public void StartUpSubSemesterBySearch(string ACYear, string Semester)
        {
            CCAcAdmin = new ClientContext("http://" + HostName + "/en/ac_admin");
            listAcAdmin = CCAcAdmin.Web.Lists.GetByTitle("Subject By Semester");
            CCAcAdmin.Load(listAcAdmin);
            CamlQuery camlAC = new CamlQuery();
            camlAC.ViewXml = "<View><Query><Where><And><Eq><FieldRef Name='AcademicYear'/><Value Type='Text'>" + ACYear + "</Value></Eq><Eq><FieldRef Name='Semester'/><Value Type='Text'>" + Semester + "</Value></Eq></And></Where></Query></View>";

            listItemAC = listAcAdmin.GetItems(camlAC);
            CCAcAdmin.Load(listItemAC);
            CCAcAdmin.ExecuteQueryAsync(new ClientRequestSucceededEventHandler(DispatchCreateSubBySemester), null);
        }

        private void DispatchCreateCurriculumObj(object sender, ClientRequestSucceededEventArgs args)
        {
            Dispatcher.BeginInvoke(CreateCurriculumObj);
        }
        public void CreateCurriculumObj()
        {
            CreateListCurriculumObj();
            //BindingListBox();
        }
        private void DispatchCreateSubBySemester(object sender, ClientRequestSucceededEventArgs args)
        {
            Dispatcher.BeginInvoke(CreateSubBySemester);
        }
        public void CreateSubBySemester()
        {
            CreateListSubBySemester();
        }
               #endregion

        #region Command Method

        #region Generate List
        private void CreateListCurriculumObj()
        {
            listCurriculum = new List<SubjectBySemester>();

            foreach (ListItem item in listItemTQF2008)
            {
                SubjectBySemester curriculum = new SubjectBySemester();
                curriculum.MainID = item.Id;
                curriculum.CourseID = item["Title"] != null ? item["Title"].ToString() : "";
                curriculum.CourseNameThai = item["Column2"] != null ? item["Column2"].ToString() : "";
                curriculum.CourseNameEnglish = item["Column3"] != null ? item["Column3"].ToString() : "";
                curriculum.Credit = item["Column4"] != null ? int.Parse(item["Column4"].ToString()) : 0;
                curriculum.CreditDesc = item["Column5"] != null ? item["Column5"].ToString() : "";
                curriculum.Prerequisite = item["Column6"] != null ? item["Column6"].ToString() : "";
                curriculum.CourseDescription = item["Column7"] != null ? item["Column7"].ToString() : "";
                curriculum.CurriculumYear = item["Column8"] != null ? item["Column8"].ToString() : "";
                curriculum.Group = item["Column9"] != null ? item["Column9"].ToString() : "";
                curriculum.Course = item["Course"] != null ? item["Course"].ToString() : "";
                listCurriculum.Add(curriculum);

            }
        }
        private void CreateListSubBySemester()
        {
            try
            {
            listSubSemester = new List<SubjectBySemester>();

            if (listItemAC != null)
            {
                foreach (ListItem item in listItemAC)
                {
                    SubjectBySemester subSemester = new SubjectBySemester();
                    subSemester.MainID = item.Id;
                    subSemester.CourseID = item["Title"] != null ? item["Title"].ToString() : "";
                    subSemester.CourseNameThai = item["CourseNameThai"] != null ? item["CourseNameThai"].ToString() : "";
                    subSemester.CourseNameEnglish = item["CourseNameEnglish"] != null ? item["CourseNameEnglish"].ToString() : "";
                    subSemester.Credit = item["Credit"] != null ? int.Parse(item["Credit"].ToString()) : 0;
                    subSemester.CreditDesc = item["CreditDesc"] != null ? item["CreditDesc"].ToString() : "";
                    subSemester.Prerequisite = item["Prerequisite"] != null ? item["Prerequisite"].ToString() : "";
                    subSemester.CourseDescription = item["CourseDesc"] != null ? item["CourseDesc"].ToString() : "";
                    subSemester.CurriculumYear = item["Curriculum"] != null ? item["Curriculum"].ToString() : "";
                    subSemester.Group = item["Group"] != null ? item["Group"].ToString() : "";
                    subSemester.Course = item["Course"] != null ? item["Course"].ToString() : "";
                    listSubSemester.Add(subSemester);
                }
            }
            BindingListBox3();
            }
            catch (Exception)
            {

                MessageBox.Show("Empty Data");
            }
        }
        #endregion

        #region Binding Data
        private void ComboBinding()
        {
            int year = 2012;
            cboAcademicYear.Items.Add("--Choose--");
            for (int i = 0; i < 20; i++)
            {
                cboAcademicYear.Items.Add(year.ToString());
                year += 1;
            }

            cboSemester.Items.Add("--Choose--");
            cboSemester.Items.Add("First Semester");
            cboSemester.Items.Add("Second Semester");
            cboSemester.Items.Add("Summer Semester");

            cboSemester.SelectedIndex = 0;
            cboAcademicYear.SelectedIndex = 0;
        }
        private void BindingListBox()
        {
            listBox1.Items.Clear(); listBox2.Items.Clear();
            foreach (SubjectBySemester item in (listCurriculum.Where(x => x.CurriculumYear == "2008")))
            {

                listBox1.Items.Add(item);
                //listBox1.DisplayMemberPath = "Course";
            }
            foreach (SubjectBySemester item in (listCurriculum.Where(x => x.CurriculumYear == "2012")))
            {

                listBox2.Items.Add(item);
                //listBox2.DisplayMemberPath = "Course";
            }

        }

        private void BufferListBox12Item()
        {
            listBox3.Items.Clear();

            if (listCurriculumDuplicate.Count > 0)
            {
                foreach (var item in listCurriculumDuplicate)
                {
                    listCurriculum.Add(item);
                }
                listCurriculum = listCurriculum.OrderBy(x => x.Course).ToList();
                //listCurriculumDuplicate = new List<SubjectBySemester>();
            }
            else
            {
                listCurriculum = listCurriculum.OrderBy(x => x.Course).ToList();
            }
        }
        private void BindingListBox3()
        {

            BufferListBox12Item();

            if (listSubSemester != null)
            {
                foreach (SubjectBySemester item in listSubSemester)
                {
                    listBox3.Items.Add(item);
                    var data = listCurriculum.SingleOrDefault(x => x.Course == item.Course &&
                        x.AcademicYear == item.AcademicYear &&
                        x.Semester == item.Semester);
                    if (data != null)
                    {
                        listCurriculum.Remove(data);
                        //listCurriculumDuplicate.Add(data);
                    }
                }
                sumCourse = listSubSemester.Count;
                label4.Content = "Course: " + sumCourse;

            } 
            BindingListBox();
           
        }
        private void StartupControl()
        {
            listBox3.DisplayMemberPath = "Course";
            listCurriculumDuplicate = new List<SubjectBySemester>();
        }
        #endregion

        #region Add Item
        List<SubjectBySemester> listSubject;
        private void GetDataFromListbox()
        {
            listSubject = new List<SubjectBySemester>();
            for (int i = 0; i < listBox3.Items.Count; i++)
            {
                listSubject.Add((SubjectBySemester)listBox3.Items[i]);
            }
        }
        private void DeleteEmptyData()
        {
            if (listSubSemester != null)
            {


                foreach (var item in listSubSemester)
                {
                    if (listSubject.SingleOrDefault(x => x.Course == item.Course) == null)
                    {
                        ListItem Data = listItemAC.GetById(item.MainID);
                        Data.DeleteObject();
                        Data.Update();
                        CCAcAdmin.ExecuteQueryAsync(null, null);
                    }
                }
            }
            else
            {
                MessageBox.Show("you have to search for Academic/Semester you wnat to see");
            }
        }
        private void AddNewItem()
        {
            List listData = CCAcAdmin.Web.Lists.GetByTitle("Subject By Semester");
            foreach (var item in listSubject)
            {
                if (listSubSemester.SingleOrDefault(x => x.Course == item.Course) == null)
                {
                    ListItemCreationInformation listInfo = new ListItemCreationInformation();
                    ListItem newItem = listData.AddItem(listInfo);

                    newItem["Title"] = item.CourseID;
                    newItem["CourseNameThai"] = item.CourseNameThai;
                    newItem["CourseNameEnglish"] = item.CourseNameEnglish;
                    newItem["Credit"] = item.Credit;
                    newItem["CreditDesc"] = item.CreditDesc;
                    newItem["Prerequisite"] = item.Prerequisite;
                    newItem["Curriculum"] = item.CurriculumYear;
                    newItem["Group"] = item.Group;
                    newItem["AcademicYear"] = cboAcademicYear.SelectedItem.ToString();
                    newItem["Semester"] = cboSemester.SelectedItem.ToString();
                    newItem["CourseDesc"] = item.CourseDescription.ToString();
                    //newItem["Lecturer"] = "";
                    //newItem["Count"] = "1";
                    newItem.Update();
                    CCAcAdmin.ExecuteQueryAsync(null, null);
                }
            }
        }
        private void AddComplete()
        {
            MessageBox.Show("Add Course Complete");
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            cboAcademicYear.SelectedIndex = 0;
            cboSemester.SelectedIndex = 0;
            label3.Content = "";
        }
        #endregion

        #endregion

        #region Events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cboAcademicYear.SelectedItem.ToString() != "--Choose--" && cboSemester.SelectedItem.ToString() != "--Choose--")
            {
                listCurriculumDuplicate = new List<SubjectBySemester>();
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                CreateListCurriculumObj();
                StartUpSubSemesterBySearch(cboAcademicYear.SelectedItem.ToString(), cboSemester.SelectedItem.ToString());
                label3.Content = "AcademicYear :" + cboAcademicYear.SelectedItem.ToString() + "  Semester :" + cboSemester.SelectedItem.ToString();
                comboBox1.IsEnabled = true;
                textBox1.IsEnabled = true;
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                List<SubjectBySemester> dataBuffer = new List<SubjectBySemester>();
                foreach (var item in listBox1.SelectedItems)
                {
                    dataBuffer.Add((SubjectBySemester)item);
                }

                foreach (var item in dataBuffer)
                {
                    listCurriculum.Remove(item);
                    listBox1.Items.Remove(item);
                    listBox3.Items.Add(item);                   
                    label4.Content = "Course: " + listBox3.Items.Count;
                }
                listBox1.SelectedIndex = -1;
               

            }
            else if (tabControl1.SelectedIndex == 1)
            {
                List<SubjectBySemester> dataBuffer = new List<SubjectBySemester>();
                foreach (var item in listBox2.SelectedItems)
                {
                    dataBuffer.Add((SubjectBySemester)item);
                }

                foreach (var item in dataBuffer)
                {
                    listCurriculum.Remove(item);
                    listBox2.Items.Remove(item);
                    listBox3.Items.Add(item);
                    label4.Content = "Course: " + listBox3.Items.Count;
                }
                listBox2.SelectedIndex = -1;
               
            }

            
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBox3.SelectedItems != null)
            {
                List<SubjectBySemester> dataBuffer = new List<SubjectBySemester>();
                foreach (var item in listBox3.SelectedItems)
                {
                    dataBuffer.Add((SubjectBySemester)item);
                }

                foreach (SubjectBySemester item in dataBuffer)
                {
                    listBox3.Items.Remove(item);
                    if (item.CurriculumYear == "2008")
                    {
                        listCurriculum.Add(item);
                        listBox1.Items.Add(item);
                        label4.Content = "Course: " + listBox3.Items.Count;
                    }
                    else
                    {
                        listCurriculum.Add(item);
                        listBox2.Items.Add(item);
                        label4.Content = "Course: " + listBox3.Items.Count;
                    }

                }


                BindingListBox();
                

            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetDataFromListbox();
                DeleteEmptyData();
                AddNewItem();
                AddComplete();
                label4.Content = "";


            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Search
        private void textBox1_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text != "")
            {
                if (comboBox1.SelectedItem.ToString() != "--Choose--")
                {
                    if (comboBox1.SelectedIndex == 1)
                    {
                        listBox1.Items.Clear();
                        listBox2.Items.Clear();

                        var listItem = listCurriculum.Where(r => r.CourseID.ToLower().StartsWith(textBox1.Text.ToLower()));
                        foreach (SubjectBySemester item in listItem)
                        {
                            if (item.CurriculumYear == "2008")
                            {
                                listBox1.Items.Add(item);
                            }
                            else
                            {
                                listBox2.Items.Add(item);
                            }
                        }
                    }
                    else if (comboBox1.SelectedIndex == 2)
                    {
                        listBox1.Items.Clear();
                        listBox2.Items.Clear();

                        var listItem = listCurriculum.Where(r => r.CourseNameThai.ToLower().Contains(textBox1.Text.ToLower()));
                        foreach (SubjectBySemester item in listItem)
                        {
                            if (item.CurriculumYear == "2008")
                            {
                                listBox1.Items.Add(item);
                            }
                            else
                            {
                                listBox2.Items.Add(item);
                            }
                        }
                    }
                    else if(comboBox1.SelectedIndex == 3)
                    {
                        listBox1.Items.Clear();
                        listBox2.Items.Clear();

                        var listItem = listCurriculum.Where(r => r.Group.ToLower().Contains(textBox1.Text.ToLower()));
                        foreach (SubjectBySemester item in listItem)
                        {
                            if (item.CurriculumYear == "2008")
                            {
                                listBox1.Items.Add(item);
                            }
                            else
                            {
                                listBox2.Items.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (SubjectBySemester item in listCurriculum)
                {
                    if (item.CurriculumYear == "2008")
                    {
                        listBox1.Items.Add(item);
                    }
                    else
                    {
                        listBox2.Items.Add(item);
                    }
                }
            }
        }
        #endregion
    }
}



