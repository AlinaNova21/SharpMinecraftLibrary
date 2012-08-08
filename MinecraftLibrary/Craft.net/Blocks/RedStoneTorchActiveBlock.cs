﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craft.Net.Server.Blocks
{
    /// <summary>
    /// An Active Redstone Torch (ID = 76)
    /// </summary>
    /// <remarks></remarks>
    public class RedstoneTorchActiveBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (76)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 76; }
        }
    }
}
