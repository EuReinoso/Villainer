using MgEngine.Component;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villainer.Script
{
    public class Player : EntityAnimated
    {
        public Player() : base()
        {

        }

        public void Load(ContentManager content)
        {
            var anim = new Animator(content);

            anim.Add("Player/Player_idle", "Idle", 25, 32, new() { 8,8,8,8});
            anim.Add("Player/Player_walk", "Walk", 24, 32, new() { 7, 7, 7, 7, 7, 7, 7, 7 });

            SetAnimator(anim);
            SetAction("Idle");
        }
    }
}
