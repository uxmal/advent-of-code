using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2024.Day06.Core;

public class ArgsParser
{
    public static ViewModel EatArgs(string[] args)
    {
        bool interactive = false;
        bool extraObstacle = false;
        for (int iFile = 0; iFile < args.Length; iFile++)
        {
            if (args[iFile] == "-i")
            {
                interactive = true;
            }
            else if (args[iFile] == "-o")
            {
                extraObstacle = true;
            }
            else
            {
                var filename = args[iFile];
                var state = LaboratoryState.ReadLaboratoryMap(filename);
                return new ViewModel
                {
                    Interactive = interactive,
                    ExtraObstacle = extraObstacle,
                    State = state
                };

            }
        }
        throw new ArgumentException("No filename");
    }

}
