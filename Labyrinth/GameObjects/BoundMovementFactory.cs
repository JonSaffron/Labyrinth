using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth.GameObjects
    {
    public class BoundMovementFactory : IBoundMovementFactory
        {
        public BoundMovementFactory()
            {

            }

        public IBoundMovement GetExplicitBoundary()
            {
            throw new NotImplementedException();
            }

        public IBoundMovement GetBoundedInRoom()
            {
            throw new NotImplementedException();
            }
        }
    }
