using MgEngine.Audio;
using MgEngine.Component;
using MgEngine.Effect;
using MgEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel.Design;
using System.Net.Security;

namespace Villainer.Script
{
    public class Wall : Recall
    {
        public bool IsFalling = false;
        public float FallVelocity = 5;
        public bool StartExplode = true;
        public bool Exploded = false;

        public Wall(Texture2D texture) : base(texture)
        {
        }

        public Wall()
        {

        }

        public override void Update(Inputter inputter, float dt)
        {
            if (IsFalling)
            {
                Y += FallVelocity * dt;
                Particlerr.Add(ParticleShape.Circle, Particles.Fall, 1, Rect.LeftTop, Color.White);
                Particlerr.Add(ParticleShape.Circle, Particles.Fall, 1, Rect.RightTop, Color.White);
            }
        }

        public void Explode()
        {
            Particlerr.Add(ParticleShape.Triangle, Particles.Explosion, 5, Pos, Color.White);
            if (IsRecallActive)
                Particlerr.Add(ParticleShape.Triangle, Particles.Explosion, 20, Pos, RecallColor);
            else
                Particlerr.Add(ParticleShape.Triangle, Particles.Explosion, 20, Pos, Color.SteelBlue);

            Singer.PlaySound("WallDestroy");
        }
    }
}
