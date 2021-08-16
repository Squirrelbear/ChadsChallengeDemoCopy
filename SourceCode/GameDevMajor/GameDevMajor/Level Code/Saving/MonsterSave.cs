using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace GameDevMajor
{
    [Serializable]
    public class MonsterSave
    {
        /// <summary>
        /// The type of monster.
        /// </summary>
        public Monster.MonsterType monsterType;

        /// <summary>
        /// Where the monster is supposed to be located.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// The direction/state of the monster.
        /// </summary>
        public GameObject.Direction facing;

        /// <summary>
        /// Additional parameters that may be filled for this save.
        /// </summary>
        public ArrayList variableData;

        public MonsterSave()
        {
            monsterType = 0;
            position = Vector2.Zero;
            facing = 0;
            variableData = new ArrayList();
        }

        public MonsterSave(Monster.MonsterType monsterType, Vector2 position, GameObject.Direction facing)
        {
            this.monsterType = monsterType;
            this.position = position;
            this.facing = facing;
        }
    }
}
