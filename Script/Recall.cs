using MgEngine.Component;
using MgEngine.Shape;
using MgEngine.Time;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MgEngine.Map.Tiled;

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

        public void MeasureRecall(Player player, out bool recall, out bool damage, out float recallDepth)
        {
            recall = false;
            damage = false;
            recallDepth = 0;

            BorderWidth = 1;

            if (IsRecallActive && Polygon.CollidePolygon(Rect.GetVertices(), player.RecallRect.GetVertices(), out Vector2 normal, out float depth))
            {
                if (player.IsRecallActive)
                {
                    player.Recall();
                    recall = true;
                }

                recallDepth = depth;
                BorderWidth = 4;
            }

            if (!recall && !player.InvencibleTimer.IsActivate && Polygon.CollidePolygon(Rect.GetVertices(), player.Rect.GetVertices(), out Vector2 normalB, out float depthB))
            {
                player.Damage();
                damage = true;
            }
        }

        public void ActivateRecall()
        {
            IsRecallActive = true;
            ColorEffect = RecallColor;
            BorderColor = Color.GreenYellow;
        }
    }
}
