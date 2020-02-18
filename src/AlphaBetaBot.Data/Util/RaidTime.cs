using System;
using NodaTime;
using NodaTime.Text;

namespace AlphaBetaBot
{
    public struct RaidTime
    {
        public static readonly string[] Formats = new [] { "M/d htt", "M/d h tt", "M/d/yyyy htt", "M/d/yyyy h tt" };

        private DateTimeOffset _dto;
        private OffsetDateTime _odt;

        public RaidTime(OffsetDateTime dateTime)
        {
            _odt = dateTime;
            _dto = _odt.ToDateTimeOffset();
        }

        public static implicit operator DateTimeOffset(RaidTime value)
            => value._dto;

        public static RaidTime Parse(string value)
        {
            foreach (string format in Formats)
            {
                var parse = LocalDateTimePattern.CreateWithInvariantCulture(format).Parse(value);

                if (!parse.Success)
                    continue;

                var localDt = new LocalDateTime(DateTimeOffset.Now.Year, parse.Value.Month, parse.Value.Day, parse.Value.Hour, parse.Value.Minute);
                var offsetDt = new OffsetDateTime(localDt, Offset.FromHours(-8));
                return new RaidTime(offsetDt);
            }

            throw new FormatException("Couldn't parse value to any format");
        }

        public static bool TryParse(string value, out RaidTime raidTime)
        {
            try
            {
                raidTime = Parse(value);
                return true;
            }
            catch
            {
                raidTime = default;
                return false;
            }
        }

        public override string ToString() => _dto.ToString("MM/dd @ hh tt");
    }
}
