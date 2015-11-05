using System.Linq;

namespace Labyrinth.Test
    {
    public static class Helpers
        {
        internal static bool IsAnythingMoving()
            {
            var result = GlobalServices.GameState.GetSurvivingInteractiveItems().Any(mi => mi.IsMoving);
            return result;
            }

        }
    }
