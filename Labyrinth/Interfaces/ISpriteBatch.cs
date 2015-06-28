using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public interface ISpriteBatch : IDisposable  
        {
        /// <summary>
        /// Sets or returns the top-left co-ordinates of the game window's view upon the world
        /// </summary>
        Vector2 WindowOffset { get; set; }

        /// <summary>
        /// Begins a drawing batch
        /// </summary>
        void Begin();
        /// <summary>
        /// Signals the end of a drawing batch
        /// </summary>
        void End();

        /// <summary>
        /// Draws the specified texture at the specified co-ordinates
        /// </summary>
        /// <param name="texture">Specifies the texture to draw</param>
        /// <param name="position">Specifies the top-left corner co-ordinate to draw the texture</param>
        void DrawEntireTexture(Texture2D texture, Vector2 position);
        /// <summary>
        /// Draws the specified texture at the specified co-ordinates, specifying a source rectangle, rotation, origin and effectS
        /// </summary>
        /// <param name="texture">Specifies the texture to draw</param>
        /// <param name="position">Specifies the co-ordinate to draw the texture</param>
        /// <param name="sourceRectangle">Specifies the section of the texture to draw</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate around the origin</param>
        /// <param name="origin">Specifies the origin point of the texture</param>
        /// <param name="effects">Optionally specifies what effect(s) to apply to the texture</param>
        void DrawTexture(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects = SpriteEffects.None);
        /// <summary>
        /// Draws a solid rectangle at the specified co-ordinates in the specified colour
        /// </summary>
        /// <param name="r">The rectangle to draw</param>
        /// <param name="colour">The colour to draw the rectangle in</param>
        void DrawRectangle(Rectangle r, Color colour);
        }
    }
