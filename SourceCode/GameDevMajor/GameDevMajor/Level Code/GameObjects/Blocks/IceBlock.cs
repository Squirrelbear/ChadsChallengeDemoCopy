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
    class IceBlock : Block
    {
        public IceBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Ice), new ArrayList(), facing, BlockType.Ice, lvl)
        {
           
           spritePosList.Add(new Vector2(0, 0));

           spritePosList.Add(new Vector2(50, 0));
 
           spritePosList.Add(new Vector2(150, 0));

           spritePosList.Add(new Vector2(100, 0));

           spritePosList.Add(new Vector2(200, 0));

           if (facing == Direction.Neutral)
           {
               setCanEnterMonsterAll(true);
               setCanEnterPlayerAll(true);
           }
           else
           {
               bool[] enter = new bool[4];
               for (int i = 0; i < 4; i++)
               {
                   if (i == (int)lvl.getFlippedDirection(facing) || ((int)facing == 3 && i == 0) || i == (int)facing + 1)
                   {
                       enter[i] = true;
                   }
                   else
                   {
                       enter[i] = false;
                   }
               }

               setCanEnterPlayer(enter);
               setCanEnterMonster(enter);
           }
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            if (obj.getType() == Type.Player)
                if (lvl.getPlayer().hasItem(ItemCode.IceBoots))
                    return;

            if (facing == Direction.Neutral)
            {
                //Debug.Print("Facing is: " + obj.getFacing());
                //Debug.Assert(false);
                if (!lvl.moveObject(obj, obj.getFacing()))
                {
                    Direction nDir = lvl.getFlippedDirection(obj.getFacing());
                    obj.setFacing(nDir);
                    lvl.moveObject(obj, nDir);
                }
                //obj.setFacing(obj.getFacing());
                return;
            }

            int outDir = (int)obj.getFacing();

            if (obj.getFacing() == facing)
            {
                    outDir++;
                    if (outDir > 3)
                        outDir = 0;
            }
            else
            {
                    outDir--;
                    if (outDir < 0)
                        outDir = 3;
            }
            
            Direction newDir = (Direction)outDir;
            //Vector2 newPos = lvl.getMoveFromDirection(obj, newDir);

            Direction oldfacing = obj.getFacing() ;
            if(facing != Direction.Neutral)
                obj.setFacing(newDir);

            if (!lvl.moveObject(obj, newDir))
            {
                newDir = lvl.getFlippedDirection(oldfacing);
                obj.setFacing(newDir);
                lvl.moveObject(obj, newDir);
            }
            
        }
    }
}

