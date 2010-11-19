using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LearningXNA
{
    class VerticalMovableTile
    {
        private Texture2D texture;
        private Vector2 origin;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        public Vector2 Velocity
        {
            get { return velocity; }
        }
        Vector2 velocity;

        /// <summary>  
        /// Gets whether or not the player's feet are on the MovableTile.  
        /// </summary>  
        public bool PlayerIsOn { get; set; }

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public FaceDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        FaceDirection direction = FaceDirection.Left;
        

        public TileCollision Collision
        {
            get { return collision; }
            set { collision = value; }
        }
        private TileCollision collision;

        private Rectangle localBounds;
        private float waitTime;
        private const float MaxWaitTime = 0.7f;
        private const float MoveSpeed = 120.0f;

        public VerticalMovableTile(Level level, Vector2 position, TileCollision collision)
        {
            this.level = level;
            this.position = position;
            this.collision = collision;

            LoadContent();
        }

        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>("Tiles/verticalMovingPlatform");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            // Calculate bounds within texture size.  
            localBounds = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate tile position based on the side we are moving towards.  
            float posX = Position.X + localBounds.Width / 2;
            float posY = Position.Y + localBounds.Height / 2;

            int tileX = (int)Math.Floor(posX / Tile.Width)-1;
            int tileY;
            if((int)direction< 0)
                tileY = (int)Math.Floor((posY + (localBounds.Height * (int)direction)) / Tile.Height);
            else
                tileY = (int)Math.Floor((posY + (localBounds.Height * (int)direction)) / Tile.Height)-1;

            if (waitTime > 0)
            {
                // Wait for some amount of time.  
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Then turn around.  
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                //If we're about to run into a wall that isn't a MovableTile move in other direction.  
                if (Level.GetCollision(tileX, tileY) == TileCollision.Impassable ||
                    Level.GetCollision(tileX, tileY) == TileCollision.Platform ||
                    Level.GetCollision(tileX, tileY) == TileCollision.PlatformCollider)
                {
                    velocity = new Vector2(0.0f, 0.0f);
                    waitTime = MaxWaitTime;
                }
                else
                {
                    // Move in the current direction.  
                    velocity = new Vector2(0.0f, (int)direction * MoveSpeed * elapsed);
                    position = position + velocity;
                }
            }

            if (level.verticalMovableTiles.Count > 0)
            {
                //If we're about to run into a MovableTile move in other direction.  
                foreach (var verticalMovableTile in level.verticalMovableTiles)
                {
                    if (BoundingRectangle != verticalMovableTile.BoundingRectangle)
                    {
                        if (BoundingRectangle.Intersects(verticalMovableTile.BoundingRectangle))
                        {
                            direction = (FaceDirection)(-(int)direction);
                            velocity = new Vector2(0.0f, (int)direction * MoveSpeed * elapsed);
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}