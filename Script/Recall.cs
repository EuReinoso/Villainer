using MgEngine.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villainer.Script
{
    public class Recall : Entity
    {
        public bool IsRecallActive;
        public Color RecallColor = Color.GreenYellow;

        public Recall()
        {
        }

        public Recall(Texture2D texture) : base(texture)
        {
        }

        public void ActivateRecall()
        {
            IsRecallActive = true;
            ColorEffect = RecallColor;
            BorderColor = Color.GreenYellow;
        }
    }
}
