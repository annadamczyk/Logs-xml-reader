using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Service.Models
{
    public class Log: IComparable<Log>
    {
        public string Logger { get; set; }
        public long Timestamp { get; set; }
        public string Level { get; set; }
        public string Thread { get; set; }
        public string Message { get; set; }

        public int CompareTo(Log other)
        {
            return other.Level.CompareTo(this.Level);
        }
    }
}