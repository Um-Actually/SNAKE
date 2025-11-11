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
        private List<Vector2> predchoziPozice;

        int v = 5;
        public Had(GraphicsDevice graphicsDevice, Rectangle startRect)
        {
            Rect = startRect;
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            telo = new List<Rectangle>();
            predchoziPozice = new List<Vector2>();
        }
        public void Pohnout(KeyboardState state,int velikostOknaX,int VelikostOknaY)
        {
            predchoziPozice.Insert(0, new Vector2(Rect.X, Rect.Y));

            int x = Rect.X;
            int y = Rect.Y;

            if (state.IsKeyDown(Keys.A))
                x -= v;
            if (state.IsKeyDown(Keys.D))
                x += v;
            if (state.IsKeyDown(Keys.W))
                y -= v;
            if (state.IsKeyDown(Keys.S))
                y += v;
            if (x < 0)
                x = velikostOknaX;
            if (x > velikostOknaX)
                x = 0;
            if (y < 0)
                y = VelikostOknaY;
            if (y > VelikostOknaY)
                y = 0;


            Rect = new Rectangle(x, y, Rect.Width, Rect.Height);

            for (int i = 0; i < telo.Count; i++)
            {
                if (i < predchoziPozice.Count)
                {
                    telo[i] = new Rectangle(
                        (int)predchoziPozice[i].X,
                        (int)predchoziPozice[i].Y,
                        telo[i].Width,
                        telo[i].Height
                    );
                }
            }


            if (predchoziPozice.Count > telo.Count + 50)
            {
                predchoziPozice.RemoveAt(predchoziPozice.Count - 1);
            }
        }

        public void PridatSegment()
        {

            Rectangle novySegment = new Rectangle(Rect.X, Rect.Y, Rect.Width, Rect.Height);
            telo.Add(novySegment);
        }
        public void zvetsit(int oKolik)
        {
            Rect = new Rectangle(Rect.X, Rect.Y, Rect.Width + oKolik, Rect.Height + oKolik);
        }
        public void zpomalit()
        {
            v = (int)(v * 0.9);
            if (v < 1) v = 1;
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
        public bool Koliduje(Had jiny)
        {
            return Rect.Intersects(jiny.Rect);
        }
    }
}