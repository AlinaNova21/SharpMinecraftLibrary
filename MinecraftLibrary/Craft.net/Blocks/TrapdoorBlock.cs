﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craft.Net.Server.Blocks
{
    /// <summary>
    /// A Trapdoor block (ID = 96)
    /// </summary>
    /// <remarks></remarks>
    public class TrapdoorBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (96)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 96; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonCubeSolid; }
        }
    }
}
