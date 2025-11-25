using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SNAKE
{
    public enum StavHry
    {
        Menu,
        Hra,
        GameOver,
        Vitezstvi
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Had hrac;
        private List<Had> aiHadi;
        private List<ISbiratelny> jidlo;
        private List<PowerUp> powerUpy;
        private List<IEfekt> efekty;
        private Random random;
        private Texture2D pixelTexture;
        private KeyboardState predchoziKeyboard;

        private StavHry stavHry = StavHry.Menu;
        private int segmentSize = 20;
        private int level = 1;
        private float casDoNovehoPowerUpu = 5f;
        private float casDoDalsihoLevelu = 30f;

        private Color[] barvyAI = new Color[]
        {
            Color.Orange,
            Color.Purple,
            Color.Yellow,
            Color.Pink,
            Color.Brown
        };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            random = new Random();

            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 700;
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

            aiHadi = new List<Had>();
            jidlo = new List<ISbiratelny>();
            powerUpy = new List<PowerUp>();
            efekty = new List<IEfekt>();
        }

        private void ZacitHru()
        {
            // Vytvořit hráče
            int startX = (_graphics.PreferredBackBufferWidth / 2 / segmentSize) * segmentSize;
            int startY = (_graphics.PreferredBackBufferHeight / 2 / segmentSize) * segmentSize;
            hrac = new Had(GraphicsDevice, new Rectangle(startX, startY, segmentSize, segmentSize), true, Color.LimeGreen);

            // Vytvořit AI hady
            aiHadi.Clear();
            int pocetAI = Math.Min(2 + level, 5);
            for (int i = 0; i < pocetAI; i++)
            {
                VytvoritAIHada(i);
            }

            // Vytvořit jídlo
            jidlo.Clear();
            for (int i = 0; i < 15; i++)
            {
                PridatJidlo();
            }

            powerUpy.Clear();
            efekty.Clear();
            level = 1;
            casDoNovehoPowerUpu = 5f;
            casDoDalsihoLevelu = 30f;
            stavHry = StavHry.Hra;
        }

        private void VytvoritAIHada(int index)
        {
            int maxX = _graphics.PreferredBackBufferWidth / segmentSize;
            int maxY = _graphics.PreferredBackBufferHeight / segmentSize;

            int x = random.Next(5, maxX - 5) * segmentSize;
            int y = random.Next(5, maxY - 5) * segmentSize;

            Color barva = barvyAI[index % barvyAI.Length];
            aiHadi.Add(new Had(GraphicsDevice, new Rectangle(x, y, segmentSize, segmentSize), false, barva));
        }

        private void PridatJidlo()
        {
            int maxX = _graphics.PreferredBackBufferWidth / segmentSize;
            int maxY = _graphics.PreferredBackBufferHeight / segmentSize;

            int x = random.Next(0, maxX) * segmentSize;
            int y = random.Next(0, maxY) * segmentSize;

            // Jednoduchá implementace jídla jako Rectangle wrapped v pomocné třídě
            jidlo.Add(new JidloWrapper(GraphicsDevice, new Rectangle(x, y, segmentSize, segmentSize)));
        }

        private void PridatPowerUp()
        {
            int maxX = _graphics.PreferredBackBufferWidth / segmentSize;
            int maxY = _graphics.PreferredBackBufferHeight / segmentSize;

            int x = random.Next(0, maxX) * segmentSize;
            int y = random.Next(0, maxY) * segmentSize;

            TypPowerUpu typ = (TypPowerUpu)random.Next(4);
            powerUpy.Add(new PowerUp(GraphicsDevice, new Rectangle(x, y, segmentSize, segmentSize), typ));
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            switch (stavHry)
            {
                case StavHry.Menu:
                    if (state.IsKeyDown(Keys.Space) && !predchoziKeyboard.IsKeyDown(Keys.Space))
                    {
                        ZacitHru();
                    }
                    break;

                case StavHry.Hra:
                    AktualizovatHru(state, deltaTime);
                    break;

                case StavHry.GameOver:
                case StavHry.Vitezstvi:
                    if (state.IsKeyDown(Keys.Space) && !predchoziKeyboard.IsKeyDown(Keys.Space))
                    {
                        ZacitHru();
                    }
                    if (state.IsKeyDown(Keys.M) && !predchoziKeyboard.IsKeyDown(Keys.M))
                    {
                        stavHry = StavHry.Menu;
                    }
                    break;
            }

            predchoziKeyboard = state;
            base.Update(gameTime);
        }

        private void AktualizovatHru(KeyboardState state, float deltaTime)
        {
            // Pohyb hráče
            hrac.Pohnout(state, predchoziKeyboard, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, deltaTime);

            // AI hadi
            foreach (var ai in aiHadi.Where(h => h.JeZivy))
            {
                // Najít nejbližší jídlo
                Rectangle nejblizsiCil = jidlo.Count > 0 ? jidlo[0].Pozice : hrac.Rect;
                float nejblizsiVzdalenost = float.MaxValue;

                foreach (var j in jidlo)
                {
                    float vzdalenost = Vector2.Distance(
                        new Vector2(ai.Rect.X, ai.Rect.Y),
                        new Vector2(j.Pozice.X, j.Pozice.Y)
                    );
                    if (vzdalenost < nejblizsiVzdalenost)
                    {
                        nejblizsiVzdalenost = vzdalenost;
                        nejblizsiCil = j.Pozice;
                    }
                }

                ai.RozhodnoutSmer(nejblizsiCil, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                ai.Pohnout(state, predchoziKeyboard, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, deltaTime);
            }

            // Kontrola smrti hráče
            if (!hrac.JeZivy)
            {
                efekty.Add(new PartiklovaExploze(GraphicsDevice, new Vector2(hrac.Rect.X, hrac.Rect.Y), hrac.Barva, 30));
                stavHry = StavHry.GameOver;
                return;
            }

            // Kolize mezi hady
            foreach (var ai in aiHadi.Where(h => h.JeZivy))
            {
                if (hrac.KolidujeS(ai))
                {
                    efekty.Add(new PartiklovaExploze(GraphicsDevice, new Vector2(hrac.Rect.X, hrac.Rect.Y), hrac.Barva, 30));
                    stavHry = StavHry.GameOver;
                    return;
                }
            }

            // AI kolize mezi sebou
            for (int i = 0; i < aiHadi.Count; i++)
            {
                if (!aiHadi[i].JeZivy) continue;

                for (int j = i + 1; j < aiHadi.Count; j++)
                {
                    if (!aiHadi[j].JeZivy) continue;

                    if (aiHadi[i].KolidujeS(aiHadi[j]))
                    {
                        efekty.Add(new PartiklovaExploze(GraphicsDevice, new Vector2(aiHadi[i].Rect.X, aiHadi[i].Rect.Y), aiHadi[i].Barva, 20));
                        aiHadi[i].JeZivy = false;
                    }
                }
            }

            // Sbírání jídla
            for (int i = jidlo.Count - 1; i >= 0; i--)
            {
                bool sebrano = false;

                if (hrac.Koliduje(jidlo[i].Pozice))
                {
                    jidlo[i].Sebrat();
                    hrac.PridatSegment();
                    sebrano = true;
                }

                if (!sebrano)
                {
                    foreach (var ai in aiHadi.Where(h => h.JeZivy))
                    {
                        if (ai.Koliduje(jidlo[i].Pozice))
                        {
                            jidlo[i].Sebrat();
                            ai.PridatSegment();
                            sebrano = true;
                            break;
                        }
                    }
                }

                if (sebrano)
                {
                    jidlo.RemoveAt(i);
                    PridatJidlo();
                }
            }

            // Magnet efekt
            if (hrac.MaMagnet)
            {
                for (int i = jidlo.Count - 1; i >= 0; i--)
                {
                    float vzdalenost = Vector2.Distance(
                        new Vector2(hrac.Rect.X, hrac.Rect.Y),
                        new Vector2(jidlo[i].Pozice.X, jidlo[i].Pozice.Y)
                    );

                    if (vzdalenost < 150)
                    {
                        jidlo[i].Sebrat();
                        hrac.PridatSegment();
                        jidlo.RemoveAt(i);
                        PridatJidlo();
                    }
                }
            }

            // Power-upy
            casDoNovehoPowerUpu -= deltaTime;
            if (casDoNovehoPowerUpu <= 0)
            {
                PridatPowerUp();
                casDoNovehoPowerUpu = 8f;
            }

            for (int i = powerUpy.Count - 1; i >= 0; i--)
            {
                powerUpy[i].Aktualizovat(deltaTime);

                if (!powerUpy[i].JeAktivni)
                {
                    powerUpy.RemoveAt(i);
                    continue;
                }

                if (hrac.Koliduje(powerUpy[i].Pozice))
                {
                    hrac.AktivovatPowerUp(powerUpy[i].Typ);
                    powerUpy[i].Sebrat();
                    powerUpy.RemoveAt(i);
                }
            }

            // Efekty
            for (int i = efekty.Count - 1; i >= 0; i--)
            {
                efekty[i].Aktualizovat(deltaTime);
                if (!efekty[i].JeAktivni)
                {
                    efekty.RemoveAt(i);
                }
            }

            // Level systém
            casDoDalsihoLevelu -= deltaTime;
            if (casDoDalsihoLevelu <= 0)
            {
                level++;
                casDoDalsihoLevelu = 30f;

                // Přidat nového AI hada
                if (aiHadi.Count < 5)
                {
                    VytvoritAIHada(aiHadi.Count);
                }
            }

            // Kontrola vítězství
            if (!aiHadi.Any(h => h.JeZivy) && level >= 3)
            {
                stavHry = StavHry.Vitezstvi;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 40));

            _spriteBatch.Begin();

            if (stavHry == StavHry.Menu)
            {
                KresliMenu();
            }
            else
            {
                // Nakreslit jídlo
                foreach (var j in jidlo)
                {
                    j.Kreslit(_spriteBatch);
                }

                // Nakreslit power-upy
                foreach (var p in powerUpy)
                {
                    p.Kreslit(_spriteBatch);
                }

                // Nakreslit hady
                foreach (var ai in aiHadi.Where(h => h.JeZivy))
                {
                    ai.Draw(_spriteBatch);
                }

                if (hrac != null)
                {
                    hrac.Draw(_spriteBatch);
                }

                // Nakreslit efekty
                foreach (var efekt in efekty)
                {
                    efekt.Kreslit(_spriteBatch);
                }

                // HUD
                if (stavHry == StavHry.Hra)
                {
                    KresliHUD();
                }
                else if (stavHry == StavHry.GameOver)
                {
                    KresliGameOver();
                }
                else if (stavHry == StavHry.Vitezstvi)
                {
                    KresliVitezstvi();
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void KresliMenu()
        {
            string nadpis = "SNAKE ARENA";
            string popis1 = "Battle Royale - Poraz vsechny AI hady!";
            string popis2 = "WASD - pohyb";
            string popis3 = "Sbierej power-upy a prežij!";
            string start = "SPACE - Start";

            KresliText(nadpis, new Vector2(500, 200), 2f, Color.Yellow);
            KresliText(popis1, new Vector2(500, 280), 1f, Color.White);
            KresliText(popis2, new Vector2(500, 320), 1f, Color.LightGreen);
            KresliText(popis3, new Vector2(500, 360), 1f, Color.Cyan);
            KresliText(start, new Vector2(500, 450), 1.5f, Color.Orange);
        }

        private void KresliHUD()
        {
            string skore = $"Skore: {hrac.Skore}";
            string delka = $"Delka: {hrac.GetDelka()}";
            string level_text = $"Level: {level}";
            string nepratele = $"Nepratel: {aiHadi.Count(h => h.JeZivy)}";

            KresliText(skore, new Vector2(10, 10), 1f, Color.White);
            KresliText(delka, new Vector2(10, 35), 1f, Color.White);
            KresliText(level_text, new Vector2(10, 60), 1f, Color.Yellow);
            KresliText(nepratele, new Vector2(10, 85), 1f, Color.Orange);

            if (hrac.MaNezranitelnost)
            {
                KresliText("NEZRANITELNOST!", new Vector2(500, 10), 1f, Color.Cyan);
            }
            if (hrac.MaMagnet)
            {
                KresliText("MAGNET!", new Vector2(500, 35), 1f, Color.Magenta);
            }
        }

        private void KresliGameOver()
        {
            string text1 = "GAME OVER!";
            string text2 = $"Finalni skore: {hrac.Skore}";
            string text3 = "SPACE - Restart | M - Menu";

            KresliText(text1, new Vector2(500, 300), 2f, Color.Red);
            KresliText(text2, new Vector2(500, 360), 1.2f, Color.White);
            KresliText(text3, new Vector2(500, 410), 1f, Color.Gray);
        }

        private void KresliVitezstvi()
        {
            string text1 = "VITEZSTVI!";
            string text2 = $"Porazil jsi vsechny! Skore: {hrac.Skore}";
            string text3 = "SPACE - Restart | M - Menu";

            KresliText(text1, new Vector2(500, 300), 2f, Color.Gold);
            KresliText(text2, new Vector2(500, 360), 1.2f, Color.White);
            KresliText(text3, new Vector2(500, 410), 1f, Color.Gray);
        }

        private void KresliText(string text, Vector2 pozice, float scale, Color barva)
        {
            // Jednoduchý pixel-art text
            int charWidth = (int)(8 * scale);
            int charHeight = (int)(12 * scale);
            int x = (int)(pozice.X - (text.Length * charWidth) / 2);
            int y = (int)pozice.Y;

            for (int i = 0; i < text.Length; i++)
            {
                Rectangle rect = new Rectangle(x + i * charWidth, y, charWidth - 2, charHeight);
                _spriteBatch.Draw(pixelTexture, rect, barva * 0.8f);
            }
        }
    }

    // Pomocná třída pro jídlo
    public class JidloWrapper : ISbiratelny
    {
        private Texture2D texture;
        public Rectangle Pozice { get; private set; }
        public bool JeAktivni { get; private set; }

        public JidloWrapper(GraphicsDevice gd, Rectangle pozice)
        {
            texture = new Texture2D(gd, 1, 1);
            texture.SetData(new[] { Color.White });
            Pozice = pozice;
            JeAktivni = true;
        }

        public void Sebrat()
        {
            JeAktivni = false;
        }

        public void Kreslit(SpriteBatch sb)
        {
            if (!JeAktivni) return;
            sb.Draw(texture, Pozice, Color.Red);
            Rectangle vnitrni = new Rectangle(Pozice.X + 4, Pozice.Y + 4, Pozice.Width - 8, Pozice.Height - 8);
            sb.Draw(texture, vnitrni, Color.DarkRed);
        }
    }
}