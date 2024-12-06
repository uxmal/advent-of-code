using Advent2024.Day06.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2024.Day06.Core
{
    public class ViewModel
    {
        public bool Interactive { get; set; }

        public bool ExtraObstacle { get; set; }

        public LaboratoryState? State { get; set; }
    }
}
