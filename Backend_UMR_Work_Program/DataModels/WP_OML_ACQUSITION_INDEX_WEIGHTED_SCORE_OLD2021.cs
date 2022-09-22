﻿using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class WP_OML_ACQUSITION_INDEX_WEIGHTED_SCORE_OLD2021
    {
        public string? CompanyName { get; set; }
        public int? Quantum_Acquired { get; set; }
        public int? Quantum_Approved { get; set; }
        public decimal? Unscaled_Score { get; set; }
        public decimal? Unscaled_Score_sum { get; set; }
        public decimal? Scaled_by_Reciprocal_GrandTotal_RGT { get; set; }
        public string? Year_of_WP { get; set; }
        public decimal? MAX_RGT { get; set; }
        public decimal? MIN_RGT { get; set; }
        public decimal? Recalibrated_Scaled_Index { get; set; }
        public string? INDEX_TYPE { get; set; }
        public string? Weight { get; set; }
        public decimal? Weighted_Score { get; set; }
        public string Consession_Type { get; set; } = null!;
    }
}