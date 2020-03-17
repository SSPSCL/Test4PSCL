using System;
using System.IO;

namespace PSCL.RoverHost
{
    class Program
    {
        static int Main (string [] args)
        {
            if ((args == null) || (args.Length != 1))
            {
                Console.WriteLine ("RoverHost requires a single argument, the name of a plain text file containing a plateau definition and rover instructions.");

                return 1;
            }
            if (!File.Exists(args [0]))
            {
                Console.WriteLine ("RoverHost requires a single argument, the name of a plain text file containing a plateau definition and rover instructions.");

                return 1;
            }

            var roverController = new PSCL.Rover.RoversController (args [0]);
            var output = roverController.Execute ();
            Console.Write (output);

            return 0;
        }
    }
}
