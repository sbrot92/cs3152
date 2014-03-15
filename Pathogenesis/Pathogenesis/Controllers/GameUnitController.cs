﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pathogenesis.Models;

namespace Pathogenesis
{
    /*
     * Controls all unit logic and management,
     * including movement, pathfinding, AI, and updating
     */ 
    public class GameUnitController
    {
        #region Constants
        public const int ENEMY_CHASE_RANGE = 300;   // Distance at which an enemy will start chasing the player
        #endregion

        // A list of all the units currently in the game
        public ArrayList Units { get; set; }

        // The player object
        public Player Player { get; set; }

        #region Initialization
        public GameUnitController()
        {
            Units = new ArrayList();
        }

        /*
         * Add a unit to the game
         */
        public void AddUnit(GameUnit unit)
        {
            Units.Add(unit);
        }
        #endregion

        #region Update
        /*
         * Updates all game units
         */ 
        public void Update(Level level)
        {
            // Set the next move for each unit
            foreach (GameUnit unit in Units)
            {
                setNextMove(unit, level);
            }

            // Apply the next move for each unit
            foreach (GameUnit unit in Units)
            {
                moveUnit(unit);
            }
        }
        #endregion

        #region Movement and Pathfinding

        /*
         * Determine the next move for this unit with
         * targeting specific to each unit type AI
         */ 
        public void setNextMove(GameUnit unit, Level level)
        {
            // Select target
            if (Player != null && Player.Exists && Player.inRange(unit, ENEMY_CHASE_RANGE))
            {
                switch (unit.Type)
                {
                    case UnitType.TANK:
                        // tank AI
                        break;
                    case UnitType.RANGED:
                        // ranged AI
                        break;
                    case UnitType.FLYING:
                        // flying AI
                        break;
                    default:
                        // Player case, do nothing
                        break;
                }
            }
            else
            {
                // Random walk
                //unit.Target
                
            }

            if(unit.HasTarget())
            {
                // Calculate direction of acceleration
                Vector2 vel = unit.Vel;
                float x_mod = unit.Accel * (unit.Target.X - unit.Position.X) > 0? 1 : -1;
                float y_mod = unit.Accel * (unit.Target.Y - unit.Position.Y) > 0? 1 : -1;
                vel += new Vector2(x_mod, y_mod);

                // Clamp values to max speeds
                vel.X = MathHelper.Clamp(vel.X, -unit.Speed, unit.Speed);
                vel.Y = MathHelper.Clamp(vel.Y, -unit.Speed, unit.Speed);
                unit.Vel = vel;
            }   
        }

        /*
         * Move this unit according to its current velocity vector
         */ 
        private void moveUnit(GameUnit unit)
        {
            unit.Position += unit.Vel;
        }

        #endregion
    }
}