using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MgEngine.Scene;
using MgEngine.Input;
using MgEngine.Time;
using MgEngine.Shape;
using MgEngine.Screen;
using Villainer.Script;
using MgEngine.Map;
using MgEngine.Util;
using System.Collections.Generic;
using System;
using MgEngine.Component;
using MgEngine.Audio;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using MgEngine.Effect;

namespace Villainer
{
    public class MainScene : Scene
    {
        #region Variables

        Player _player;
        SnakeShake _enemy;
        Tiled _map;
        Physics _physics;

        List<Wall> _walls;
        List<Wall> _wallsFall = new();
        List<Entity> _terrain;

        float _floorY;
        #endregion

        #region Constructor
        public MainScene(Window window, Camera camera) : base(window, camera)
        { 
        }
        #endregion

        #region Loop
        public override void Initialize()
        {
            _player = new();
            _player.Pos = new(100, 500);

            _enemy = new();
            _enemy.Pos = new(1000, 500);

            _physics = new();
            _physics.Gravity = new Vector2(0, 25);

            Scroller.SetEntityTarget(_player);
            LoadMoveEffects();
        }

        public override void LoadContent(ContentManager content)
        {
            _player.Load(content);
            _enemy.Load(content);

            var spriteSheet = new SpriteSheet(content, "Map/Tiles", 32, 32);

            _map =  new Tiled(spriteSheet);
            _map.ReadMap(MgFiles.RootDirectory + "/Content/Map/map1.json");
            _terrain = _map.GetLayer<Entity>("Terrain");
            _walls = _map.GetLayer<Wall>("Walls");

            Scroller.CalculateBorders(_map.GetLayer<Entity>("Terrain"),0, 30);
            _floorY = Scroller.MaxY + Window.Canvas.Height;

            Singer.PlayMusic("SnakeShake");
            Singer.MusicVolume = .5f;
            Singer.MasterVolume = 0.1f;
        }

        public override void Update(float dt, Inputter inputter)
        {

            Scroller.Update();

            _player.Animate(dt);
            _player.UpdateMove(inputter, dt);
            _player.Move(dt);
            _player.UpdateCollision(_terrain);
            _player.UpdatePhysics(_physics, dt);

            _enemy.Animate(dt);
            _enemy.Move(dt);
            _enemy.UpdateCollision(_terrain);

            _wallsFall.UpdateList(inputter, dt);
            UpdateWallFall();

            Particlerr.Update(dt);
            Color color = Color.White;

            if (inputter.IsKeyDown(Keys.LeftShift))
                color = MgUtil.RandomColor();

            var mPos = inputter.GetMousePos();
            if (inputter.IsKeyDown(Keys.Z))
            {
                Particlerr.Add(ParticleShape.Circle, "wallExplosion", 2, mPos, color);
            }
            if (inputter.IsKeyDown(Keys.X))
            {
                Particlerr.Add(ParticleShape.Rect, "wallExplosion", 2, mPos, color);
            }
            if (inputter.IsKeyDown(Keys.C))
            {
                Particlerr.Add(ParticleShape.Triangle, "wallExplosion", 2, mPos, color);
            }
            if (inputter.KeyDown(Keys.E))
            {
                StartWallFall(10);
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch, ShapeBatch shapeBatch)
        {
            //_map.Draw(spriteBatch, -Scroller.X, -Scroller.Y);
            //_enemy.DrawRect(shapeBatch, new Color(255, 0, 0, 0.001f), -Scroller.X, -Scroller.Y);

            Entity.DrawList(_terrain, spriteBatch, -Scroller.X, -Scroller.Y);
            Entity.DrawList(_walls, spriteBatch, -Scroller.X, -Scroller.Y);
            Entity.DrawList(_wallsFall, spriteBatch, -Scroller.X, -Scroller.Y);

            NextLayer();
            _enemy.Draw(spriteBatch, -Scroller.X, -Scroller.Y);

            _player.Draw(spriteBatch, -Scroller.X, -Scroller.Y);

            Particlerr.Draw(spriteBatch, -Scroller.X, -Scroller.Y);
        }
        #endregion

        #region Methods
        private void LoadMoveEffects()
        {
            var wallExplosion = new ParticleMoveEffect();
            wallExplosion.SizeMinStart = 10;
            wallExplosion.SizeMaxStart = 20;
            wallExplosion.VelocityMinStart = new Vector2(-3, -3);
            wallExplosion.VelocityMaxStart = new Vector2(3, 3);
            wallExplosion.SizeDecay = 0.6f;
            wallExplosion.MinRotationVelocity = -.2f;
            wallExplosion.MaxRotationVelocity = .2f;

            Particlerr.AddMoveEffect("wallExplosion", wallExplosion);
        }

        private void StartWallFall(int quant = 30)
        {
            var rand = new Random();

            for (int i = 0; i < quant; i++)
            {
                int index = rand.Next(_walls.Count);

                _walls[index].IsFalling = true;

                _wallsFall.Add(_walls[index]);
                _walls.RemoveAt(index);
            }
        }

        private void UpdateWallFall()
        {
            for(int i = _wallsFall.Count -1; i >= 0; i--)
            {
                var wall = _wallsFall[i];
                bool explode = false;

                foreach(var terrain in _terrain)
                {
                    if (Polygon.CollidePolygon(wall.Rect.Vertices.ToList(), terrain.Rect.Vertices.ToList(), out Vector2 normal, out float depth))
                    {
                        if (normal.Y < 0)
                            return;

                        wall.Pos -= normal * depth;
                        explode = true;
                    }
                }

                if (Polygon.CollidePolygon(wall.Rect.GetVertices(), _player.Rect.GetVertices(), out Vector2 normalB, out float depthB))
                    _player.Damage();

                if (explode)
                {
                    wall.Explode();
                    _wallsFall.Remove(wall);
                }
            }
        }
        #endregion

    }
}
