using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SNAKE
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Had hrac;
        private List<Rectangle> jablka;
        private List<Prekazka> prekazky;
        private Random random;
        private Texture2D pixelTexture;
        private SpriteFont font;
        private KeyboardState predchoziKeyboard;

        private int skore = 0;
        private bool gameOver = false;
        private int segmentSize = 20;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            random = new Random();

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            try
            {
                font = Content.Load<SpriteFont>("Font");
            }
            catch
            {
                font = null;
            }

            int startX = (_graphics.PreferredBackBufferWidth / 2 / segmentSize) * segmentSize;
            int startY = (_graphics.PreferredBackBufferHeight / 2 / segmentSize) * segmentSize;
            hrac = new Had(GraphicsDevice, new Rectangle(startX, startY, segmentSize, segmentSize));

            jablka = new List<Rectangle>();
            for (int i = 0; i < 5; i++)
            {
                PridatJablko();
            }


            prekazky = new List<Prekazka>();
            PridatPrekazky();
        }

        private void PridatPrekazky()
        {
  
            for (int i = 0; i < 50; i++)
            {
                int maxX = _graphics.PreferredBackBufferWidth / segmentSize;
                int maxY = _graphics.PreferredBackBufferHeight / segmentSize;

                int x = random.Next(0, maxX) * segmentSize;
                int y = random.Next(0, maxY) * segmentSize;

                Rectangle pozice = new Rectangle(x, y, segmentSize, segmentSize);
                prekazky.Add(new Prekazka(GraphicsDevice, pozice, Color.Gray));
            }
        }

        private void PridatJablko()
        {
            int maxX = _graphics.PreferredBackBufferWidth / segmentSize;
            int maxY = _graphics.PreferredBackBufferHeight / segmentSize;

            int x = random.Next(0, maxX) * segmentSize;
            int y = random.Next(0, maxY) * segmentSize;

            jablka.Add(new Rectangle(x, y, segmentSize, segmentSize));
        }

        private void RestartHry()
        {
            int startX = (_graphics.PreferredBackBufferWidth / 2 / segmentSize) * segmentSize;
            int startY = (_graphics.PreferredBackBufferHeight / 2 / segmentSize) * segmentSize;
            hrac = new Had(GraphicsDevice, new Rectangle(startX, startY, segmentSize, segmentSize));

            jablka.Clear();
            for (int i = 0; i < 5; i++)
            {
                PridatJablko();
            }

            prekazky.Clear();
            PridatPrekazky();

            skore = 0;
            gameOver = false;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            if (gameOver)
            {
                if (state.IsKeyDown(Keys.Space) && !predchoziKeyboard.IsKeyDown(Keys.Space))
                {
                    RestartHry();
                }
                predchoziKeyboard = state;
                return;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            hrac.Pohnout(state, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, deltaTime);

            if (!hrac.JeZivy)
            {
                gameOver = true;
            }

            for (int i = jablka.Count - 1; i >= 0; i--)
            {
                if (hrac.Koliduje(jablka[i]))
                {
                    jablka.RemoveAt(i);
                    PridatJablko();
                    hrac.PridatSegment();
                    skore += 10;
                }
            }

            foreach (var prekazka in prekazky)
            {
                if (prekazka.JeAktivni && hrac.Koliduje(prekazka.Rect))
                {
                    gameOver = true;
                    break;
                }
            }

            predchoziKeyboard = state;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

     
            foreach (var prekazka in prekazky)
            {
                prekazka.Vykreslit(_spriteBatch);
            }


            foreach (var jablko in jablka)
            {
                _spriteBatch.Draw(pixelTexture, jablko, Color.Red);
            }

            hrac.Draw(_spriteBatch, Color.LimeGreen);

            if (font != null)
            {
                _spriteBatch.DrawString(font, $"Skore: {skore}  Delka: {hrac.GetDelka()}", new Vector2(10, 10), Color.White);

                if (gameOver)
                {
                    string gameOverText = "GAME OVER!";
                    string restartText = "Stiskni SPACE pro restart";
                    Vector2 gameOverSize = font.MeasureString(gameOverText);
                    Vector2 restartSize = font.MeasureString(restartText);

                    _spriteBatch.DrawString(font, gameOverText,
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - gameOverSize.X / 2,
                                    _graphics.PreferredBackBufferHeight / 2 - 30), Color.Red);
                    _spriteBatch.DrawString(font, restartText,
                        new Vector2(_graphics.PreferredBackBufferWidth / 2 - restartSize.X / 2,
                                    _graphics.PreferredBackBufferHeight / 2 + 10), Color.White);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}