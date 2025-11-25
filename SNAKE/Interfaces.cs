using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SNAKE
{
   
    public interface ISbiratelny
    {
        Rectangle Pozice { get; }
        bool JeAktivni { get; }
        void Sebrat();
        void Kreslit(SpriteBatch spriteBatch);
    }

    public interface IAI
    {
        void RozhodnoutSmer(Rectangle cil, int velikostOknaX, int velikostOknaY);
    }

    public interface IEfekt
    {
        bool JeAktivni { get; }
        void Aktualizovat(float deltaTime);
        void Kreslit(SpriteBatch spriteBatch);
    }
}