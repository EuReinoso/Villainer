using MgEngine.Audio;
using MgEngine.Component;
using MgEngine.Effect;
using MgEngine.Shape;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villainer.Script
{
    public class Player : Platformer
    {
        public byte Life = 3;
        public int RecallArea = 30;
        public bool RecallActive = false;
        private Vector2 RecallForce = new Vector2(0, -10);

        public Player() : base()
        {

        }

        public void Load(ContentManager content)
        {
            var anim = new Animator(content);

            anim.Add("Player/Player_idle", "Idle", 25, 32, new() { 8, 8, 8, 8 });
            anim.Add("Player/Player_walk", "Walk", 24, 32, new() { 7, 7, 7, 7, 7, 7, 7, 7 });

            SetAnimator(anim);
            SetAction("Idle");

            JumpKeyDown += () => { RecallActive = true; };
        }

        public Rect RecallRect { get { return new Rect(X, Y, Width + RecallArea, Height + RecallArea);} }

        public void Damage(byte amount = 1)
        {
            Life -= amount;

            Singer.PlaySound("PlayerDamage");
            Particlerr.Add(ParticleShape.Circle, Particles.Explosion, 20, Pos, Color.OrangeRed);
        }

        public void Recall()
        {
            Velocity += RecallForce;
            RecallActive = false;

            Singer.PlaySound("Recall");
            Particlerr.Add(ParticleShape.Circle, Particles.Explosion, 20, Pos, Color.GreenYellow);
        }
    }
}
