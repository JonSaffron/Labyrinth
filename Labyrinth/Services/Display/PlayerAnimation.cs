using System;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    internal class PlayerAnimation: IRenderAnimation
        {
        private readonly Player _player;
        private const double AnimationLength = 2 * Constants.GameClockResolution;
        private bool _whichFootFlag;

        private double TimeWithinAnimation { get; set; }

        public PlayerAnimation(Player player)
            {
            this._player = player ?? throw new ArgumentNullException(nameof(player));
            }

        public void Update(GameTime gameTime)
            {
            if (this._player.CurrentMovement.IsMoving)
                {
                this.TimeWithinAnimation = (this.TimeWithinAnimation + gameTime.ElapsedGameTime.TotalSeconds) % AnimationLength;
                }
            else
                {
                this.TimeWithinAnimation = 0;
                this._whichFootFlag = false;
                }
            }

        public void ResetAnimation()
            {
            this.TimeWithinAnimation = 0;
            this._whichFootFlag = false;
            }

        public void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            if (!this._player.IsAlive())
                return;

            var drawParameters = SetDrawParametersForFacedDirection(spriteLibrary);
            var frameCount = (drawParameters.Texture.Width / Constants.TileLength);
            var position = this.TimeWithinAnimation / AnimationLength;
            int frameIndex = (int) Math.Floor(frameCount * position);

            var currentFoot = position >= 0.5;
            if (this._whichFootFlag != currentFoot)
                {
                this._player.PlaySound(currentFoot ? GameSound.PlayerMovesFirstFoot : GameSound.PlayerMovesSecondFoot);
                this._whichFootFlag = currentFoot;
                }

            // Calculate the source rectangle of the current frame.
            drawParameters.AreaWithinTexture = new Rectangle(frameIndex * Constants.TileLength, 0, Constants.TileLength, Constants.TileLength);

            // Draw the current frame.
            spriteBatch.DrawTexture(drawParameters);
            }

        private DrawParameters SetDrawParametersForFacedDirection(ISpriteLibrary spriteLibrary)
            {
            DrawParameters drawParameters = default;
            switch (this._player.CurrentDirectionFaced)
                {
                case Direction.Left:
                    drawParameters.Texture = spriteLibrary.GetSprite("Sprites/Player/PlayerLeftFacing");
                    drawParameters.Effects = SpriteEffects.None;
                    break;
                case Direction.Right:
                    drawParameters.Texture = spriteLibrary.GetSprite("Sprites/Player/PlayerLeftFacing");
                    drawParameters.Effects = SpriteEffects.FlipHorizontally;
                    break;
                case Direction.Up:
                    drawParameters.Texture = spriteLibrary.GetSprite("Sprites/Player/PlayerUpFacing");
                    drawParameters.Effects = SpriteEffects.None;
                    break;
                case Direction.Down:
                    drawParameters.Texture = spriteLibrary.GetSprite("Sprites/Player/PlayerDownFacing");
                    drawParameters.Effects = SpriteEffects.None;
                    break;
                default:
                    throw new InvalidOperationException();
                }
            drawParameters.Position = this._player.Position;
            return drawParameters;
            }
        }
    }
