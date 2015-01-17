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
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace SilverlightApplication1
{
    public interface IClientMessageInspector
    {
        object BeforeSendRequest(ref Message request, IClientChannel channel);
        void AfterReceiveReply(ref Message reply, object correlationState);
    }
}
