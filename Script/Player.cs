using MgEngine.Audio;
using MgEngine.Component;
using MgEngine.Effect;
using MgEngine.Input;
using MgEngine.Shape;
using MgEngine.Time;
using MgEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Villainer.Script
{
    public class Player : Platformer
    {
        public byte Life = 3;
        public int RecallArea = 30;
        public bool RecallActive = false;
        public Vector2 RecallForce = new Vector2(0, -10);
        public float DashForce = 10;
        public float DashDecay = 0.5f;
        public bool IsDashing = false;
        public bool IsDashActive = false;
        public int InvencibleTime = 60;
        public Timer InvencibleTimer;
        public List<Tuple<Entity, Timer>> DashEffect = new();

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
            KeyJump = Keys.Up;
            KeyUp = Keys.None;

            InvencibleTimer = new Timer(InvencibleTime);
            InvencibleTimer.Elapsed += () => { ColorEffect = Color.White; };

            OnCollideBottom += () => { IsDashActive = true; };

            IsBorderAutoUpdate = true;
        }

        public void Update(float dt)
        {
            InvencibleTimer.Update(dt);

            for (int i = DashEffect.Count - 1; i >= 0; i--)
            {
                Timer timer = DashEffect[i].Item2;

                timer.Update(dt);

                if (!timer.IsActivate)
                    DashEffect.RemoveAt(i);
            }

            if (InvencibleTimer.IsActivate)
            {
                if (Math.Sin(InvencibleTimer.ElapsedTime) > 0.5f)
                    ColorEffect = Color.Gray;
                else
                    ColorEffect = Color.White;
            }
        }

        public override void UpdateMove(Inputter inputter)
        {
            base.UpdateMove(inputter);

            if (IsDashActive && !IsDashing && inputter.KeyDown(Keys.D))
            {
                if (Effect == SpriteEffects.FlipHorizontally)
                    Velocity += new Vector2(-DashForce, 0);
                else
                    Velocity += new Vector2(DashForce, 0);

                IsDashing = true;
                IsDashActive = false;
                Singer.PlaySound("Dash");
            }
        }

        public override void Move(float dt)
        {
            base.Move(dt);

            if (Effect == SpriteEffects.None && Velocity.X > 0)
            {
                Velocity -= new Vector2(DashDecay, 0);
                AddDashEffect(5);
            }
            else if (Effect == SpriteEffects.FlipHorizontally && Velocity.X < 0)
            {
                Velocity += new Vector2(DashDecay, 0);
                AddDashEffect(5);
            }
            else
            {
                Velocity = new Vector2(0, Velocity.Y);
                IsDashing = false;
            }
        }

        public Rect RecallRect { get { return new Rect(X, Y, Width + RecallArea, Height + RecallArea);} }

        public void Damage(byte amount = 1)
        {
            if (InvencibleTimer.IsActivate)
                return;

            Life -= amount;
            InvencibleTimer.Start();

            Singer.PlaySound("PlayerDamage");
            Particlerr.Add(20, ParticleShape.Circle, Particles.Explosion, Pos, Color.SandyBrown);
        }

        public void Recall()
        {
            Velocity += RecallForce;
            RecallActive = false;
            IsDashActive = true;
            AddDashEffect(20);
            Singer.PlaySound("Recall");
            Particlerr.Add(20, ParticleShape.Circle, Particles.Explosion, Pos, Color.GreenYellow);
        }

        public void AddDashEffect(int duration)
        {
            var entity = new Entity();
            entity.Pos = Pos;
            entity.SetTexture(GetBorderTexture(), _sourceRectangle);
            entity.Effect = Effect;

            var timer = new Timer(duration);

            timer.Start();

            DashEffect.Add(new Tuple<Entity, Timer>(entity, timer));
        }

        public void DrawDashEffect(SpriteBatch spriteBatch, float scrollX, float scrollY)
        {
            foreach(var item in DashEffect)
            {
                item.Item1.Draw(spriteBatch, scrollX, scrollY);
            }
        }
    }
}
