using FlowTimer.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowTimer.Data
{
    public class Repository
    {
        public Repository()
        {
            using (var context = new FlowContext())
            {
                context.Database.EnsureCreated();
            }
        }

        public async Task<ItemDTO> Create(string text)
        {
            using (var context = new FlowContext())
            {
                var record = new Record
                {
                    Text = text,
                    Status = Status.New,
                    Timestamp = DateTime.Now
                };

                context.Records.Add(record);
                await context.SaveChangesAsync();

                var result = new ItemDTO
                {
                    Id = record.Id,
                    Text = record.Text,
                    Timestamp = record.Timestamp,
                    Status = record.Status
                };

                return result;
            }
        }

        public void Delete(int id)
        {
            using (var context = new FlowContext())
            {
                context.Remove(new Record { Id = id });
            }
        }

        public async Task Update(ItemDTO item)
        {
            using (var context = new FlowContext())
            {
                var record = context.Records.FirstOrDefault(x => x.Id == item.Id.Value);
                if (record == null)
                {
                    return;
                }
                record.Status = item.Status;
                record.Text = item.Text;
                record.TotalTimeMinutes = item.TotalTimeMinutes;
                record.CurrentTimeSeconds = item.CurrentTimeSeconds;
                context.Update(record);

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ItemDTO>> GetItems()
        {
            using (var context = new FlowContext())
            {
                var today = DateTime.Today;
                var recentPast = today.AddDays(-4);
                var records = await context.Records
                    .Where(x => (x.Timestamp >= today && x.Timestamp < today.AddDays(1)) ||
                    ((x.Status == Status.Stoped || x.Status == Status.New) && x.Timestamp > recentPast))
                    .OrderByDescending(x => x.Timestamp)
                    .ThenBy(x => x.Status)
                    .ToListAsync();
                var results = new List<ItemDTO>();
                foreach(var record in records)
                {
                    var item = RecordToItem(record);
                    results.Add(item);
                }

                return results;
            }
        }

        public async Task<List<ReportDTO>> GetForReportSummary()
        {
            using (var context = new FlowContext())
            {
                var result = await context.Records
                    .GroupBy(x => new { Date = x.Timestamp.Date })
                    .Select(
                    x => new ReportDTO()
                    {
                        Date = x.Key.Date,
                        TotalTime = x.Sum(r => r.TotalTimeMinutes),
                        Completed = x.Count(r => r.Status == Status.Completed),
                    })
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                return result;
            }
        }

        public async Task<List<ItemDTO>> GetForReportFull()
        {
            using (var context = new FlowContext())
            {
                var records = await context.Records
                    .OrderByDescending(x => x.Timestamp)
                    .ThenBy(x =>x.Status)
                    .ToListAsync();

                var results = new List<ItemDTO>();
                foreach (var record in records) 
                {
                    var item = RecordToItem(record);
                    results.Add(item);
                }

                return results;
            }
        }

        private static ItemDTO RecordToItem(Record record)
        {
            return new ItemDTO
            {
                Id = record.Id,
                Timestamp = record.Timestamp,
                CurrentTimeSeconds = record.CurrentTimeSeconds,
                Status = record.Status,
                Text = record.Text,
                TotalTimeMinutes = record.TotalTimeMinutes
            };
        }
    }
}
