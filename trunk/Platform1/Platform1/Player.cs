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

        // Define if the player can shape change to monster cat
        public bool CanBeCat
        {
            get { return canBeCat; }
            set { canBeCat = value; }
        }
        private bool canBeCat;

        // Define if the player can shape change to monster duck
        public bool CanBeDuck
        {
            get { return canBeDuck; }
            set { canBeDuck = value; }
        }
        private bool canBeDuck;
       


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
        private Animation monsterCatClimbAnimation;
        private Animation monsterCatClimbIdleAnimation;


        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;

        // Sounds
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;
        private SoundEffect eatCatSound;
        private SoundEffect eatDuckSound;

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
        private const float monsterMoveAcceleration = 10000.0f;
        private const float monsterMaxMoveSpeed = 10000.0f;
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
        private const float monsterCatWallDragFactor = 0.45f;


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
        /// Last user movement input on the X axis. Used for the climbing algorithm.
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

        private bool isClimbing;
        public bool IsClimbing
        {
            get { return isClimbing; }
        }

        private bool wasClimbing;


        private bool isDead;


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

            // Reset the shape changing abilities
            CanBeCat = false;
            CanBeDuck = false;

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
            monsterCatClimbAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catClimb"), 0.1f, true);
            monsterCatClimbIdleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catClimbIdle"), 0.1f, false);

            // Load sounds.            
            killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
            eatCatSound = Level.Content.Load<SoundEffect>("Sounds/EatCat");
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
            isDead = false;
            sprite.PlayAnimation(monsterIdleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, resets some values and animates the player sprite.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            GetInput();

            ApplyPhysics(gameTime);

            if (IsAlive)
            {
                if (isOnGround)
                {
                    if (Math.Abs(Velocity.X) > 0)
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
                else if (isClimbing)
                {
                    if (Math.Abs(Velocity.Y) > 0)
                    {
                        sprite.PlayAnimation(monsterCatClimbAnimation);
                    }
                    else
                    {
                        sprite.PlayAnimation(monsterCatClimbIdleAnimation);
                    }
                }
            }
            else
            {
                isDead = true;
            }

            // Clear input.
            movementX = 0.0f;
            movementY = 0.0f;
            isJumping = false;

            wasClimbing = isClimbing;

            if(!isDoingSpecialAction)
            {
                switch (animalShape)
                {
                    case MONSTER:
                        isClimbing = false;
                        break;

                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput()
        {
            // Get input state.
            KeyboardState keyboardState = Keyboard.GetState();

            isClimbing = false;
            isDoingSpecialAction = false;

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                isDoingSpecialAction = true;

                if (animalShape == MONSTER_CAT)
                {
                    if (canClimbOnCeiling() || canClimb())
                    {
                        isClimbing = true;
                        isJumping = false;
                        isOnGround = false;
                    }
                }

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

            // Key press used for debug reasons
            if (keyboardState.IsKeyDown(Keys.S))
            {
                movementY = movementY;
                //position.X = 2004;
                //position.Y = 673;
            }

            // Check if the player wants to jump.
            if (!isClimbing)
            {
                isJumping = keyboardState.IsKeyDown(Keys.Up);
            }
            else
            {
                isJumping = false;
            }

            if( keyboardState.IsKeyDown(Keys.D1))
            {
                changeShape(MONSTER);
            }
            else if (keyboardState.IsKeyDown(Keys.D2) && canBeCat)
            {
                changeShape(MONSTER_CAT);
            }

            
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

                    if (isDoingSpecialAction && isClimbing)
                    {
                        velocity.Y = movementY * MoveAcceleration * elapsed;
                        // NEED TO BE CHANGED IN CLIMB DRAG FACTOR
                        velocity.Y *= monsterCatWallDragFactor;

                        velocity.X += movementX * MoveAcceleration * elapsed;
                        velocity.X *= monsterCatWallDragFactor;

                        velocity.Y = MathHelper.Clamp(velocity.Y, -MaxMoveSpeed, MaxMoveSpeed);

                        // Apply velocity.
                        Position += velocity * elapsed;
                        Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

                    }
                    else if (wasClimbing && !isClimbing)
                    {
                        // Base velocity is a combination of horizontal movement control and
                        // acceleration downward due to gravity.
                        velocity.X += movementX * MoveAcceleration * elapsed;
                        velocity.X *= AirDragFactor;

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
                switch (animalShape)
                {
                    case MONSTER:
                        // Begin or continue a jump
                        if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                        {
                            if (jumpTime == 0.0f)
                                jumpSound.Play();

                            jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            sprite.PlayAnimation(monsterJumpAnimation);
                        }
                        break;

                    case MONSTER_CAT:
                        if ((!wasJumping && IsOnGround && !canClimb()) || jumpTime > 0.0f)
                        {
                            if (jumpTime == 0.0f)
                                jumpSound.Play();

                            jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            sprite.PlayAnimation(monsterCatJumpAnimation);
                        }
                        break;
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

            // Debug stuff
            //Console.WriteLine("leftTile: " + leftTile + "| rightTile: " + rightTile + "| topTile: " + topTile + "| bottomTile: " + bottomTile);
            //Console.WriteLine("x: " + position.X + "| y: " + position.Y);

            // Reset flag to search for ground collision.
            isOnGround = false;

            //NEW STUFF ABOUT MOVING PLATFORM 
            //For each potentially colliding movable tile.  
            foreach (var movableTile in level.movableTiles)
            {
                // Reset flag to search for movable tile collision.  
                movableTile.PlayerIsOn = false;

                //check to see if player is on tile.  
                if ((BoundingRectangle.Bottom == movableTile.BoundingRectangle.Top + 1) &&
                    (BoundingRectangle.Left >= movableTile.BoundingRectangle.Left - (BoundingRectangle.Width / 2) &&
                    BoundingRectangle.Right <= movableTile.BoundingRectangle.Right + (BoundingRectangle.Width / 2)))
                {
                    movableTile.PlayerIsOn = true;
                }

                bounds = HandleCollision(bounds, movableTile.Collision, movableTile.BoundingRectangle);
            } 
            //END OF MOVING PLATFORM

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision == TileCollision.KillerTile)
                    {
                        this.OnKilled(null);
                    }
                    if (collision != TileCollision.Passable && collision != TileCollision.PlatformCollider && collision != TileCollision.KillerTile)
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
                                if ((collision == TileCollision.Impassable || collision == TileCollision.LevelFrame) || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable || collision == TileCollision.LevelFrame) // Ignore platforms.
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
//// MOVING PLATFORM STUFF
private Rectangle HandleCollision(Rectangle bounds, TileCollision collision, Rectangle tileBounds)
{
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
    return bounds;
} 
//// END OF MOVING PLATFORM STUFF
        /// <summary>
        /// Called to check if the player can climb.
        /// </summary>
        bool canClimb()
        {

            Rectangle bounds = BoundingRectangle;
            int topTile = (int)Math.Round((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Round(((float)bounds.Bottom / Tile.Height));
            int side;
            int distance;

            // Change movementX in lastMovementX if you want to use only X key for climbing
            if (lastMovementX > 0)
            {
                side = (int)Math.Round((float)bounds.Right / Tile.Width);
                distance = Math.Abs(bounds.Right - side * Tile.Width);
            }
            else if (lastMovementX < 0)
            {
                side = (int)Math.Round((float)bounds.Left / Tile.Width) - 1;
                distance = Math.Abs(bounds.Left - (side + 1) * Tile.Width);
            }
            else
            {
                return false;
            }

            // Debug writing
            //Console.WriteLine("Side: " + side + "| top: " + topTile + "| bottom: " + bottomTile + "| distance: " + distance + "| MovementX: " + lastMovementX);

            for (int y = topTile+1; y <= bottomTile; y++)
            {
                TileCollision collision = Level.GetCollision(side, y);
                if (collision != TileCollision.Impassable || distance > 0)
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Called to check if the player can climb on the ceiling.
        /// </summary>
        bool canClimbOnCeiling()
        {

            Rectangle bounds = BoundingRectangle;
            int topTile = (int)Math.Round((float)bounds.Top / Tile.Height)-1;

            int centralTile = (int)Math.Floor((float)bounds.Center.X / Tile.Width);
            
            // Debug writing
            //Console.WriteLine("topTile: " + topTile + "| centralTile: " + centralTile);

            TileCollision collision = Level.GetCollision(centralTile, topTile);
            if (collision == TileCollision.Impassable)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            else if (!isDead)
            {
                fallSound.Play();
            }

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
        /// Called when the player eats an animal.
        /// </summary>
        /// <param name="killedBy">
        /// The animal who was eaten by the player.
        /// </param>
        public void OnPlayerEated(Animal eatenAnimal)
        {
            switch (eatenAnimal.AnimalShape)
            {
                case MONSTER_CAT:
                    canBeCat = true;
                    changeShape(MONSTER_CAT);
                    eatCatSound.Play();
                    break;
            }
            
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
