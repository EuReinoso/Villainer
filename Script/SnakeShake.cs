using MgEngine.Component;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villainer.Script
{
    public class SnakeShake : Platformer
    {
        public SnakeShake() : base()
        {

        }

        public void Load(ContentManager content)
        {
            var anim = new Animator(content);

            anim.Add("Enemy/SnakeShake_idle", "Idle", 32, 32, new() { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 });

            SetAnimator(anim);
            SetAction("Idle");

            ResizeScale(4);

            Effect = SpriteEffects.None;
        }
    }
}
