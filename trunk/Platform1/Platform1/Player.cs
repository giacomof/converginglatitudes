using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LearningXNA
{
    /// <summary>
    /// Our character: the monster
    /// </summary>
    class Player
    {
        // Shape state of the character; starting from 0 as a monster
        const short MONSTER             = 0;
        const short MONSTER_CAT         = 1;
        const short MONSTER_DUCK        = 2;
        private short animalShape = MONSTER;


        // Animations
        private Animation monsterIdleAnimation;
        private Animation monsterRunAnimation;
        private Animation monsterJumpAnimation;
        private Animation monsterCelebrateAnimation;
        private Animation monsterDieAnimation;

        private Animation monsterCatIdleAnimation;
        private Animation monsterCatRunAnimation;
        private Animation monsterCatJumpAnimation;
        private Animation monsterCatCelebrateAnimation;
        private Animation monsterCatDieAnimation;


        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;

        // Sounds
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        //***********MONSTER SHAPE MOVEMENTS*****************
        // Constants for controling horizontal movement
        private const float monsterMoveAcceleration = 6000.0f;
        private const float monsterMaxMoveSpeed = 6000.0f;
        private const float monsterGroundDragFactor = 0.58f;
        private const float monsterAirDragFactor = 0.65f;

        // Constants for controlling vertical movement
        private const float monsterMaxJumpTime = 0.35f;
        private const float monsterJumpLaunchVelocity = -2000.0f;
        private const float monsterGravityAcceleration = 3500.0f;
        private const float monsterMaxFallSpeed = 1200.0f;
        private const float monsterJumpControlPower = 0.14f;
        //****************************************************

        //***********MONSTER-CAT SHAPE MOVEMENTS*****************
        // Constants for controling horizontal movement
        private const float monsterCatMoveAcceleration = 14000.0f;
        private const float monsterCatMaxMoveSpeed = 14000.0f;
        private const float monsterCatGroundDragFactor = 0.58f;
        private const float monsterCatAirDragFactor = 0.65f;

        // Constants for controlling vertical movement
        private const float monsterCatMaxJumpTime = 0.35f;
        private const float monsterCatJumpLaunchVelocity = -4000.0f;
        private const float monsterCatGravityAcceleration = 3500.0f;
        private const float monsterCatMaxFallSpeed = 600.0f;
        private const float monsterCatJumpControlPower = 0.14f;
        //*******************************************************

        // Constants for controling horizontal movement
        private float MoveAcceleration = 14000.0f;
        private float MaxMoveSpeed = 14000.0f;
        private float GroundDragFactor = 0.58f;
        private float AirDragFactor = 0.65f;

        // Constants for controlling vertical movement
        private float MaxJumpTime = 0.35f;
        private float JumpLaunchVelocity = -4000.0f;
        private float GravityAcceleration = 3500.0f;
        private float MaxFallSpeed = 600.0f;
        private float JumpControlPower = 0.14f;

        // Input configuration
        private const float MoveStickScale = 1.0f;
        private const Buttons JumpButton = Buttons.A;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        /// <summary>
        /// Current user movement input on the X axis.
        /// </summary>
        private float movementX;

        /// <summary>
        /// Last user movement input on the X axis.
        /// </summary>
        private float lastMovementX;

        /// <summary>
        /// Current user movement input on the Y axis.
        /// </summary>
        private float movementY;

        /// <summary>
        /// Current user state about being doing a special action.
        /// </summary>
        private bool isDoingSpecialAction;


        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
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

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player(Level level, Vector2 position)
        {
            this.level = level;

            LoadContent();

            // I start the level as a MONSTER
            changeShape(MONSTER);

            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            // Load animated textures.
            monsterIdleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/monsterIdle"), 0.2f, true);
            monsterRunAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/monsterRun"), 0.1f, true);
            monsterJumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, false);
            monsterCelebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false);
            monsterDieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false);

            monsterCatIdleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catIdle"), 0.1f, true);
            monsterCatRunAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catRun"), 0.1f, true);
            monsterCatJumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catJump"), 0.1f, false);
            monsterCatCelebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catCelebrate"), 0.1f, false);
            monsterCatDieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catDie"), 0.1f, false);

            // Load sounds.            
            killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(monsterIdleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            GetInput();

            ApplyPhysics(gameTime);

            if (IsAlive && IsOnGround)
            {
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                {
                    switch (animalShape)
                    {
                        case MONSTER:
                            sprite.PlayAnimation(monsterRunAnimation);
                            break;

                        case MONSTER_CAT:
                            sprite.PlayAnimation(monsterCatRunAnimation);
                            break;
                    }

                }
                else
                {
                    switch (animalShape)
                    {
                        case MONSTER:
                            sprite.PlayAnimation(monsterIdleAnimation);
                            break;

                        case MONSTER_CAT:
                            sprite.PlayAnimation(monsterCatIdleAnimation);
                            break;
                    }
                }
            }

            // Clear input.
            movementX = 0.0f;
            movementY = 0.0f;
            isJumping = false;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput()
        {
            // Get input state.
            KeyboardState keyboardState = Keyboard.GetState();

            //switch (animalShape)
            //{
            //    case MONSTER:
                    // If any digital horizontal movement input is found, override the analog movement.
            if (keyboardState.IsKeyDown(Keys.X))
            {
                isDoingSpecialAction = true;
            }
            else
            {
                isDoingSpecialAction = false;
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                movementX = -1.0f;
                lastMovementX = -1.0f;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                movementX = 1.0f;
                lastMovementX = 1.0f;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                movementY = -1.0f;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                movementY = 1.0f;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                movementY = movementY;
            }

            // Check if the player wants to jump.
            isJumping = keyboardState.IsKeyDown(Keys.Space);

            if( keyboardState.IsKeyDown(Keys.D1))
            {
                changeShape(MONSTER);
            }
            else if (keyboardState.IsKeyDown(Keys.D2))
            {
                changeShape(MONSTER_CAT);
            }

            

            //        break;

            //    default:
            //        break;
            //}
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            switch (animalShape)
            {
                case MONSTER:

                    // Base velocity is a combination of horizontal movement control and
                    // acceleration downward due to gravity.
                    velocity.X += movementX * MoveAcceleration * elapsed;
                    velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

                    velocity.Y = DoJump(velocity.Y, gameTime);

                    // Apply pseudo-drag horizontally.
                    if (IsOnGround)
                        velocity.X *= GroundDragFactor;
                    else
                        velocity.X *= AirDragFactor;

                    // Prevent the player from running faster than his top speed.            
                    velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

                    // Apply velocity.
                    Position += velocity * elapsed;
                    Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

                    break;

                case MONSTER_CAT:

                    if (isDoingSpecialAction && canClimb())
                    {
                        //velocity.X += MathHelper.Clamp(velocity.X + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

                        velocity.Y = movementY * MoveAcceleration * elapsed;
                        // NEED TO BE CHANGED IN CLIMB DRAG FACTOR
                        velocity.Y *= GroundDragFactor;

                        //velocity.Y = MathHelper.Clamp(velocity.Y, -MaxMoveSpeed, MaxMoveSpeed);

                        // Apply velocity.
                        Position += velocity * elapsed;
                        Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

                    }
                    else
                    {
                        // Base velocity is a combination of horizontal movement control and
                        // acceleration downward due to gravity.
                        velocity.X += movementX * MoveAcceleration * elapsed;
                        velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

                        velocity.Y = DoJump(velocity.Y, gameTime);

                        // Apply pseudo-drag horizontally.
                        if (IsOnGround)
                            velocity.X *= GroundDragFactor;
                        else
                            velocity.X *= AirDragFactor;

                        // Prevent the player from running faster than his top speed.            
                        velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

                        // Apply velocity.
                        Position += velocity * elapsed;
                        Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

                    }
                    break;
            }
            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;

        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && (IsOnGround || canClimb())) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                        jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    switch (animalShape)
                    {
                        case MONSTER:
                            sprite.PlayAnimation(monsterJumpAnimation);
                            break;

                        case MONSTER_CAT:
                            sprite.PlayAnimation(monsterCatJumpAnimation);
                            break;
                    }
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Called to check if the player can climb.
        /// </summary>
        bool canClimb()
        {
            bool canClimb = true;
            Rectangle bounds = BoundingRectangle;

            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            int side;
            int xPosition = bounds.X + bounds.Width/10;

            if (lastMovementX > 0)
            {
                side = ((int)Math.Ceiling((float)xPosition / Tile.Width));
            }
            else
            {
                side = ((int)Math.Floor((float)xPosition / Tile.Width)) - 1;
            }          

            for (int y = topTile+2; y <= bottomTile+1; ++y)
            {
                TileCollision collision = Level.GetCollision(side, y);
                if (collision != TileCollision.Impassable)
                {
                    canClimb = false;
                    return canClimb;
                }
            }
            return canClimb;
        }


        

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public void OnKilled(Enemy killedBy)
        {
            isAlive = false;

            if (killedBy != null)
                killedSound.Play();
            else
                fallSound.Play();

            switch (animalShape)
            {
                case MONSTER:
                    sprite.PlayAnimation(monsterDieAnimation);
                    break;
                case MONSTER_CAT:
                    sprite.PlayAnimation(monsterCatDieAnimation);
                    break;
            }

            // Get beck to MONSTER when killed
            changeShape(MONSTER);
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            switch (animalShape)
            {
                case MONSTER:
                    sprite.PlayAnimation(monsterCelebrateAnimation);
                    break;
                case MONSTER_CAT:
                    sprite.PlayAnimation(monsterCatCelebrateAnimation);
                    break;
            }
        }

        /// <summary>
        /// Called when the player change shape
        /// </summary>
        public void changeShape(short shapeFlag)
        {
            animalShape = shapeFlag;

            // Calculate standard bounding box dimension
            int width = (int)(monsterIdleAnimation.FrameWidth * 0.4);
            int left = (monsterIdleAnimation.FrameWidth - width) / 2;
            int height = (int)(monsterIdleAnimation.FrameWidth * 0.8);
            int top = monsterIdleAnimation.FrameHeight - height;

            switch(shapeFlag)
            {
                case MONSTER:
                    // Constants for controling horizontal movement
                    MoveAcceleration = monsterMoveAcceleration;
                    MaxMoveSpeed = monsterMaxMoveSpeed;
                    GroundDragFactor = monsterGroundDragFactor;
                    AirDragFactor = monsterAirDragFactor;

                    // Constants for controlling vertical movement
                    MaxJumpTime = monsterMaxJumpTime;
                    JumpLaunchVelocity = monsterJumpLaunchVelocity;
                    GravityAcceleration = monsterGravityAcceleration;
                    MaxFallSpeed = monsterMaxFallSpeed;
                    JumpControlPower = monsterJumpControlPower;

                    break;

                case MONSTER_CAT:
                    // Constants for controling horizontal movement
                    MoveAcceleration = monsterCatMoveAcceleration;
                    MaxMoveSpeed = monsterCatMaxMoveSpeed;
                    GroundDragFactor = monsterCatGroundDragFactor;
                    AirDragFactor = monsterCatAirDragFactor;

                    // Constants for controlling vertical movement
                    MaxJumpTime = monsterCatMaxJumpTime;
                    JumpLaunchVelocity = monsterCatJumpLaunchVelocity;
                    GravityAcceleration = monsterCatGravityAcceleration;
                    MaxFallSpeed = monsterCatMaxFallSpeed;
                    JumpControlPower = monsterCatJumpControlPower;

                    // Calculate bounding box dimension
                    width = (int)(monsterCatIdleAnimation.FrameWidth * 0.4);
                    left = (monsterCatIdleAnimation.FrameWidth - width) / 2;
                    height = (int)(monsterCatIdleAnimation.FrameWidth * 0.8);
                    top = monsterCatIdleAnimation.FrameHeight - height;

                    break;
            }

            // Change bounding box dimension        
            localBounds = new Rectangle(left, top, width, height);
        }


        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}
