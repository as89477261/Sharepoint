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

namespace SLLecAssignment
{
    public partial class MainPage : UserControl
    {
        string HostName;
        List listAcSemester;
        ListItemCollection listACItem;
        List<ACItemSemester> listACItemSemester;
        List<User> ListUser;
        List<Group> listGroup;
        ClientContext ACContext;
        ClientContext TqfContext;
        GroupCollection groupCollection;
        public MainPage()
        {
           
            listACItemSemester = new List<ACItemSemester>();
            HostName = Application.Current.Host.Source.Host;
            InitializeComponent();
            HideControl();
            QueryData();


        }

        #region Binding Control
        public void AssignYear()
        {
            comboBoxFirst1.Items.Add("--Choose--");
            comboBoxFirst2.Items.Add("--Choose--");
            comboBoxFirst2.Items.Add("First Semester");
            comboBoxFirst2.Items.Add("Second Semester");
            comboBoxFirst2.Items.Add("Summer Semester");
            comboBoxFirst1.SelectedIndex = 0;
            comboBoxFirst2.SelectedIndex = 0;
            IEnumerable<IGrouping<string, ACItemSemester>> query = listACItemSemester.GroupBy(z => z.AcademicYear).OrderBy(x => x.Key);
            foreach (IGrouping<string, ACItemSemester> item in query)
            {
                comboBoxFirst1.Items.Add(item.Key);
            }
        }
        public void AssignLecUser()
        {
            GetAllGroup();
            comboBox4.Items.Add("All User");
            comboBox4.SelectedIndex = 0;


        }
        #endregion

