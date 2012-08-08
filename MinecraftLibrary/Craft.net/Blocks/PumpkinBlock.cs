﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craft.Net.Server.Blocks
{
    /// <summary>
    /// A Pumpkin block (ID = 86)
    /// </summary>
    /// <remarks></remarks>
    public class PumpkinBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (86)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 86; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }
    }
}
