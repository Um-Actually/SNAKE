using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SNAKE
{
    internal class Had : IAI
    {
        private Texture2D texture;
        public Rectangle Rect { get; private set; }
        private List<Rectangle> telo;
        private int segmentSize = 20;

        private int smerX = 1;
        private int smerY = 0;
        private float pohybTimer = 0f;
        private float zakladniInterval = 0.12f;
        private float pohybInterval;

        public bool JeZivy { get; private set; } = true;
        public bool JeHrac { get; private set; }
        public Color Barva { get; private set; }
        public int Skore { get; private set; }

        // Power-up efekty
        private float nezranitelnostTimer = 0f;
        private float rychlostTimer = 0f;
        private float magnetTimer = 0f;
        public bool MaNezranitelnost => nezranitelnostTimer > 0;
        public bool MaMagnet => magnetTimer > 0;

        // AI
        private Random random;
        private float aiRozhodovaniTimer = 0f;

        public Had(GraphicsDevice graphicsDevice, Rectangle startRect, bool jeHrac, Color barva)
        {
            Rect = startRect;
            segmentSize = startRect.Width;
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            telo = new List<Rectangle>();
            JeHrac = jeHrac;
            Barva = barva;
            pohybInterval = zakladniInterval;
            random = new Random(Guid.NewGuid().GetHashCode());

            // AI začíná s náhodným směrem
            if (!jeHrac)
            {
                int nahodnySmer = random.Next(4);
                switch (nahodnySmer)
                {
                    case 0: smerX = 1; smerY = 0; break;
                    case 1: smerX = -1; smerY = 0; break;
                    case 2: smerX = 0; smerY = 1; break;
                    case 3: smerX = 0; smerY = -1; break;
                }
            }
        }

        public void AktivovatPowerUp(TypPowerUpu typ)
        {
            switch (typ)
            {
                case TypPowerUpu.Rychlost:
                    rychlostTimer = 5f;
                    break;
                case TypPowerUpu.Nezranitelnost:
                    nezranitelnostTimer = 5f;
                    break;
                case TypPowerUpu.Magnet:
                    magnetTimer = 5f;
                    break;
                case TypPowerUpu.ZmenseniBodu:
                    if (segmentSize > 10)
                    {
                        segmentSize -= 2;
                       
                        Rect = new Rectangle(Rect.X, Rect.Y, segmentSize, segmentSize);
                        for (int i = 0; i < telo.Count; i++)
                        {
                            var seg = telo[i];
                            telo[i] = new Rectangle(seg.X, seg.Y, segmentSize, segmentSize);
                        }
                    }
                    break;
            }
        }

        public void Pohnout(KeyboardState state, KeyboardState predchoziState, int velikostOknaX, int velikostOknaY, float deltaTime)
        {

            if (nezranitelnostTimer > 0) nezranitelnostTimer -= deltaTime;
            if (rychlostTimer > 0) rychlostTimer -= deltaTime;
            if (magnetTimer > 0) magnetTimer -= deltaTime;

       
            pohybInterval = rychlostTimer > 0 ? zakladniInterval * 0.5f : zakladniInterval;

            if (JeHrac)
            {
           
                if (state.IsKeyDown(Keys.A) && !predchoziState.IsKeyDown(Keys.A) && smerX == 0)
                {
                    smerX = -1; smerY = 0;
                }
                if (state.IsKeyDown(Keys.D) && !predchoziState.IsKeyDown(Keys.D) && smerX == 0)
                {
                    smerX = 1; smerY = 0;
                }
                if (state.IsKeyDown(Keys.W) && !predchoziState.IsKeyDown(Keys.W) && smerY == 0)
                {
                    smerY = -1; smerX = 0;
                }
                if (state.IsKeyDown(Keys.S) && !predchoziState.IsKeyDown(Keys.S) && smerY == 0)
                {
                    smerY = 1; smerX = 0;
                }
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

    
            if (x < 0) x = velikostOknaX - segmentSize;
            if (x >= velikostOknaX) x = 0;
            if (y < 0) y = velikostOknaY - segmentSize;
            if (y >= velikostOknaY) y = 0;

            Rect = new Rectangle(x, y, segmentSize, segmentSize);

            if (!MaNezranitelnost)
            {
                foreach (var segment in telo)
                {
                    if (Rect.Intersects(segment))
                    {
                        JeZivy = false;
                        break;
                    }
                }
            }
        }

        public void RozhodnoutSmer(Rectangle cil, int velikostOknaX, int velikostOknaY)
        {
            aiRozhodovaniTimer -= 0.016f;

            if (aiRozhodovaniTimer <= 0)
            {
                aiRozhodovaniTimer = 0.3f;

                int deltaX = cil.X - Rect.X;
                int deltaY = cil.Y - Rect.Y;

                if (random.Next(100) < 70) 
                {
                    if (Math.Abs(deltaX) > Math.Abs(deltaY))
                    {
                        if (deltaX > 0 && smerX == 0) { smerX = 1; smerY = 0; }
                        else if (deltaX < 0 && smerX == 0) { smerX = -1; smerY = 0; }
                        else if (smerY == 0)
                        {
                            smerY = random.Next(2) == 0 ? 1 : -1;
                            smerX = 0;
                        }
                    }
                    else
                    {
                        if (deltaY > 0 && smerY == 0) { smerY = 1; smerX = 0; }
                        else if (deltaY < 0 && smerY == 0) { smerY = -1; smerX = 0; }
                        else if (smerX == 0)
                        {
                            smerX = random.Next(2) == 0 ? 1 : -1;
                            smerY = 0;
                        }
                    }
                }
                else
                {
                  
                    if (smerX != 0)
                    {
                        smerY = random.Next(2) == 0 ? 1 : -1;
                        smerX = 0;
                    }
                    else
                    {
                        smerX = random.Next(2) == 0 ? 1 : -1;
                        smerY = 0;
                    }
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
            Skore += 10;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color teloBarva = Barva * 0.7f;

            if (MaNezranitelnost && (int)(nezranitelnostTimer * 10) % 2 == 0)
            {
                teloBarva = Color.Cyan;
            }

          
            foreach (var segment in telo)
            {
                spriteBatch.Draw(texture, segment, teloBarva);
                
                Rectangle okraj = new Rectangle(segment.X + 1, segment.Y + 1, segment.Width - 2, segment.Height - 2);
                spriteBatch.Draw(texture, okraj, teloBarva * 0.5f);
            }

          
            spriteBatch.Draw(texture, Rect, Barva);
            Rectangle okrajHlavy = new Rectangle(Rect.X + 2, Rect.Y + 2, Rect.Width - 4, Rect.Height - 4);
            spriteBatch.Draw(texture, okrajHlavy, Color.White * 0.8f);
        }

        public bool KolidujeS(Had jiny)
        {
            if (MaNezranitelnost) return false;

            if (Rect.Intersects(jiny.Rect))
                return true;

        
            foreach (var segment in jiny.telo)
            {
                if (Rect.Intersects(segment))
                    return true;
            }

            return false;
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