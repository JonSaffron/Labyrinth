using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labyrinth.Test
    {
    public static class Helpers
        {
        internal static bool IsAnythingMoving(this World world)
            {
            var result = GlobalServices.GameState.GetSurvivingInteractiveItems().Any(mi => mi.IsMoving);
            return result;
            }

        }
    }
