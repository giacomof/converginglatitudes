using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;


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

        Video initialVideo;
        VideoPlayer videoPlayer;
        private bool checkVideo = true;


        // Global content.
        private SpriteFont hudFont;
        private Texture2D hud;
        private Texture2D hud2;
        private Texture2D monsterAvailable;
        private Texture2D monsterActive;
        private Texture2D monsterCatNotAvailable;
        private Texture2D monsterCatAvailable;
        private Texture2D monsterCatActive;
        private Texture2D monsterDuckNotAvailable;
        private Texture2D monsterDuckAvailable;
        private Texture2D monsterDuckActive;

        private Texture2D monsterSomethingNotAvailable;
               

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


        private Random random = new Random();
        private bool alreadyLost = false;

        const short YOULOSE = 0;

        private SoundEffect youlose0;
        private SoundEffect youlose1;
        private SoundEffect youlose2;
        private SoundEffect youlose3;
        private SoundEffect youlose4;


        // Meta-level game state.
        private int levelIndex = 2;

        private Level level;
        private bool wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);


        private const int TargetFrameRate = 60;
        private const int BackBufferWidth = 1280;
        private const int BackBufferHeight = 720;

        // Used to store tha ability to change animal between levels
        public bool canBeCat = true; //DEBUG REASON
        public bool canBeDuck = true;
        public bool canBeMole = true;

        //DEBUG INTERFACE CHANGE
        public bool changeInterface;
        public bool wasChangedInterface;

        // Shape state of the character; starting from 0 as a monster
        const short MONSTER = 0;
        const short MONSTER_CAT = 1;
        const short MONSTER_DUCK = 2;
        const short MONSTER_MOLE = 3;

        public int totalScore = 0;

        public PlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            graphics.PreferredBackBufferHeight = BackBufferHeight;

            Content.RootDirectory = "Content";

            videoPlayer = new VideoPlayer();

            // Framerate differs between platforms.
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);

            changeInterface = true;
            wasChangedInterface = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            initialVideo = Content.Load<Video>("Video/Story");

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
            monsterDuckNotAvailable = Content.Load<Texture2D>("Overlays/hud/monsterduck-notavailable");
            monsterDuckAvailable = Content.Load<Texture2D>("Overlays/hud/monsterduck-available");
            monsterDuckActive = Content.Load<Texture2D>("Overlays/hud/monsterduck-active");

            monsterSomethingNotAvailable = Content.Load<Texture2D>("Overlays/hud/monstersomething-notavailable");


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

            youlose0 = Content.Load<SoundEffect>("Sounds/youlose/youlose0");
            youlose1 = Content.Load<SoundEffect>("Sounds/youlose/youlose1");
            youlose2 = Content.Load<SoundEffect>("Sounds/youlose/youlose2");
            youlose3 = Content.Load<SoundEffect>("Sounds/youlose/youlose3");
            youlose4 = Content.Load<SoundEffect>("Sounds/youlose/youlose4");


            MediaPlayer.IsRepeating = true;

            videoPlayer.Play(initialVideo);

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

            if (checkVideo)
            {
                if (videoPlayer.State == MediaState.Stopped)
                {
                    videoPlayer.Dispose();
                    MediaPlayer.Play(Content.Load<Song>("Sounds/human_beat"));
                    checkVideo = false;
                }
            }
            else
            {

                level.Update(gameTime);

                base.Update(gameTime);
            }
            
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Exit the game when back is pressed.
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.I))
            {
                if(!wasChangedInterface)
                    changeInterface = !changeInterface;
                wasChangedInterface = true;
            }
            else
            {
                wasChangedInterface = false;
            }

            bool continuePressed = keyboardState.IsKeyDown(Keys.Space);

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!wasContinuePressed && continuePressed)
            {
                if(!videoPlayer.IsDisposed)
                    videoPlayer.Stop();

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
                        level.Player.CanBeCat = canBeCat;
                        level.Player.CanBeDuck = canBeDuck;
                        level.Player.CanBeMole = canBeMole;
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
                if (level.ReachedExit)
                {
                    // Save the change shape abilities before disposing the actual level
                    canBeCat = level.Player.CanBeCat;
                    canBeDuck = level.Player.CanBeDuck;
                    canBeMole = level.Player.CanBeMole;
                }
                level.Dispose();
            }
            // Load the level.
            level = new Level(Services, levelPath, levelIndex);
            level.ScoreAtBeginning = totalScore;
            level.Score = totalScore;
            // Apply the old change shape abilities after loading the new level
            level.Player.CanBeCat = canBeCat;
            level.Player.CanBeDuck = canBeDuck;
            level.Player.CanBeMole = canBeMole;
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
            graphics.GraphicsDevice.Clear(Color.Black);

            if (!videoPlayer.IsDisposed)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, initialVideo.Width, initialVideo.Height), Color.White);
                spriteBatch.End();
            }
            else
            {

                level.Draw(gameTime, spriteBatch);

                DrawHud();

                base.Draw(gameTime);
            }
        }

        private void DrawHud()
        {
            spriteBatch.Begin();


            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);
            
            // Positions for timer and cookie counter
            Vector2 timerPos;
            Vector2 cookiePos;
            Vector2 livesPos;
            Vector2 monsterIconPos;
            Vector2 catIconPos;
            Vector2 duckIconPos;
            Vector2 somethingIconPos;

            if (changeInterface)
            {
                timerPos = new Vector2(53, 12);
                cookiePos = new Vector2(290, 12);
                livesPos = new Vector2(178, 12);
                monsterIconPos = new Vector2(1016, 0);
                catIconPos = new Vector2(1080, 0);
                duckIconPos = new Vector2(1144, 0);
                somethingIconPos = new Vector2(1210, 0);

                spriteBatch.Draw(hud, hudLocation, Color.White);
            }
            else
            {
                timerPos = new Vector2(53, 12);
                cookiePos = new Vector2(290, 12);
                livesPos = new Vector2(178, 12);
                monsterIconPos = new Vector2(0, 58);
                catIconPos = new Vector2(0, 121);
                duckIconPos = new Vector2(0, 185);
                somethingIconPos = new Vector2(0, 244);

                spriteBatch.Draw(hud2, hudLocation, Color.White);
            }

            Texture2D monsterIcon = null;
            Texture2D catIcon = null;
            Texture2D duckIcon = null;
            Texture2D somethingIcon = null;

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

            if (level.Player.animalShape == MONSTER_DUCK)
                duckIcon = monsterDuckActive;
            else if (level.Player.CanBeDuck)
                duckIcon = monsterDuckAvailable;
            else
                duckIcon = monsterDuckNotAvailable;

            somethingIcon = monsterSomethingNotAvailable; 
            

            spriteBatch.Draw(monsterIcon, hudLocation + monsterIconPos, Color.White);
            spriteBatch.Draw(catIcon, hudLocation + catIconPos, Color.White);
            spriteBatch.Draw(duckIcon, hudLocation + duckIconPos, Color.White);
            spriteBatch.Draw(somethingIcon, hudLocation + somethingIconPos, Color.White);


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
                    if (!alreadyLost)
                    {
                        playRandomSound(YOULOSE);
                        alreadyLost = true;
                    }
                    status = loseOverlay;
                }
            }
            else if (!level.Player.IsAlive)
            {
                status = diedOverlay;
                alreadyLost = false;
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

        public void playRandomSound(short category)
        {
            int index;
            switch (category)
            {
                case YOULOSE:
                    index = random.Next(5);
                    switch (index)
                    {
                        case 0:
                            youlose0.Play();
                            break;
                        case 1:
                            youlose1.Play();
                            break;
                        case 2:
                            youlose2.Play();
                            break;
                        case 3:
                            youlose3.Play();
                            break;
                        case 4:
                            youlose4.Play();
                            break;
                    }
                    break;

            }

        }
    }
}
