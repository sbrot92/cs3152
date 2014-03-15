﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Pathogenesis
{
    public class ContentFactory
    {
        #region Fields
            // Used to load the sounds and graphics (CONTROLLER CLASS)
            protected ContentManager content;

            // Dictionary of all textures mapped as <filename, texture>
            protected Dictionary<String, Texture2D> textures;

            protected SpriteFont font;

            // Content directories and filenames
            private const string CHARACTERS_DIR = "Characters/";
            private const string BACKGROUNDS_DIR = "Backgrounds/";
            private const string ENVIRONMENT_DIR = "Environment/";

            private const string MAINPLAYER = CHARACTERS_DIR + "mainplayer";

            private const string ENEMY_TANK = CHARACTERS_DIR + "enemy_tank";
            private const string ENEMY_RANGED = CHARACTERS_DIR + "enemy_ranged";
            private const string ENEMY_FLYING = CHARACTERS_DIR + "enemy_flying";

            private const string ALLY_TANK = CHARACTERS_DIR + "ally_tank";
            private const string ALLY_RANGED = CHARACTERS_DIR + "ally_ranged";
            private const string ALLY_FLYING = CHARACTERS_DIR + "ally_flying";

            private const string BACKGROUND1 = BACKGROUNDS_DIR + "background1";
            private const string BACKGROUND2 = BACKGROUNDS_DIR + "background2";
            private const string BACKGROUND3 = BACKGROUNDS_DIR + "background3";
        #endregion

        #region Initialization
            public ContentFactory(ContentManager content)
            {
                // Tell the program to load all files relative to the "Content" directory.
                this.content = content;
                content.RootDirectory = "Content";

                this.textures = new Dictionary<string, Texture2D>();
            }

            // Loads all content from content directory
            public void LoadAllContent()
            {
                // Load textures into the textures map
                textures.Add(MAINPLAYER, content.Load<Texture2D>(MAINPLAYER));

                textures.Add(ENEMY_TANK, content.Load<Texture2D>(ENEMY_TANK));
                textures.Add(ENEMY_RANGED, content.Load<Texture2D>(ENEMY_RANGED));
                textures.Add(ENEMY_FLYING, content.Load<Texture2D>(ENEMY_FLYING));

                textures.Add(ALLY_TANK, content.Load<Texture2D>(ALLY_TANK));
                textures.Add(ALLY_RANGED, content.Load<Texture2D>(ALLY_RANGED));
                textures.Add(ALLY_FLYING, content.Load<Texture2D>(ALLY_FLYING));

                textures.Add(BACKGROUND1, content.Load<Texture2D>(BACKGROUND1));
                textures.Add(BACKGROUND2, content.Load<Texture2D>(BACKGROUND2));
                textures.Add(BACKGROUND3, content.Load<Texture2D>(BACKGROUND3));

                // Load fonts
                font = content.Load<SpriteFont>("Fonts/font");
            }

            public void UnloadAll()
            {
                content.Unload();
            }
        #endregion

        #region Instantiation functions
            // Returns an instance of Player
            public Player createPlayer(Vector2 pos)
            {
                Player p = new Player(textures[MAINPLAYER]);
                p.Position = pos;
                return p;
            }
        
            // Returns an instance of a unit of the given type and faction
            public GameUnit createUnit(UnitType type, UnitFaction faction, Vector2 pos)
            {
                GameUnit enemy;
                switch (type)
                {
                    case UnitType.TANK:
                        enemy = new GameUnit(faction == UnitFaction.ALLY? textures[ALLY_TANK] : textures[ENEMY_TANK], type, faction);
                        break;
                    case UnitType.RANGED:
                        enemy = new GameUnit(faction == UnitFaction.ALLY ? textures[ALLY_RANGED] : textures[ENEMY_RANGED], type, faction);
                        break;
                    case UnitType.FLYING:
                        enemy = new GameUnit(faction == UnitFaction.ALLY ? textures[ALLY_FLYING] : textures[ENEMY_FLYING], type, faction);
                        break;
                    default:
                        enemy = null;
                        break;
                }
                if (enemy != null)
                {
                    enemy.Position = pos;
                }
                return enemy;
            }

            // Returns the game font
            public SpriteFont getFont()
            {
                return font;
            }

        #endregion
    }
}