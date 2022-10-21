using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class MyDesk
    {
        public int DeskID { get; set; }
        public int ProcessID { get; set; }
        public int StaffID { get; set; }
        public int? AppId { get; set; }
        public int Sort { get; set; }
        public bool HasWork { get; set; }
        public bool HasPushed { get; set; }
        public int? FromStaffID { get; set; }
        public string? FromSBU { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Comment { get; set; }
    }
}
