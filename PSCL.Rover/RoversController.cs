using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PSCL.Rover
{
    public class RoversController
    {
        #region Private Fields

        // Private array containing a list of rover instructions.
        private string [] roverInstructions;

        // Private array containing the rovers for this instance.
        private Rover [] rovers;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Creates a set of rovers from a file.
        /// </summary>
        /// <param name="instructionFileName">The filename.</param>
        public RoversController (string instructionFileName)
        {
            if (string.IsNullOrWhiteSpace (instructionFileName))
            {
                throw new ArgumentNullException (nameof(instructionFileName));
            }
            if (!File.Exists (instructionFileName))
            {
                throw new FileNotFoundException (nameof (instructionFileName));
            }

            using (var fileStream = new FileStream (instructionFileName, FileMode.Open, FileAccess.Read))
            {
                InitialiseFromStream (fileStream);
                fileStream.Close ();
            }
        }

        /// <summary>
        /// Creates a set of rovers from a stream.
        /// </summary>
        /// <param name="instructionFileStream">The input stream. Will be left open.</param>
        public RoversController (Stream instructionFileStream)
        {
            if (instructionFileStream == null)
            {
                throw new ArgumentNullException (nameof (instructionFileStream));
            }

            InitialiseFromStream (instructionFileStream);
        }

        #endregion

        #region Pulic Methods
        
        // Orders all the rovers to drive.
        public string Execute()
        {
            var resultBuilder = new StringBuilder ();

            if (rovers == null)
            {
                throw new InvalidOperationException ("Not initialised.");
            }

            for (var countRover = 0; countRover < rovers.Length; countRover++)
            {
                var currentRover = rovers [countRover];
                currentRover.Drive (roverInstructions[countRover]);

                resultBuilder.AppendLine ($"{currentRover.X} {currentRover.Y} {currentRover.OrientationAsString}");
            }

            var result = resultBuilder.ToString ();

            return result;
        }

        #endregion

        #region

        private void InitialiseFromStream (Stream instructionStream)
        {

            int boundaryX = 0;
            int boundaryY = 0;
            var rovers = new List<Rover> ();
            var roverInstructions = new List<string> ();

            using (var streamReader = new StreamReader (instructionStream, System.Text.Encoding.UTF8, true, 4096, true))
            {
                // Obtain the boundaries.
                var boundariesLine = streamReader.ReadLine ();
                var boundariesSplit = boundariesLine.Split (' ');

                if (boundariesSplit.Length != 2)
                {
                    throw new InvalidOperationException ($"Plateau definition in unsupported format '{boundariesLine}'.");
                }
                if (!int.TryParse(boundariesSplit [0], out boundaryX))
                {
                    throw new InvalidOperationException ($"Plateau definition in unsupported format '{boundariesLine}'.");
                }
                if (!int.TryParse (boundariesSplit [1], out boundaryY))
                {
                    throw new InvalidOperationException ($"Plateau definition in unsupported format '{boundariesLine}'.");
                }
                if ((boundaryX <= 0) || (boundaryY <= 0))
                {
                    throw new InvalidOperationException ($"Plateau definition in unsupported format '{boundariesLine}'.");
                }

                // Initialise the rovers.
                int countRover = 1;
                int line = 2;
                while (streamReader.Peek() != -1)
                {
                    // Determine rover location and orientation.
                    var roverLocationAndOrientation = streamReader.ReadLine ();
                    if (string.IsNullOrWhiteSpace(roverLocationAndOrientation))
                    {
                        break;
                    }

                    var roverLocationAndOrientationSplit = roverLocationAndOrientation.Split(' ');

                    RoverOrientation roverOrientation;
                    int roverX = 0;
                    int roverY = 0;
                    if (roverLocationAndOrientationSplit.Length != 3)
                    {
                        throw new InvalidOperationException ($"Rover definition {countRover} on line {line} in unsupported format '{roverLocationAndOrientation}'.");
                    }
                    if (!int.TryParse(roverLocationAndOrientationSplit [0], out roverX))
                    {
                        throw new InvalidOperationException ($"Rover definition {countRover} on line {line} in unsupported format '{roverLocationAndOrientation}'.");
                    }
                    if (!int.TryParse (roverLocationAndOrientationSplit [1], out roverY))
                    {
                        throw new InvalidOperationException ($"Rover definition {countRover} on line {line} in unsupported format '{roverLocationAndOrientation}'.");
                    }
                    if ((roverX < 0) || (roverX > boundaryX) || (roverY < 0) || (roverY > boundaryY))
                    {
                        throw new InvalidOperationException ($"Rover definition {countRover} on line {line} in unsupported format '{roverLocationAndOrientation}'.");
                    }
                    
                    switch (roverLocationAndOrientationSplit[2].ToUpper())
                    {
                        case "N":
                            roverOrientation = RoverOrientation.North;
                            break;
                        case "E":
                            roverOrientation = RoverOrientation.East;
                            break;
                        case "S":
                            roverOrientation = RoverOrientation.South;
                            break;
                        case "W":
                            roverOrientation = RoverOrientation.West;
                            break;

                        default:
                            throw new InvalidOperationException ($"Rover definition {countRover} on line {line} in unsupported format '{roverLocationAndOrientation}'.");
                    }

                    rovers.Add(new Rover (roverX, roverY,  roverOrientation, boundaryX, boundaryY));
                    line++;
                    
                    if (streamReader.Peek() == -1)
                    {
                        throw new InvalidOperationException ($"Rover definition {countRover} ends midway on line {line}.");
                    }
                    var roverInstruction = streamReader.ReadLine ();
                    roverInstructions.Add (roverInstruction);
                    line++;
                }

                if  (rovers.Count == 0)
                {
                    throw new InvalidOperationException ("No rovers defined in stream after plateau definition.");
                }
            }

            this.rovers = rovers.ToArray();
            this.roverInstructions = roverInstructions.ToArray ();
        }
        
        #endregion
    }
}
