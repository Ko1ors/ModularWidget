﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularWidget.Data
{
    public class BlockReward : Root
    {
        [JsonProperty("result")]
        public BlockRewardResult Result;
    }
}
