﻿using Goblintools.RPI.Processing;
using Iot.Device.Ssd13xx;
using Iot.Device.Ssd13xx.Commands;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Clear();
                //SetValue($"0123456789");
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

        public void Clear()
        {
            Oled.SendCommand(new Commands.SetColumnAddress());
            Oled.SendCommand(new Commands.SetPageAddress(Commands.PageAddress.Page0, Commands.PageAddress.Page3));

            for (int i = 0; i < 32; i++)
            {
                byte[] data = new byte[16];

                Oled.SendData(data);
            }
        }

        public void SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                value = null;

            if (Oled != null)
            {
                if (value == null)
                {
                    Clear();
                }
                else
                {
                    Clear();

                    Oled.SendCommand(new Commands.SetColumnAddress());
                    Oled.SendCommand(new Commands.SetPageAddress(Commands.PageAddress.Page0, Commands.PageAddress.Page3));

                    foreach (char character in value)
                    {
                        var data = BasicFont.GetCharacterBytes(character);

                        Oled.SendData(data);
                    }

                    //byte[] buffer = new byte[128 * 4];

                    //int index = 0;

                    //for (int i = 0; i < value.Length; i++)
                    //{
                    //    var data = BasicFont.GetCharacterBytes(value[i]);

                    //    data.CopyTo(buffer, index);

                    //    index += data.Length;
                    //}

                    //for (int i = 0; i < buffer.Length; i += 16)
                    //{
                    //    byte[] data = buffer.Skip(i * 16).Take(16).ToArray();

                    //    Oled.SendData(data);
                    //}
                }

                //Oled.SendCommand(new Commands.SetColumnAddress());
                //Oled.SendCommand(new Commands.SetPageAddress(Commands.PageAddress.Page0, Commands.PageAddress.Page3));




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
