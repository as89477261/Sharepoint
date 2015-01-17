using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace SilverlightApplication1
{
    public class BasicHttpMessageInspectorBinding : BasicHttpBinding
    {
        private MessageInspectorBindingElement channelBindingElement;

        public BasicHttpMessageInspectorBinding(IClientMessageInspector messageInspector)
        {
            channelBindingElement = new MessageInspectorBindingElement();
            channelBindingElement.MessageInspector = messageInspector;
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection bindingElements = base.CreateBindingElements();
            bindingElements.Insert(bindingElements.Count - 1, channelBindingElement);

            return bindingElements;
        }
    }
}
