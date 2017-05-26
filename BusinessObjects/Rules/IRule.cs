﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.BusinessObjects
{
    public interface IRule
    {
        bool IsSatisfied(params object[] variables);
        object Results { get; }
    }
}
