using System;
using System.Collections.Generic;
using Labyrinth.Services.Display;

namespace Labyrinth
    {
    interface IAnimationState
        {
        IEnumerable<DrawParameters> GetDrawParameters();
        }
    }
