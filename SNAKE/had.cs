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

     
        private KeyboardState lastState;

      
        private float pohybTimer = 0f;
        private float pohybInterval = 0.12f;

        public bool JeZivy { get; private set; } = true;

        public Had(GraphicsDevice graphicsDevice, Rectangle startRect)
        {
            Rect = startRect;
            segmentSize = startRect.Width;

            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

            telo = new List<Rectangle>();
        }

        public void Pohnout(KeyboardState state, int velikostOknaX, int velikostOknaY, float deltaTime)
        {
    
            if (state.IsKeyDown(Keys.A) && !lastState.IsKeyDown(Keys.A))
            {
               
                if (smerX == 1 && smerY == 0)       
                {
                    smerX = 0; smerY = -1;
                }
                else if (smerX == 0 && smerY == -1) 
                {
                    smerX = -1; smerY = 0;
                }
                else if (smerX == -1 && smerY == 0) 
                {
                    smerX = 0; smerY = 1;
                }
                else if (smerX == 0 && smerY == 1)  
                {
                    smerX = 1; smerY = 0;
                }
            }

            lastState = state;

 
            pohybTimer += deltaTime;
            if (pohybTimer < pohybInterval)
                return;

            pohybTimer = 0f;


            Rectangle predchozi = Rect;

            if (telo.Count > 0)
            {
                for (int i = telo.Count - 1; i > 0; i--)
                    telo[i] = telo[i - 1];

                telo[0] = predchozi;
            }


            int x = Rect.X + smerX * segmentSize;
            int y = Rect.Y + smerY * segmentSize;


            if (x < 0 || x >= velikostOknaX || y < 0 || y >= velikostOknaY)
            {
                JeZivy = false;
                return;
            }

            Rect = new Rectangle(x, y, segmentSize, segmentSize);


            foreach (var s in telo)
            {
                if (Rect.Intersects(s))
                {
                    JeZivy = false;
                    break;
                }
            }
        }

        public void PridatSegment()
        {
            Rectangle novy;

            if (telo.Count > 0)
                novy = telo[telo.Count - 1];
            else
                novy = Rect;

            telo.Add(novy);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            Color teloBarva = new Color(
                (int)(color.R * 0.6f),
                (int)(color.G * 0.6f),
                (int)(color.B * 0.6f)
            );

            foreach (var segment in telo)
                spriteBatch.Draw(texture, segment, teloBarva);

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