using System;

namespace FlowTimer.Data.Model
{
    public class ItemDTO
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        public Status Status { get; set; }
        public int TotalTimeMinutes { get; set; }
        public int CurrentTimeSeconds { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
