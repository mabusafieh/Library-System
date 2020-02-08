using System;

namespace Library_System
{
    class Record
    {
        public Record(string person, string iSBN, DateTimeOffset timestamp, string action)
        {
            Person = person;
            ISBN = iSBN;
            Action = action;
            Timestamp = timestamp;
        }

        public string Person { get; set; }

        public string ISBN { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Action { get; set; }

        public long Ticks { get { return Timestamp.Ticks; } }
    }
}
