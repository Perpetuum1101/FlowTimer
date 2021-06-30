using FlowTimer.Data;
using System;
using System.IO;
using System.Text;

namespace FlowTimer.UI.Utils
{
    public static class Report
    {
        const string separator = ",";

        public static async void GenerateSummary(Repository repo)
        {
            var items = await repo.GetForReportSummary();
            var csv = new StringBuilder();
            csv.AppendLine($"Date{separator}Total flow minutes{separator}Completed tasks");
            var totalTotalMinutes = 0;
            var totalCompleted = 0;
            foreach (var item in items)
            {
                totalTotalMinutes += item.TotalTime;
                totalCompleted += item.Completed;
                csv.AppendLine($"{item.Date.ToShortDateString()}{separator}{item.TotalTime}{separator}{item.Completed}");
            }

            csv.AppendLine($"Total days: {items.Count}{separator}Total minutes: {totalTotalMinutes}{separator} Total completed: {totalCompleted}");
            await File.WriteAllTextAsync($"Summary_{DateTime.Now.Ticks}.csv", csv.ToString());
        }

        public static async void GenerateFull(Repository repo)
        {
            var items = await repo.GetForReportFull();
            var csv = new StringBuilder();
            csv.AppendLine($"Date{separator}Task{separator}Time{separator}Status");
            foreach (var item in items)
            {
                csv.AppendLine($"{item.Timestamp:yyyy-MM-dd HH:mm}{separator}{item.Text}{separator}{item.TotalTimeMinutes}{separator}{item.Status}");
            }

            await File.WriteAllTextAsync($"Full_{DateTime.Now.Ticks}.csv", csv.ToString());
        }
    }
}
