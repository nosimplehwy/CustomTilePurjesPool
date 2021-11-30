using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Logging;
using Crestron.RAD.Common.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTilePurjesPool
{
    class CustomTileTransport : ATransportDriver
    {
        public CustomTileTransport()
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "CustomTileTransport", "constructor");
        }
        public override void SendMethod(string message, object[] paramaters)
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "SendMethod", message);
        }

        public override void Start()
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Start", "Start method");
        }

        public override void Stop()
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Stop", "Stop method");
        }
    }
}
