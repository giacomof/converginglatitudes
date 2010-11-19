using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LearningXNA
{
    /// <summary>
    /// A monster who is impeding the progress of our fearless adventurer.
    /// </summary>
    class JumpingEnemy
    {
        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Position in world space of the bottom center of this enemy.
        /// </summary>
        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;
        Vector2 initialPosition;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this enemy in world space.
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

        //Jumping stuff
        float elapsedTotal;
        public float GravityAcceleration = 3500.0f;
        public float MaxFallSpeed = 600.0f;
        public float jumpTime;
        public float MaxJumpTime =0.4f;
        public bool isJumping =true;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;
        private float JumpLaunchVelocity = -4000.0f;
        private float JumpControlPower = 0.14f;

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public JumpingEnemy(Level level, Vector2 position, string spriteSet)
        {
            this.level = level;
            this.position = position;
            this.initialPosition = position;
            this.velocity.X = 0;
            this.velocity.Y = 0;
            this.jumpTime = 0.0f;
            this.elapsedTotal = 0;

            LoadContent(spriteSet);
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
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


       public void Update(GameTime gameTime)
        {
            //Console.WriteLine(jumpTime + " " + isJumping);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            int seconds = gameTime.TotalRealTime.Seconds;
            if ((seconds / 3) % 2 == 0 && isJumping == false)
            {
                isJumping = true;
            }
           //elapsedTotal+= elapsed;
            
            //jump
           

            if (isJumping == true)
           { 
                jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump


                    velocity.Y = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));


                }
                else
                {
                    //Console.WriteLine("hej");
                    // Reached the apex of the jump
                    velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                    
                    

                }
               

              
            }
            // Apply velocity.
            position += velocity * elapsed;
            position = new Vector2(Position.X, (float)Math.Round(Position.Y));
            //Console.WriteLine(position.Y + " " + isJumping + " " + initialPosition);
            if (position.Y >= initialPosition.Y)
            {
                jumpTime = 0.0f;
                isJumping = false;
                position = initialPosition;
            }
            else
                isJumping = true;
                

         }
       // Calculate tile position based on the side we are walking towards.

       //float posY = Position.Y + localBounds.Height / 2 ;

       //int tileY;
       //int tileX = (int)Math.Floor(Position.X / Tile.Width);

            //wasJumping = isJumping;
            //jump

        /// <summary>
        /// Draws the animated enemy.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Stop running when the game is paused or before turning around.
            if (!Level.Player.IsAlive ||
                Level.ReachedExit ||
                Level.TimeRemaining == TimeSpan.Zero)
            {
                sprite.PlayAnimation(idleAnimation);
            }
            else
            {
                sprite.PlayAnimation(runAnimation);
            }
            sprite.Draw(gameTime, spriteBatch, Position, SpriteEffects.None);


        }
         }
            
        

       
    }

