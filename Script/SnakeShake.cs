using MgEngine.Component;
using MgEngine.Time;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;

namespace Villainer.Script
{
    public class SnakeShake : Enemy
    {
        private Timer Atack1Timer = new(100);

        public enum Actions
        {
            Idle,
            Atack1Pre,
            Atack1End,
            Atack1,
            Atack1Reverse,
        }

        public SnakeShake() : base()
        {

        }

        public void Load(ContentManager content)
        {
            var anim = new Animator(content);

            anim.Add("Enemy/SnakeShake_idle", Actions.Idle, 32, 32, new List<int>() { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 });
            anim.Add("Enemy/SnakeShake_1_pre", Actions.Atack1Pre, 31, 54, new List<int>() { 8, 8, 8, 8, 8, 8, 8, 8 }, 1, OnResetAtack1Pre, false, new Vector2( 16, 54-16));
            anim.Add("Enemy/SnakeShake_1_pre", Actions.Atack1End, 31, 54, new List<int>() { 8, 8, 8, 8, 8, 8, 8, 8 }, 1, OnResetAtack1End, true, new Vector2(16, 54-16));
            anim.Add("Enemy/SnakeShake_1", Actions.Atack1, 65, 54, 1, 1, OnResetAtack1, false, new Vector2(65-16, 54-16));
            anim.Add("Enemy/SnakeShake_1", Actions.Atack1Reverse, 65, 54, 1, 1, OnResetAtack1Reverse, true, new Vector2(65-16, 54-16));

            SetAnimator(anim);

            Scale = 6;
            SetAction(Actions.Atack1Pre);
            Effect = SpriteEffects.None;
        }

        public override void Update(float dt)
        {
            Atack1Timer.Update(dt);

            base.Update(dt);
        }

        private void OnResetAtack1Pre()
        {
            SetAction(Actions.Atack1);
            Atack1Timer.Start();
        }

        private void OnResetAtack1End()
        {
            SetAction(Actions.Idle);
        }

        private void OnResetAtack1()
        {
            SetAction(Actions.Atack1Reverse);

            if (Effect == SpriteEffects.FlipHorizontally)
                OffSet = new Vector2(16, 54 - 16);

        }

        private void OnResetAtack1Reverse()
        {
            if (!Atack1Timer.IsActivate)
            {
                SetAction(Actions.Atack1End);
                return;
            }

            SetAction(Actions.Atack1);
            FlipHorizontal();

            if (Effect == SpriteEffects.FlipHorizontally)
                OffSet = new Vector2(16, 54 - 16);
        }

        private void FlipHorizontal()
        {
            if (Effect == SpriteEffects.None)
            {
                Effect = SpriteEffects.FlipHorizontally;
                return;
            }

            Effect = SpriteEffects.None;
        }
    }
}
