﻿
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;


namespace LearningXNA
{
    
    /// <summary>
    /// A uniform grid of tiles with collections of Cookies and enemies.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    class Level : IDisposable
    {
        public struct Coordinates
        {
            public int x;
            public int y;

            public Coordinates(int xCoord, int yCoord)
            {
                x = xCoord;
                y = yCoord;
            }
        }

        // Shape state of the character; starting from 0 as a monster
        const short MONSTER         = 0;
        const short MONSTER_CAT     = 1;
        const short MONSTER_DUCK    = 2;

        const short WIN             = 0;
        const short LIGHTBULB       = 1;

        int scaredDistance = 250;

        private int SwitchTimerMilliseconds = 500;
        private bool horizontalMovingPlatformsActive = false;
        private float horizontalSwitchTimerClock = 0;
        private bool verticalMovingPlatformsActive = false;
        private float verticalSwitchTimerClock = 0;

        private bool destroyableWall1Active = true;
        private float destroyableWall1SwitchTimerClock = 0;
        private bool destroyableWall2Active = true;
        private float destroyableWall2SwitchTimerClock = 0;
        private bool destroyableWall3Active = true;
        private float destroyableWall3SwitchTimerClock = 0;
        private bool destroyableWall4Active = true;
        private float destroyableWall4SwitchTimerClock = 0;
        private bool destroyableWall5Active = true;
        private float destroyableWall5SwitchTimerClock = 0;

        public double elapsedTime = 0;
        public bool changeCollider = false;

        public int levelIndex;

        // Physical structure of the level.
        public Tile[,] tiles;

        private Layer[] layers;
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        private SpriteFont hudFont;
        private Vector2 numberPos;

        private bool rightCallingDistance;
        private bool alreadyRightCallingDistance;

        private Texture2D disappearingTileOpen;
        private Texture2D switchTileOn;
        private Texture2D switch1On;
        private Texture2D switch2On;
        private Texture2D switch3On;
        private Texture2D switch4On;
        private Texture2D switch5On;

        private Texture2D lightBulb;
        private Texture2D exclamationMark;

        // Entities in the level.
        
        public Player Player
        {
            get { return player; }
        }
        Player player;

        
        public List<MovableTile> movableTiles = new List<MovableTile>();
        public List<VerticalMovableTile> verticalMovableTiles = new List<VerticalMovableTile>();
        private List<Cookie> cookies = new List<Cookie>();
        private List<Enemy> enemies = new List<Enemy>();
        private List<EnemyOnCeiling> enemiesOnCeiling = new List<EnemyOnCeiling>();
        private List<JumpingEnemy> jumpingenemies = new List<JumpingEnemy>();
        private List<FlyingEnemy> flyingenemies = new List<FlyingEnemy>();
        private List<CircleFlyingEnemy> circleFlyingenemies = new List<CircleFlyingEnemy>();
        private List<HorizontalFlyingEnemy> horizontalFlyingenemies = new List<HorizontalFlyingEnemy>();
        private List<HorizontalFlyingEnemy> horizontalFlyingenemies2 = new List<HorizontalFlyingEnemy>();
        private List<HorizontalFlyingEnemy> horizontalFlyingenemies3 = new List<HorizontalFlyingEnemy>();
        private List<Animal> animals = new List<Animal>();
        private List<DogEnemy> dogEnemies = new List<DogEnemy>();
        private List<FallingObject> fallingObjects = new List<FallingObject>();
        public List<Coordinates> destroyableWalls1 = new List<Coordinates>();
        public List<Coordinates> destroyableWalls2 = new List<Coordinates>();
        public List<Coordinates> destroyableWalls3 = new List<Coordinates>();
        public List<Coordinates> destroyableWalls4 = new List<Coordinates>();
        public List<Coordinates> destroyableWalls5 = new List<Coordinates>();

        // Key locations in the level.        
        private Vector2 start;
        //start checkpoint
        public Vector2 checkpoint;
        public int maxLives = 4;
        public int actualLives = 0;
        //end checkpoint
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        // Level game state.
        private Random random = new Random();
        private float cameraPosition;


        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        int score;

        public int ScoreAtBeginning
        {
            get { return scoreAtBeginning; }
            set { scoreAtBeginning = value; }
        }
        int scoreAtBeginning;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;

        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
        }
        TimeSpan timeRemaining;

        private int secondsCounter = 0;
        private const int PointsPer15Seconds = 1;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private SoundEffect win0;
        private SoundEffect win1;
        private SoundEffect win2;
        private SoundEffect win3;

        private SoundEffect lightbulb0;
        private SoundEffect lightbulb1;

        private SoundEffect leverSound;

        #region Loading

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="path">
        /// The absolute path to the level file to be loaded.
        /// </param>
        public Level(IServiceProvider serviceProvider, string path, int level)
        {
            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "Content");

            timeRemaining = TimeSpan.FromMinutes(10.0);

            levelIndex = level;

            LoadTiles(path);

            // Load background layer textures. For now, all levels must
            // use the same backgrounds and only use the left-most part of them.
            layers = new Layer[4];
            string layerPath = "Backgrounds/level" + (levelIndex+1);
            layers[0] = new Layer(Content, layerPath + "/Layer0", 0.1f);
            layers[1] = new Layer(Content, layerPath + "/Layer1", 0.5f);

