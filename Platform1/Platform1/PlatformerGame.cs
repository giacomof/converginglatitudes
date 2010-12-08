using System;
using System.IO;
using System.Xml.Serialization;
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

    [Serializable]
    public struct HighScoreData
    {
        public int[] Score;

        public int Count;

        public HighScoreData(int count)
        {
            Score = new int[count];
            Count = count;
        }
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

        public readonly string HighScoresFilename = "highscores.lst";

        // Global content.
        private SpriteFont hudFont;
        private SpriteFont highScoreFont;
        private Texture2D hud;
        private Texture2D hud2;
        private Texture2D hud3;
        private Texture2D monsterAvailable;
        private Texture2D monsterActive;
        private Texture2D monsterCatNotAvailable;
        private Texture2D monsterCatAvailable;
        private Texture2D monsterCatActive;
        private Texture2D monsterDuckNotAvailable;
        private Texture2D monsterDuckAvailable;
        private Texture2D monsterDuckActive;
        private Texture2D monsterMoleNotAvailable;
        private Texture2D monsterMoleAvailable;
        private Texture2D monsterMoleActive;
               

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

        private Texture2D gameTitle;
        private Texture2D highScore;
        private Texture2D credits;


        private Random random = new Random();
        private bool alreadyLost = false;

        const short YOULOSE = 0;

        private SoundEffect youlose0;
        private SoundEffect youlose1;
        private SoundEffect youlose2;
        private SoundEffect youlose3;
        private SoundEffect youlose4;

        private int levelIndex = -1;

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
        public int actualHud = 2;
        public bool changeInterface;
        public bool wasChangedInterface;

        // Shape state of the character; starting from 0 as a monster
        const short MONSTER             = 0;
        const short MONSTER_CAT         = 1;
        const short MONSTER_DUCK        = 2;
        const short MONSTER_MOLE        = 3;

        public int totalScore = 0;

        const int SHOW_TITLE     = 0;
        const int SHOW_VIDEO     = 1;
        const int NORMAL_PLAY    = 2;
        const int HIGH_SCORE     = 3;
        const int CREDITS        = 4;
        
        public int gameState = 0;

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
            highScoreFont = Content.Load<SpriteFont>("Fonts/HighScore");

            // Load HUD stuff
            hud = Content.Load<Texture2D>("Overlays/hud/interface");
            hud2 = Content.Load<Texture2D>("Overlays/hud/interface2");
            hud3 = Content.Load<Texture2D>("Overlays/hud/interface3");
            monsterAvailable = Content.Load<Texture2D>("Overlays/hud/monster-available");
            monsterActive = Content.Load<Texture2D>("Overlays/hud/monster-active");
            monsterCatNotAvailable = Content.Load<Texture2D>("Overlays/hud/monstercat-notavailable");
            monsterCatAvailable = Content.Load<Texture2D>("Overlays/hud/monstercat-available");
            monsterCatActive = Content.Load<Texture2D>("Overlays/hud/monstercat-active");
            monsterDuckNotAvailable = Content.Load<Texture2D>("Overlays/hud/monsterduck-notavailable");
            monsterDuckAvailable = Content.Load<Texture2D>("Overlays/hud/monsterduck-available");
            monsterDuckActive = Content.Load<Texture2D>("Overlays/hud/monsterduck-active");
            monsterMoleNotAvailable = Content.Load<Texture2D>("Overlays/hud/monstermole-notavailable");
            monsterMoleAvailable = Content.Load<Texture2D>("Overlays/hud/monstermole-available");
            monsterMoleActive = Content.Load<Texture2D>("Overlays/hud/monstermole-active");


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

            youlose0 = Content.Load<SoundEffect>("Sounds/youlose/youlose0");
            youlose1 = Content.Load<SoundEffect>("Sounds/youlose/youlose1");
            youlose2 = Content.Load<SoundEffect>("Sounds/youlose/youlose2");
            youlose3 = Content.Load<SoundEffect>("Sounds/youlose/youlose3");
            youlose4 = Content.Load<SoundEffect>("Sounds/youlose/youlose4");


            gameTitle = Content.Load<Texture2D>("Overlays/title");
            highScore = Content.Load<Texture2D>("Overlays/highScore");
            credits = Content.Load<Texture2D>("Overlays/credits");


            MediaPlayer.IsRepeating = true;

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

            switch (gameState)
            {
                case SHOW_TITLE:
                    //do something
                    break;

                case SHOW_VIDEO:
                    if (videoPlayer.State == MediaState.Stopped)
                    {
                        videoPlayer.Dispose();
                        MediaPlayer.Play(Content.Load<Song>("Sounds/human_beat"));
                        gameState = NORMAL_PLAY;
                    }
                    break;

                case NORMAL_PLAY:
                    level.Update(gameTime);
                    base.Update(gameTime);
                    break;

                case HIGH_SCORE:
                    //do something
                    break;

                case CREDITS:
                    //do something
                    break;
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
                if (!wasChangedInterface)
                {
                    changeInterface = !changeInterface;
                    actualHud++;
                    if (actualHud > 2)
                        actualHud = 0;
                }

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
                switch (gameState)
                {
                    case SHOW_TITLE:
                        gameState = SHOW_VIDEO;
                        videoPlayer.Play(initialVideo);
                        break;

                    case SHOW_VIDEO:
                        gameState = NORMAL_PLAY;
                        videoPlayer.Stop();
                        MediaPlayer.Play(Content.Load<Song>("Sounds/human_beat"));
                        break;

                    case NORMAL_PLAY:
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
                        break;

                    case HIGH_SCORE:
                        gameState = CREDITS;
                        break;

                    case CREDITS:
                        Exit();
                        break;
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
                if (levelIndex == 7)
                {
                    gameState = HIGH_SCORE;
                    SaveHighScore(totalScore);
                    break;
                }
                if (File.Exists(levelPath))
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (levelIndex == 0)
                    throw new Exception("No levels found.");

                // Whenever we can't find a level, start over again at 0.
                levelIndex = -1;
            }

            if (levelIndex != 7)
            {
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

            switch (gameState)
            {
                case SHOW_TITLE:
                    spriteBatch.Begin();
                    spriteBatch.Draw(gameTitle, new Rectangle(0, 0, 1280, 720), Color.White);
                    spriteBatch.End();
                    break;

                case SHOW_VIDEO:
                    spriteBatch.Begin();
                    spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, initialVideo.Width, initialVideo.Height), Color.White);
                    spriteBatch.End();
                    break;

                case NORMAL_PLAY:
                    level.Draw(gameTime, spriteBatch);
                    DrawHud();
                    base.Draw(gameTime);
                    break;

                case HIGH_SCORE:
                    spriteBatch.Begin();
                    spriteBatch.Draw(highScore, new Rectangle(0, 0, 1280, 720), Color.White);
                    printHighScore();
                    spriteBatch.End();
                    break;

                case CREDITS:
                    spriteBatch.Begin();
                    spriteBatch.Draw(credits, new Rectangle(0, 0, 1280, 720), Color.White);
                    spriteBatch.End();
                    break;
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
            Vector2 timerPos = Vector2.Zero;
            Vector2 cookiePos = Vector2.Zero;
            Vector2 livesPos = Vector2.Zero;
            Vector2 monsterIconPos = Vector2.Zero;
            Vector2 catIconPos = Vector2.Zero;
            Vector2 duckIconPos = Vector2.Zero;
            Vector2 moleIconPos = Vector2.Zero;

            switch (actualHud)
            {
                case 0:
                    timerPos = new Vector2(53, 12);
                    cookiePos = new Vector2(290, 12);
                    livesPos = new Vector2(178, 12);
                    monsterIconPos = new Vector2(1016, 0);
                    catIconPos = new Vector2(1080, 0);
                    duckIconPos = new Vector2(1144, 0);
                    moleIconPos = new Vector2(1210, 0);

                    spriteBatch.Draw(hud, hudLocation, Color.White);
                    break;

                case 1:
                    timerPos = new Vector2(53, 12);
                    cookiePos = new Vector2(290, 12);
                    livesPos = new Vector2(178, 12);
                    monsterIconPos = new Vector2(0, 58);
                    catIconPos = new Vector2(0, 121);
                    duckIconPos = new Vector2(0, 185);
                    moleIconPos = new Vector2(0, 244);

                    spriteBatch.Draw(hud2, hudLocation, Color.White);
                    break;

                case 2:
                    timerPos = new Vector2(1209, 12);
                    cookiePos = new Vector2(1095, 12);
                    livesPos = new Vector2(984, 12);
                    monsterIconPos = new Vector2(5, 0);
                    catIconPos = new Vector2(73, 0);
                    duckIconPos = new Vector2(137, 0);
                    moleIconPos = new Vector2(200, 0);

                    spriteBatch.Draw(hud3, hudLocation, Color.White);
                    break;
            }

            Texture2D monsterIcon = null;
            Texture2D catIcon = null;
            Texture2D duckIcon = null;
            Texture2D moleIcon = null;

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

            if (level.Player.animalShape == MONSTER_MOLE)
                moleIcon = monsterMoleActive;
            else if (level.Player.CanBeMole)
                moleIcon = monsterMoleAvailable;
            else
                moleIcon = monsterMoleNotAvailable;
            

            spriteBatch.Draw(monsterIcon, hudLocation + monsterIconPos, Color.White);
            spriteBatch.Draw(catIcon, hudLocation + catIconPos, Color.White);
            spriteBatch.Draw(duckIcon, hudLocation + duckIconPos, Color.White);
            spriteBatch.Draw(moleIcon, hudLocation + moleIconPos, Color.White);


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
                    case 10:
                        status = tutorialOverlay10;
                        break;
                    case 11:
                        status = tutorialOverlay11;
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

        public static void SaveHighScores(HighScoreData data, string filename)
        {
            // Get the path of the save game
            string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            // Open the file, creating it if necessary
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
            try
            {
                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                // Close the file
                stream.Close();
            }
        }

        public static HighScoreData LoadHighScores(string filename)
        {
            HighScoreData data;

            // Get the path of the save game
            string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            // Open the file
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate,
            FileAccess.Read);
            try
            {

                // Read the data from the file
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                data = (HighScoreData)serializer.Deserialize(stream);
            }
            finally
            {
                // Close the file
                stream.Close();
            }

            return (data);
        }

        protected override void Initialize()
        {
            // Get the path of the save game
            string fullpath = Path.Combine(StorageContainer.TitleLocation, HighScoresFilename);

            // Check to see if the save exists
            if (!File.Exists(fullpath))
            {
                //If the file doesn't exist, make a fake one...
                // Create the data to save
                HighScoreData data = new HighScoreData(10);
                data.Score[0] = 500;

                data.Score[1] = 400;

                data.Score[2] = 300;

                data.Score[3] = 200;

                data.Score[4] = 100;

                SaveHighScores(data, HighScoresFilename);
            }

            base.Initialize();
        }

        private void SaveHighScore(int score)
        {
            // Create the data to save
            HighScoreData data = LoadHighScores(HighScoresFilename);

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = data.Count - 1; i > scoreIndex; i--)
                {
                    data.Score[i] = data.Score[i - 1];
                }

                data.Score[scoreIndex] = score;

                SaveHighScores(data, HighScoresFilename);
            }
        }

        private void printHighScore()
        {
            HighScoreData data = LoadHighScores(HighScoresFilename);

            int startingX = 450;
            int startingY = 200;
            int actualX = 0;
            int actualY = 0;

            Color textColor;

            bool alreadyRed = false;

            for (int i = 0; i < 10; i++)
            {
                if (i / 5 == 0)
                    actualX = startingX;
                else
                    actualX = 900;

                actualY = startingY + (i % 5) * 50;

                if (data.Score[i] == totalScore && !alreadyRed)
                {
                    textColor = Color.Red;
                    alreadyRed = true;
                }
                else
                    textColor = new Color(234, 194, 57);

                if(i < 9)
                    DrawShadowedString(highScoreFont, i + 1 + ".  Player: " + data.Score[i], new Vector2(actualX, actualY), textColor);
                else
                    DrawShadowedString(highScoreFont, i + 1 + ". Player: " + data.Score[i], new Vector2(actualX, actualY), textColor);
            }
        }
    }
}
