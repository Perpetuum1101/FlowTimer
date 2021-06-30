using Microsoft.EntityFrameworkCore;
using System;

namespace FlowTimer.Data
{
    public class FlowContext : DbContext
    {
        public DbSet<Record> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=record.db");
    }

    public class Record
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public Status Status { get; set; }
        public int CurrentTimeSeconds { get; set; }
        public int TotalTimeMinutes { get; set; }
    }

    public enum Status
    {
        New,
        InProgress,
        Stoped,
        Completed
    }
}
