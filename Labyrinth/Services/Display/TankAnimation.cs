using System;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.Display
    {
    class TankAnimation : IDynamicAnimation
        {
        private readonly Tank _tank;

        /// <summary>
        /// The name of the texture to render
        /// </summary>
        private const string TextureName = "Sprites/Monsters/Tank";

        /// <summary>
        /// How long it takes to play the whole animation in seconds
        /// </summary>
        private const float LengthOfTrackAnimation = 8 * Constants.GameClockResolution;

        private double _leftTrackTime;

        private double _rightTrackTime;

        public TankAnimation(Tank tank)
            {
            this._tank = tank;
            }

        public void Update(GameTime gameTime)
            {
            this._rightTrackTime += gameTime.ElapsedGameTime.TotalSeconds * (int) this._tank.RightTrack;
            this._leftTrackTime += gameTime.ElapsedGameTime.TotalSeconds * (int) this._tank.LeftTrack;
            }

        public void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            if (!this._tank.IsExtant)
                return;

            this._rightTrackTime %= LengthOfTrackAnimation;
            if (this._rightTrackTime < 0)
                this._rightTrackTime += LengthOfTrackAnimation;
            var pctPositionInRightTrackAnimation = this._rightTrackTime / LengthOfTrackAnimation;
            DrawTrack(spriteBatch, spriteLibrary, 32, pctPositionInRightTrackAnimation);

            this._leftTrackTime %= LengthOfTrackAnimation;
            if (this._leftTrackTime < 0)
                this._leftTrackTime += LengthOfTrackAnimation;
            var pctPositionInLeftTrackAnimation = this._leftTrackTime / LengthOfTrackAnimation;
            DrawTrack(spriteBatch, spriteLibrary, 64, pctPositionInLeftTrackAnimation);

            DrawHull(spriteBatch, spriteLibrary);

            DrawTurret(spriteBatch, spriteLibrary);
            }

        private void DrawTrack(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary, int p2, double pctPositionInTrackAnimation)
            {
            DrawParameters drawParameters = default;
            drawParameters.Texture = spriteLibrary.GetSprite(TextureName);
            var frameCount = (drawParameters.Texture.Width / Constants.TileLength);
            int frameIndex = (int) Math.Floor(frameCount * pctPositionInTrackAnimation);

            // Calculate the source rectangle of the current frame.
            drawParameters.AreaWithinTexture = new Rectangle(frameIndex * Constants.TileLength, p2, Constants.TileLength, Constants.TileLength);
            drawParameters.Position = this._tank.Position;
            drawParameters.Rotation = this._tank.HullRotation;

            // Draw the current frame.
            spriteBatch.DrawTexture(drawParameters);
            }

        private void DrawHull(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            DrawParameters drawParameters = default;
            drawParameters.Texture = spriteLibrary.GetSprite(TextureName);

            // Calculate the source rectangle of the current frame.
            drawParameters.AreaWithinTexture = new Rectangle(0, 0, Constants.TileLength, Constants.TileLength);
            drawParameters.Position = this._tank.Position;
            drawParameters.Rotation = this._tank.HullRotation;

            // Draw the current frame.
            spriteBatch.DrawTexture(drawParameters);
            }

        private void DrawTurret(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            Vector2 centreOfRotationForTurret = new Vector2(0, 3);
            double x = centreOfRotationForTurret.Y * Math.Sin(this._tank.HullRotation) + centreOfRotationForTurret.X * Math.Cos(this._tank.HullRotation);
            double y = centreOfRotationForTurret.Y * Math.Cos(this._tank.HullRotation) - centreOfRotationForTurret.X * Math.Sin(this._tank.HullRotation);
            Vector2 centreOfRotationForTurretAfterRotation = new Vector2((float) -x, (float) y);
            
            DrawParameters drawParameters = default;
            drawParameters.Texture = spriteLibrary.GetSprite(TextureName);

            // Calculate the source rectangle of the current frame.
            drawParameters.AreaWithinTexture = new Rectangle(32, 0, Constants.TileLength, Constants.TileLength);
            drawParameters.Position = this._tank.Position + centreOfRotationForTurretAfterRotation;
            drawParameters.Rotation = this._tank.TurretRotation;
            drawParameters.Centre = new Vector2(16, 19);    // this is always the same - this is the point where we rotate the turret around in the source rectangle

            // Draw the current frame.
            spriteBatch.DrawTexture(drawParameters);
            }
        }
    }
