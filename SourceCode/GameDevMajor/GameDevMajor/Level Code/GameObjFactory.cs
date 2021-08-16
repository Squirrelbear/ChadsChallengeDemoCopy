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
using System.Collections;
using System.Diagnostics;

namespace GameDevMajor
{
    public class GameObjFactory
    {
        protected Level lvl;

        public GameObjFactory(Level level)
        {
            lvl = level;
        }

        public GameObject createObject(BlockSave block)
        {
            switch (block.blockType)
            {
                case Block.BlockType.Wall:
                    return new Blocks.WallBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Invisible:
                    return new Blocks.InvisibleBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Dissapearing:
                    return new Blocks.DissapearingBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Tile:
                    return new Blocks.TileBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Ice:
                    return new Blocks.IceBlock(block.position, block.facing, lvl);
                case Block.BlockType.Fire:
                    return new Blocks.FireBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Theif:
                    return new Blocks.TheifBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Gravel:
                    return new Blocks.GravelBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Water:
                    return new Blocks.WaterBlock(block.position, block.facing, lvl);
                case Block.BlockType.Guard:
                    return new Blocks.GuardBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Door:
                    return new Blocks.DoorBlock(block.position, block.facing, lvl);
                case Block.BlockType.Key:
                    return new Blocks.KeyBlock(block.position, block.facing, lvl);
                case Block.BlockType.Item:
                    return new Blocks.ItemBlock(block.position, block.facing, lvl);
                case Block.BlockType.Coin:
                    return new Blocks.CoinBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Speed:
                    return new Blocks.SpeedBlock(block.position, block.facing, lvl);
                case Block.BlockType.Red:
                    return new Blocks.RedBlock(block.position, block.facing, lvl);
                case Block.BlockType.Yellow:
                    return new Blocks.YellowBlock(block.position, block.facing, lvl);
                case Block.BlockType.Green:
                    return new Blocks.GreenBlock(block.position, block.facing, lvl);
                case Block.BlockType.Blue:
                    return new Blocks.BlueBlock(block.position, block.facing, lvl);
                case Block.BlockType.RedButton:
                    return new Blocks.RedButtonBlock(block.position, lvl);
                case Block.BlockType.YellowButton:
                    return new Blocks.YellowButtonBlock(block.position, lvl);
                case Block.BlockType.GreenButton:
                    return new Blocks.GreenButtonBlock(block.position, lvl);
                case Block.BlockType.BlueButton:
                    return new Blocks.BlueButtonBlock(block.position, lvl);


                    //Armor = 26, Bomb = 27, Bow = 28, Spawner = 29, Trap = 30, Win = 31
                case Block.BlockType.Portal:
                    return new Blocks.PortalBlock(block.position, GameObject.Direction.Up, (Vector2)block.variableData[0], lvl);
                case Block.BlockType.Armor:
                    return new Blocks.ArmorBlock(block.position, block.facing, lvl);
                case Block.BlockType.Bomb:
                    return new Blocks.BombBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.Bow:
                    return new Blocks.BowBlock(block.position, block.facing, lvl);
                case Block.BlockType.Spawner:
                    return new Blocks.SpawnerBlock(block.position, block.facing, lvl);
                case Block.BlockType.Trap:
                    return new Blocks.TrapBlock(block.position, block.facing, lvl);
                case Block.BlockType.Win:
                    return new Blocks.WinBlock(block.position, GameObject.Direction.Up, lvl);
                case Block.BlockType.FallingWall:
                    return new Blocks.FallingWallBlock(block.position, GameObject.Direction.Up, lvl);

                case Block.BlockType.Toggle:
                    Debug.Print("WARNING: Block Block.BlockType.Toggle used. This item is deprecated!!!");
                    break;
                case Block.BlockType.Button:
                    return new Blocks.ButtonBlock(block.position, (Vector2)block.variableData[0], lvl);
                default:
                    Debug.Assert(false, "Error, a block of type " + block.blockType + " could not be created!\nData: " + block.ToString());
                    break;
            }

            return null;
        }

        public GameObject createObject(MonsterSave monster)
        {
            switch (monster.monsterType)
            {
                case Monster.MonsterType.Assassin:
                    return new Monsters.AssassinMonster(monster.position, monster.facing, lvl);
                case Monster.MonsterType.Armor:
                    return new Monsters.ArmorMonster(monster.position, monster.facing, lvl);
                case Monster.MonsterType.Ice:
                    return new Monsters.IceMonster(monster.position, monster.facing, lvl);
                case Monster.MonsterType.Imp:
                    return new Monsters.ImpMonster(monster.position, monster.facing, lvl);
                case Monster.MonsterType.PatrolMonster:
                    return new Monsters.PatrolMonster(monster.position, monster.facing, monster.variableData, lvl); 
                case Monster.MonsterType.Rat:
                    return new Monsters.RatMonster(monster.position, monster.facing, lvl);
                case Monster.MonsterType.Moveable:
                    return new Monsters.WallMonster(monster.position, monster.facing, lvl);
                case Monster.MonsterType.Arrow:
                    return new Monsters.ArrowMonster(monster.position, monster.facing, lvl);
                default:
                    Debug.Assert(false, "Error, a monster of type " + monster.monsterType + " could not be created!\nData: " + monster.ToString());
                    break;
            }

            return null;
        }
    }
}
