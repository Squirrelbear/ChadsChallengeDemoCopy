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
    class TheifBlock : Block
    {
        public TheifBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Theif), new ArrayList(), facing, BlockType.Theif, lvl)
        {

            spritePosList.Add(new Vector2(0, 0));

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(true);
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            if (obj.getType() == Type.Player)
                lvl.getPlayer().removeEquipment();

        }
    }
}

