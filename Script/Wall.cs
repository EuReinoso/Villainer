using MgEngine.Component;
using MgEngine.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Villainer.Script
{
    public class Wall : Entity
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
                Y += FallVelocity * dt;
        }

        public void Explode()
        {

        }
    }
}
