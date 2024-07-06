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

        Timer _damageEffectTimer = new(2);
        #endregion

        #region Constructor
        public MainScene(Window window, Camera camera, Clock clock) : base(window, camera, clock)
        { 
        }
        #endregion

        #region Loop
        public override void Initialize()
        {
            _damageEffectTimer.Elapsed += DamageEffectTimer_Elapsed;

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

            Singer.PlayMusicEffect("Snake Shake");
        }

        public override void Update(float dt, Inputter inputter)
        {
            RecallReset();
            Scroller.Update(dt);

            _player.Update(dt);
            _player.Animate(dt);
            _player.UpdateMove(inputter);
            _player.Move(dt);
            _player.UpdateCollision(_terrain);
            _player.UpdatePhysics(_physics, dt);

            _enemy.Animate(dt);
            _enemy.Move(dt);
            _enemy.UpdateCollision(_terrain);

            _wallsFall.UpdateList(inputter, dt);
            UpdateWallFall();

            Particlerr.Update(dt);

            if (inputter.KeyDown(Keys.E))
                StartWallFall(10);

            _damageEffectTimer.Update(dt);
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

            Particlerr.Draw(spriteBatch, -Scroller.X, -Scroller.Y);

            _player.DrawDashEffect(spriteBatch, -Scroller.X, -Scroller.Y);
            _player.Draw(spriteBatch, -Scroller.X, -Scroller.Y);
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

            Particlerr.AddMoveEffect(Particles.Explosion, wallExplosion);

            var wallFall = new ParticleMoveEffect();
            wallFall.SizeMinStart = 3;
            wallFall.SizeMaxStart = 3;
            wallFall.VelocityMinStart = new Vector2(-.5f, -1);
            wallFall.VelocityMaxStart = new Vector2(.5f, -1);
            wallFall.SizeDecay = 0.2f;

            Particlerr.AddMoveEffect(Particles.Fall, wallFall);

            var dash = new ParticleMoveEffect();
            dash.SizeMinStart = 3;
            dash.SizeMaxStart = 3;
            dash.VelocityMinStart = new Vector2(0, 0);
            dash.VelocityMaxStart = new Vector2(0, 0);
            dash.SizeDecay = 0.1f;

            Particlerr.AddMoveEffect(Particles.Dash, dash);


            var playerDamage = new ParticleMoveEffect();
            playerDamage.SizeMinStart = 30;
            playerDamage.SizeMaxStart = 40;
            playerDamage.VelocityMinStart = new Vector2(-20, -4);
            playerDamage.VelocityMaxStart = new Vector2(20, 4);
            playerDamage.SizeDecay = 2f;
            playerDamage.MinRotationVelocity = -.2f;
            playerDamage.MaxRotationVelocity = .2f;

            Particlerr.AddMoveEffect(Particles.PlayerDamage, playerDamage);

            var recallExplosion = new ParticleMoveEffect();
            recallExplosion.SizeMinStart = 20;
            recallExplosion.SizeMaxStart = 30;
            recallExplosion.VelocityMinStart = new Vector2(-10, -10);
            recallExplosion.VelocityMaxStart = new Vector2(10, 10);
            recallExplosion.SizeDecay = 1f;
            recallExplosion.MinRotationVelocity = -.2f;
            recallExplosion.MaxRotationVelocity = .2f;

            Particlerr.AddMoveEffect(Particles.RecallExplosion, recallExplosion);
        }

        private void StartWallFall(int quant = 30)
        {
            var rand = new Random();

            for (int i = 0; i < quant; i++)
            {
                if (_walls.Count <= 0)
                    continue;

                int index = rand.Next(_walls.Count);

                _walls[index].IsFalling = true;
                _walls[index].ColorEffect = Color.LightGray;
                _walls[index].IsBorderEnabled = true;

                if (rand.NextSingle() <= 0.3f)
                    _walls[index].ActivateRecall();

                _wallsFall.Add(_walls[index]);
                _walls.RemoveAt(index);
            }

            Singer.PlaySound("StartFall");
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

                wall.MeasureRecall(_player, out bool recall, out bool damage, out float recallDepth);

                if (damage)
                    StartDamageEffect();

                if (explode || recall || damage)
                {
                    wall.Explode();
                    _wallsFall.Remove(wall);
                }
            }
            
        }

        private void RecallReset()
        {
            _player.IsRecallActive = false;
        }

        protected void StartDamageEffect()
        {
            Clock.Speed = .1f;
            Singer.SetSpeed(-.9f);
            Singer.MusicVolume = 0.1f;
            Singer.VolumeFlush();
            Scroller.Shake(20, -5, 5);
            Window.Canvas.ColorEffect = Color.LightGray;
            _damageEffectTimer.Start();
        }

        private void DamageEffectTimer_Elapsed()
        {
            Clock.Speed = 1;
            Singer.SetSpeed(0);
            Singer.MusicVolume = 1f;
            Singer.VolumeFlush();
            Window.Canvas.ColorEffect = Color.White;
        }
        #endregion

    }
}
