﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craft.Net.Server.Blocks
{
    /// <summary>
    /// A Torch Block (ID = 50)
    /// </summary>
    /// <remarks></remarks>
    public class TorchBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (50)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 50; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonSolid; }
        }
    }
}
