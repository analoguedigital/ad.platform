﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMethods.Survey.Models.Entities
{
    public interface IHasAdHocDataList
    {
        DataList DataList { set; get; }
    }
}