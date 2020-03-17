using System;

namespace PSCL.Rover
{
    public class Rover
    {
        #region Constructor

        public Rover (int x, int y, RoverOrientation orientation, int boundaryX, int boundaryY)
        {
            X = x;
            Y = y;
            Orientation = orientation;
            BoundaryX = boundaryX;
            BoundaryY = boundaryY;
        }

        #endregion

        #region Public Properties

        public int BoundaryX { get; private set; }

        public int BoundaryY { get; private set; }

        public RoverOrientation Orientation { get; private set; }

        public string OrientationAsString
        {
            get
            {
                switch (Orientation)
                {
                    case RoverOrientation.North:
                        return "N";
                    case RoverOrientation.East:
                        return "E";
                    case RoverOrientation.South:
                        return "S";
                    case RoverOrientation.West:
                        return "W";

                    default:
                        throw new InvalidOperationException ("Unsupported orientation.");
                }
            }
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes a sequence of instructions.
        /// </summary>
        /// <param name="instructions">A sequence of instructions to the Rover. Safety features mean instructions to drive off the edge of the plateau are silently ignored.</param>
        public void Drive (string instructions)
        {
            if (string.IsNullOrWhiteSpace(instructions))
            {
                throw new ArgumentNullException (nameof(instructions));
            }
            var instructionsRegular = instructions.ToUpper ();

            for (var countChar = 0; countChar < instructionsRegular.Length; countChar++)
            {
                DriveStep (instructionsRegular[countChar]);
            }
        }

        #endregion

        #region Private Methods

        private void DriveStep (char instruction)
        {
            int orientationOrdinal = 0;

            switch (instruction)
            {
                case 'L':
                    orientationOrdinal = ((int) Orientation) - 1;
                    if (orientationOrdinal < 0)
                    {
                        orientationOrdinal = 3;
                    }
                    Orientation = (RoverOrientation) orientationOrdinal;
                    break;

                case 'M':
                    switch (Orientation)
                    {
                        case RoverOrientation.North:
                            if (Y < BoundaryY)
                            {
                                Y = Y + 1;
                            }
                            break;
                        case RoverOrientation.East:
                            if (X < BoundaryX)
                            {
                                X = X + 1;
                            }
                            break;
                        case RoverOrientation.South:
                            if (Y > 0)
                            {
                                Y = Y - 1;
                            }
                            break;
                        case RoverOrientation.West:
                            if (X > 0)
                            {
                                X = X - 1;
                            }
                            break;
                    }
                    break;

                case 'R':
                    orientationOrdinal = ((int) Orientation) + 1;
                    if (orientationOrdinal > 3)
                    {
                        orientationOrdinal = 0;
                    }
                    Orientation = (RoverOrientation) orientationOrdinal;
                    break;


                default:
                    throw new InvalidOperationException ($"Unknown Rover instruction character '{instruction}'");
            }
        }

        #endregion
    }
}
