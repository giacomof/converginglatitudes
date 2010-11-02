using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace LearningXNA
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    class Cookie
    {
        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect collectedSound;

        public const int PointValue = 30;
        public readonly Color Color = Color.BurlyWood;

        // The Cookie is animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this Cookie in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        /// <summary>
        /// Gets a circle which bounds this Cookie in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        /// <summary>
        /// Constructs a new Cookie.
        /// </summary>
        public Cookie(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;

            LoadContent();
        }

        /// <summary>
        /// Loads the Cookie texture and collected sound.
        /// </summary>
        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>("Sprites/Cookie");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            collectedSound = Level.Content.Load<SoundEffect>("Sounds/CookieCollected");
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring Cookies bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }

        /// <summary>
        /// Called when this Cookie has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this Cookie. Although currently not used, this parameter would be
        /// useful for creating special powerup Cookies. For example, a Cookie could make the player invincible.
        /// </param>
        public void OnCollected(Player collectedBy)
        {
            collectedSound.Play();
        }

        /// <summary>
        /// Draws a Cookie in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
