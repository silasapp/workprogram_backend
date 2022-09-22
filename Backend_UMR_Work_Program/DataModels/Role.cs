﻿using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class Role
    {
        public Role()
        {
            Funcs = new HashSet<Functionality>();
        }

        public int id { get; set; }
        public string RoleId { get; set; } = null!;
        public string? Description { get; set; }
        public string? RoleName { get; set; }

        public virtual ICollection<Functionality> Funcs { get; set; }
    }
}