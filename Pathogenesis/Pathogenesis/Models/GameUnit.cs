﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Pathogenesis
{

#region Enums
    /// <summary>
    /// Type of the game unit
    /// </summary>
    /// <remarks>
    /// Different types of units behave with different AI and have different stats.
    /// </remarks>
    public enum UnitType
    {
        TANK,       // High defense, slow
        RANGED,     // High speed, low defense, ranged attack
        FLYING,     // Can fly
        PLAYER      // Controlled by player, able to use player powers
    };

    /// <summary>
    /// Faction of the game unit
    /// </summary>
    /// <remarks>
    /// Determines which side the unit fights on and who it will attack.
    /// </remarks>
    public enum UnitFaction
    {
        ALLY,       // Fights for the player
        ENEMY      // Fights against player
    };

#endregion

    /// <summary>
    /// GameUnit encapsulates the logic specific to a game unit that moves and fights
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class GameUnit : GameEntity
    {
        // Constants
        public const int UNIT_SIZE = 40;

        #region Properties
        // Unit movement data
        public Vector2 Vel { get; set; }
        public Vector2 Target { get; set; }
        public Vector2 NextMove { get; set; }
        public int Accel { get; set; }

        // Unit type data
        public UnitType Type { get; set; }
        public UnitFaction Faction { get; set; }

        // Unit stat fields
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public int Range { get; set; }
        public int AttackSpeed { get; set; }
        #endregion

        #region Initialization
        public GameUnit(Texture2D texture, UnitType type, UnitFaction faction) : base(texture)
        {
            Type = type;
            Faction = faction;

            InitStats();
        }

        private void InitStats()
        {
            // TODO load stats from a config file
        }
        #endregion

        public bool HasTarget()
        {
            return Target.X >= 0 && Target.Y >= 0;
        }

        public void Draw(GameCanvas canvas)
        {
            // test
            canvas.DrawSprite(Texture, Color.White, Position);
        }
    }
}