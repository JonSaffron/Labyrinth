﻿using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public interface IGameObject
        {
        /// <summary>
        /// Gets the position of the object
        /// </summary>
        /// <remarks>This is the location of the object's centre</remarks>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the <see cref="TilePos">tile position</see> that contains its <see cref="Position"/>
        /// </summary>
        TilePos TilePosition { get; }

        /// <summary>
        /// Gets the rough boundaries of the game object's visible area for basic collision detection purposes
        /// </summary>
        /// <remarks>The object's <see cref="Position"/> is at the center of the rectangle</remarks>
        Rectangle BoundingRectangle { get; }

        /// <summary>
        /// Gets how much energy the object currently has
        /// </summary>
        /// <remarks>Range is 0 to 255</remarks>
        int Energy { get; }

        /// <summary>
        /// Gets whether the object remains in the game
        /// </summary>
        /// <remarks>Reasons for not being extant include:
        /// - the object's energy has been reduced to 0
        /// - the object has been taken by the player
        /// - the object's animation has finished 
        /// Objects that are no longer extant are removed from the game</remarks>
        bool IsExtant { get; }

        /// <summary>
        /// Gets an indication of how solid the object is
        /// </summary>
        /// <remarks>This is used to determine how </remarks>
        ObjectSolidity Solidity { get; }

        /// <summary>
        /// Determines the z-order that this object is drawn with
        /// </summary>
        /// <remarks>Objects with a low DrawOrder are drawn before those with a higher DrawOrder.
        /// Objects with the same DrawOrder may be drawn in any order.</remarks>
        int DrawOrder { get; }

        /// <summary>
        /// Removes the specified amount of energy from the object
        /// </summary>
        /// <param name="energyToRemove">The amount to reduce the object's energy by</param>
        void ReduceEnergy(int energyToRemove);

        /// <summary>
        /// Reduce the object's energy to zero
        /// </summary>
        /// <returns>The amount of energy the object had before it expired</returns>
        void InstantlyExpire();

        /// <summary>
        /// Draws the object if it has any energy remaining
        /// </summary>
        /// <param name="gt">The current gametime</param>
        /// <param name="spriteBatch">The spritebatch to draw to</param>
        void Draw(GameTime gt, ISpriteBatch spriteBatch);
        
        /// <summary>
        /// Plays a sound which is centred on this instance
        /// </summary>
        /// <param name="gameSound">Sets which sound to play</param>
        // todo this shouldn't be part of this interface (or part of its implementation): move to an extension method
        void PlaySound(GameSound gameSound);
        }
    }
