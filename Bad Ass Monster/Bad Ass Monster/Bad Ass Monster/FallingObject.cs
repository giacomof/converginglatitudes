using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace LearningXNA
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    class FallingObject
    {
        static Random random = new Random();

        private Texture2D texture;
        private Vector2 origin;

        public readonly Color Color = Color.White;

        // The FallingObject is animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;

        private const float GravityAcceleration = 2000.0f;
        private float MaxFallSpeed = 200.0f;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this FallingObject in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        public Vector2 originalPosition;

        private int flag;


        /// <summary>
        /// Gets a circle which bounds this FallingObject in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        /// <summary>
        /// Constructs a new FallingObject.
        /// </summary>
        public FallingObject(Level level, Vector2 position, int receivedFlag)
        {
            this.level = level;
            this.basePosition = position;
            this.originalPosition = position;
            flag = receivedFlag;

            if (flag == 2)
                MaxFallSpeed = 500.0f;


            LoadContent(flag);
        }

        /// <summary>
        /// Loads the FallingObject texture and collected sound.
        /// </summary>
        public void LoadContent(int flag)
        {
            int index;
            switch (flag)
            {
                case 0:
                    index = random.Next(4);
                    texture = Level.Content.Load<Texture2D>("Sprites/FallingObject/lego" + index);
                    break;
                case 1:
                    index = random.Next(2);
                    texture = Level.Content.Load<Texture2D>("Tiles/pendingSpikes" + index);
                    break;
                case 2:
                    index = random.Next(2);
                    texture = Level.Content.Load<Texture2D>("Tiles/pendingSpikes" + index);
                    break;
            }
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
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
            // Include the X coordinate so that neighboring FallingObjects bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
            ApplyPhysics(gameTime);
            if (basePosition.Y > 15 * Tile.Height)
            {
                LoadContent(flag);
                basePosition = originalPosition;
            }

        }

        /// <summary>
        /// Simulate the falling of the object.
        /// </summary>
        private void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
            basePosition += velocity * elapsed;
        }

        /// <summary>
        /// Draws a FallingObject in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
