using Crestron.RAD.Common;
using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.Common.Logging;
using Crestron.RAD.DeviceTypes.ExtensionDevice;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;


namespace CustomTilePurjesPool
{
    public class CustomTile : AExtensionDevice, ICloudConnected
    {
        #region Constants

        //UI Definition
        private const string TileStatusKey = "TileStatus";
        private const string MainIconKey = "MainIcon";
        private const string MainPageTitleKey = "MainPageTitle";
        private const string MainPageSubHeaderKey = "MainPageSubHeader";
        private const string OnButtonKey = "OnText";
        private const string OffButtonKey = "OffText";


        #endregion Constants

        #region Fields

        private bool _deviceState;
        private PropertyValue<string> _tileStatusText;
        private PropertyValue<string> _tileStatusIcon;
        private PropertyValue<string> _mainPageTitle;
        private PropertyValue<string> _mainPageSubheader;
        private PropertyValue<string> _tileStatusOnText;
        private PropertyValue<string> _tileStatusOffText;

        private CustomTileProtocol _protocol;


        #endregion Fields

        #region Constructor
        public CustomTile()
        {

            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Constructor", "Purjes Pool");

            CreateDeviceDefinition();

        }

        #endregion Constructor

        #region AExtensionDevice Members

        protected override IOperationResult DoCommand(string command, string[] parameters)
        {

            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "DoCommand", command);

            if (string.IsNullOrEmpty(command))
                return new OperationResult(OperationResultCode.Error, "command string is empty");

            switch (command)
            {
                case "ToggleCommand":
                    {
                        CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Switch", command);
                        if (_deviceState == true)
                            OnTileStateOff();
                        else
                        {
                            OnTileStateOn();
                        }

                        break;
                    }
                case "OffCommand":
                    {
                        CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Switch", command);
                        OnTileStateOff();
                        break;
                    }
                case "OnCommand":
                    {
                        CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Switch", command);
                        OnTileStateOn();
                        break;
                    }
                default:
                    {
                        CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Switch", "Unhandled command!");
                        break;
                    }
            }

            Commit();
            return new OperationResult(OperationResultCode.Success);
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "SetDriverPropertyValue", propertyKey);

            return new OperationResult(OperationResultCode.Error, "The property with object does not exist.");

            
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "SetDriverPropertyValueWithObject", propertyKey);
            return new OperationResult(OperationResultCode.Error, "The property with object does not exist.");
        }

        #endregion AExtensionDevice Members

        #region ICloudConnected Members

        public void Initialize()
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Initialize", "CustomTile");

            var transport = new CustomTileTransport
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger,
                EnableRxDebug = InternalEnableRxDebug,
                EnableTxDebug = InternalEnableTxDebug
            };
            ConnectionTransport = transport;

            _protocol = new CustomTileProtocol(transport, Id)
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
            };

            _protocol.Initialize(DriverData);
            DeviceProtocol = _protocol;

        }


        #endregion ICloudConnected Members

        #region IConnection Members

        public override void Connect()
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "Connect", "Connect");

            Connected = true;
            UpdateTileState(_deviceState);

        }


        #endregion ICloudConnected Members

        #region Programmable Operations

        [ProgrammableOperation]
        public void SetHighTempOn()
        {
            UpdateTileState(true);
        }

        [ProgrammableOperation]
        public void SetLowTempOn()
        {
            UpdateTileState(false);
        }
        #endregion Programmable Operations

        #region Programmable Events


        [ProgrammableEvent("High Temp turned On")]
        public event EventHandler TileStateOn;

        [ProgrammableEvent("Low Temp turned On")]
        public event EventHandler TileStateOff;


        #endregion Programmable Events

        #region Private Methods
        private void CreateDeviceDefinition()
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "CreateDeviceDefinition", "");

            //Tile
            _tileStatusText = CreateProperty<string>(new PropertyDefinition(TileStatusKey, String.Empty, DevicePropertyType.String));
            _tileStatusIcon = CreateProperty<string>(new PropertyDefinition(MainIconKey, String.Empty, DevicePropertyType.String));

            //Main Page
            _mainPageTitle = CreateProperty<string>(new PropertyDefinition(MainPageTitleKey, String.Empty, DevicePropertyType.String));
            _mainPageSubheader = CreateProperty<string>(new PropertyDefinition(MainPageSubHeaderKey, String.Empty, DevicePropertyType.String));
            _tileStatusOnText = CreateProperty<string>(new PropertyDefinition(OnButtonKey, String.Empty, DevicePropertyType.String));
            _tileStatusOffText = CreateProperty<string>(new PropertyDefinition(OffButtonKey, String.Empty, DevicePropertyType.String));



            //Initialize property values
            _mainPageTitle.Value = "Pool";
            _tileStatusOnText.Value = "High Temp";
            _tileStatusOffText.Value = "Low Temp";
            _tileStatusIcon.Value = "icPoolRegular";

            
            UpdateTileState(false);

        }


        private void UpdateTileState(bool state)
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "UpdateTileState", state.ToString());
            _deviceState = state;
            _tileStatusText.Value = _deviceState == true ? _tileStatusOnText.Value : _tileStatusOffText.Value;
            _mainPageSubheader.Value = _deviceState == true ? _tileStatusOnText.Value : _tileStatusOffText.Value;

            Commit();

        }



        #endregion Private Methods

        #region Helper Methods

        private void OnTileStateOn()
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "RaiseStatusOnEvent", "Tile Turned On");
            TileStateOn?.Invoke(this, EventArgs.Empty);
        }

        private void OnTileStateOff()
        {
            CustomTileLog.Log(EnableLogging, Log, LoggingLevel.Debug, "RaiseStatusOffEvent", "Tile Turned Off");
            TileStateOff?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Events


        #endregion Events
    }
}
