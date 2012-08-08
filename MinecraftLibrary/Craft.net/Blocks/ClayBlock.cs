using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craft.Net.Server.Blocks
{
    /// <summary>
    /// A Clay Block (ID = 82)
    /// </summary>
    /// <remarks></remarks>
    public class ClayBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (82)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 82; }
        }
    }
}