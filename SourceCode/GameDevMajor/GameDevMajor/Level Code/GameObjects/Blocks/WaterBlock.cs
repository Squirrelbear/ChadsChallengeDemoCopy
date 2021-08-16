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

namespace GameDevMajor.Blocks
{
    class WaterBlock : Block
    {
        public WaterBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Water), new ArrayList(), facing, BlockType.Water, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(false);
        }

        public override bool canEnter(GameObject obj, GameObject.Direction dir, int depth)
        {
                if (obj.getType() == Type.Monster)
                {
                    Monster monster = (Monster)obj;
                    if (monster.getMonsterType() == Monster.MonsterType.Ice ||
                        monster.getMonsterType() == Monster.MonsterType.Moveable ||
                        monster.getMonsterType() == Monster.MonsterType.Arrow)
                            return true;
                }

                return base.canEnter(obj, dir, 0);
        }

        public override bool canExit(GameObject obj, GameObject.Direction dir)
        {
            if (obj.getType() == Type.Monster && ((Monster)obj).getMonsterType() == Monster.MonsterType.Arrow) return true;
            
            return base.canExit(obj, dir);
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            switch (obj.getType())
            {
                case Type.Player:
                    if (!lvl.getPlayer().hasItem(ItemCode.Flippers)) {
                        lvl.failLevel("Flippers will stop you from drowning...");
                    }
                    break;
                case Type.Monster:
                    Monster monster = (Monster)obj;
                    if (monster.getMonsterType().Equals(Monster.MonsterType.Ice))
                    {
                        Debug.Print("Im here");
                        lvl.removeMonster(monster);

                        BlockSave newBlock = new BlockSave();
                        newBlock.blockType = BlockType.Ice;
                        newBlock.facing = Direction.Neutral;
                        newBlock.position = new Vector2(position.X, position.Y);
                        GameObject block = lvl.createBlock(newBlock);
                        lvl.setBlock(position, block);
                        break;
                    }
                    if (monster.getMonsterType().Equals(Monster.MonsterType.Moveable))
                    {
                        Debug.Print("Im here 2");
                        lvl.removeMonster(monster);

                        BlockSave newBlock = new BlockSave();
                        newBlock.blockType = BlockType.Tile;
                        newBlock.facing = Direction.Neutral;
                        newBlock.position = new Vector2(position.X, position.Y);
                        GameObject block = lvl.createBlock(newBlock);
                        lvl.setBlock(position, block);
                        break;
                    }
                    break;     
            }

        }
    }
}

