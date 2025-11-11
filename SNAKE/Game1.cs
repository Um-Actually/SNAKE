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
        private List<Had> jablka;
        private Random random;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            random = new Random();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            hrac = new Had(GraphicsDevice, new Rectangle(100, 100, 50, 50));
            jablka = new List<Had>();
            for (int i = 0; i < 10; i++)
            {
                int x = random.Next(0, _graphics.PreferredBackBufferWidth - 20);
                int y = random.Next(0, _graphics.PreferredBackBufferHeight - 20);
                jablka.Add(new Had(GraphicsDevice, new Rectangle(x, y, 20, 20)));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();
            hrac.Pohnout(state, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            for (int i = jablka.Count - 1; i >= 0; i--)
            {
                if (hrac.Koliduje(jablka[i]))   
                {
                    jablka.RemoveAt(i);
                    hrac.PridatSegment();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (var jablko in jablka)
            {
                jablko.Draw(_spriteBatch, Color.Green);
            }
            hrac.Draw(_spriteBatch, Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
