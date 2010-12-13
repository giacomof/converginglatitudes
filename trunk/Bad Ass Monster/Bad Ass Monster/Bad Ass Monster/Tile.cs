﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LearningXNA
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,

        /// <summary>
        /// A virtual tile that is considered to be the building block of the frame of 
        /// level.
        /// </summary>
        LevelFrame = 3,

        /// <summary>
        /// Invisible tile for stopping moving platforms.
        /// </summary>
        PlatformCollider = 4,

        /// <summary>
        /// Tile that will kill the player if hit
        /// </summary>
        KillerTile = 5,

        /// <summary>
        /// Checkpoint Tile
        /// </summary>
        Checkpoint = 6,

        /// <summary>
        /// Tile that will make the player jump twice as high
        /// </summary>
        Bouncy = 7,

        /// <summary>
        /// Disappearing and re-appearing tile
        /// </summary>
        Disappearing = 8,

        /// <summary>
        /// Tile that shows tutorial overlay
        /// </summary>
        TutorialTile = 9,

        /// <summary>
        /// Switch for activating the movement of horizontal moving platforms
        /// </summary>
        HorizontalSwitch = 10,

        /// <summary>
        /// Switch for activating the movement of horizontal moving platforms
        /// </summary>
        VerticalSwitch = 11,

        /// <summary>
        /// Switch for deactivating walls number 1
        /// </summary>
        SwitchWall1 = 12,

        /// <summary>
        /// Switch for deactivating walls number 2
        /// </summary>
        SwitchWall2 = 13,

        /// <summary>
        /// Switch for deactivating walls number 3
        /// </summary>
        SwitchWall3 = 14,

        /// <summary>
        /// Destroyable wall number 1
        /// </summary>
        DestroyableWall1 = 15,

        /// <summary>
        /// Destroyable wall number 2
        /// </summary>
        DestroyableWall2 = 16,

        /// <summary>
        /// Destroyable wall number 3
        /// </summary>
        DestroyableWall3 = 17,

        /// <summary>
        /// Y axxis level frame
        /// </summary>
        YAxisLevelFrame = 18,
        /// <summary>
        /// Destroyable tile for the hamster
        /// </summary>
        Destroyable = 19,

        /// <summary>
        /// Switch for deactivating walls number 4
        /// </summary>
        SwitchWall4 = 20,

        /// <summary>
        /// Switch for deactivating walls number 5
        /// </summary>
        SwitchWall5 = 21,

        /// <summary>
        /// Destroyable wall number 4
        /// </summary>
        DestroyableWall4 = 22,

        /// <summary>
        /// Destroyable wall number 5
        /// </summary>
        DestroyableWall5 = 23,
    }

    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    struct Tile
    {
        public Texture2D Texture;
        public TileCollision Collision;
        public const int Width = 64;
        public const int Height = 48;
        public int tutorialFlag;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        /// <summary>
        /// Constructs a new tile.
        /// </summary>
        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
            tutorialFlag = -1;
        }
        public Tile(Texture2D texture, TileCollision collision, int tutorial)
        {
            Texture = texture;
            Collision = collision;
            tutorialFlag = tutorial;
        }
    }
}