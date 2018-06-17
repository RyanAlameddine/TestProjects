using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeTraversal
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        MouseState ms;
        KeyboardState ks;
        Board board = new Board(21, 21, 50);
        int screenWidth = 1050;
        int screenHeight = 1050;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        bool pressed = false;
        byte selectedPointType = 1;
        byte pressedPointType = 0;
        byte drawing = 0;
        Texture2D[] textures = new Texture2D[4];
        bool generating = false;

        public static float timeTillClear = -1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textures[0] = Content.Load<Texture2D>("Zero");
            textures[1] = Content.Load<Texture2D>("One");
            textures[2] = Content.Load<Texture2D>("Two");
            textures[3] = Content.Load<Texture2D>("Three");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            Keys[] keys = ks.GetPressedKeys();
            bool tab = false;
            foreach(Keys key in keys)
            {
                if (key > Keys.D0 && key < Keys.D9)
                {
                    selectedPointType = (byte) (key - Keys.D0);
                }
                if(key == Keys.Enter)
                {
                    board.overLaySpots.Clear();
                    board.AStar();
                    timeTillClear = 10;
                }
                if(key == Keys.Tab)
                {
                    tab = true;
                    if (!generating)
                    {
                        board.Prim();
                        timeTillClear = 10;
                        generating = true;
                    }
                }
            }
            if(!tab) { generating = false; }

            if(ms.LeftButton == ButtonState.Pressed)
            {
                int x = ms.X / board.size;
                int y = ms.Y / board.size;

                if (x < 0 || y < 0 || ms.X > screenWidth || ms.Y > screenWidth)
                {
                    base.Update(gameTime);
                    return;
                }

                byte typeOfPressed = board.spots[x, y];
                if (pressed)
                {
                    if (typeOfPressed == pressedPointType)
                    {
                        board.spots[x, y] = drawing;
                        if (drawing == 2)
                        {
                            if (board.spots[(int)board.startPoint.X, (int)board.startPoint.Y] == 2) { board.spots[(int)board.startPoint.X, (int)board.startPoint.Y] = 0; }
                            board.startPoint = new Vector2(x, y);
                        }
                        else if (drawing == 3)
                        {
                            if (board.spots[(int)board.endPoint.X, (int)board.endPoint.Y] == 3) { board.spots[(int)board.endPoint.X, (int)board.endPoint.Y] = 0; }
                            board.endPoint = new Vector2(x, y);
                        }
                    }
                }
                else
                {
                    pressed = true;
                    
                    if(typeOfPressed != 0)
                    {
                        if (typeOfPressed != 2 && typeOfPressed != 3)
                        {
                            pressedPointType = typeOfPressed;
                            drawing = 0;
                            board.spots[x, y] = 0;
                        }
                    }
                    else
                    {
                        pressedPointType = 0;
                        drawing = selectedPointType;
                        board.spots[x, y] = drawing;
                        if (drawing == 2)
                        {
                            if (board.spots[(int)board.startPoint.X, (int)board.startPoint.Y] == 2) { board.spots[(int)board.startPoint.X, (int)board.startPoint.Y] = 0; }
                            board.startPoint = new Vector2(x, y);
                        }
                        else if (drawing == 3)
                        {
                            if (board.spots[(int)board.endPoint.X, (int)board.endPoint.Y] == 3) { board.spots[(int)board.endPoint.X, (int)board.endPoint.Y] = 0; }
                            board.endPoint = new Vector2(x, y);
                        }
                    }
                }
            }
            else
            {
                pressed = false;
            }

            if(timeTillClear != -1)
            {
                timeTillClear -= gameTime.ElapsedGameTime.Milliseconds / 100f;
            }
            if(timeTillClear <= 0)
            {
                board.overLaySpots.Clear();
                timeTillClear = -1;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            for(int x = 0; x < board.width; x++)
            {
                for(int y = 0; y < board.height; y++)
                {
                    spriteBatch.Draw(textures[board.spots[x, y]], new Vector2(x, y) * board.size, Color.White);
                }
            }
            foreach(Vector2 position in board.overLaySpots)
            {
                spriteBatch.Draw(textures[0], position * board.size, Color.Blue);
            }
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
