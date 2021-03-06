using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pathogenesis.Models;

namespace Pathogenesis
{
    #region Enum
    /*
     * Possible game states
     */ 
    public enum GameState
    {
        MAIN_MENU,  // Player is at the main menu
        IN_GAME,    // Player is playing the game
        VICTORY,    // Player has completed the level
        PAUSED      // Player has activated the pause menu
    }
    #endregion

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameEngine : Microsoft.Xna.Framework.Game
    {
        #region Fields
        // First time game loaded?
        private bool firstLoop = true;

        private SpriteBatch spriteBatch;

        // Used to draw the game onto the screen (VIEW CLASS)
        private GameCanvas canvas;

        // Used to load content and create game objects
        private ContentFactory factory;

        // Game camera, position determines portion of map drawn on screen
        private Camera camera;

        // Game operation controllers
        private InputController input_controller;
        private CollisionController collision_controller;

        // Game entity controllers
        private GameUnitController unit_controller;
        private ItemController item_controller;
        private LevelController level_controller;

        private HUD HUD_display;
        private GameState game_state;
        #endregion

        #region Initialization
        public GameEngine()
        {
            canvas = new GameCanvas(this);
            factory = new ContentFactory(new ContentManager(Services));
            camera = new Camera(canvas.Width, canvas.Height);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (firstLoop) factory.LoadAllContent();
            canvas.Initialize(this);

            // Game starts at the main menu
            game_state = GameState.MAIN_MENU;

            // Initialize controllers
            input_controller = new InputController();
            collision_controller = new CollisionController();

            level_controller = new LevelController();
            unit_controller = new GameUnitController(factory);
            item_controller = new ItemController();



            // TEST
            HUD_display = factory.createHUD(unit_controller.Player);
            level_controller.NextLevel(factory, unit_controller, item_controller);
            unit_controller.SetLevel(level_controller.CurLevel);

            bool test = level_controller.CurLevel.Map.rayCastHasObstacle(
                new Vector2(0*Map.TILE_SIZE, 0*Map.TILE_SIZE), new Vector2(20*Map.TILE_SIZE, 11*Map.TILE_SIZE),
                Map.TILE_SIZE);
            System.Diagnostics.Debug.WriteLine(test);
            base.Initialize();
            firstLoop = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            canvas.LoadContent(factory);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //canvas.Dispose();
            factory.UnloadAll();
            
        }
    #endregion

        #region Update Functions
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            input_controller.Update();    // Receive and process input
            switch (game_state)
            {
                case GameState.MAIN_MENU:
                    // for now
                    game_state = GameState.IN_GAME;
                    break;
                case GameState.IN_GAME:
                    // Remove later
                    Random rand = new Random();
                    if (rand.NextDouble() < 0.02 && unit_controller.Units.Count < 200)
                    {
                        int level = rand.NextDouble() < 0.2 ? (rand.NextDouble() < 0.2? 3 : 2) : 1;
                        unit_controller.AddUnit(factory.createUnit(UnitType.TANK, UnitFaction.ENEMY, level,
                            new Vector2(rand.Next(level_controller.CurLevel.Width), rand.Next(level_controller.CurLevel.Height)),
                            rand.NextDouble() < 0.1 ? true : false));
                        item_controller.AddItem(factory.createPickup(new Vector2(rand.Next(level_controller.CurLevel.Width), rand.Next(level_controller.CurLevel.Height)),
                            rand.NextDouble() < 0.2? ItemType.HEALTH : ItemType.PLASMID));
                    }
                    //
                    /* TODO: Remove DEBUG CODE HERE (remove later)
                     * 
                     */
                    if (input_controller.Spawn_Enemy)
                    {
                        unit_controller.AddUnit(factory.createUnit(UnitType.TANK, UnitFaction.ENEMY, 1,
                            new Vector2(rand.Next(level_controller.CurLevel.Width), rand.Next(level_controller.CurLevel.Height)), false));
                    }
                    if (input_controller.Spawn_Ally && unit_controller.Player != null)
                    {
                        unit_controller.AddUnit(factory.createUnit(UnitType.TANK, UnitFaction.ALLY, 1,
                            unit_controller.Player.Position + new Vector2(1, 1), false));
                    }
                    if (input_controller.Spawn_Plasmid)
                    {
                        item_controller.AddItem(factory.createPickup(new Vector2(rand.Next(level_controller.CurLevel.Width), rand.Next(level_controller.CurLevel.Height)),
                            ItemType.PLASMID));
                    }
                    //Restart
                    if (input_controller.Restart)
                    {
                        Initialize();
                        level_controller.ResetLevel(factory, unit_controller, item_controller);
                        item_controller.Reset();
                    }

                    // Process level environment logic
                    bool victory = level_controller.Update();
                    if (victory)
                    {
                        level_controller.NextLevel(factory, unit_controller, item_controller);
                    }

                    // Process and update all units
                    unit_controller.Update(level_controller.CurLevel, input_controller);
                    // Process and handle collisions
                    collision_controller.Update(unit_controller.Units, unit_controller.Player,
                        level_controller.CurLevel, item_controller);
                    // Update the HUD
                    HUD_display.Update(input_controller);
                    
                    if (unit_controller.Player != null)
                    {
                        camera.Position = unit_controller.Player.Position;
                    }
                    break;
                case GameState.PAUSED:
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            canvas.Reset();
            canvas.BeginSpritePass(BlendState.AlphaBlend, camera);

            switch (game_state)
            {
                case GameState.IN_GAME:
                    level_controller.Draw(canvas);
                    HUD_display.DrawLayerOne(canvas, unit_controller.Units, unit_controller.Player);
                    item_controller.Draw(canvas);
                    unit_controller.Draw(canvas);
                    HUD_display.DrawLayerTwo(canvas, unit_controller.Units, unit_controller.Player);
                    break;
                case GameState.MAIN_MENU:
                    break;
                case GameState.PAUSED:
                    break;
                case GameState.VICTORY:
                    break;
            }

            canvas.EndSpritePass();
            base.Draw(gameTime);
        }
        #endregion
    }
}
