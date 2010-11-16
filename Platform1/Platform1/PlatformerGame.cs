using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;


namespace LearningXNA
{

    /// <summary>
    /// Facing direction along the X axis. Used to flip sprites of Animals and Enemies.
    /// </summary>
    enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Global content.
        private SpriteFont hudFont;
        private Texture2D hud;
        private Texture2D hud2;
        private Texture2D monsterAvailable;
        private Texture2D monsterActive;
        private Texture2D monsterCatNotAvailable;
        private Texture2D monsterCatAvailable;
        private Texture2D monsterCatActive;

        

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

        private Texture2D tutorialOverlay0;
        private Texture2D tutorialOverlay1;
        private Texture2D tutorialOverlay2;
        private Texture2D tutorialOverlay3;
        private Texture2D tutorialOverlay4;
        private Texture2D tutorialOverlay5;
        private Texture2D tutorialOverlay6;
        private Texture2D tutorialOverlay7;
        private Texture2D tutorialOverlay8;
        private Texture2D tutorialOverlay9;
        private Texture2D tutorialOverlay10;
        private Texture2D tutorialOverlay11;
        private Texture2D tutorialOverlay12;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;
        private bool wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);


        private const int TargetFrameRate = 60;
        private const int BackBufferWidth = 1280;
        private const int BackBufferHeight = 720;

        // Used to store tha ability to change animal between levels
        public bool canBeCat = false; //DEBUG REASON
        public bool canBeDuck = false;

        // Shape state of the character; starting from 0 as a monster
        const short MONSTER = 0;
        const short MONSTER_CAT = 1;
        const short MONSTER_DUCK = 2;

        public int totalScore = 0;

        public PlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;

            Content.RootDirectory = "Content";

            // Framerate differs between platforms.
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            // Load HUD stuff
            hud = Content.Load<Texture2D>("Overlays/hud/interface");
            hud2 = Content.Load<Texture2D>("Overlays/hud/interface2");
            monsterAvailable = Content.Load<Texture2D>("Overlays/hud/monster-available");
            monsterActive = Content.Load<Texture2D>("Overlays/hud/monster-active");
            monsterCatNotAvailable = Content.Load<Texture2D>("Overlays/hud/monstercat-notavailable");
            monsterCatAvailable = Content.Load<Texture2D>("Overlays/hud/monstercat-available");
            monsterCatActive = Content.Load<Texture2D>("Overlays/hud/monstercat-active");


            // Load overlay textures
            winOverlay = Content.Load<Texture2D>("Overlays/you_win");
            loseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
            diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

            tutorialOverlay0 = Content.Load<Texture2D>("Overlays/tutorial0");
            tutorialOverlay1 = Content.Load<Texture2D>("Overlays/tutorial1");
            tutorialOverlay2 = Content.Load<Texture2D>("Overlays/tutorial2");
            tutorialOverlay3 = Content.Load<Texture2D>("Overlays/tutorial3");
            tutorialOverlay4 = Content.Load<Texture2D>("Overlays/tutorial4");
            tutorialOverlay5 = Content.Load<Texture2D>("Overlays/tutorial5");
            tutorialOverlay6 = Content.Load<Texture2D>("Overlays/tutorial6");
            tutorialOverlay7 = Content.Load<Texture2D>("Overlays/tutorial7");
            tutorialOverlay8 = Content.Load<Texture2D>("Overlays/tutorial8");
            tutorialOverlay9 = Content.Load<Texture2D>("Overlays/tutorial9");
            tutorialOverlay10 = Content.Load<Texture2D>("Overlays/tutorial10");
            tutorialOverlay11 = Content.Load<Texture2D>("Overlays/tutorial11");
            tutorialOverlay12 = Content.Load<Texture2D>("Overlays/tutorial12");


            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Content.Load<Song>("Sounds/human_beat"));

            LoadNextLevel();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            level.Update(gameTime);

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Exit the game when back is pressed.
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            bool continuePressed = keyboardState.IsKeyDown(Keys.Space);

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!wasContinuePressed && continuePressed)
            {
                if (!level.Player.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                    {
                        totalScore += level.Score - totalScore;
                        LoadNextLevel();
                    }
                    else
                    {
                        totalScore = level.ScoreAtBeginning;
                        ReloadCurrentLevel();
                    }
                }
            }

            wasContinuePressed = continuePressed;
        }

        private void LoadNextLevel()
        {
            // Find the path of the next level.
            string levelPath;

            // Loop here so we can try again when we can't find a level.
            while (true)
            {
                // Try to find the next level. They are sequentially numbered txt files.
                levelPath = String.Format("Levels/{0}.txt", ++levelIndex);
                levelPath = Path.Combine(StorageContainer.TitleLocation, "Content/" + levelPath);
                if (File.Exists(levelPath))
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (levelIndex == 0)
                    throw new Exception("No levels found.");

                // Whenever we can't find a level, start over again at 0.
                levelIndex = -1;
            }

            // Unloads the content for the current level before loading the next one.
            if (level != null)
            {
                // Save the change shape abilities before disposing the actual level
                canBeCat = level.Player.CanBeCat;
                canBeDuck = level.Player.CanBeDuck;
                level.Dispose();
            }
            // Load the level.
            level = new Level(Services, levelPath);
            level.ScoreAtBeginning = totalScore;
            level.Score = totalScore;
            // Apply the old change shape abilities after loading the new level
            level.Player.CanBeCat = canBeCat;
            level.Player.CanBeDuck = canBeDuck;
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            

            level.Draw(gameTime, spriteBatch);

            DrawHud();

            

            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            spriteBatch.Begin();

            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);
            
            // Positions for timer and cookie counter
            Vector2 timerPos = new Vector2(53, 12);
            Vector2 cookiePos = new Vector2(290, 12);
            Vector2 livesPos = new Vector2(178, 12);
            Vector2 monsterIconPos = new Vector2(1016, 0);
            Vector2 catIconPos = new Vector2(1080, 0);

            spriteBatch.Draw(hud, hudLocation, Color.White);


            //Vector2 timerPos = new Vector2(53, 12);
            //Vector2 cookiePos = new Vector2(290, 12);
            //Vector2 livesPos = new Vector2(178, 12);
            //Vector2 monsterIconPos = new Vector2(-5, 60);
            //Vector2 catIconPos = new Vector2(-5, 125);

            //spriteBatch.Draw(hud2, hudLocation, Color.White);

            Texture2D monsterIcon = null;
            Texture2D catIcon = null;

            if (level.Player.animalShape == MONSTER)
                monsterIcon = monsterActive;
            else
                monsterIcon = monsterAvailable;

            if (level.Player.animalShape == MONSTER_CAT)
                catIcon = monsterCatActive;
            else if (level.Player.CanBeCat)
                catIcon = monsterCatAvailable;
            else
                catIcon = monsterCatNotAvailable;

            spriteBatch.Draw(monsterIcon, hudLocation + monsterIconPos, Color.White);
            spriteBatch.Draw(catIcon, hudLocation + catIconPos, Color.White);


            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if (level.TimeRemaining > WarningTime ||
                level.ReachedExit ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }
            DrawShadowedString(hudFont, timeString, hudLocation+timerPos, timeColor);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, level.Score.ToString(), hudLocation + cookiePos, Color.Yellow);

            // Draw Lives
            DrawShadowedString(hudFont, level.actualLives.ToString(), hudLocation + livesPos, Color.Yellow);

            // Determine the status overlay message to show.
            Texture2D status = null;
            if (level.TimeRemaining == TimeSpan.Zero)
            {
                if (level.ReachedExit)
                {
                    status = winOverlay;
                }
                else
                {
                    status = loseOverlay;
                }
            }
            else if (!level.Player.IsAlive)
            {
                status = diedOverlay;
            }

            if (Player.needTutorial != -1)
            {
                switch (Player.needTutorial)
                {
                    case 0:
                        status = tutorialOverlay0;
                        break;
                    case 1:
                        status = tutorialOverlay1;
                        break;
                    case 2:
                        status = tutorialOverlay2;
                        break;
                    case 3:
                        status = tutorialOverlay3;
                        break;
                    case 4:
                        status = tutorialOverlay4;
                        break;
                    case 5:
                        status = tutorialOverlay5;
                        break;
                    case 6:
                        status = tutorialOverlay6;
                        break;
                    case 7:
                        status = tutorialOverlay7;
                        break;
                    case 8:
                        status = tutorialOverlay8;
                        break;
                    case 9:
                        status = tutorialOverlay9;
                        break;
                    case 10:
                        status = tutorialOverlay10;
                        break;
                    case 11:
                        status = tutorialOverlay11;
                        break;
                    case 12:
                        status = tutorialOverlay12;
                        break;
                }
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }

            spriteBatch.End();
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
