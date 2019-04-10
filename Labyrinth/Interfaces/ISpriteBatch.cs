﻿using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public interface ISpriteBatch : IDisposable  
        {
        /// <summary>
        /// Gets or sets the offset for the view on the world for relative positioning
        /// </summary>
        Vector2 WindowPosition { get; set; }
        
        /// <summary>
        /// Begins a drawing batch
        /// </summary>
        /// <param name="effect">The pixel shader to apply to the batch</param>
        void Begin(Effect effect = null);

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
        /// <remarks>Used to draw the score</remarks>
        void DrawTexture(Texture2D texture, Vector2 absolutePosition, Rectangle? sourceRectangle);

        /// <summary>
        /// Draws a solid rectangle at the specified co-ordinates in the specified colour
        /// </summary>
        /// <param name="r">The rectangle to draw</param>
        /// <param name="colour">The colour to draw the rectangle in</param>
        void DrawRectangle(Rectangle r, Color colour);

        /// <summary>
        /// Draws text to the window with centred alignment
        /// </summary>
        /// <param name="font">The font to render the text in</param>
        /// <param name="text">The text to draw</param>
        /// <param name="y">The Y co-ordinate to draw the text at</param>
        /// <param name="color">The colour to draw the text in</param>
        void DrawCentredString(SpriteFont font, string text, int y, Color color);
        }
    }
