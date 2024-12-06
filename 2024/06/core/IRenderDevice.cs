using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2024.Day06.Core
{
    public interface IRenderDevice
    {
        void BeginFrame();
        void Clear();
        char ReadKey();
        void WriteGlyph(char c);
        void WriteLine();
        void WriteStatusLine(string v);
    }
}
