using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public class WorldWindow : ICentrePointProvider
        {
        private static readonly Vector2 CentreOfRoom = Constants.RoomSizeInPixels / 2.0f;
        private const float CurrentVelocity = 750;

        public Vector2 WindowPosition { get; private set; }

        public Vector2 CentrePoint => this.WindowPosition + CentreOfRoom;

        public void ResetPosition(Vector2 newPosition)
            {
            this.WindowPosition = newPosition;
            }
        
        public Vector2 RecalculateWindow(GameTime gameTime)
            {
            var player = GlobalServices.GameState.Player;
            var roomRectangle = World.GetContainingRoom(player.Position);
            var movingTowards = new Vector2(roomRectangle.Left, roomRectangle.Top);
            Vector2 position = this.WindowPosition;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float remainingMovement = CurrentVelocity * elapsed;

            var distanceToDestination = Vector2.Distance(position, movingTowards);
            bool hasArrivedAtDestination = (distanceToDestination <= remainingMovement);
            if (hasArrivedAtDestination)
                {
                position = movingTowards;
                }
            else
                {
                var vectorBetweenPoints = movingTowards - position;
                var unitVectorOfTravel = Vector2.Normalize(vectorBetweenPoints);
                var displacement = unitVectorOfTravel * remainingMovement;
                position += displacement;
                }
            
            this.WindowPosition = position;
            return position;
            }
        }
    }
