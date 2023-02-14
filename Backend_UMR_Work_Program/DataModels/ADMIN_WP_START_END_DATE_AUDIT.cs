﻿using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.DataModels;

public partial class ADMIN_WP_START_END_DATE_AUDIT
{
    public int Id { get; set; }

    public string? start_date { get; set; }

    public string? end_date { get; set; }

    public string? categories_Desc { get; set; }

    public string? Created_by { get; set; }

    public string? Updated_by { get; set; }

    public DateTime? Date_Created { get; set; }

    public DateTime? Date_Updated { get; set; }
}
