using Goblintools.RPI.Processing;
using Iot.Device.Ssd13xx;
using Iot.Device.Ssd13xx.Commands;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using Commands = Iot.Device.Ssd13xx.Commands.Ssd1306Commands;

namespace Goblintools.RPI.Actors
{
    public class OledActor : I2cActor
    {
        public override string Code => "SSD1306";

        public Observation Value { get; private set; }
        private Ssd1306 Oled { get; set; }

        public OledActor(string friendlyName) : base(friendlyName, 0x3D, TimeSpan.FromSeconds(1))
        {
            Value = new Observation(Category, FriendlyName, null, string.Empty, Code);

            ValueChanged.OnReceive.Subscribe(o => Value = o);

            if (Device != null)
            {
                Oled = new Ssd1306(Device);
                Initialize();
                SetValue(null);
                SetValue($"0x{0x3D.ToString("X2")}");
            }

            HardwareDevice = new HardwareDevice
            {
                Name = Code,
                Description = FriendlyName,
                Type = "I2C",
                Address = $"0x{0x3D.ToString("X2")}",
                Manufacturer = "Adafruit",
                Reference = "https://www.adafruit.com/product/938",
            };
        }

        private void Initialize()
        {
            Oled.SendCommand(new SetDisplayOff());
            Oled.SendCommand(new Commands.SetDisplayClockDivideRatioOscillatorFrequency(0x00, 0x08));
            Oled.SendCommand(new SetMultiplexRatio(0x1F));
            Oled.SendCommand(new Commands.SetDisplayOffset(0x00));
            Oled.SendCommand(new Commands.SetDisplayStartLine(0x00));
            Oled.SendCommand(new Commands.SetChargePump(true));
            Oled.SendCommand(new Commands.SetMemoryAddressingMode(Commands.SetMemoryAddressingMode.AddressingMode.Horizontal));
            Oled.SendCommand(new Commands.SetSegmentReMap(true));
            Oled.SendCommand(new Commands.SetComOutputScanDirection(false));
            Oled.SendCommand(new Commands.SetComPinsHardwareConfiguration(false, false));
            Oled.SendCommand(new SetContrastControlForBank0(0x8F));
            Oled.SendCommand(new Commands.SetPreChargePeriod(0x01, 0x0F));
            Oled.SendCommand(new Commands.SetVcomhDeselectLevel(Commands.SetVcomhDeselectLevel.DeselectLevel.Vcc1_00));
            Oled.SendCommand(new Commands.EntireDisplayOn(false));
            Oled.SendCommand(new Commands.SetNormalDisplay());
            Oled.SendCommand(new SetDisplayOn());
            Oled.SendCommand(new Commands.SetColumnAddress());
            Oled.SendCommand(new Commands.SetPageAddress(Commands.PageAddress.Page1, Commands.PageAddress.Page3));
        }

        public override void Work(object value)
        {
            try
            {
                if (value is Heartbeat heartbeat)
                    OnHeartbeat(heartbeat);
            }
            catch (Exception ex)
            {
                WriteToConsole(ex.Message, ConsoleColor.Red);
            }
        }

        public void OnHeartbeat(Heartbeat heartbeat)
        {
            //
        }

        public void SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                value = null;

            if (Oled != null)
            {
                Oled.SendCommand(new Commands.SetColumnAddress());
                Oled.SendCommand(new Commands.SetPageAddress(Commands.PageAddress.Page0, Commands.PageAddress.Page3));

                for (int cnt = 0; cnt < 32; cnt++)
                {
                    byte[] data = new byte[16];

                    Oled.SendData(data);
                }

                if (value != null)
                {
                    Oled.SendCommand(new Commands.SetColumnAddress());
                    Oled.SendCommand(new Commands.SetPageAddress(Commands.PageAddress.Page0, Commands.PageAddress.Page3));

                    foreach (char character in value)
                    {
                        Oled.SendData(BasicFont.GetCharacterBytes(character));
                    }
                }

                Value = new Observation(Category, FriendlyName, value, value, Code);

                ValueChanged.Send(Value);
            }
        }

        public new void Dispose()
        {
            SetValue(null);

            base.Dispose();
        }
    }
}
