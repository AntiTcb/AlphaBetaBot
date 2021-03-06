﻿using System;
using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace AlphaBetaBot
{
    public struct RaidTime
    {
        public static readonly string[] Formats = new [] { "M/d htt", "M/d h tt", "M/d/yyyy htt", "M/d/yyyy h tt" };

        private readonly DateTimeOffset _dto;
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
                var nowLocalDt = new LocalDateTime(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day, DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute);

                if (localDt.CompareTo(nowLocalDt) <= 0)
                {
                    localDt = localDt.PlusYears(1);
                }

                int offset = -8;
                if (TimeZoneInfo.Local.IsDaylightSavingTime(localDt.ToDateTimeUnspecified()))
                {
                    offset = -7;
                }

                var offsetDt = new OffsetDateTime(localDt, Offset.FromHours(offset));

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
