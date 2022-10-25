using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class ApplicationDeskHistory
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public int? StaffID { get; set; }
        public string Comment { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
