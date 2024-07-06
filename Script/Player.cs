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

namespace Villainer.Script
{
    public class Player : Platformer
    {
        public byte Life = 3;
        public int RecallArea = 80;
        public bool IsRecallActive = false;
        public Vector2 RecallForce = new Vector2(0, -10);
        public float DashForce = 10;
        public float DashDecay = 0.5f;
        public bool IsDashing = false;
        public bool IsDashActive = false;
        public bool IsShooting = false;
        public int InvencibleTime = 60;
        public Timer InvencibleTimer;
        public List<Tuple<Entity, Timer>> DashEffect = new();
        public int Shoots = 0;
        public int ShootsLimit = 3;
        public Animator ShootAnimator;
        public List<Shoot> ShootsList = new();

        public Player() : base()
        {

        }

        public void Load(ContentManager content)
        {
            var anim = new Animator(content);

            anim.Add("Player/Player_idle", "Idle", 25, 32, new() { 8, 8, 8, 8 });
            anim.Add("Player/Player_walk", "Walk", 24, 32, new() { 7, 7, 7, 7, 7, 7, 7, 7 });
            anim.Add("Player/Player_dash", "Dash", 25, 32, new() { 7, 7, 7 }, onReset : DefaultAction);
            anim.Add("Player/Player_shoot", "Shoot", 23, 32, new() { 3, 3, 3, 6, 3, 3 }, onReset : DefaultAction);
            anim.Add("Player/Player_shoot_up", "ShootUp", 23, 32, new() { 3, 3, 3, 6, 3, 3 }, onReset: DefaultAction);

            SetAnimator(anim);
            SetAction("Idle");

            JumpKeyDown += () => { IsRecallActive = true; };
            KeyJump = Keys.Space;
            KeyUp = Keys.Up;

            InvencibleTimer = new Timer(InvencibleTime);
            InvencibleTimer.Elapsed += () => { ColorEffect = Color.White; };

            OnCollideBottom += () => { IsDashActive = true; };

            IsBorderAutoUpdate = true;

            ShootAnimator = new(content);
            ShootAnimator.Add("Player/Shoot", "Shoot", 16, 16, new() { 10, 10, 10 });
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

            for (int i = ShootsList.Count - 1; i >= 0; i--)
            {
                var shoot = ShootsList[i];

                shoot.Update(dt);

                if (shoot.IsExploded)
                    ShootsList.RemoveAt(i);
            }
        }

        public override void UpdateMove(Inputter inputter)
        {
            base.UpdateMove(inputter);

            if (IsShooting)
            {
                Velocity = Vector2.Zero;
                return;
            }

            if (Shoots > 0 && inputter.KeyDown(Keys.D))
                ShootSetup(inputter);
            else if (IsDashActive && !IsDashing && inputter.KeyDown(Keys.LeftShift))
                DashSetup();

            if (inputter.KeyDown(Keys.LeftControl))
            {
                EnableHorizontalMovement = false;
            }
            else if (inputter.KeyUp(Keys.LeftControl))
            {
                EnableHorizontalMovement = true;
            }
        }

        public override void Move(float dt)
        {
            if (IsShooting)
                return;

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
            Particlerr.Add(50, ParticleShape.Circle, Particles.PlayerDamage, Pos, Color.SandyBrown);
        }

        public void Recall()
        {
            Velocity += RecallForce;
            IsRecallActive = false;
            IsDashActive = true;
            AddDashEffect(60);

            if (Shoots < ShootsLimit)
                Shoots++;

            Singer.PlaySound("Recall");
            Particlerr.Add(40, ParticleShape.Circle, Particles.RecallExplosion, Pos, Color.GreenYellow);
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

        private void DefaultAction()
        {
            Rotation = 0;
            IsShooting = false;
            PriorityActionActive = false;
        }

        private void AddShoot(Vector2 direction)
        {
            Shoots--;
            var shoot = new Shoot(ShootAnimator, direction);
            shoot.Pos = Pos;
            ShootsList.Add(shoot);
        }

        private void ShootSetup(Inputter inputter)
        {
            if (IsDashing)
                PriorityActionActive = false;

            if (inputter.IsKeyDown(KeyUp) && inputter.IsKeyDown(KeyRight))
            {
                SetAction("Shoot");
                AddShoot(new Vector2(1, -1));
            }
            else if (inputter.IsKeyDown(KeyUp) && inputter.IsKeyDown(KeyLeft))
            {
                SetAction("Shoot");
                AddShoot(new Vector2(-1, -1));
            }
            else if (inputter.IsKeyDown(KeyUp))
            {
                SetAction("ShootUp");
                AddShoot(-Vector2.UnitY);
            }
            else if (inputter.IsKeyDown(KeyDown))
            {
                SetAction("ShootUp");
                Rotation = (float)Math.PI;
                AddShoot(Vector2.UnitY);
            }
            else
            {
                SetAction("Shoot");

                if (Effect == SpriteEffects.FlipHorizontally)
                    AddShoot(-Vector2.UnitX);
                else
                    AddShoot(Vector2.UnitX);
            }

            IsShooting = true;
            PriorityActionActive = true;
            Singer.PlaySound("Shoot");
        }

        private void DashSetup()
        {
            if (Effect == SpriteEffects.FlipHorizontally)
                Velocity += new Vector2(-DashForce, 0);
            else
                Velocity += new Vector2(DashForce, 0);

            IsDashing = true;
            IsDashActive = false;
            SetAction("Dash");
            PriorityActionActive = true;
            Singer.PlaySound("Dash", 0.7f);
        }

        public override void Draw(SpriteBatch spriteBatch, float scrollX, float scrollY)
        {
            base.Draw(spriteBatch, scrollX, scrollY);

            foreach (var shoot in ShootsList)
            {
                shoot.Draw(spriteBatch, scrollX, scrollY);
            }
        }
    }
}
