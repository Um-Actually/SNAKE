using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SNAKE
{
    internal class PartiklovaExploze : IEfekt
    {
        private class Particle
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Life;
            public float MaxLife;
        }

        private Texture2D texture;
        private List<Particle> particles;
        private Random random;
        private Color color;

        public bool JeAktivni { get; private set; }

        public PartiklovaExploze(GraphicsDevice gd, Vector2 pozice, Color barva, int pocet)
        {
            texture = new Texture2D(gd, 1, 1);
            texture.SetData(new[] { Color.White });

            particles = new List<Particle>(pocet);
            random = new Random(Guid.NewGuid().GetHashCode());
            color = barva;
            JeAktivni = true;

            for (int i = 0; i < pocet; i++)
            {
                float angle = (float)(random.NextDouble() * Math.PI * 2.0);
                float speed = (float)(random.NextDouble() * 100 + 30);
                var p = new Particle
                {
                    Position = pozice + new Vector2(0.5f * random.Next(-4, 5), 0.5f * random.Next(-4, 5)),
                    Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed,
                    Life = (float)(random.NextDouble() * 0.6 + 0.4f),
                    MaxLife = (float)(random.NextDouble() * 0.6 + 0.4f)
                };
                particles.Add(p);
            }
        }

        public void Aktualizovat(float deltaTime)
        {
            if (!JeAktivni) return;

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var p = particles[i];
                p.Life -= deltaTime;
                if (p.Life <= 0f)
                {
                    particles.RemoveAt(i);
                    continue;
                }
                // jednoduchá gravitace a odpor
                p.Velocity += new Vector2(0, 200f) * deltaTime;
                p.Position += p.Velocity * deltaTime;
            }

            if (particles.Count == 0)
                JeAktivni = false;
        }

        public void Kreslit(SpriteBatch sb)
        {
            if (!JeAktivni) return;

            foreach (var p in particles)
            {
                float t = MathHelper.Clamp(p.Life / p.MaxLife, 0f, 1f);
                Color c = color * t;
                Rectangle rect = new Rectangle((int)p.Position.X, (int)p.Position.Y, 3, 3);
                sb.Draw(texture, rect, c);
            }
        }
    }
}