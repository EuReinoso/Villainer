using MgEngine.Component;
using MgEngine.Shape;
using MgEngine.Time;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Villainer.Script
{
    public class Enemy : Platformer
    {
        protected int _life { get; set; } = 30;
        private Timer _damageTimer { get; set; } = new Timer(4);

        public Enemy()
        {
            Initialize();
        }

        public Enemy(Animator animator) : base(animator)
        {
            Initialize();
        }

        public void Initialize()
        {
            _damageTimer.Elapsed += DamageTimer_Elapsed;
        }

        private void DamageTimer_Elapsed()
        {
            ColorEffect = Color.White;
        }

        public virtual void Update(float dt)
        {
            _damageTimer.Update(dt);
        }

        public void CollideShoot(List<Shoot> shootsList)
        {
            var rect = Rect;

            foreach(var shoot in shootsList)
            {
                if (rect.CollideRect(shoot.Rect, 50, 50))
                {
                    shoot.Explode();
                    Damage();
                }

            }
        }

        public void Damage(int amount = 1)
        {
            ColorEffect = Color.LightGray;
            _damageTimer.Start();
        }
    }
}
