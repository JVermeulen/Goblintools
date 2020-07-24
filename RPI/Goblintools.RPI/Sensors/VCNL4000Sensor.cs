using Goblintools.RPI.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Goblintools.RPI.Sensors
{
    public class VCNL4000Sensor : I2cSensor
    {
        public const int DefaultI2cAddress = 0x13; //19

        private enum VCNL4000_Constants
        {
            VCNL4000_ADDRESS = 0x13,
            VCNL4000_COMMAND = 0x80,
            VCNL4000_PRODUCTID = 0x81,
            VCNL4000_IRLED = 0x83,
            VCNL4000_AMBIENTPARAMETER = 0x84,
            VCNL4000_AMBIENTDATA = 0x85,
            VCNL4000_PROXIMITYDATA_1 = 0x87,
            VCNL4000_PROXIMITYDATA_2 = 0x88,
            VCNL4000_SIGNALFREQ = 0x89,
            VCNL4000_PROXINITYADJUST = 0x8A,
            VCNL4000_3M125 = 0,
            VCNL4000_1M5625 = 1,
            VCNL4000_781K25 = 2,
            VCNL4000_390K625 = 3,
            VCNL4000_MEASUREAMBIENT = 0x10,
            VCNL4000_MEASUREPROXIMITY = 0x08,
            VCNL4000_AMBIENTREADY = 0x40,
            VCNL4000_PROXIMITYREADY = 0x20
        }

        // https://cdn-shop.adafruit.com/product-files/466/vcnl4010.pdf
        private enum VCNL4000_CommandRegister
        {
            Selftimed_en = 0,
            Prox_en = 1,
            Als_en = 2,
            Prox_od = 3,
            Als_od = 4,
            Prox_data_rdy = 5,
            Als_data_rdy = 6,
            Config_lock = 7,
        }

        private enum VCNL4000_Frequency
        {
            VCNL4000_3M125 = 0,
            VCNL4000_1M5625 = 1,
            VCNL4000_781K25 = 2,
            VCNL4000_390K625 = 3,
        }

        public override string Code => "VCNL4000";
        public Observation Proximity { get; private set; }
        public Observation AmbientLight { get; private set; }

        public VCNL4000Sensor(string friendlyName) : base(friendlyName, DefaultI2cAddress)
        {
            try
            {
                WriteToConsole($"Setting {Code} to 200 mA.", ConsoleColor.Cyan);

                Initialize();

                HardwareDevice = new HardwareDevice
                {
                    Name = Code,
                    Description = FriendlyName,
                    Type = "I2C",
                    Address = $"0x{DefaultI2cAddress.ToString("X2")}",
                    Manufacturer = "Adafruit",
                    Reference = "https://www.adafruit.com/product/466",
                };

                WriteToConsole($"{Code} productId: {ReadCommand1(VCNL4000_Constants.VCNL4000_IRLED)}", ConsoleColor.Cyan);
                WriteToConsole($"{Code} signal frequenct: {(VCNL4000_Frequency)ReadCommand1(VCNL4000_Constants.VCNL4000_SIGNALFREQ)}", ConsoleColor.Cyan);
            }
            catch (Exception ex)
            {
                WriteToConsole(ex.Message, ConsoleColor.Red);
            }
        }

        private void Initialize()
        {
            Device.Write(new byte[] { (byte)VCNL4000_Constants.VCNL4000_IRLED, 20 });
            Device.Write(new byte[] { (byte)VCNL4000_Constants.VCNL4000_PROXINITYADJUST, 0x81 });
        }

        private byte ReadCommand1(VCNL4000_Constants command)
        {
            byte[] writeBuffer = { (byte)command.GetHashCode() };
            var readBuffer = new byte[1];

            Device.WriteRead(writeBuffer, readBuffer);

            return readBuffer[0];
        }

        private int ReadCommand2(VCNL4000_Constants command)
        {
            byte[] writeBuffer = { (byte)command.GetHashCode() };
            var readBuffer = new byte[2];

            Device.WriteRead(writeBuffer, readBuffer);

            return readBuffer[0] << 8 | readBuffer[1];
        }

        public override void Start()
        {
            base.Start();

            Read();
        }

        public override void Work(object value)
        {
            try
            {
                if (value is Heartbeat heartbeat)
                    Read();
            }
            catch (Exception ex)
            {
                WriteToConsole(ex.Message, ConsoleColor.Red);
            }
        }

        private void RequestSensorToStartCollectingData()
        {
            var command = (byte)(VCNL4000_Constants.VCNL4000_MEASUREPROXIMITY.GetHashCode() | VCNL4000_Constants.VCNL4000_MEASUREAMBIENT.GetHashCode());

            WriteCommandToCommandRegister(command);
        }

        private void WriteCommandToCommandRegister(byte commandToWriteToCommandRegister)
        {
            var commandRegister = ReadCommandRegister();

            byte[] registerCommand = { (byte)(commandRegister | VCNL4000_Constants.VCNL4000_COMMAND.GetHashCode()), commandToWriteToCommandRegister };
            Device.Write(registerCommand);
        }

        private byte ReadCommandRegister()
        {
            var commandRegisterBuffer = new byte[1];

            Device.WriteRead(new[] { (byte)VCNL4000_Constants.VCNL4000_COMMAND }, commandRegisterBuffer);

            return commandRegisterBuffer[0];
        }

        public void Read()
        {
            try
            {
                if (!base.IsRunning)
                    throw new ApplicationException("Unable to read. Sensor has not started yet.");

                RequestSensorToStartCollectingData();

                SpinWait.SpinUntil(() => ProximityReady, TimeSpan.FromMilliseconds(300));
                if (ProximityReady)
                {
                    var proximity = ReadCommand2(VCNL4000_Constants.VCNL4000_PROXIMITYDATA_1);

                    Proximity = new Observation(Category, "Proximity", proximity, $"{proximity}", Code);

                    ValueChanged.Send(Proximity);
                }

                SpinWait.SpinUntil(() => AmbientLightReady, TimeSpan.FromMilliseconds(300));
                if (AmbientLightReady)
                {
                    var ambientLight = AmbientLightReady ? ReadCommand2(VCNL4000_Constants.VCNL4000_AMBIENTDATA) : -1;

                    AmbientLight = new Observation(Category, "AmbientLight", ambientLight, $"{ambientLight}lux", Code);

                    ValueChanged.Send(AmbientLight);
                }
            }
            catch (Exception ex)
            {
                WriteToConsole(ex.Message, ConsoleColor.Red);
            }
        }

        private bool ProximityReady => GetCommandRegister(VCNL4000_CommandRegister.Prox_data_rdy);
        private bool AmbientLightReady => GetCommandRegister(VCNL4000_CommandRegister.Als_data_rdy);

        private bool GetCommandRegister(VCNL4000_CommandRegister command)
        {
            var value = ReadCommandRegister();

            return ((value >> (int)command) & 1) != 0;
        }
    }
}
