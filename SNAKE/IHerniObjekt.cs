using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SNAKE
{
    public interface IHerniObjekt
    {
        Rectangle Rect { get; }
        bool JeAktivni { get; }
        void Aktualizovat(GameTime gameTime);
        void Vykreslit(SpriteBatch spriteBatch);
    }
}