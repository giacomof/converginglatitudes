
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
        // Shape state of the character; starting from 0 as a monster
        const short MONSTER = 0;
        const short MONSTER_CAT = 1;
        const short MONSTER_DUCK = 2;



        public double elapsedTime = 0;
        public bool changeCollider = false;
        // Physical structure of the level.
        public Tile[,] tiles;
        private Layer[] layers;
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        // Entities in the level.
        
        public Player Player
        {
            get { return player; }
        }
        Player player;

        public List<MovableTile> movableTiles = new List<MovableTile>();
        private List<Cookie> cookies = new List<Cookie>();
        private List<Enemy> enemies = new List<Enemy>();
        private List<Animal> animals = new List<Animal>();
        private List<DogEnemy> dogEnemies = new List<DogEnemy>();
        private List<FallingObject> fallingObjects = new List<FallingObject>();

        // Key locations in the level.        
        private Vector2 start;
        //start checkpoint
        public Vector2 checkpoint;
        //end checkpoint
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        // Level game state.
        private Random random = new Random(354668); // Arbitrary, but constant seed
        private float cameraPosition;


        public int Score
        {
            get { return score; }
        }
        int score;

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

        private const int PointsPerSecond = 5;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private SoundEffect exitReachedSound;

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
        public Level(IServiceProvider serviceProvider, string path)
        {
            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "Content");

            timeRemaining = TimeSpan.FromMinutes(2.0);

            LoadTiles(path);

            // Load background layer textures. For now, all levels must
            // use the same backgrounds and only use the left-most part of them.
            layers = new Layer[3];
            layers[0] = new Layer(Content, "Backgrounds/Layer0", 0.1f);
            layers[1] = new Layer(Content, "Backgrounds/Layer1", 0.5f);
            layers[2] = new Layer(Content, "Backgrounds/Layer2", 1.0f);

            // Load sounds.
            exitReachedSound = Content.Load<SoundEffect>("Sounds/ExitReached");
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
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Exit
                case 'X':
                    return LoadExitTile(x, y);

                // Cookie
                case '°':
                    return LoadCookieTile(x, y);

                // Falling object
                case 'f':
                    return LoadFallingObjectTile(x, y);

                // Floating platform
                case '-':
                    return LoadTile("Platform", TileCollision.Platform);

                // Various enemies
                case 'A':
                    return LoadEnemyTile(x, y, "MonsterA");
                case 'B':
                    return LoadEnemyTile(x, y, "MonsterB");
                case 'D':
                    return LoadEnemyTile(x, y, "MonsterD");

                // Various animals
                case 'C':
                    return LoadAnimalTile(x, y, "Cat");

                // Various enemy animals
                case 'd':
                    return LoadDogEnemyTile(x, y);

                // Platform block
                case '~':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Platform);

                // Passable block
                case ':':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Passable);

                // Player 1 start point
                case '1':
                    return LoadStartTile(x, y);

                // Impassable block
                case '#':
                    return LoadVarietyTile("BlockA", 7, TileCollision.Impassable);

                // Bouncing Object
                case '§':
                    return LoadTile("Platform", TileCollision.Bouncy);
                // Spikes
                case '╩':
                    return LoadTile("Spikes", TileCollision.KillerTile);
                // Checkpoint
                case '¤':
                    return LoadTile("BlockA0", TileCollision.Checkpoint);


                //MOVING PLATFORM STUFF
                // Moving platform - Horizontal
                case '<':
                    return LoadMovableTile(x, y, TileCollision.Platform);
                case '|':
                    return LoadTile("Platform", TileCollision.PlatformCollider);
                //END OF MOVING PLATFORM STUFF

                case '*':
                    return LoadTile("Platform", TileCollision.Disappearing);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }


        /// <summary>
        /// Loads a moving tile.
        /// </summary>
        private Tile LoadMovableTile(int x, int y, TileCollision collision)
        {
            Point position = GetBounds(x, y).Center;
            movableTiles.Add(new MovableTile(this, new Vector2(position.X, position.Y), collision));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Loads a falling object.
        /// </summary>
        private Tile LoadFallingObjectTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            fallingObjects.Add(new FallingObject(this, new Vector2(position.X, position.Y)));
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
                return TileCollision.Passable;

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
                timeRemaining -= TimeSpan.FromSeconds(seconds);
                score += seconds * PointsPerSecond;
            }
            else
            {
                timeRemaining -= gameTime.ElapsedGameTime;


                Player.Update(gameTime);

                // Update moving platforms
                UpdateMovableTiles(gameTime);

                UpdateCookies(gameTime);

                UpdateFallingObjects(gameTime);

                // Falling off the bottom of the level kills the player.
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(null);

                UpdateDisappearingTile(gameTime);
                UpdateEnemies(gameTime);
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
                    OnPlayerKilled(null);
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
                movableTile.Update(gameTime);

                if (movableTile.PlayerIsOn)
                {
                    //Make player move with tile if the player is on top of tile  
                    player.Position += movableTile.Velocity;
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
                    OnPlayerKilled(enemy);
                }
            }
        }

        /// <summary>
        /// Animates each animal and allow them to be eated by the player.
        /// </summary>
        private void UpdateAnimals(GameTime gameTime)
        {
            foreach (Animal animal in animals)
            {
                animal.Update(gameTime);

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
                if (distance < 200 && !player.isScared && player.animalShape == MONSTER_CAT )
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
        private void OnPlayerKilled(Enemy killedBy)
        {
            Player.OnKilled(killedBy);
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
            exitReachedSound.Play();
            reachedExit = true;
        }

        /// <summary>
        /// Restores the player to the starting point to try the level again.
        /// </summary>
        /// 
        //start checkpoint
        public void StartNewLife()
        {
            if (checkpoint != Vector2.Zero) Player.Reset(checkpoint);
            else
                Player.Reset(start);
        }
        //end checkpoint


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

            ////MOVING PLATFORM STUFF
            foreach (MovableTile tile in movableTiles)
                tile.Draw(gameTime, spriteBatch);
            ////END OF MOVING PLATFORM STUFF

            foreach (Cookie cookie in cookies)
                cookie.Draw(gameTime, spriteBatch);

            foreach (FallingObject fallingObject in fallingObjects)
                fallingObject.Draw(gameTime, spriteBatch);

            Player.Draw(gameTime, spriteBatch);

            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime, spriteBatch);

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

                    //Level.tiles[x, y].Texture = null;
                    TileCollision collision = this.GetCollision(x, y);
                    if (texture != null && collision != TileCollision.PlatformCollider)
                    {
                        if (!(collision == TileCollision.Disappearing && changeCollider))
                        {
                            // Draw it in screen space.
                            Vector2 position = new Vector2(x, y) * Tile.Size;
                            spriteBatch.Draw(texture, position, Color.White);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
