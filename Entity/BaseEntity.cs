﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public abstract class BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}