using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public interface ISpriteBatch : IDisposable  
        {
        /// <summary>
        /// Begins a drawing batch
        /// </summary>
        /// <param name="windowPosition">Sets the top-left co-ordinates of the game window's view upon the world</param>
        void Begin(Vector2 windowPosition);

        /// <summary>
        /// Signals the end of a drawing batch
        /// </summary>
        void End();

        /// <summary>
        /// Draws the specified texture at the specified co-ordinates relative to the window position
        /// </summary>
        /// <param name="texture">Specifies the texture to draw</param>
        /// <param name="relativePosition">Specifies the top-left corner co-ordinate to draw the texture</param>
        void DrawEntireTextureInWindow(Texture2D texture, Vector2 relativePosition);

        /// <summary>
        /// Draws the specified texture at the absolute co-ordinates specified
        /// </summary>
        /// <param name="texture">Specifies the texture to draw</param>
        /// <param name="absolutePosition">Specifies the top-left corner co-ordinate to draw the texture</param>
        void DrawEntireTexture(Texture2D texture, Vector2 absolutePosition);

        /// <summary>
        /// Draws the specified texture at the specified co-ordinates relative to the window position, specifying a source rectangle, rotation, origin and effects
        /// </summary>
        /// <param name="texture">Specifies the texture to draw</param>
        /// <param name="relativePosition">Specifies the top-left corner co-ordinate to draw the texture</param>
        /// <param name="sourceRectangle">Specifies the section of the texture to draw</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate around the origin</param>
        /// <param name="origin">Specifies the origin point of the texture</param>
        /// <param name="effects">Specifies what effect(s) to apply to the texture</param>
        void DrawTextureInWindow(Texture2D texture, Vector2 relativePosition, Rectangle? sourceRectangle, float rotation, Vector2 origin, SpriteEffects effects);

        /// <summary>
        /// Draws the specified texture at the sbsolute co-ordinates specified, specifying a source rectangle
        /// </summary>
        /// <param name="texture">Specifies the texture to draw</param>
        /// <param name="absolutePosition">Specifies the top-left corner co-ordinate to draw the texture</param>
        /// <param name="sourceRectangle">Specifies the section of the texture to draw</param>
        void DrawTexture(Texture2D texture, Vector2 absolutePosition, Rectangle? sourceRectangle);

        /// <summary>
        /// Draws a solid rectangle at the specified co-ordinates in the specified colour
        /// </summary>
        /// <param name="r">The rectangle to draw</param>
        /// <param name="colour">The colour to draw the rectangle in</param>
        void DrawRectangle(Rectangle r, Color colour);

        /// <summary>
        /// Draws text to the window
        /// </summary>
        /// <param name="font">The font to render the text in</param>
        /// <param name="text">The text to draw</param>
        /// <param name="pos">The position to draw the text at, relative to the origin</param>
        /// <param name="color">The colour to draw the text in</param>
        /// <param name="origin">The origin to calculate position relative to</param>
        void DrawString(SpriteFont font, string text, Vector2 pos, Color color, Vector2 origin);

        /// <summary>
        /// The magnification applied to the display
        /// </summary>
        float Zoom { get; }
        }
    }
