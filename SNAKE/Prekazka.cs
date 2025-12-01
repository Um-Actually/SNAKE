using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SNAKE
{
    public class Prekazka : IHerniObjekt
    {
        private Texture2D texture;
        public Rectangle Rect { get; private set; }
        public bool JeAktivni { get; private set; }
        private Color barva;

        public Prekazka(GraphicsDevice graphicsDevice, Rectangle pozice, Color barva)
        {
            this.Rect = pozice;
            this.barva = barva;
            this.JeAktivni = true;

            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
        }

        public void Aktualizovat(GameTime gameTime)
        {
            // Překážka se nepohybuje
        }

        public void Vykreslit(SpriteBatch spriteBatch)
        {
            if (JeAktivni)
            {
                spriteBatch.Draw(texture, Rect, barva);
            }
        }

        public void Deaktivovat()
        {
            JeAktivni = false;
        }
    }
}