using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SNAKE
{
    internal enum TypPowerUpu
    {
        Rychlost,
        Nezranitelnost,
        Magnet,
        ZmenseniBodu
    }

    internal class PowerUp : ISbiratelny
    {
        private Texture2D texture;
        public Rectangle Pozice { get; private set; }
        public bool JeAktivni { get; private set; }
        public TypPowerUpu Typ { get; private set; }

        private float zivotnost;

        public PowerUp(GraphicsDevice gd, Rectangle pozice, TypPowerUpu typ)
        {
            texture = new Texture2D(gd, 1, 1);
            texture.SetData(new[] { Color.White });
            Pozice = pozice;
            Typ = typ;
            JeAktivni = true;
            zivotnost = 8f; // čas než powerup zmizí
        }

        public void Sebrat()
        {
            JeAktivni = false;
        }

        public void Aktualizovat(float deltaTime)
        {
            if (!JeAktivni) return;
            zivotnost -= deltaTime;
            if (zivotnost <= 0f)
                JeAktivni = false;
        }

        public void Kreslit(SpriteBatch sb)
        {
            if (!JeAktivni) return;

            Color barva = Color.White;
            switch (Typ)
            {
                case TypPowerUpu.Rychlost: barva = Color.Orange; break;
                case TypPowerUpu.Nezranitelnost: barva = Color.Cyan; break;
                case TypPowerUpu.Magnet: barva = Color.Magenta; break;
                case TypPowerUpu.ZmenseniBodu: barva = Color.Purple; break;
            }

            sb.Draw(texture, Pozice, barva * 0.9f);
            Rectangle vnitrni = new Rectangle(Pozice.X + 3, Pozice.Y + 3, Math.Max(1, Pozice.Width - 6), Math.Max(1, Pozice.Height - 6));
            sb.Draw(texture, vnitrni, Color.Black * 0.2f);
        }
    }
}