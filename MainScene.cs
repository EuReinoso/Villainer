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

namespace Villainer
{
    public class MainScene : Scene
    {
        #region Variables

        Player _player;
        Tiled _map;

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
            _player.Pos = new(100, 600);
        }

        public override void LoadContent(ContentManager content)
        {
            _player.Load(content);

            var spriteSheet = new SpriteSheet(content, "Map/Tiles", 32, 32);

            _map =  new Tiled(spriteSheet);
            _map.ReadMap(MgFiles.RootDirectory + "/Content/Map/map1.json");
        }

        public override void Update(float dt, Inputter inputter)
        {
            _player.Animate(dt);
        }
        
        public override void Draw(SpriteBatch spriteBatch, ShapeBatch sprites)
        {
            _map.Draw(spriteBatch);

            _player.Draw(spriteBatch);
        }
        #endregion

        #region Methods

        #endregion

    }
}
