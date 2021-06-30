using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowTimer.Data.Model
{
    public class ReportDTO
    {
        public DateTime Date { get; set; }
        public int TotalTime { get; set; }
        public int Completed { get; set; }
    }
}
