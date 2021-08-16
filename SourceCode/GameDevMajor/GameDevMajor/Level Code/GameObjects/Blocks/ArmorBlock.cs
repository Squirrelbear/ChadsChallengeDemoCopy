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

namespace GameDevMajor.Blocks
{
    class ArmorBlock : Block
    {
        public ArmorBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Armor), new ArrayList(), facing, BlockType.Armor, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 50));
            spritePosList.Add(new Vector2(0, 100));
            spritePosList.Add(new Vector2(0, 150));

            /*spritePosList.Add(new Vector2(300, 0));
            spritePosList.Add(new Vector2(300, 50));
            spritePosList.Add(new Vector2(300, 100));
            spritePosList.Add(new Vector2(300, 150));*/

            setCanEnterPlayerAll(false);
            setCanEnterMonsterAll(false);
        }
    }
}
