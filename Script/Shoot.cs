using MgEngine.Component;
using MgEngine.Effect;
using MgEngine.Time;
using MgEngine.Util;
using Microsoft.Xna.Framework;

namespace Villainer.Script
{
    public class Shoot : EntityAnimated
    {
        public float Speed { get; set; } = 12;
        public bool IsExploded { get; set; } = false;

        public Timer ExplodeTimer { get; set; } = new(500);

        public Shoot()
        {
        }

        public Shoot(Animator animator, Vector2 direction) : base(animator)
        {
            Velocity = direction * Speed;
            Rotation = MgMath.GetRotationDirection(direction);
            SetAction("Shoot");
            ExplodeTimer.Elapsed += () => { IsExploded = true; };
            ExplodeTimer.Start();
            ResizeScale(1.2f);
        }

        public void Update(float dt)
        {
            Pos += Velocity * dt;

            ExplodeTimer.Update(dt);

            Animate(dt);
        }

        public void Explode()
        {

        }
    }
}
