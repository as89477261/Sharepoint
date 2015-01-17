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
using System.ServiceModel;
using SilverlightApplication1.UPSLibrary.UserProfileManager;
using SLLecAssignment;

using AsmxKnownTypeTest;


using System.Collections.Generic;
using SLLecAssignment.TQFUserProfileService;
using SilverlightApplication1.Model;


namespace SilverlightApplication1.UPSLibrary.UserProfileManager
{
    public class UPSManager
    {
        UserProfileServiceSoapClient _proxy;

        //public long IndexCount { get; set; }
        public string IndexCount { get; set; }
        public List<UPSModel> ListUserName { get; set; }
        public UPSModel UserProfile { get; set; }
        string HostName;

        public UPSManager()
        {
            HostName = Application.Current.Host.Source.Host;
            ListUserName = new List<UPSModel>();
            SPAsmxMessageInspector messageInspector = new SPAsmxMessageInspector();
            BasicHttpMessageInspectorBinding miBinding = new BasicHttpMessageInspectorBinding(messageInspector);
            EndpointAddress endpoint = new EndpointAddress("http://" + HostName + "/eng/tqf/_vti_bin/userprofileservice.asmx");

            _proxy = new UserProfileServiceSoapClient(miBinding, endpoint);
            _proxy.Endpoint.Behaviors.Add(new AsmxBehavior());

            _proxy.GetUserProfileByIndexCompleted += new EventHandler<GetUserProfileByIndexCompletedEventArgs>(GetListUserProfileAsync);
            _proxy.GetCommonColleaguesCompleted += new EventHandler<GetCommonColleaguesCompletedEventArgs>(GetIndexUserProfileAsync);
            _proxy.GetUserProfileByNameCompleted += new EventHandler<GetUserProfileByNameCompletedEventArgs>(GetUserProfileAsync);
        }
        public void GetUserProfile(string UserName)
        {
            _proxy.GetUserProfileByNameAsync(UserName);
        }
        void GetUserProfileAsync(object sender, GetUserProfileByNameCompletedEventArgs e)
        {
            UserProfile = new UPSModel()
            {
                UserName = e.Result[1].Values.Count > 0 ? e.Result[1].Values[0].Value.ToString() : "",
                Name = e.Result[2].Values.Count > 0 ? e.Result[2].Values[0].Value.ToString() : "",
                Email = e.Result[42].Values.Count > 0 ? e.Result[42].Values[0].Value.ToString() : ""
                //UserName = e.Result[1].Values[0].Value.ToString(),
                //Name = e.Result[2].Values[0].Value.ToString(),
                //Email = e.Result[42].Values[0].Value.ToString()
            };
        }

        public void GetIndexUserProfile()
        {
           // _proxy.GetUserProfileCountAsync();
            _proxy.GetCommonColleaguesAsync("eng\\suparerk_man");
        }
        private void GetIndexUserProfileAsync(object sender, GetCommonColleaguesCompletedEventArgs e)
        {
           // IndexCount = e.Result;
            IndexCount = e.Result[0].AccountName[0].ToString();

        }
        public void GetListUserProfile()
        {
            //for (int i = 0; i < IndexCount; i++)
            //{
            //    _proxy.GetUserProfileByIndexAsync(i);
            //}

        }

        void GetListUserProfileAsync(object sender, GetUserProfileByIndexCompletedEventArgs e)
        {

            if (e.Error == null)
            {
                ListUserName.Add(new UPSModel()
                {
                    UserName = e.Result.UserProfile[1].Values.Count > 0 ? e.Result.UserProfile[1].Values[0].Value.ToString() : "",
                    Name = e.Result.UserProfile[2].Values.Count > 0 ? e.Result.UserProfile[2].Values[0].Value.ToString() : "",
                    Email = e.Result.UserProfile[42].Values.Count > 0 ? e.Result.UserProfile[42].Values[0].Value.ToString() : ""
                });
            }

        }
    }
}
