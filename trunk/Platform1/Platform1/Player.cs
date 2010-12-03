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
        const short MONSTER_MOLE        = 3;
        public short animalShape = MONSTER;


        // Sound Categories
        const short DIE                 = 0;
        const short JUMP                = 1;
        const short EATCAT              = 2;
        const short EATDUCK             = 3;
        const short FLAP                = 4;
        const short TRANSFORMATION      = 5;
        const short EXCLAMATION         = 6;
        

        static Random random = new Random();

        // Tutorial variables
        static public int needTutorial;

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

        // Define if the player can shape change to monster Mole
        public bool CanBeMole
        {
            get { return canBeMole; }
            set { canBeMole = value; }
        }
        private bool canBeMole;
       


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
        private Animation monsterCatClimbOnCeilingAnimation;
        private Animation monsterCatClimbOnCeilingIdleAnimation;

        private Animation monsterDuckIdleAnimation;
        private Animation monsterDuckRunAnimation;
        private Animation monsterDuckJumpAnimation;
        private Animation monsterDuckFlyAnimation;
        private Animation monsterDuckDieAnimation;


        private Animation transformationAnimation;
        private int transformationTimerMilliseconds = 100;
        private float transformationTimerClock;

        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;

        // Sounds
        private SoundEffect die0;
        private SoundEffect die1;
        private SoundEffect die2;
        private SoundEffect die3;

        private SoundEffect jump0;
        private SoundEffect jump1;
        private SoundEffect jump2;
        private SoundEffect jump3;
        private SoundEffect jump4;

        private SoundEffect eatcat0;
        private SoundEffect eatcat1;
        private SoundEffect eatcat2;

        private SoundEffect eatduck0;
        private SoundEffect eatduck1;

        private SoundEffect flap0;
        private SoundEffect flap1;
        private SoundEffect flap2;
        private SoundEffect flap3;
        private SoundEffect flap4;

        private SoundEffect transformation0;
        private SoundEffect transformation1;
        private SoundEffect transformation2;

        private SoundEffect exclamation0;
        private SoundEffect exclamation1;
        private SoundEffect exclamation2;

        private SoundEffect checkpoint;

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
        private const float monsterMoveAcceleration = 14000.0f;
        private const float monsterMaxMoveSpeed = 14000.0f;
        private const float monsterGroundDragFactor = 0.58f;
        private const float monsterAirDragFactor = 0.65f;

        // Constants for controlling vertical movement
        private const float monsterMaxJumpTime = 0.35f;
        private const float monsterJumpLaunchVelocity = -4000.0f;
        private const float monsterGravityAcceleration = 3500.0f;
        private const float monsterMaxFallSpeed = 600.0f;
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
        public const float monsterCatJumpLaunchVelocity = -4000.0f;
        private const float monsterCatGravityAcceleration = 3500.0f;
        private const float monsterCatMaxFallSpeed = 600.0f;
        private const float monsterCatJumpControlPower = 0.14f;
        //*******************************************************

        //***********MONSTER-DUCK SHAPE MOVEMENTS*****************
        // Constants for controling horizontal movement
        private const float monsterDuckMoveAcceleration = 14000.0f;
        private const float monsterDuckMaxMoveSpeed = 14000.0f;
        private const float monsterDuckGroundDragFactor = 0.58f;
        private const float monsterDuckAirDragFactor = 0.65f;
        private const float monsterDuckWallDragFactor = 0.45f;


        // Constants for controlling vertical movement
        private const float monsterDuckMaxJumpTime = 0.15f;
        public const float monsterDuckJumpLaunchVelocity = -4000.0f;
        private const float monsterDuckGravityAcceleration = 3500.0f;
        private const float monsterDuckMaxFallSpeed = 250.0f;
        private const float monsterDuckJumpControlPower = 0.10f;
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

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
            set { isOnGround = value; }
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

        private bool isClimbingOnCeiling;
        public bool IsClimbingOnCeiling
        {
            get { return isClimbingOnCeiling; }
        }

        private bool wasClimbingOnCeiling;


        public bool IsCallingAnimal
        {
            get { return isCallingAnimal; }
        }
        private bool isCallingAnimal;

        public bool IsFlappingWings
        {
            get { return isFlappingWings; }
        }
        private bool isFlappingWings;
        private bool wasFlappingWings;


        public bool isScared;
        public bool wasScared;
        public int scaredDirection;
        private int scaredTimerMilliseconds = 2000;
        private float scaredTimerClock;

        private bool isDead;
        private bool isIdle;
        private bool hasReachedExit;

        // Jumping state
        private bool isBouncing;
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        private int flyAnimationTimerMilliseconds = 500;
        private float flyAnimationTimer = 0;

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
            CanBeDuck = true;
            CanBeMole = true;

            isIdle = true;
            hasReachedExit = false;

            needTutorial = -1;

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
            monsterCatClimbOnCeilingAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catClimbOnCeiling"), 0.1f, true);
            monsterCatClimbOnCeilingIdleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/catClimbOnCeilingIdle"), 0.1f, false);

            monsterDuckIdleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/duckIdle"), 0.1f, true);
            monsterDuckRunAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/duckRun"), 0.1f, true);
            monsterDuckJumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/duckJump"), 0.1f, false);
            monsterDuckFlyAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/duckFly"), 0.03f, true);
            monsterDuckDieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/duckDie"), 0.1f, false);


            transformationAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/transformation"), 0.1f, false);

            // Load sounds.           
            die0 = Level.Content.Load<SoundEffect>("Sounds/die/die0");
            die1 = Level.Content.Load<SoundEffect>("Sounds/die/die1");
            die2 = Level.Content.Load<SoundEffect>("Sounds/die/die2");
            die3 = Level.Content.Load<SoundEffect>("Sounds/die/die3");

            jump0 = Level.Content.Load<SoundEffect>("Sounds/jump/jump0");
            jump1 = Level.Content.Load<SoundEffect>("Sounds/jump/jump1");
            jump2 = Level.Content.Load<SoundEffect>("Sounds/jump/jump2");
            jump3 = Level.Content.Load<SoundEffect>("Sounds/jump/jump3");
            jump4 = Level.Content.Load<SoundEffect>("Sounds/jump/jump4");

            eatcat0 = Level.Content.Load<SoundEffect>("Sounds/eatcat/eatcat0");
            eatcat1 = Level.Content.Load<SoundEffect>("Sounds/eatcat/eatcat1");
            eatcat2 = Level.Content.Load<SoundEffect>("Sounds/eatcat/eatcat2");

            eatduck0 = Level.Content.Load<SoundEffect>("Sounds/eatduck/eatduck0");
            eatduck1 = Level.Content.Load<SoundEffect>("Sounds/eatduck/eatduck1");

            flap0 = Level.Content.Load<SoundEffect>("Sounds/flap/flap0");
            flap1 = Level.Content.Load<SoundEffect>("Sounds/flap/flap1");
            flap2 = Level.Content.Load<SoundEffect>("Sounds/flap/flap2");
            flap3 = Level.Content.Load<SoundEffect>("Sounds/flap/flap3");
            flap4 = Level.Content.Load<SoundEffect>("Sounds/flap/flap4");

            transformation0 = Level.Content.Load<SoundEffect>("Sounds/transformation/transformation0");
            transformation1 = Level.Content.Load<SoundEffect>("Sounds/transformation/transformation1");
            transformation2 = Level.Content.Load<SoundEffect>("Sounds/transformation/transformation2");

            exclamation0 = Level.Content.Load<SoundEffect>("Sounds/exclamation/exclamation0");
            exclamation1 = Level.Content.Load<SoundEffect>("Sounds/exclamation/exclamation1");
            exclamation2 = Level.Content.Load<SoundEffect>("Sounds/exclamation/exclamation2");

            checkpoint = Level.Content.Load<SoundEffect>("Sounds/checkpoint");
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
            isScared = false;
            scaredTimerClock = 0;

            isIdle = true;
            hasReachedExit = false;

            needTutorial = -1;

            // Get beck to MONSTER when killed
            changeShape(MONSTER);

            sprite.PlayAnimation(monsterIdleAnimation);            
        }

        /// <summary>
        /// Handles input, performs physics, resets some values and animates the player sprite.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            needTutorial = -1;
            GetInput();
            ApplyPhysics(gameTime);


            if (transformationTimerClock > 0)
            {
                transformationTimerClock -= gameTime.ElapsedGameTime.Milliseconds;
                sprite.PlayAnimation(transformationAnimation);
            }
            else
            {

                if (isScared)
                {
                    scaredTimerClock += gameTime.ElapsedGameTime.Milliseconds;
                    if (scaredTimerClock >= scaredTimerMilliseconds)
                    {
                        isScared = false;
                        scaredTimerClock = 0;
                    }
                }

                if (IsAlive)
                {
                    isIdle = false;
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

                                case MONSTER_DUCK:
                                    sprite.PlayAnimation(monsterDuckRunAnimation);
                                    break;
                            }

                        }
                        else
                        {
                            isIdle = true;
                            switch (animalShape)
                            {
                                case MONSTER:
                                    sprite.PlayAnimation(monsterIdleAnimation);
                                    break;
                                case MONSTER_CAT:
                                    sprite.PlayAnimation(monsterCatIdleAnimation);
                                    break;
                                case MONSTER_DUCK:
                                    sprite.PlayAnimation(monsterDuckIdleAnimation);
                                    break;
                            }
                        }
                    }
                    else if (isClimbingOnCeiling)
                    {
                        if (Math.Abs(Velocity.X) > 0)
                        {
                            sprite.PlayAnimation(monsterCatClimbOnCeilingAnimation);
                        }
                        else
                        {
                            sprite.PlayAnimation(monsterCatClimbOnCeilingIdleAnimation);
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
            }
            
            // Clear input.
            movementX = 0.0f;
            movementY = 0.0f;
            isJumping = false;
            

            wasClimbing = isClimbing;
            wasClimbingOnCeiling = isClimbingOnCeiling;
            wasFlappingWings = isFlappingWings;
            wasScared = isScared;

            if(!isDoingSpecialAction)
            {
                isClimbing = false;
                isClimbingOnCeiling = false;
                isFlappingWings = false;
                switch (animalShape)
                {
                    case MONSTER:
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
            isClimbingOnCeiling = false;
            isCallingAnimal = false;
            isDoingSpecialAction = false;
            isFlappingWings = false;


            // Key press used for debug reasons
            if (keyboardState.IsKeyDown(Keys.S))
            {
                movementY = movementY; // added just for having a line for debugging
            }

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                isDoingSpecialAction = true;

                switch (animalShape)
                {
                    case MONSTER:
                        isCallingAnimal = true;
                        break;
                    case MONSTER_CAT:
                        if (canClimb())
                        {
                            isClimbing = true;
                            isJumping = false;
                            isOnGround = false;
                        }
                        else if (canClimbOnCeiling())
                        {
                            isClimbing = true;
                            isClimbingOnCeiling = true;
                            isJumping = false;
                            isOnGround = false;
                        }
                        break;
                    case MONSTER_DUCK:
                        isFlappingWings = true;
                        break;
                    default:
                        break;
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

            if (isScared)
            {
                movementX = scaredDirection;
                lastMovementX = scaredDirection;
                if (!wasScared)
                    playRandomSound(EXCLAMATION);
            }

            // Check if the player wants to jump.
            if (!isClimbing && !isBouncing)
            {
                isJumping = keyboardState.IsKeyDown(Keys.Up);
            }
            else
            {
                isJumping = false;
            }

            if (keyboardState.IsKeyDown(Keys.D1))
            {
                changeShape(MONSTER);
            }
            else if (keyboardState.IsKeyDown(Keys.D2) && canBeCat)
            {
                changeShape(MONSTER_CAT);
            }
            else if (keyboardState.IsKeyDown(Keys.D3) && canBeDuck)
            {
                changeShape(MONSTER_DUCK);
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
                        // NEED TO BE CHANGED IN CLIMB DRAG FACTOR
                        velocity.Y = movementY * MoveAcceleration * elapsed;
                        velocity.Y *= monsterCatWallDragFactor;
                        velocity.Y = MathHelper.Clamp(velocity.Y, -MaxMoveSpeed, MaxMoveSpeed);

                        velocity.X += movementX * MoveAcceleration * elapsed;
                        velocity.X *= monsterCatWallDragFactor;

                       
                        // Apply velocity.
                        Position += velocity * elapsed;
                        Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

                    }
                    else if (wasClimbing && !isClimbing && !wasClimbingOnCeiling)
                    {
                        // Apply velocity.
                        Position += velocity * elapsed;
                        Position = new Vector2((float)Math.Round(Position.X+lastMovementX*5), (float)Math.Round(Position.Y-3));
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

                case MONSTER_DUCK:

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
            if (flyAnimationTimer > 0)
                flyAnimationTimer -= gameTime.ElapsedGameTime.Milliseconds;

            // If the player wants to jump
            if ((isJumping || isBouncing || !isOnGround))
            {
                switch (animalShape)
                {
                    case MONSTER:
                        
                        if(!isOnGround)
                            sprite.PlayAnimation(monsterJumpAnimation);
                        // Begin or continue a jump
                        if ((!wasJumping && IsOnGround) || isBouncing || jumpTime > 0.0f)
                        {
                            if (jumpTime == 0.0f)
                                playRandomSound(JUMP);

                            jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        break;

                    case MONSTER_CAT:
                        if (!isOnGround)
                            sprite.PlayAnimation(monsterCatJumpAnimation);
                        if ((!wasJumping && IsOnGround ) || isBouncing || jumpTime > 0.0f)
                        {
                            if (jumpTime == 0.0f)
                                playRandomSound(JUMP);

                            jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        break;

                    case MONSTER_DUCK:
                        if (!isOnGround)
                        {
                            if (flyAnimationTimer > 0)
                                sprite.PlayAnimation(monsterDuckFlyAnimation);
                            else
                                sprite.PlayAnimation(monsterDuckJumpAnimation);
                        }
                        // Begin or continue a jump
                        if (isFlappingWings && !wasFlappingWings)
                        {
                            sprite.PlayAnimation(monsterDuckFlyAnimation);
                            jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            isJumping = true;
                            flyAnimationTimer = flyAnimationTimerMilliseconds;
                            playRandomSound(FLAP);
                        }

                        else if ((!wasJumping && IsOnGround) || isBouncing || jumpTime > 0.0f)
                        {
                            if (jumpTime == 0.0f)
                                playRandomSound(JUMP);

                            jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        break;
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                   if ((isJumping && !isBouncing) || isFlappingWings)
                        velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                    else if (isBouncing)
                    {
                        if (animalShape == MONSTER)
                            velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower)) * 2;
                        else
                            velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                    }
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                    isBouncing = false;
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

            // Moving platform collision code 
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

            foreach (var verticalMovableTile in level.verticalMovableTiles)
            {
                // Reset flag to search for movable tile collision.  
                verticalMovableTile.PlayerIsOn = false;

                if ((Math.Abs(BoundingRectangle.Bottom - verticalMovableTile.BoundingRectangle.Top) < 20) &&
                    (BoundingRectangle.Left > verticalMovableTile.BoundingRectangle.Left - (BoundingRectangle.Width / 2) &&
                    BoundingRectangle.Right <= verticalMovableTile.BoundingRectangle.Right + (BoundingRectangle.Width / 2)))
                {
                    verticalMovableTile.PlayerIsOn = true;
                    IsOnGround = true;
                }
                bounds = HandleCollision(bounds, verticalMovableTile.Collision, verticalMovableTile.BoundingRectangle);
            } 

            //only checking for bouncy objects underneath the monster
            for (int x = leftTile; x <= rightTile; x++)
            {
                TileCollision collision = Level.GetCollision(x, bottomTile);
                if (collision == TileCollision.Bouncy && isAlive)
                {
                    isBouncing = true;
                }
                
             }
            //end of checking bouncy object

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);


                    if (collision == TileCollision.TutorialTile)
                    {
                        needTutorial = level.tiles[x, y].tutorialFlag;
                    }

                    if (animalShape == MONSTER && collision == TileCollision.HorizontalSwitch && isDoingSpecialAction)
                        level.activateHorizontalSwitch();
                    if (animalShape == MONSTER && collision == TileCollision.VerticalSwitch && isDoingSpecialAction)
                        level.activateVerticalSwitch();
                    if (animalShape == MONSTER && collision == TileCollision.SwitchWall1 && isDoingSpecialAction)
                        level.activateWall1Switch();
                    if (animalShape == MONSTER && collision == TileCollision.SwitchWall2 && isDoingSpecialAction)
                        level.activateWall2Switch();
                    if (animalShape == MONSTER && collision == TileCollision.SwitchWall3 && isDoingSpecialAction)
                        level.activateWall3Switch();
                    if (animalShape == MONSTER && collision == TileCollision.SwitchWall4 && isDoingSpecialAction)
                         level.activateWall4Switch();
                    if (animalShape == MONSTER && collision == TileCollision.SwitchWall5 && isDoingSpecialAction)
                        level.activateWall5Switch();


                    //start Checkpoint
                    if (collision == TileCollision.Checkpoint)
                    {
                        if (level.checkpoint.X != x*Tile.Width+(Tile.Width / 2))
                        {
                            level.actualLives = level.maxLives;
                            level.checkpoint = new Vector2(x, y) * Tile.Size;
                            level.checkpoint.X += Tile.Width / 2;
                            checkpoint.Play();
                        }
                    }
                    //end Checkpoint

                    if (collision == TileCollision.KillerTile)
                    {
                        this.OnKilled(false);
                    }
      
                    if (collision != TileCollision.Passable && 
                        collision != TileCollision.PlatformCollider && 
                        collision != TileCollision.KillerTile &&
                        collision != TileCollision.Checkpoint &&
                        collision == TileCollision.Disappearing )
                    {
                        if (Level.changeCollider == true)
                        {
                            //NOTHING
                            
                        }
                        else if (Level.changeCollider == false)
                        {
                            

                            Rectangle tileBounds = Level.GetBounds(x, y);
                            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                            // If we crossed the top of a tile, we are on the ground.
                            if (previousBottom <= tileBounds.Top)
                                isOnGround = true;
                            // Ignore platforms, unless we are on the ground.
                            if ((collision == TileCollision.Impassable || collision == TileCollision.LevelFrame || collision == TileCollision.Bouncy || collision == TileCollision.Destroyable) || IsOnGround)
                            {
                                // Resolve the collision along the Y axis.
                                Position = new Vector2(Position.X, Position.Y + depth.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                    //End of Disappearing platform

                    if (collision != TileCollision.Passable && 
                        collision != TileCollision.PlatformCollider && 
                        collision != TileCollision.KillerTile && 
                        collision != TileCollision.Disappearing &&
                        collision != TileCollision.Checkpoint &&
                        collision != TileCollision.TutorialTile &&
                        collision != TileCollision.HorizontalSwitch &&
                        collision != TileCollision.VerticalSwitch &&
                        collision != TileCollision.SwitchWall1 &&
                        collision != TileCollision.SwitchWall2 &&
                        collision != TileCollision.SwitchWall3 &&
                        collision != TileCollision.SwitchWall4 &&
                        collision != TileCollision.SwitchWall5 &&
                        collision != TileCollision.DestroyableWall1 &&
                        collision != TileCollision.DestroyableWall2 &&
                        collision != TileCollision.DestroyableWall3 &&
                        collision != TileCollision.DestroyableWall4 &&
                        collision != TileCollision.DestroyableWall5 &&
                        collision != TileCollision.YAxisLevelFrame)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            //start destroy tile
                            // RIGHT

                            TileCollision monsterLeftTopCollision = level.GetCollision(leftTile, topTile);
                            TileCollision monsterLeftBottomCollision = level.GetCollision(leftTile, topTile+1);
                            TileCollision monsterRightTopCollision = level.GetCollision(rightTile, topTile);
                            TileCollision monsterRightBottomCollision = level.GetCollision(rightTile, topTile+1);
                            float previousLeft = bounds.Left;
                            float previousRight = bounds.Right;
                            KeyboardState keyboardState = Keyboard.GetState();
                            
                            if (monsterRightTopCollision == TileCollision.Destroyable &&
                                keyboardState.IsKeyDown(Keys.Z) &&
                                keyboardState.IsKeyDown(Keys.Right) &&
                                isJumping != true &&
                                
                               // animalShape == MONSTER_MOLE && 
                                previousRight >= tileBounds.Right )
                            {
                               // x++;
                                //y--;
                                
                                Level.tiles[rightTile, topTile].Texture = null;
                                Level.tiles[rightTile, topTile].Collision = TileCollision.Passable;
                            }
                            if (monsterRightBottomCollision == TileCollision.Destroyable &&
                               keyboardState.IsKeyDown(Keys.Z) &&
                               keyboardState.IsKeyDown(Keys.Right) &&
                               isJumping != true &&

                              // animalShape == MONSTER_MOLE && 
                               previousRight >= tileBounds.Right)
                            {
                                Level.tiles[rightTile, topTile+1].Texture = null;
                                Level.tiles[rightTile, topTile+1].Collision = TileCollision.Passable;
                            }
                            // LEFT
                            if (monsterLeftTopCollision == TileCollision.Destroyable &&
                            keyboardState.IsKeyDown(Keys.Z) &&
                            keyboardState.IsKeyDown(Keys.Left) &&
                            //animalShape == MONSTER_MOLE &&
                            isJumping != true &&
                                previousLeft <= tileBounds.Left)
                            {

                               // x--;
                                //y--;
                                Level.tiles[leftTile, topTile].Texture = null;
                                Level.tiles[leftTile, topTile].Collision = TileCollision.Passable;

                            }
                            if (monsterLeftBottomCollision == TileCollision.Destroyable &&
                            keyboardState.IsKeyDown(Keys.Z) &&
                            keyboardState.IsKeyDown(Keys.Left) &&
                                //animalShape == MONSTER_MOLE &&
                            isJumping != true &&
                                previousLeft <= tileBounds.Left)
                            {
                                Level.tiles[leftTile, topTile+1].Texture = null;
                                Level.tiles[leftTile, topTile+1].Collision = TileCollision.Passable;
                            }

                            //end destroy tile 

                           

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if ((collision == TileCollision.Impassable || collision == TileCollision.LevelFrame || collision == TileCollision.Bouncy) || IsOnGround || collision == TileCollision.Destroyable)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable || collision == TileCollision.LevelFrame || collision == TileCollision.Bouncy || collision == TileCollision.Destroyable) // Ignore platforms.
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
                    if (collision == TileCollision.Impassable || IsOnGround || collision == TileCollision.Destroyable)
                    {
                        // Resolve the collision along the Y axis.  
                        Position = new Vector2(Position.X, Position.Y + depth.Y);

                        // Perform further collisions with the new bounds.  
                        bounds = BoundingRectangle;
                    }
                }
                else if (collision == TileCollision.Impassable || collision == TileCollision.Destroyable) // Ignore platforms.  
                {
                    // Resolve the collision along the X axis.  
                    Position = new Vector2(Position.X + depth.X, Position.Y);

                    // Perform further collisions with the new bounds.  
                    bounds = BoundingRectangle;
                }
            }
            return bounds;
        } 

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

            for (int y = topTile+1; y <= bottomTile; y++)
            {
                TileCollision collision = Level.GetCollision(side, y);
                if ((collision != TileCollision.Impassable && collision!= TileCollision.Destroyable)|| distance > 0 )
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
            int distance;

            distance = Math.Abs(bounds.Top - (topTile+1)*Tile.Height);

            TileCollision collision = Level.GetCollision(centralTile, topTile);
            if ((collision == TileCollision.Impassable && distance == 0) || (collision == TileCollision.Destroyable && distance == 0))
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
        public void OnKilled(bool something)
        {
            isAlive = false;

            if (!isDead)
            {
                isDead = true;
                playRandomSound(DIE);
            }

            switch (animalShape)
            {
                case MONSTER:
                    sprite.PlayAnimation(monsterDieAnimation);
                    break;
                case MONSTER_CAT:
                    sprite.PlayAnimation(monsterCatDieAnimation);
                    break;
                case MONSTER_DUCK:
                    sprite.PlayAnimation(monsterDuckDieAnimation);
                    break;
            }
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
                    playRandomSound(EATCAT);
                    break;
                case MONSTER_DUCK:
                    canBeDuck = true;
                    changeShape(MONSTER_DUCK);
                    playRandomSound(EATDUCK);
                    break;
            }
            
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            hasReachedExit = true;
            sprite.PlayAnimation(monsterCelebrateAnimation);
        }

        /// <summary>
        /// Called when the player change shape
        /// </summary>
        public void changeShape(short shapeFlag)
        {
            // Calculate standard bounding box dimension
            int width = (int)(monsterIdleAnimation.FrameWidth * 0.4);
            int left = (monsterIdleAnimation.FrameWidth - width) / 2;
            int height = (int)(monsterIdleAnimation.FrameWidth * 0.8);
            int top = monsterIdleAnimation.FrameHeight - height;

            if (animalShape != shapeFlag)
            {
                playRandomSound(TRANSFORMATION);

                transformationTimerClock = transformationTimerMilliseconds;

                animalShape = shapeFlag;

                

                switch (shapeFlag)
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

                        isScared = false;
                        scaredTimerClock = 0;

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

                    case MONSTER_DUCK:
                        // Constants for controling horizontal movement
                        MoveAcceleration = monsterDuckMoveAcceleration;
                        MaxMoveSpeed = monsterDuckMaxMoveSpeed;
                        GroundDragFactor = monsterDuckGroundDragFactor;
                        AirDragFactor = monsterDuckAirDragFactor;

                        // Constants for controlling vertical movement
                        MaxJumpTime = monsterDuckMaxJumpTime;
                        JumpLaunchVelocity = monsterDuckJumpLaunchVelocity;
                        GravityAcceleration = monsterDuckGravityAcceleration;
                        MaxFallSpeed = monsterDuckMaxFallSpeed;
                        JumpControlPower = monsterDuckJumpControlPower;

                        // Calculate bounding box dimension
                        width = (int)(monsterDuckIdleAnimation.FrameWidth * 0.4);
                        left = (monsterDuckIdleAnimation.FrameWidth - width) / 2;
                        height = (int)(monsterDuckIdleAnimation.FrameWidth * 0.8);
                        top = monsterDuckIdleAnimation.FrameHeight - height;

                        break;
                }

            }
            // Change bounding box dimension        
            localBounds = new Rectangle(left, top, width, height);
        }

        public void playRandomSound(short category)
        {
            int index;
            switch (category)
            {
                case DIE:
                    index = random.Next(4);
                    switch (index)
                    {
                        case 0:
                            die0.Play();
                            break;
                        case 1:
                            die1.Play();
                            break;
                        case 2:
                            die2.Play();
                            break;
                        case 3:
                            die3.Play();
                            break;
                    }
                    break;

                case JUMP:
                    index = random.Next(5);
                    switch (index)
                    {
                        case 0:
                            jump0.Play();
                            break;
                        case 1:
                            jump1.Play();
                            break;
                        case 2:
                            jump2.Play();
                            break;
                        case 3:
                            jump3.Play();
                            break;
                        case 4:
                            jump4.Play();
                            break;
                    }
                    break;

                case EATCAT:
                    index = random.Next(3);
                    switch (index)
                    {
                        case 0:
                            eatcat0.Play();
                            break;
                        case 1:
                            eatcat1.Play();
                            break;
                        case 2:
                            eatcat2.Play();
                            break;
                    }
                    break;

                case EATDUCK:
                    index = random.Next(2);
                    switch (index)
                    {
                        case 0:
                            eatduck0.Play();
                            break;
                        case 1:
                            eatduck1.Play();
                            break;
                    }
                    break;

                case FLAP:
                    index = random.Next(5);
                    switch (index)
                    {
                        case 0:
                            flap0.Play();
                            break;
                        case 1:
                            flap1.Play();
                            break;
                        case 2:
                            flap2.Play();
                            break;
                        case 3:
                            flap3.Play();
                            break;
                        case 4:
                            flap4.Play();
                            break;
                    }
                    break;

                case TRANSFORMATION:
                    index = random.Next(3);
                    switch (index)
                    {
                        case 0:
                            transformation0.Play();
                            break;
                        case 1:
                            transformation1.Play();
                            break;
                        case 2:
                            transformation2.Play();
                            break;
                    }
                    break;

                case EXCLAMATION:
                    index = random.Next(3);
                    switch (index)
                    {
                        case 0:
                            exclamation0.Play();
                            break;
                        case 1:
                            exclamation1.Play();
                            break;
                        case 2:
                            exclamation2.Play();
                            break;
                    }
                    break;
            }

        }


        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (isIdle || hasReachedExit || transformationTimerClock > 0)
                flip = SpriteEffects.None;
            else if (lastMovementX > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (lastMovementX < 0)
                flip = SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);

        }

        /// <summary>
        /// Get the position of player.
        /// </summary>
        public Vector2 getPosition()
        {
            return position;
        }
    }
}
