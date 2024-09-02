using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public interface ISpriteBatch : IDisposable  
        {
        /// <summary>
        /// Sets the offset for the view on the world for relative positioning
        /// </summary>
        Vector2 WindowPosition { set; }
        
        /// <summary>
        /// Returns the width of the screen divided by 2
        /// </summary>
        public int ScreenCentreWidth { get; }

        /// <summary>
        /// Returns the current scaling factor applied to text and textures drawn to the screen
        /// </summary>
        float Zoom { get; }

        /// <summary>
        /// Begins a drawing batch
        /// </summary>
        /// <param name="effect">The pixel shader to apply to the batch</param>
        void Begin(Effect? effect = null);

        /// <summary>
        /// Signals the end of a drawing batch
        /// </summary>
        void End();

        /// <summary>
        /// Draws the specified texture at the specified co-ordinates relative to the window position, specifying a source rectangle, rotation, origin and effects
        /// </summary>
        /// <param name="drawParameters">Specifies the values to use for the draw</param>
        void DrawTexture(DrawParameters drawParameters);

        /// <summary>
        /// Draws the specified texture at the absolute co-ordinates specified, specifying a source rectangle
        /// </summary>
        /// <param name="texture">Specifies the texture to draw</param>
        /// <param name="absolutePosition">Specifies the top-left corner co-ordinate to draw the texture</param>
        /// <param name="sourceRectangle">Specifies the section of the texture to draw</param>
        /// <remarks>Used to draw anything for the heads-up display</remarks>
        void DrawTexture(Texture2D texture, Vector2 absolutePosition, Rectangle? sourceRectangle);

        /// <summary>
        /// Draws a solid rectangle at the specified co-ordinates in the specified colour
        /// </summary>
        /// <param name="r">The rectangle to draw</param>
        /// <param name="colour">The colour to draw the rectangle in</param>
        void DrawRectangle(Rectangle r, Color colour);

        /// <summary>
        /// Draws a texture across a region, applying a colour to the texture
        /// </summary>
        /// <param name="texture">The texture to draw</param>
        /// <param name="r">The absolute screen co-ordinates to draw to</param>
        /// <param name="colour">The colour to apply to the texture</param>
        void DrawTextureOverRegion(Texture2D texture, Rectangle r, Color colour);

        /// <summary>
        /// Draws text to the window
        /// </summary>
        /// <param name="spriteFont">The font to use</param>
        /// <param name="text">The text which will be drawn</param>
        /// <param name="position">The drawing location</param>
        /// <param name="colour">A colour mask</param>
        /// <param name="rotation">The rotation to apply</param>
        /// <param name="origin">The centre of the rotation</param>
        /// <param name="scale">A scale to apply</param>
        /// <param name="effects">Any effects to apply</param>
        /// <param name="layerDepth">The depth of the layer</param>
        void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color colour, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);

        /// <summary>
        /// Draws text to the window with centred alignment
        /// </summary>
        /// <param name="font">The font to render the text in</param>
        /// <param name="text">The text to draw</param>
        /// <param name="y">The Y co-ordinate to draw the text at</param>
        /// <param name="colour">The colour to draw the text in</param>
        void DrawCentredString(SpriteFont font, string text, float y, Color colour);
        }
    }
