using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace AsmxKnownTypeTest
{
    public class AsmxMessageInspector : IClientMessageInspector
    {
        XNamespace WcfNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";
        XNamespace XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            MemoryStream ms = new MemoryStream();
            XmlDictionaryWriter w = XmlDictionaryWriter.CreateTextWriter(ms);
            reply.WriteMessage(w);
            w.Flush();
            ms.Position = 0;
            XElement envelope = XElement.Load(ms);

            XName XsiTypeAttirbute = XsiNamespace + "type";

            IEnumerable<XElement> asmxTypes = envelope.DescendantsAndSelf().Where<XElement>(
                x => x.Attributes().Any<XAttribute>(
                    y => y.Name.Equals(XsiTypeAttirbute) &&
                         (y.Value.EndsWith(":char") || y.Value.EndsWith(":guid"))));

            if (asmxTypes.Count<XElement>() != 0)
            {
                XName WcfNamespaceAttribute = XNamespace.Xmlns + "wcf";

                foreach (XElement e in asmxTypes)
                {
                    e.SetAttributeValue(WcfNamespaceAttribute, WcfNamespace);
                    XAttribute typeAttribute = e.Attribute(XsiTypeAttirbute);
                    typeAttribute.Value = "wcf:" + typeAttribute.Value.Split(':')[1];
                }

            }

            Message newMessage = Message.CreateMessage(envelope.CreateReader(), int.MaxValue, reply.Version);
            newMessage.Properties.CopyProperties(reply.Properties);

            reply = newMessage;
        }


        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            return null;
        }
    }
}
