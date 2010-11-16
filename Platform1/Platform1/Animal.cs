using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LearningXNA
{
    /// <summary>
    /// A monster who is impeding the progress of our fearless adventurer.
    /// </summary>
    class Animal
    {   
        // Animal Flags
        const short MONSTER_CAT = 1;
        const short MONSTER_DUCK = 2;

        /// <summary>
        /// Shape flag of the animal.
        /// </summary>
        public short AnimalShape
        {
            get { return animalShape; }
        }
        short animalShape;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Position in world space of the bottom center of this animal.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        private bool isCalled;
        int calledTimerMilliseconds = 2000;
        float calledTimerClock;
        int maxCallingDistance = 500;
        private const float GravityAcceleration = 600.0f;
        private const float MaxFallSpeed = 600.0f;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this animal in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        // Animations
        private Animation runAnimation;
        private Animation idleAnimation;
        private AnimationPlayer sprite;

        /// <summary>
        /// The direction this animal is facing and moving along the X axis.
        /// </summary>
        private FaceDirection direction = FaceDirection.Left;

        /// <summary>
        /// How long this animal has been waiting before turning around.
        /// </summary>
        private float waitTime;

        /// <summary>
        /// How long to wait before turning around.
        /// </summary>
        private const float MaxWaitTime = 1.5f;

        /// <summary>
        /// The speed at which this animal moves along the X axis.
        /// </summary>

        private const float MoveSpeed = 128.0f;

        /// <summary>
        /// Constructs a new Animal.
        /// </summary>
        public Animal(Level level, Vector2 position, string spriteSet)
        {
            this.level = level;
            this.position = position;
            calledTimerClock = 0;

            // Change the flag of the actual animal in relation to the sprite set requested by the parser in the level loader
            if (spriteSet == "Cat")
            {
                animalShape = MONSTER_CAT;
            }

            LoadContent(spriteSet);
        }

        /// <summary>
        /// Loads a particular animal sprite sheet and sounds.
        /// </summary>
        public void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, true);
            idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.15f, true);
            sprite.PlayAnimation(idleAnimation);

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }


        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public void Update(GameTime gameTime, bool isBeingCalled, Vector2 playerPosition)
        {
            float distance = Vector2.Distance(position, playerPosition);
            if (!isCalled && isBeingCalled && distance <= maxCallingDistance)
            {
                isCalled = isBeingCalled;
                calledTimerClock = 0;
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            int groundTileX = (int)Math.Floor((float)Position.X / Tile.Width);
            int groundTileY = (int)Math.Floor((float)Position.Y / Tile.Height);

            Vector2 velocity;

            if (!isCalled)
            {
                
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
                else if (Level.GetCollision(groundTileX, groundTileY) != TileCollision.Passable)
                {
                    // If we are about to run into a wall or off a cliff, start waiting.
                    if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                        Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.LevelFrame ||
                        Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                    {
                        waitTime = MaxWaitTime;
                    }
                    else
                    {
                        // Move in the current direction.
                        velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                        position = position + velocity;
                    }
                }
                else
                {
                    velocity = new Vector2(0, 0);
                    velocity.X = (int)direction * MoveSpeed * elapsed;
                    velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                    position = position + velocity;
                }
            }
            else
            {

                calledTimerClock += gameTime.ElapsedGameTime.Milliseconds;
                if (calledTimerClock >= calledTimerMilliseconds)
                {
                    isCalled = false;
                }

                if (playerPosition.X > position.X)
                {
                    direction = FaceDirection.Right;
                }
                else
                {
                    direction = FaceDirection.Left;
                }

                // If we are about to run into a wall or off a cliff, start waiting.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.LevelFrame)
                {
                    //// Move in the current direction.
                    velocity = new Vector2(0, 0);
                    //velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    //position = position + velocity;
                }
                else
                {
                    velocity = new Vector2(0, 0) ;
                    velocity.X = (int)direction * MoveSpeed * elapsed;
                    if (Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                    {
                        velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                    }
                    position = position + velocity;
                }


            }
        }

        /// <summary>
        /// Draws the animated animal.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Stop running when the game is paused or before turning around.
            if ((!Level.Player.IsAlive ||
                Level.ReachedExit ||
                Level.TimeRemaining == TimeSpan.Zero ||
                waitTime > 0 )&& !isCalled)
            {
                sprite.PlayAnimation(idleAnimation);
            }
            else
            {
                sprite.PlayAnimation(runAnimation);
            }


            // Draw facing the way the animal is moving.
            SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}
