using System;

namespace Goblintools.RPI
{
    public class Color
    {
        public byte Brightness { get; set; }

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public Color(byte brightness, byte red = byte.MaxValue, byte green = byte.MaxValue, byte blue = byte.MaxValue)
        {
            Brightness = brightness;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Color CreateInversed()
        {
            return new Color((byte)(byte.MaxValue - Brightness), Red, Green, Blue);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = (Color)obj;

            return other.Brightness == Brightness && 
                other.Red == Red && 
                other.Green == Green && 
                other.Blue == Blue;
        }

        public override int GetHashCode()
        {
            var value = $"{Brightness}_{Red}_{Green}_{Blue}";

            return value.GetHashCode();
        }
    }
}