            if (level+1 == 1)
                layers[2] = new Layer(Content, layerPath + "/Layer2", 0.8f);
            else
                layers[2] = new Layer(Content, layerPath + "/Layer2", 1.0f);

            layers[3] = new Layer(Content, layerPath + "/Layer3", 1.0f);

            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            disappearingTileOpen = Content.Load<Texture2D>("Tiles/drawerOpen");
            switchTileOn = Content.Load<Texture2D>("Tiles/switchOn");

            switch1On = Content.Load<Texture2D>("Tiles/switch1On");
            switch2On = Content.Load<Texture2D>("Tiles/switch2On");
            switch3On = Content.Load<Texture2D>("Tiles/switch3On");
            switch4On = Content.Load<Texture2D>("Tiles/switch4On");
            switch5On = Content.Load<Texture2D>("Tiles/switch5On");

            lightBulb = Content.Load<Texture2D>("EventIcons/lightBulb");
            exclamationMark = Content.Load<Texture2D>("EventIcons/exclamation");

            rightCallingDistance = false;

            actualLives = maxLives;
            checkpoint = Vector2.Zero;

            // Load sounds.
            win0 = Content.Load<SoundEffect>("Sounds/win/win0");
            win1 = Content.Load<SoundEffect>("Sounds/win/win1");
            win2 = Content.Load<SoundEffect>("Sounds/win/win2");
            win3 = Content.Load<SoundEffect>("Sounds/win/win3");

            lightbulb0 = Content.Load<SoundEffect>("Sounds/lightbulb/lightbulb0");
            lightbulb1 = Content.Load<SoundEffect>("Sounds/lightbulb/lightbulb1");

