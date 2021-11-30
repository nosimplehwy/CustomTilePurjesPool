using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.Logging;
using Crestron.RAD.Common.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomTilePurjesPool
{
    public class CustomTileProtocol : ABaseDriverProtocol
    {
        public event EventHandler<BoolAttributeChangedEventArgs> BoolAttributeChanged;
        public event EventHandler<StringAttributeChangedEventArgs> StringAttributeChanged;

        public CustomTileProtocol(ISerialTransport transport, byte id) : base(transport, id)
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "CustomTileProtocol", "constructor");
        }

        protected override void ConnectionChangedEvent(bool connection)
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Error, "ConnectionChangedEvent", connection.ToString());
        }

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Error, "ChooseDeconstructMethod", validatedData.Data);
        }

        public override void SetUserAttribute(string attributeId, string attributeValue)
        {
            if (string.IsNullOrEmpty(attributeValue))
            {
                CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Error, "SetUserAttribute",
                    "Attribute value is null or empty");
                return;
            }

            StringAttributeChanged?.Invoke(this,
                new StringAttributeChangedEventArgs { Id = attributeId, Value = attributeValue });
        }

        public override void SetUserAttribute(string attributeId, bool attributeValue)
        {
            BoolAttributeChanged?.Invoke(this,
                new BoolAttributeChangedEventArgs { Id = attributeId, Value = attributeValue });
        }

        public override void Dispose()
        {
            // Do nothing for now, this is due to a bug in the base class Dispose method
        }


        public class BoolAttributeChangedEventArgs : EventArgs
        {
            public string Id;
            public bool Value;
        }

        public class StringAttributeChangedEventArgs : EventArgs
        {
            public string Id;
            public string Value;
        }
    }
}