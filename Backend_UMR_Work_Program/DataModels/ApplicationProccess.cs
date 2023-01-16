using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.DataModels;

public partial class ApplicationProccess
{
    public int ProccessID { get; set; }

    public int CategoryID { get; set; }

    public int RoleID { get; set; }

    public int SBU_ID { get; set; }

    public int Sort { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public bool DeleteStatus { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
}