        #region Events
        private void comboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox4.SelectedItem != null && comboBox4.SelectedValue.ToString() != "--Choose--")
            {
                Group g = listGroup.SingleOrDefault(x => x.Title == "TQF");

                comboBox3.Items.Clear();
                foreach (User item in GetAllUser(g))
                {
                    comboBox3.Items.Add(item.Title);
                }
                comboBox3.SelectedIndex = 0;
            }
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxFirst1.SelectedItem.ToString() != "--Choose--" && comboBoxFirst2.SelectedValue.ToString() != "--Choose--")
            {
                grid2.Visibility = System.Windows.Visibility.Visible;
                QueryCourseData();

            }
        }
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedValue != null)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox3.SelectedValue.ToString() != "--Choose--")
            {
                if (comboBox3.SelectedValue != null)
                {
                    if (listBox1.Items.Count == 0)
                    {
                        listBox1.Items.Add(comboBox3.SelectedValue);
                    }
                    else
                    {
                        int count = 0;
                        for (int i = 0; i < listBox1.Items.Count; i++)
                        {
                            if (listBox1.Items[i].ToString() != comboBox3.SelectedValue.ToString())
                            {
                                count += 1;
                            }
                        }
                        if (count == listBox1.Items.Count)
                        {
                            listBox1.Items.Add(comboBox3.SelectedValue);
                        }
                    }
                }
            }
        }
        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] UserNameBuffer;
            listBox1.Items.Clear();

            if (dataGrid1.SelectedItem != null)
            {
                ACItemSemester a = (ACItemSemester)dataGrid1.SelectedItem;
                if (a.Lecturer != null)
                {
                    UserNameBuffer = a.Lecturer.Split(';');
                    foreach (string itemString in UserNameBuffer)
                    {
                        if (itemString != "")
                        {
                            listBox1.Items.Add(itemString.Trim());
                        }
                    }
                }
            }
        }
        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion

        #region Query Data 
        ListItemCollection usersList;
        Group b;
        public void QueryData()
        {
            ACContext = new ClientContext("http://" + HostName + "/en/ac_admin");

            ACContext.Load(ACContext.Web);
            listAcSemester = ACContext.Web.Lists.GetByTitle("Subject By Semester");
            ACContext.Load(listAcSemester);
            CamlQuery query = new CamlQuery();
            query.ViewXml = null;
            listACItem = listAcSemester.GetItems(query);
            ACContext.Load(listACItem);
            ACContext.ExecuteQueryAsync(new ClientRequestSucceededEventHandler(AssignObjectItem), null);

        }
        public void AssignObjectItem(object sender, ClientRequestSucceededEventArgs e)
        {
            Dispatcher.BeginInvoke(AssignObject);
        }
        public void AssignObject()
        {
            try
            {
                foreach (ListItem itemAc in listACItem)
                {
                    ACItemSemester ACSemesterObj = new ACItemSemester();
                    ACSemesterObj.CourseID = itemAc["Title"] != null ? itemAc["Title"].ToString() : "";
                    ACSemesterObj.CourseNameThai = itemAc["CourseNameThai"] != null ? itemAc["CourseNameThai"].ToString() : "";
                    ACSemesterObj.Course = itemAc["Course"] != null ? itemAc["Course"].ToString() : "";
                    ACSemesterObj.Credit = itemAc["Credit"] != null ? int.Parse(itemAc["Credit"].ToString()) : 0;
                    ACSemesterObj.CreditDesc = itemAc["CreditDesc"] != null ? itemAc["CreditDesc"].ToString() : "";
                    ACSemesterObj.AcademicYear = itemAc["AcademicYear"] != null ? itemAc["AcademicYear"].ToString() : "";
                    ACSemesterObj.Curriculum = itemAc["Curriculum"] != null ? itemAc["Curriculum"].ToString() : "";
                    ACSemesterObj.Semester = itemAc["Semester"] != null ? itemAc["Semester"].ToString() : "";
                    ACSemesterObj.keyId = itemAc.Id.ToString();
                    if (itemAc["Lecturer"] != null)
                    {
                        FieldUserValue[] listUser = itemAc["Lecturer"] as FieldUserValue[];
                        for (int i = 0; i < listUser.Count(); i++)
                        {
                            if (listUser[i] != listUser[listUser.Count() - 1])
                            {
                                ACSemesterObj.Lecturer += listUser[i].LookupValue.ToString() + ";" + Environment.NewLine;
                            }
                            else
                            {
                                ACSemesterObj.Lecturer += listUser[i].LookupValue.ToString() + ";";
                            }
                        }
                    }

                    listACItemSemester.Add(ACSemesterObj);
                }


                QueryUserData();
            }
            catch (Exception)
            {


            }

        }       
        private void QueryUserData()
        {
            TqfContext = new ClientContext("http://" + HostName + "/en/tqf");

            groupCollection = TqfContext.Web.SiteGroups;
            TqfContext.Load(groupCollection);
            TqfContext.ExecuteQueryAsync(new ClientRequestSucceededEventHandler(QuertUserItem), null);
        }
        public void QuertUserItem(object sender, ClientRequestSucceededEventArgs e)
        {
            Dispatcher.BeginInvoke(QueryUser);
        }
        public void QueryUser()
        {
            try
            {
                //b = groupCollection[30];
                foreach (var item in groupCollection)
                {
                    if (item.Title == "TQF")
                    {
                        b = item;
                    }
                }
                TqfContext.Load(b, g => g.Users, g => g.Title);
                TqfContext.ExecuteQueryAsync(new ClientRequestSucceededEventHandler(DispatcherAssignUser), null);

            }
            catch (Exception)
            {


            }

        }
        public void DispatcherAssignUser(object sender, ClientRequestSucceededEventArgs e)
        {
            Dispatcher.BeginInvoke(AssignUser);
        }
        public void AssignUser()
        {
            try
            {
                AssignYear();
                AssignLecUser();
            }
            catch (Exception)
            {

            }

        }
        #endregion

        #region Command 
        public void QueryCourseData()
        {
            try
            {
                List<ACItemSemester> listAC = listACItemSemester.Where(x => x.AcademicYear == comboBoxFirst1.SelectedValue.ToString() && x.Semester == (comboBoxFirst2.SelectedItem).ToString()).ToList();
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = listAC;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error Refresh Data");
            }
        }
        public List<Group> GetAllGroup()
        {
            listGroup = new List<Group>();
            foreach (var item in groupCollection)
            {
                listGroup.Add(item);
            }


            return listGroup;
        }
        public List<User> GetAllUser(Group groupItem)
        {
            ListUser = new List<User>();
            foreach (User itemGroup in groupItem.Users)
            {
                ListUser.Add(itemGroup);

            }
            return ListUser;
        }
        public FieldUserValue[] ConvertLoginName()
        {
            FieldUserValue[] listUserValue = new FieldUserValue[listBox1.Items.Count];
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                User requireduser = ACContext.Web.EnsureUser(listBox1.Items[i].ToString());
                listUserValue[i].LookupId = requireduser.Id;
            }
            return listUserValue;
        }

        public void UpdateData()
        {
            ACItemSemester acItemSemester = (ACItemSemester)dataGrid1.SelectedItem;

            using (var clientContext = new ClientContext("http://" + HostName + "/en/ac_admin"))
            {
                var list = clientContext.Web.Lists.GetByTitle("Subject By Semester");
                clientContext.Load(list);

                var productItem = list.GetItemById(acItemSemester.keyId);
                clientContext.Load(productItem);

                FieldUserValue[] listUserValue = new FieldUserValue[listBox1.Items.Count];
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    listUserValue[i] = FieldUserValue.FromUser(listBox1.Items[i].ToString());
                }
                productItem["Lecturer"] = listUserValue;

                productItem.Update();
                clientContext.Load(productItem);
                clientContext.ExecuteQueryAsync(null, null);


                ACItemSemester a = listACItemSemester.SingleOrDefault(x =>
                    x.Course == acItemSemester.Course &&
                    x.AcademicYear == acItemSemester.AcademicYear &&
                    x.Semester == acItemSemester.Semester &&
                    x.Curriculum == acItemSemester.Curriculum);

                a.Lecturer = "";
                for (int i = 0; i < listUserValue.Count(); i++)
                {
                    if (listUserValue[i] != listUserValue[listUserValue.Count() - 1])
                    {
                        a.Lecturer += listUserValue[i].LookupValue.ToString() + ";" + Environment.NewLine;
                    }
                    else
                    {
                        a.Lecturer += listUserValue[i].LookupValue.ToString() + ";";
                    }
                }
                QueryCourseData();
            }
        }
        public void HideControl()
        {
            grid2.Visibility = Visibility.Collapsed;
        }
        #endregion




    }
}