            leverSound = Content.Load<SoundEffect>("Sounds/leverSound");
        }

        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="path">
        /// The absolute path to the level file to be loaded.
        /// </param>
        private void LoadTiles(string path)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");

        }

        /// <summary>
        /// Loads an individual tile's appearance and behavior.
        /// </summary>
        /// <param name="tileType">
        /// The character loaded from the structure file which
        /// indicates what should be loaded.
        /// </param>
        /// <param name="x">
        /// The X location of this tile in tile space.
        /// </param>
        /// <param name="y">
        /// The Y location of this tile in tile space.
        /// </param>
        /// <returns>The loaded tile.</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // EQUALS FOR EVERY LEVEL
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);
                // Exit
                case 'X':
                    return LoadExitTile(x, y);
                // Cookie
                case '°':
                    return LoadCookieTile(x, y);

                // SPECIAL TILES
                // Falling object
                case 'f':
                    return LoadFallingObjectTile(x, y, 0);
                case 'ƭ':
                    return LoadFallingObjectTile(x, y, 1);
                case 'ƒ':
                    return LoadFallingObjectTile(x, y, 2);
                // Floating platform
                case '-':
                    return LoadTile("Platform", TileCollision.Platform);

                // Various enemies
                case 'A':
                    return LoadEnemyTile(x, y, "RCCarEnemy");
                case 'S':
                    return LoadEnemyTile(x, y, "Spider");
                case 'F':
                    return LoadFlyingEnemyTile(x, y, "Helicopter");
                case 'B':
                    return LoadFlyingEnemyTile(x, y, "Bumblebee");

                case 'J':
                    return LoadJumpingEnemyTile(x, y, "Piranha");

                case 'Ǒ':
                    return LoadCircleFlyingEnemyTile(x, y, "Helicopter");
                case 'ß':
                    return LoadCircleFlyingEnemyTile(x, y, "Bumblebee");

                case 'Ƚ':
                    return LoadHorizontalFlyingEnemyTile(x, y, false, "Bumblebee");
                case 'ᶅ':
                    return LoadHorizontalFlyingEnemyTile(x, y, true, "Bumblebee");

                case 'L':
                    return LoadHorizontalFlyingEnemyTile2(x, y, false, "Bumblebee");
                case 'K':
                    return LoadHorizontalFlyingEnemyTile3(x, y, false, "Bumblebee");

                case 'W':
                    return LoadEnemyOnCeilingTile(x, y, "CeilingSpider");

                // Various animals
                case 'C':
                    return LoadAnimalTile(x, y, "Cat");
                case 'D':
                    return LoadAnimalTile(x, y, "Duck");
                case 'M':
                    return LoadAnimalTile(x, y, "Mole");

                // Various enemy animals
                // Load dog
                case 'd':
                    return LoadDogEnemyTile(x, y);

                // Platform block
                case '~':
                    return LoadVarietyTile("bookshelf", 3, TileCollision.Platform);

                case 'Ǝ':
                    return LoadVarietyTile("treetrunk", 3, TileCollision.Platform);

                case 'À':
                    return LoadTile("branch0", TileCollision.Platform);
                case 'Á':
                    return LoadTile("branch1", TileCollision.Platform);
                case 'Â':
                    return LoadTile("branch2", TileCollision.Platform);

                case 'Ã':
                    return LoadTile("branch5", TileCollision.Platform);
                case 'Ä':
                    return LoadTile("branch4", TileCollision.Platform);
                case 'Å':
                    return LoadTile("branch3", TileCollision.Platform);

                // Passable block
                case ':':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Passable);


                // Player 1 start point
                case 'P':
                    return LoadStartTile(x, y);

                // Impassable block
                case '#':
                    return LoadVarietyTile("BlockA", 3, TileCollision.Impassable);
                case '=':
                    return LoadVarietyTile("Tile", 2, TileCollision.Impassable);
                case '∏':
                    return LoadVarietyTile("rock", 3, TileCollision.Impassable);
                case 'H':
                    return LoadVarietyTile("stones", 3, TileCollision.Impassable);
                case '∂':
                    return LoadVarietyTile("water", 3, TileCollision.Passable);


                case '□':
                    return LoadInvisibleTile(TileCollision.Impassable);
                case '¯':
                    return LoadInvisibleTile(TileCollision.Platform);

                // Bouncing Object
                case '♣':
                    return LoadTile("flower", TileCollision.Bouncy);
                case 'Ω':
                    return LoadTile("mushroom", TileCollision.Bouncy);
                case '§':
                    return LoadTile("Trampolin", TileCollision.Bouncy);
                case '≈':
                    return LoadVarietyTile("cloud", 3, TileCollision.Bouncy);
                // Spikes
                case '╩':
                    return LoadVarietyTile("Cactus", 2, TileCollision.KillerTile);
                case 'Ѱ':
                    return LoadVarietyTile("spikes", 2, TileCollision.KillerTile);

                case '┘':
                    return LoadVarietyTile("rightSpikes", 2, TileCollision.KillerTile);
                case '└':
                    return LoadVarietyTile("leftSpikes", 2, TileCollision.KillerTile);

                case 'Ж':
                    return LoadVarietyTile("pendingSpikes", 2, TileCollision.KillerTile);
                // Invisible killing tile
                case '⌂':
                    return LoadInvisibleTile(TileCollision.KillerTile);
                // Checkpoint
                case '¤':
                    return LoadTile("checkPoint", TileCollision.Checkpoint);


                //MOVING PLATFORM STUFF
                case '^':
                    return LoadVerticalMovableTile(x, y, TileCollision.Platform, false);
                case '∆':
                    return LoadVerticalMovableTile(x, y, TileCollision.Platform, true);
                case '<':
                    return LoadMovableTile(x, y, TileCollision.Platform, false, 0);
                case 'Ƨ':
                    return LoadMovableTile(x, y, TileCollision.Platform, false, 1);
                case '≤':
                    return LoadMovableTile(x, y, TileCollision.Platform, true, 0);
                case 'Ƶ':
                    return LoadMovableTile(x, y, TileCollision.Platform, true, 1);

                case '┤':
                    return LoadTile("switchOff", TileCollision.HorizontalSwitch);
                case '┴':
                    return LoadTile("switchOff", TileCollision.VerticalSwitch);

                case '|':
                    return LoadTile("Platform", TileCollision.PlatformCollider);
                //END OF MOVING PLATFORM STUFF

                case '˥':
                    return LoadTile("switch1Off", TileCollision.SwitchWall1);
                case '˦':
                    return LoadTile("switch2Off", TileCollision.SwitchWall2);
                case '˧':
                    return LoadTile("switch3Off", TileCollision.SwitchWall3);
                case '†':
                    return LoadTile("switch4Off", TileCollision.SwitchWall4);
                case '‡':
                    return LoadTile("switch5Off", TileCollision.SwitchWall5);
                case '░':
                    return LoadDestroyableTile("lockTile1", 1, x, y);
                case '▒':
                    return LoadDestroyableTile("lockTile2", 2, x, y);
                case '▓':
                    return LoadDestroyableTile("lockTile3", 3, x, y);
                case '█':
                    return LoadDestroyableTile("lockTile4", 4, x, y);
                case '▐':
                    return LoadDestroyableTile("lockTile5", 5, x, y);

                case '*':
                    return LoadTile("drawerOpen", TileCollision.Disappearing);
                case '+':
                    return LoadVarietyTile("rock", 3, TileCollision.Disappearing);


                case '„':
                    return LoadVarietyTile("grass", 3, TileCollision.Impassable);

                case 'Æ':
                    return LoadVarietyTile("earth", 3, TileCollision.Destroyable);


                case 'ü':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 0);
                case 'é':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 1);
                case 'ä':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 2);
                case 'ů':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 3);
                case 'ç':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 4);
                case 'ł':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 5);
                case 'ë':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 6);
                case 'ő':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 7);
                case 'Ź':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 8);
                case 'ÿ':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 9);
                case 'Ș':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 10);
                case 'Ț':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 11);
                case 'ĉ':
                    return LoadTutorialTile("Tutorial", TileCollision.TutorialTile, 12);


                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadVerticalMovableTile(int x, int y, TileCollision collision, bool controllable)
        {
            Point position = GetBounds(x, y).Center;
            verticalMovableTiles.Add(new VerticalMovableTile(this, new Vector2(position.X, position.Y), collision, controllable));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Loads a moving tile.
        /// </summary>
        private Tile LoadMovableTile(int x, int y, TileCollision collision, bool controllable, int flag)
        {
            Point position = GetBounds(x, y).Center;
            movableTiles.Add(new MovableTile(this, new Vector2(position.X, position.Y), collision, controllable, flag));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Loads a falling object.
        /// </summary>
        private Tile LoadFallingObjectTile(int x, int y, int flag)
        {
            Point position = GetBounds(x, y).Center;
            fallingObjects.Add(new FallingObject(this, new Vector2(position.X, position.Y), flag));
            return new Tile(null, TileCollision.Passable);
        }


        /// <summary>
        /// Creates a new tile. The other tile loading methods typically chain to this
        /// method after performing their special logic.
        /// </summary>
        /// <param name="name">
        /// Path to a tile texture relative to the Content/Tiles directory.
        /// </param>
        /// <param name="collision">
        /// The tile collision type for the new tile.
        /// </param>
        /// <returns>The new tile.</returns>
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }

        private Tile LoadDestroyableTile(string name, int destroyableWallIndex, int x, int y)
        {
            switch (destroyableWallIndex)
            {
                case 1:
                    destroyableWalls1.Add(new Coordinates(x, y));
                    return new Tile(Content.Load<Texture2D>("Tiles/" + name), TileCollision.Impassable);
                case 2:
                    destroyableWalls2.Add(new Coordinates(x, y));
                    return new Tile(Content.Load<Texture2D>("Tiles/" + name), TileCollision.Impassable);
                case 3:
                    destroyableWalls3.Add(new Coordinates(x, y));
                    return new Tile(Content.Load<Texture2D>("Tiles/" + name), TileCollision.Impassable);
                case 4:
                    destroyableWalls4.Add(new Coordinates(x, y));
                    return new Tile(Content.Load<Texture2D>("Tiles/" + name), TileCollision.Impassable);
                case 5:
                    destroyableWalls5.Add(new Coordinates(x, y));
                    return new Tile(Content.Load<Texture2D>("Tiles/" + name), TileCollision.Impassable);
                default:
                    return new Tile();
            }
        }

        private Tile LoadSwitchTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }

        private Tile LoadInvisibleTile(TileCollision collision)
        {
            return new Tile(null, collision);
        }

        private Tile LoadTutorialTile(string name, TileCollision collision, int flag)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision, flag);
        }


        /// <summary>
        /// Loads a tile with a random appearance.
        /// </summary>
        /// <param name="baseName">
        /// The content name prefix for this group of tile variations. Tile groups are
        /// name LikeThis0.png and LikeThis1.png and LikeThis2.png.
        /// </param>
        /// <param name="variationCount">
        /// The number of variations in this group.
        /// </param>
        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }


        /// <summary>
        /// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, start);

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;

            return LoadTile("Exit", TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates an enemy and puts him in the level.
        /// </summary>
        private Tile LoadEnemyTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            enemies.Add(new Enemy(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates an enemyonceiling and puts him in the level.
        /// </summary>
        private Tile LoadEnemyOnCeilingTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            enemiesOnCeiling.Add(new EnemyOnCeiling(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates a Jumpingenemy and puts him in the level.
        /// </summary>
        private Tile LoadJumpingEnemyTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            jumpingenemies.Add(new JumpingEnemy(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates an animal and puts him in the level.
        /// </summary>
        private Tile LoadAnimalTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            animals.Add(new Animal(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates a dog and puts him in the level.
        /// </summary>
        private Tile LoadDogEnemyTile(int x, int y)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            dogEnemies.Add(new DogEnemy(this, position));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates a flying enemy and puts him in the level.
        /// </summary>
        private Tile LoadFlyingEnemyTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            flyingenemies.Add(new FlyingEnemy(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates a circle flying enemy and puts him in the level.
        /// </summary>
        private Tile LoadCircleFlyingEnemyTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            circleFlyingenemies.Add(new CircleFlyingEnemy(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadHorizontalFlyingEnemyTile(int x, int y, bool fast, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            horizontalFlyingenemies.Add(new HorizontalFlyingEnemy(this, position, fast, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }
        private Tile LoadHorizontalFlyingEnemyTile2(int x, int y, bool fast, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            horizontalFlyingenemies2.Add(new HorizontalFlyingEnemy(this, position, fast, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }
        private Tile LoadHorizontalFlyingEnemyTile3(int x, int y, bool fast, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            horizontalFlyingenemies3.Add(new HorizontalFlyingEnemy(this, position, fast, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }


        /// <summary>
        /// Instantiates a Cookie and puts it in the level.
        /// </summary>
        private Tile LoadCookieTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            cookies.Add(new Cookie(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        #region Bounds and collision

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.LevelFrame;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.YAxisLevelFrame;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            rightCallingDistance = false;

            if(horizontalSwitchTimerClock >= 0)
                horizontalSwitchTimerClock -= gameTime.ElapsedGameTime.Milliseconds;
            if (verticalSwitchTimerClock >= 0)
                verticalSwitchTimerClock -= gameTime.ElapsedGameTime.Milliseconds;
            if(destroyableWall1SwitchTimerClock >= 0)
                destroyableWall1SwitchTimerClock -= gameTime.ElapsedGameTime.Milliseconds;
            if (destroyableWall2SwitchTimerClock >= 0)
                destroyableWall2SwitchTimerClock -= gameTime.ElapsedGameTime.Milliseconds;
            if (destroyableWall3SwitchTimerClock >= 0)
                destroyableWall3SwitchTimerClock -= gameTime.ElapsedGameTime.Milliseconds;
            if (destroyableWall4SwitchTimerClock >= 0)
                destroyableWall4SwitchTimerClock -= gameTime.ElapsedGameTime.Milliseconds;
            if (destroyableWall5SwitchTimerClock >= 0)
                destroyableWall5SwitchTimerClock -= gameTime.ElapsedGameTime.Milliseconds;


            // Pause while the player is dead or time is expired.
            if (!Player.IsAlive || TimeRemaining == TimeSpan.Zero)
            {
                // Still want to perform physics on the player.
                Player.ApplyPhysics(gameTime);
            }
            else if (ReachedExit)
            {
                // Animate the time being converted into points.
                int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
                secondsCounter += seconds;
                if (secondsCounter >= 15)
                {
                    secondsCounter -= 15;
                    score += PointsPer15Seconds;
                    leverSound.Play();
                }
                timeRemaining -= TimeSpan.FromSeconds(seconds);
            }
            else
            {
                timeRemaining -= gameTime.ElapsedGameTime;


                Player.Update(gameTime);


                UpdateCookies(gameTime);

                UpdateFallingObjects(gameTime);

                // Update moving platforms
                UpdateMovableTiles(gameTime);
                UpdateVerticalMovableTiles(gameTime);

                // Falling off the bottom of the level kills the player.
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(false);

                UpdateDisappearingTile(gameTime);
                UpdateJumpingEnemies(gameTime);
                UpdateEnemies(gameTime);
                UpdateEnemiesOnCeiling(gameTime);
                UpdateFlyingEnemies(gameTime);
                UpdateCircleFlyingenemies(gameTime);
                UpdateHorizontalFlyingenemies(gameTime);
                UpdateHorizontalFlyingenemies2(gameTime);
                UpdateHorizontalFlyingenemies3(gameTime);
                UpdateAnimals(gameTime);
                UpdateDogEnemy(gameTime);

                // The player has reached the exit if they are standing on the ground and
                // his bounding rectangle contains the center of the exit tile. They can only
                // exit when they have collected all of the Cookies.
                if (Player.IsAlive &&
                    Player.IsOnGround &&
                    Player.BoundingRectangle.Contains(exit))
                {
                    OnExitReached();
                }
            }

            // Clamp the time remaining at zero.
            if (timeRemaining < TimeSpan.Zero)
                timeRemaining = TimeSpan.Zero;
        }

        // Update falling objects
        private void UpdateFallingObjects(GameTime gameTime)
        {
            for (int i = 0; i < fallingObjects.Count; ++i)
            {
                FallingObject fallingObject = fallingObjects[i];
                if (fallingObject.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(false);
                } 
                fallingObject.Update(gameTime);
            }
        }


        // Moving platform code
        private void UpdateMovableTiles(GameTime gameTime)
        {
            for (int i = 0; i < movableTiles.Count; ++i)
            {
                MovableTile movableTile = movableTiles[i];
                movableTile.Update(gameTime, horizontalMovingPlatformsActive);

                if (movableTile.PlayerIsOn && 
                    ((!movableTile.isControllable) || (horizontalMovingPlatformsActive && movableTile.isControllable)))
                {
                    //Make player move with tile if the player is on top of tile 
                    player.Position += movableTile.Velocity;
                }
            }
        }

        private void UpdateVerticalMovableTiles(GameTime gameTime)
        {
            for (int i = 0; i < verticalMovableTiles.Count; ++i)
            {
                VerticalMovableTile verticalMovableTile = verticalMovableTiles[i];
                verticalMovableTile.Update(gameTime, verticalMovingPlatformsActive);

                if (verticalMovableTile.PlayerIsOn &&
                    ((!verticalMovableTile.isControllable) || (verticalMovingPlatformsActive && verticalMovableTile.isControllable)))
                {
                    //Make player move with tile if the player is on top of tile
                    player.Position += verticalMovableTile.Velocity;
                }
            }
        }

        ////DISAPPEARING PLATFORM
        public void UpdateDisappearingTile(GameTime gameTime)
        {
            int seconds = gameTime.TotalRealTime.Seconds;
            if ((seconds / 5) % 2 == 0)
            {
                changeCollider = true;
            }
            else
            {
                changeCollider = false;
            }
               
        }
        ////END OF DISAPPEARING PLATFORM

        /// <summary>
        /// Animates each Cookie and checks to allows the player to collect them.
        /// </summary>
        private void UpdateCookies(GameTime gameTime)
        {
            for (int i = 0; i < cookies.Count; ++i)
            {
                Cookie cookie = cookies[i];
               
                cookie.Update(gameTime);

                if (cookie.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    cookies.RemoveAt(i--);
                    OnCookieCollected(cookie, Player);
                }
            }
        }

        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);

                // Touching an enemy instantly kills the player
                if (enemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }

        /// <summary>
        /// Animates each enemyonceiling and allow them to kill the player.
        /// </summary>
        private void UpdateEnemiesOnCeiling(GameTime gameTime)
        {
            foreach (EnemyOnCeiling enemyOnCeiling in enemiesOnCeiling)
            {
                enemyOnCeiling.Update(gameTime);

                // Touching an enemy instantly kills the player
                if (enemyOnCeiling.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }

        /// <summary>
        /// Animates each jumpingenemy and allow them to kill the player.
        /// </summary>
        private void UpdateJumpingEnemies(GameTime gameTime)
        {
            foreach (JumpingEnemy jumpingenemy in jumpingenemies)
            {
                jumpingenemy.Update(gameTime);

                // Touching an enemy instantly kills the player
                if (jumpingenemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }
        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateFlyingEnemies(GameTime gameTime)
        {
            foreach (FlyingEnemy flyingenemy in flyingenemies)
            {
                flyingenemy.Update(gameTime);

                // Touching an enemy instantly kills the player
                if (flyingenemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }


        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateCircleFlyingenemies(GameTime gameTime)
        {
            foreach (CircleFlyingEnemy circleFlyingenemy in circleFlyingenemies)
            {
                circleFlyingenemy.Update(gameTime);

                // Touching an enemy instantly kills the player
                if (circleFlyingenemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }

        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateHorizontalFlyingenemies(GameTime gameTime)
        {
            foreach (HorizontalFlyingEnemy horizontalFlyingenemy in horizontalFlyingenemies)
            {
                horizontalFlyingenemy.Update(gameTime);

                // Touching an enemy instantly kills the player
                if (horizontalFlyingenemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }
        private void UpdateHorizontalFlyingenemies2(GameTime gameTime)
        {
            foreach (HorizontalFlyingEnemy horizontalFlyingenemy in horizontalFlyingenemies2)
            {
                horizontalFlyingenemy.Update(gameTime);

                // Touching an enemy instantly kills the player
                if (horizontalFlyingenemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }
        private void UpdateHorizontalFlyingenemies3(GameTime gameTime)
        {
            foreach (HorizontalFlyingEnemy horizontalFlyingenemy in horizontalFlyingenemies3)
            {
                horizontalFlyingenemy.Update(gameTime);

                // Touching an enemy instantly kills the player
                if (horizontalFlyingenemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(true);
                }
            }
        }

        /// <summary>
        /// Animates each animal and allow them to be eated by the player.
        /// </summary>
        private void UpdateAnimals(GameTime gameTime)
        {
            bool playerIsCalling = Player.IsCallingAnimal;

            foreach (Animal animal in animals)
            {
                if(animal.Update(gameTime, playerIsCalling, player.getPosition()))
                    rightCallingDistance = true;

                // Touching an enemy instantly kills the player
                if (animal.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerEated(animal);
                    animals.Remove(animal);
                    break;
                }
            }
        }

        /// <summary>
        /// Animates each dog enemy.
        /// </summary>
        private void UpdateDogEnemy(GameTime gameTime)
        {
            Vector2 playerPosition = Player.getPosition();
            Vector2 dogPosition;
            float distance;
            foreach (DogEnemy dog in dogEnemies)
            {
                dogPosition = dog.getPosition();
                distance = Vector2.Distance(dogPosition, playerPosition);
                if (distance < scaredDistance && !player.isScared && player.animalShape != MONSTER)
                {
                    player.isScared = true;
                    if (dogPosition.X - playerPosition.X < 0)
                    {
                        player.scaredDirection = 1;
                    }
                    else
                    {
                        player.scaredDirection = -1;
                    }
                }
                dog.Update(gameTime);

            }
        }


        /// <summary>
        /// Called when a Cookie is collected.
        /// </summary>
        /// <param name="cookie">The Cookie that was collected.</param>
        /// <param name="collectedBy">The player who collected this Cookie.</param>
        private void OnCookieCollected(Cookie cookie, Player collectedBy)
        {
            score += Cookie.PointValue;

            cookie.OnCollected(collectedBy);
        }

        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This is null if the player was not killed by an
        /// enemy, such as when a player falls into a hole.
        /// </param>
        private void OnPlayerKilled(bool someone)
        {
            Player.OnKilled(someone);
        }

        /// <summary>
        /// Called when the player eats an animal.
        /// </summary>
        /// <param name="killedBy">
        /// The animal who was eaten by the player.
        /// </param>
        private void OnPlayerEated(Animal eatenAnimal)
        {
            Player.OnPlayerEated(eatenAnimal);
        }

        /// <summary>
        /// Called when the player reaches the level's exit.
        /// </summary>
        private void OnExitReached()
        {
            Player.OnReachedExit();
            playRandomSound(WIN);
            reachedExit = true;
        }


        /// <summary>
        /// Called when activating the horizontal switch
        /// </summary>
        public void activateHorizontalSwitch()
        {
            if (horizontalSwitchTimerClock <= 0)
            {
                leverSound.Play();
                horizontalSwitchTimerClock = SwitchTimerMilliseconds;
                horizontalMovingPlatformsActive = !horizontalMovingPlatformsActive;
            }
        }

        /// <summary>
        /// Called when activating the vertical switch
        /// </summary>
        public void activateVerticalSwitch()
        {
            if (verticalSwitchTimerClock <= 0)
            {
                leverSound.Play();
                verticalSwitchTimerClock = SwitchTimerMilliseconds;
                verticalMovingPlatformsActive = !verticalMovingPlatformsActive;
            }
        }

        /// <summary>
        /// Called to destroy walls
        /// </summary>
        public void activateWall1Switch()
        {
            if (destroyableWall1SwitchTimerClock <= 0)
            {
                leverSound.Play();
                destroyableWall1SwitchTimerClock = SwitchTimerMilliseconds;
                if(destroyableWall1Active)
                    foreach (Coordinates coordinate in destroyableWalls1)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.DestroyableWall1;
                else
                    foreach (Coordinates coordinate in destroyableWalls1)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.Impassable;

                destroyableWall1Active = !destroyableWall1Active;
            }
        }

        public void activateWall2Switch()
        {
            if (destroyableWall2SwitchTimerClock <= 0)
            {
                leverSound.Play();
                destroyableWall2SwitchTimerClock = SwitchTimerMilliseconds;
                if (destroyableWall2Active)
                    foreach (Coordinates coordinate in destroyableWalls2)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.DestroyableWall2;
                else
                    foreach (Coordinates coordinate in destroyableWalls2)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.Impassable;

                destroyableWall2Active = !destroyableWall2Active;
            }
        }

        public void activateWall3Switch()
        {
            if (destroyableWall3SwitchTimerClock <= 0)
            {
                leverSound.Play();
                destroyableWall3SwitchTimerClock = SwitchTimerMilliseconds;
                if (destroyableWall3Active)
                    foreach (Coordinates coordinate in destroyableWalls3)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.DestroyableWall3;
                else
                    foreach (Coordinates coordinate in destroyableWalls3)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.Impassable;

                destroyableWall3Active = !destroyableWall3Active;
            }
        }

        public void activateWall4Switch()
        {
            if (destroyableWall4SwitchTimerClock <= 0)
            {
                leverSound.Play();
                destroyableWall4SwitchTimerClock = SwitchTimerMilliseconds;
                if (destroyableWall4Active)
                    foreach (Coordinates coordinate in destroyableWalls4)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.DestroyableWall4;
                else
                    foreach (Coordinates coordinate in destroyableWalls4)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.Impassable;

                destroyableWall4Active = !destroyableWall4Active;
            }
        }

        public void activateWall5Switch()
        {
            if (destroyableWall5SwitchTimerClock <= 0)
            {
                leverSound.Play();
                destroyableWall5SwitchTimerClock = SwitchTimerMilliseconds;
                if (destroyableWall5Active)
                    foreach (Coordinates coordinate in destroyableWalls5)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.DestroyableWall5;
                else
                    foreach (Coordinates coordinate in destroyableWalls5)
                        tiles[coordinate.x, coordinate.y].Collision = TileCollision.Impassable;

                destroyableWall5Active = !destroyableWall5Active;
            }
        }

        /// <summary>
        /// Restores the player to the starting point to try the level again.
        /// </summary>
        /// 
        //start checkpoint
        public void StartNewLife()
        {
            if (actualLives == 0)
            {
                Player.Reset(start);
                timeRemaining = TimeSpan.Zero;
            }
            else if (checkpoint != Vector2.Zero)
            {
                Player.Reset(checkpoint);
                actualLives -= 1;
            }
            else
            {
                Player.Reset(start);
                actualLives -= 1;
            }
        }
        //end checkpoint


        

        public void playRandomSound(short category)
        {
            int index;
            switch (category)
            {
                case WIN:
                    index = random.Next(4);
                    switch (index)
                    {
                        case 0:
                            win0.Play();
                            break;
                        case 1:
                            win1.Play();
                            break;
                        case 2:
                            win2.Play();
                            break;
                        case 3:
                            win3.Play();
                            break;
                    }
                    break;

                case LIGHTBULB:
                    index = random.Next(2);
                    switch (index)
                    {
                        case 0:
                            lightbulb0.Play();
                            break;
                        case 1:
                            lightbulb1.Play();
                            break;
                    }
                    break;
            }
        }


        #endregion

        #region Draw

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int i = 0; i <= EntityLayer; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();

            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition, 0.0f, 0.0f);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, cameraTransform);

            DrawTiles(spriteBatch);


            foreach (Cookie cookie in cookies)
                cookie.Draw(gameTime, spriteBatch);

            foreach (FallingObject fallingObject in fallingObjects)
                fallingObject.Draw(gameTime, spriteBatch);

            Player.Draw(gameTime, spriteBatch);
            Vector2 eventIconPosition = player.Position + new Vector2(-10, -130);
            if (player.isScared)
            {
                spriteBatch.Draw(exclamationMark, eventIconPosition, Color.White);
            }
            if (rightCallingDistance)
            {
                if (!alreadyRightCallingDistance)
                    playRandomSound(LIGHTBULB);

                spriteBatch.Draw(lightBulb, eventIconPosition, Color.White);
                
            }
            alreadyRightCallingDistance = rightCallingDistance;

            ////MOVING PLATFORM STUFF
            foreach (MovableTile tile in movableTiles)
                tile.Draw(gameTime, spriteBatch);
            foreach (VerticalMovableTile tile in verticalMovableTiles)
                tile.Draw(gameTime, spriteBatch);
            ////END OF MOVING PLATFORM STUFF


            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime, spriteBatch);

            foreach (EnemyOnCeiling enemyOnCeiling in enemiesOnCeiling)
                enemyOnCeiling.Draw(gameTime, spriteBatch);

            foreach (JumpingEnemy jumpingenemy in jumpingenemies)
                jumpingenemy.Draw(gameTime, spriteBatch);

            foreach (FlyingEnemy flyingenemy in flyingenemies)
                flyingenemy.Draw(gameTime, spriteBatch);

            foreach (CircleFlyingEnemy circleFlyingEnemy in circleFlyingenemies)
                circleFlyingEnemy.Draw(gameTime, spriteBatch);

            foreach (HorizontalFlyingEnemy horizontalFlyingEnemy in horizontalFlyingenemies)
                horizontalFlyingEnemy.Draw(gameTime, spriteBatch);
            foreach (HorizontalFlyingEnemy horizontalFlyingEnemy in horizontalFlyingenemies2)
                horizontalFlyingEnemy.Draw(gameTime, spriteBatch);
            foreach (HorizontalFlyingEnemy horizontalFlyingEnemy in horizontalFlyingenemies3)
                horizontalFlyingEnemy.Draw(gameTime, spriteBatch);

            foreach (Animal animal in animals)
                animal.Draw(gameTime, spriteBatch);

            foreach (DogEnemy dog in dogEnemies)
                dog.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            for (int i = EntityLayer + 1; i < layers.Length; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();
        }

        private void ScrollCamera(Viewport viewport)
        {
            const float ViewMargin = 0.35f;


            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * ViewMargin;
            float marginLeft = cameraPosition + marginWidth;
            float marginRight = cameraPosition + viewport.Width - marginWidth;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.0f;
            if (Player.Position.X < marginLeft)
                cameraMovement = Player.Position.X - marginLeft;
            else if (Player.Position.X > marginRight)
                cameraMovement = Player.Position.X - marginRight;

            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPosition = Tile.Width * Width - viewport.Width;
            cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
        }


        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // Calculate the visible range of tiles.
            int left = (int)Math.Floor(cameraPosition / Tile.Width);
            int right = left + spriteBatch.GraphicsDevice.Viewport.Width / Tile.Width;
            right = Math.Min(right, Width - 1);

            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;

                    

                    TileCollision collision = this.GetCollision(x, y);
                    if (texture != null && collision != TileCollision.PlatformCollider &&
                        collision != TileCollision.DestroyableWall1 &&
                        collision != TileCollision.DestroyableWall2 &&
                        collision != TileCollision.DestroyableWall3 &&
                        collision != TileCollision.DestroyableWall4 &&
                        collision != TileCollision.DestroyableWall5) 
                        
                    {
                        Vector2 position = new Vector2(x, y) * Tile.Size;

                        switch (collision)
                        {
                            case TileCollision.Disappearing:
                                if (!changeCollider)
                                    spriteBatch.Draw(texture, position, Color.White);
                                //else
                                //    spriteBatch.Draw(disappearingTileOpen, position, Color.White);
                                break;
                            case TileCollision.HorizontalSwitch:
                                if (horizontalMovingPlatformsActive)
                                    spriteBatch.Draw(switchTileOn, position, Color.White);
                                else
                                    spriteBatch.Draw(texture, position, Color.White);
                                break;
                            case TileCollision.VerticalSwitch:
                                if (verticalMovingPlatformsActive)
                                    spriteBatch.Draw(switchTileOn, position, Color.White);
                                else
                                    spriteBatch.Draw(texture, position, Color.White);
                                break;
                            case TileCollision.SwitchWall1:
                                if (destroyableWall1Active)
                                    spriteBatch.Draw(texture, position, Color.White);
                                else
                                    spriteBatch.Draw(switch1On, position, Color.White);
                                break;
                            case TileCollision.SwitchWall2:
                                if (destroyableWall2Active)
                                    spriteBatch.Draw(texture, position, Color.White);
                                else
                                    spriteBatch.Draw(switch2On, position, Color.White);
                                break;
                            case TileCollision.SwitchWall3:
                                if (destroyableWall3Active)
                                    spriteBatch.Draw(texture, position, Color.White);
                                else
                                    spriteBatch.Draw(switch3On, position, Color.White);
                                break;
                            case TileCollision.SwitchWall4:
                                if (destroyableWall4Active)
                                    spriteBatch.Draw(texture, position, Color.White);
                                else
                                    spriteBatch.Draw(switch4On, position, Color.White);
                                break;
                            case TileCollision.SwitchWall5:
                                if (destroyableWall5Active)
                                    spriteBatch.Draw(texture, position, Color.White);
                                else
                                    spriteBatch.Draw(switch5On, position, Color.White);
                                break;
                            default:
                                spriteBatch.Draw(texture, position, Color.White);
                                break;
                        }
                    }

                    // Write number of lives over the checkpoint
                    if (collision != TileCollision.Checkpoint && (x * Tile.Width + Tile.Width / 2) == checkpoint.X)
                    {
                        numberPos = new Vector2(checkpoint.X - 5, checkpoint.Y - 30);
                        
                        spriteBatch.DrawString(hudFont, actualLives.ToString(), numberPos + new Vector2(1.0f, 1.0f), Color.Black);
                        spriteBatch.DrawString(hudFont, actualLives.ToString(), numberPos, Color.Yellow);
                    }
                }
            }
        }

        #endregion
    }
}
