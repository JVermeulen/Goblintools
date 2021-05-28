using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblintools.Types
{
    public class GTRecord
    {
        public Dictionary<string, string> Values { get; private set; }

        public long Timestamp { get; set; }
        public int Map { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Speed { get; set; }
        public double Heading { get; set; }
        public bool IsDead { get; set; }
        public bool InCombat { get; set; }
        public bool OnTaxi { get; set; }
        public bool IsFlying { get; set; }
        public bool IsIndoors { get; set; }
        public bool IsResting { get; set; }
        public bool IsMounted { get; set; }
        public bool IsSwimming { get; set; }
        public bool IsPVP { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public int XpMax { get; set; }
        public int XpRested { get; set; }
        public long Money { get; set; }
        public int Health { get; set; }
        public int HealthMax { get; set; }
        public int Power { get; set; }
        public int PowerMax { get; set; }

        public GTRecord()
        {
            Values = new Dictionary<string, string>();
        }

        public static GTRecord Create(string format, string data)
        {
            var record = new GTRecord();

            string[] header = format.Split(';');
            string[] value = data.Split(';');

            for (int i = 0; i < header.Length && i < value.Length; i++)
            {
                record.Values.Add(header[i], value[i]);
            }

            if (record.Values.ContainsKey("timestamp") && long.TryParse(record.Values["timestamp"], out long timestamp))
                record.Timestamp = timestamp;

            if (record.Values.ContainsKey("map") && int.TryParse(record.Values["map"], out int map))
                record.Map = map;

            if (record.Values.ContainsKey("x") && double.TryParse(record.Values["x"], out double x))
                record.X = x;

            if (record.Values.ContainsKey("y") && double.TryParse(record.Values["y"], out double y))
                record.Y = y;

            if (record.Values.ContainsKey("speed") && double.TryParse(record.Values["speed"], out double speed))
                record.Speed = speed;

            if (record.Values.ContainsKey("heading") && double.TryParse(record.Values["heading"], out double heading))
                record.Heading = heading;

            if (record.Values.ContainsKey("isDead") && byte.TryParse(record.Values["isDead"], out byte isDead))
                record.IsDead = (isDead == 1);

            if (record.Values.ContainsKey("inCombat") && byte.TryParse(record.Values["inCombat"], out byte inCombat))
                record.InCombat = (inCombat == 1);

            if (record.Values.ContainsKey("onTaxi") && byte.TryParse(record.Values["onTaxi"], out byte onTaxi))
                record.OnTaxi = (onTaxi == 1);

            if (record.Values.ContainsKey("isFlying") && byte.TryParse(record.Values["isFlying"], out byte isFlying))
                record.IsFlying = (isFlying == 1);

            if (record.Values.ContainsKey("isIndoors") && byte.TryParse(record.Values["isIndoors"], out byte isIndoors))
                record.IsIndoors = (isIndoors == 1);

            if (record.Values.ContainsKey("isResting") && byte.TryParse(record.Values["isResting"], out byte isResting))
                record.IsResting = (isResting == 1);

            if (record.Values.ContainsKey("isMounted") && byte.TryParse(record.Values["isMounted"], out byte isMounted))
                record.IsMounted = (isMounted == 1);

            if (record.Values.ContainsKey("isSwimming") && byte.TryParse(record.Values["isSwimming"], out byte isSwimming))
                record.IsSwimming = (isSwimming == 1);

            if (record.Values.ContainsKey("isPVP") && byte.TryParse(record.Values["isPVP"], out byte isPVP))
                record.IsPVP = (isPVP == 1);

            if (record.Values.ContainsKey("level") && int.TryParse(record.Values["level"], out int level))
                record.Level = level;

            if (record.Values.ContainsKey("xp") && int.TryParse(record.Values["xp"], out int xp))
                record.Xp = xp;

            if (record.Values.ContainsKey("xpMax") && int.TryParse(record.Values["xpMax"], out int xpMax))
                record.XpMax = xpMax;

            if (record.Values.ContainsKey("xpRested") && int.TryParse(record.Values["xpRested"], out int xpRested))
                record.XpRested = xpRested;

            if (record.Values.ContainsKey("money") && long.TryParse(record.Values["money"], out long money))
                record.Money = money;

            if (record.Values.ContainsKey("health") && int.TryParse(record.Values["health"], out int health))
                record.Health = health;

            if (record.Values.ContainsKey("healthMax") && int.TryParse(record.Values["healthMax"], out int healthMax))
                record.HealthMax = healthMax;

            if (record.Values.ContainsKey("power") && int.TryParse(record.Values["power"], out int power))
                record.Power = power;

            if (record.Values.ContainsKey("powerMax") && int.TryParse(record.Values["powerMax"], out int powerMax))
                record.PowerMax = powerMax;

            return record;
        }

        public GTPlayerState GetState()
        {
            if (IsDead)
                return GTPlayerState.Dead;
            else if (InCombat)
                return GTPlayerState.Combat;
            else if (OnTaxi)
                return GTPlayerState.Taxi;
            else if (IsMounted)
                return GTPlayerState.Mounted;
            else if (IsResting)
                return GTPlayerState.Resting;
            else
                return GTPlayerState.Normal;
        }
    }
}
