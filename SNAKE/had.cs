using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SNAKE
{
    internal class Had
    {
        private Texture2D texture;
        public Rectangle Rect { get; private set; }
        private List<Rectangle> telo;
        private int segmentSize = 20;

        private int smerX = 1;
        private int smerY = 0;
        private float pohybTimer = 0f;
        private float pohybInterval = 0.1f; 

        public bool JeZivy { get; private set; } = true;

        public Had(GraphicsDevice graphicsDevice, Rectangle startRect)
        {
            Rect = startRect;
            segmentSize = startRect.Width;
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            telo = new List<Rectangle>();
        }

        public void Pohnout(KeyboardState state, KeyboardState predchoziState, int velikostOknaX, int velikostOknaY, float deltaTime)
        {
       
            if (state.IsKeyDown(Keys.A) && !predchoziState.IsKeyDown(Keys.A) && smerX == 0)
            {
                smerX = -1;
                smerY = 0;
            }
            if (state.IsKeyDown(Keys.D) && !predchoziState.IsKeyDown(Keys.D) && smerX == 0)
            {
                smerX = 1;
                smerY = 0;
            }
            if (state.IsKeyDown(Keys.W) && !predchoziState.IsKeyDown(Keys.W) && smerY == 0)
            {
                smerY = -1;
                smerX = 0;
            }
            if (state.IsKeyDown(Keys.S) && !predchoziState.IsKeyDown(Keys.S) && smerY == 0)
            {
                smerY = 1;
                smerX = 0;
            }

     
            pohybTimer += deltaTime;
            if (pohybTimer < pohybInterval)
                return;

            pohybTimer = 0f;

            if (telo.Count > 0)
            {

                for (int i = telo.Count - 1; i > 0; i--)
                {
                    telo[i] = telo[i - 1];
                }
       
                telo[0] = Rect;
            }

       
            int x = Rect.X + (smerX * segmentSize);
            int y = Rect.Y + (smerY * segmentSize);

          
            if (x < 0)
                x = velikostOknaX - segmentSize;
            if (x >= velikostOknaX)
                x = 0;
            if (y < 0)
                y = velikostOknaY - segmentSize;
            if (y >= velikostOknaY)
                y = 0;

            Rect = new Rectangle(x, y, Rect.Width, Rect.Height);

        
            foreach (var segment in telo)
            {
                if (Rect.Intersects(segment))
                {
                    JeZivy = false;
                    break;
                }
            }
        }

        public void PridatSegment()
        {
      
            Rectangle novySegment;
            if (telo.Count > 0)
            {
                novySegment = telo[telo.Count - 1];
            }
            else
            {
                novySegment = Rect;
            }
            telo.Add(novySegment);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
 
            Color teloBarva = new Color(
                (int)(color.R * 0.6f),
                (int)(color.G * 0.6f),
                (int)(color.B * 0.6f)
            );

     
            foreach (var segment in telo)
            {
                spriteBatch.Draw(texture, segment, teloBarva);
            }


            spriteBatch.Draw(texture, Rect, color);
        }

        public bool Koliduje(Rectangle jinyRect)
        {
            return Rect.Intersects(jinyRect);
        }

        public int GetDelka()
        {
            return telo.Count + 1; 
        }
    }
}