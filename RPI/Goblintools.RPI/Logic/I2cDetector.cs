using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Text;
using System.Threading.Tasks;

namespace Goblintools.RPI.Logic
{
    public class I2cDetector
    {
        protected int BusId { get; set; }

        public I2cDetector()
        {
            BusId = 1;
        }

        public bool[] Detect()
        {
            var result = new bool[0x7f];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Detect(i);
            }

            return result;
        }

        public static List<int> GetActiveAddresses()
        {
            var result = new List<int>();

            var detector = new I2cDetector();

            for (int i = 0; i < 0x7f; i++)
            {
                var isActive = detector.Detect(i);

                if (isActive)
                    result.Add(i);
            }

            return result;
        }

        public bool Detect(int deviceAddress)
        {
            try
            {
                var settings = new I2cConnectionSettings(BusId, deviceAddress);

                using (var device = I2cDevice.Create(settings))
                {
                    var testCommand = new byte[] { 0x00, 0x0 };
                    device.Read(testCommand);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
