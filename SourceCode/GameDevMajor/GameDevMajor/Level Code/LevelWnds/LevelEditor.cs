using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GameDevMajor {

    class LevelEditor : WndBase {

        enum EditorMode { Block, Monster, SetTime, SetName };

        Game1 theGame;
        EditorMode mode;
        Boolean enteringName;

        Rectangle blockRegion, blockSourceRegion, blockFacingRegion, dashRegion, monsterRegion, monsterFacingRegion, monsterSourceRegion;

        List<Texture2D> blockSprites, facingSprites, monsterSprites;

        Texture2D wallSprites, invisibleSprites, dissapearingSprites, tileSprites, iceSprites,
             fireSprites, GravelSprites, thiefSprites, guardSprites, waterSprites, bArmourSprites,
             doorSprites, keySprites, itemSprites, coinSprites, speedSprites, greenSprites,
             blueSprites, yellowSprites, redSprites, blueButtonSprites, greenButtonSprites, redButtonSprites,
             yellowButtonSprites, bowSprites, portalSprites, exitSprites, trapSprites, sewerSprites, bombSprites, fallingWallSprites;

        Texture2D editor, theTile, theMonster, facingArrow, dashedLine;

        Texture2D armourSprites, patrolSprites, impSprites, assassinSprites, ratSprites, movingWallSprites, movingIceSprites;

        Keys[] keysToCheck = new Keys[] { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
                                        Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
                                        Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
                                        Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
                                        Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
                                        Keys.Z, Keys.Back, Keys.Space };

        //time to complete level
        int time = 0;
        String levelName;
        Block.BlockType blockType;
        Monster.MonsterType monsterType;
        GameObject.Direction blockFacing, monsterFacing;

        internal LevelEditor(Game1 game, Level level, ContentManager content)
            : base(level, level.getGameRef(), content) {

            theGame = game;
            theLevel = level;
            theContentManager = content;

            init();
            loadContent();
        }

        void init() {

            mode = EditorMode.Block;
            blockFacing = GameObject.Direction.Up;
            monsterFacing = GameObject.Direction.Up;
            blockType = Block.BlockType.Wall;
            monsterType = Monster.MonsterType.Armor;

            isVisible = true;
            enteringName = false;
            levelName = theLevel.getLevelInfo().levelName;
            time = theLevel.getTimeLeft();

            calculateDrawRegions();
        }

        //calculates the draw regions for each asset
        void calculateDrawRegions() {

            setLinePosition(.035f);

            drawRegion = new Rectangle(
                (int)(screenRes.X * .732f),
                0,
                (int)(screenRes.X * .27f),
                (int)(screenRes.Y));

            blockRegion = new Rectangle(
                (int)(screenRes.X * .835f),
                (int)(screenRes.Y * .065f),
                (int)(screenRes.X * .0582f),
                (int)(screenRes.Y * .142f));

            blockFacingRegion = new Rectangle(
                (int)(screenRes.X * .915f),
                (int)(screenRes.Y * .14f),
                (int)(screenRes.X * .02f),
                (int)(screenRes.Y * .04f));

            blockSourceRegion = new Rectangle(0, 0, 101, 171);

            monsterRegion = new Rectangle(
                (int)(screenRes.X * .825f),
                (int)(screenRes.Y * .3f),
                (int)(screenRes.X * .08f),
                (int)(screenRes.X * .08f));

            monsterFacingRegion = new Rectangle(
                (int)(screenRes.X * .915f),
                (int)(screenRes.Y * .34f),
                (int)(screenRes.X * .02f),
                (int)(screenRes.Y * .04f));

            monsterSourceRegion = new Rectangle(0, 0, 96, 96);
        }

        void loadContent() {

            editor = theContentManager.Load<Texture2D>("./LevelEditor/editor2");

            ///blocks
            wallSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/wall");
            invisibleSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/invis");
            dissapearingSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/disapearing");
            tileSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/tile");
            iceSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/ice");
            fireSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/fire");
            GravelSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/gravel");
            thiefSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/thief");
            guardSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/guard");
            waterSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/water");
            doorSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/bigDoors");
            keySprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/keys");
            itemSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/TItems");
            coinSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/coinTile");
            speedSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/bigSpeed");
            redSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/red");
            yellowSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/yellow");
            greenSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/green");
            blueSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/blue");
            blueButtonSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/LblueButton");
            greenButtonSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/LgreenButton");
            yellowButtonSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/LyellowButton");
            redButtonSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/LredButton");
            bowSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/bow");
            portalSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/portal");
            exitSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/exit");
            trapSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/trap");
            sewerSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/sewer");
            bombSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/bomb");
            bArmourSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/bArmour");
            fallingWallSprites = theContentManager.Load<Texture2D>("./LevelEditor/Blocks/fallingwall");

            //facing arrows
            facingSprites = new List<Texture2D>();
            facingSprites.Add(theContentManager.Load<Texture2D>("./LevelEditor/Facing/facingNorth"));
            facingSprites.Add(theContentManager.Load<Texture2D>("./LevelEditor/Facing/facingEast"));
            facingSprites.Add(theContentManager.Load<Texture2D>("./LevelEditor/Facing/facingSouth"));
            facingSprites.Add(theContentManager.Load<Texture2D>("./LevelEditor/Facing/facingWest"));
            facingSprites.Add(theContentManager.Load<Texture2D>("./LevelEditor/Facing/facingNeutral"));
            //

            dashedLine = theContentManager.Load<Texture2D>("./LevelEditor/dash");

            blockSprites = new List<Texture2D>();
            blockSprites.Add(wallSprites);
            blockSprites.Add(invisibleSprites);
            blockSprites.Add(dissapearingSprites);
            blockSprites.Add(tileSprites);
            blockSprites.Add(iceSprites);
            blockSprites.Add(fireSprites);
            blockSprites.Add(GravelSprites);
            blockSprites.Add(thiefSprites);
            blockSprites.Add(waterSprites);
            blockSprites.Add(guardSprites);
            blockSprites.Add(doorSprites);
            blockSprites.Add(keySprites);
            blockSprites.Add(itemSprites);
            blockSprites.Add(coinSprites);
            blockSprites.Add(speedSprites);
            blockSprites.Add(redSprites);
            blockSprites.Add(yellowSprites);
            blockSprites.Add(greenSprites);
            blockSprites.Add(blueSprites);
            blockSprites.Add(redButtonSprites);
            blockSprites.Add(yellowButtonSprites);
            blockSprites.Add(greenButtonSprites);
            blockSprites.Add(blueButtonSprites);
            blockSprites.Add(portalSprites);
            blockSprites.Add(bArmourSprites); 
            blockSprites.Add(bombSprites);
            blockSprites.Add(bowSprites);
            blockSprites.Add(sewerSprites);
            blockSprites.Add(trapSprites);
            blockSprites.Add(exitSprites);
            blockSprites.Add(fallingWallSprites);

            armourSprites = theContentManager.Load<Texture2D>("./LevelEditor/Monsters/eArmour");
            patrolSprites = theContentManager.Load<Texture2D>("./LevelEditor/Monsters/ePatrol");
            impSprites = theContentManager.Load<Texture2D>("./LevelEditor/Monsters/eImp");
            assassinSprites = theContentManager.Load<Texture2D>("./LevelEditor/Monsters/eAssassin");
            ratSprites = theContentManager.Load<Texture2D>("./LevelEditor/Monsters/eRat");
            movingIceSprites = theContentManager.Load<Texture2D>("./LevelEditor/Monsters/MovingIce");
            movingWallSprites = theContentManager.Load<Texture2D>("./LevelEditor/Monsters/movingwall");

            monsterSprites = new List<Texture2D>();
            monsterSprites.Add(armourSprites);
            monsterSprites.Add(patrolSprites);
            monsterSprites.Add(impSprites);
            monsterSprites.Add(assassinSprites);
            monsterSprites.Add(ratSprites);
            monsterSprites.Add(movingWallSprites); //moving wall
            monsterSprites.Add(movingIceSprites); //moving ice
        }

        public override void draw(SpriteBatch sb) {

            if (isVisible) {

                theTile = blockSprites.ElementAt((int)blockType - 2);
                theMonster = monsterSprites.ElementAt((int)monsterType - 1);
                facingArrow = facingSprites.ElementAt((int)blockFacing);

                sb.Draw(editor, drawRegion, Color.White);
                sb.Draw(dashedLine, dashRegion, Color.White);

                sb.Draw(theTile, blockRegion, blockSourceRegion, Color.White);
                sb.Draw(facingArrow, blockFacingRegion, Color.White);

                facingArrow = facingSprites.ElementAt((int)monsterFacing);
                sb.Draw(theMonster, monsterRegion, monsterSourceRegion, Color.White);
                sb.Draw(facingArrow, monsterFacingRegion, Color.White);

                myDrawString(sb, "" + time, getTimeRegion(), .51f);
                myDrawString(sb, "" + levelName, getNamePosition(), .7f);
            }
        }

        public override void update(GameTime gt) {

            if (isVisible) {

                kbState = theLevel.getKeyboardState();
                gpState = theLevel.getGamePadState();

                if (enteringName) {

                    foreach (Keys key in keysToCheck) {

                        if (checkKey(key)) {

                            AddCharToName(key);
                            break;
                        }
                    }
                }

                if ((checkKey(Keys.D) || checkPad(Buttons.DPadRight)) && !enteringName) {

                    if (mode == EditorMode.Block)
                        nextBlock();

                    if (mode == EditorMode.Monster)
                        nextMonster();

                    if (mode == EditorMode.SetTime)
                        updateTime(5);
                }
                if ((checkKey(Keys.A) || checkPad(Buttons.DPadLeft)) && !enteringName) {

                    if (mode == EditorMode.Block)
                        prevBlock();

                    if (mode == EditorMode.Monster)
                        prevMonster();

                    if (mode == EditorMode.SetTime)
                        updateTime(-5);
                }

                if ((checkKey(Keys.Q) || checkPad(Buttons.LeftShoulder)) && !enteringName) {

                    if (mode == EditorMode.Block)
                        prevBlockFacing();

                    if (mode == EditorMode.Monster)
                        prevMonsterFacing();
                }

                if ((checkKey(Keys.E) || checkPad(Buttons.RightShoulder)) && !enteringName) {

                    if (mode == EditorMode.Block)
                        nextBlockFacing();

                    if (mode == EditorMode.Monster)
                        nextMonsterFacing();
                }

                if ((checkKey(Keys.W) || checkPad(Buttons.DPadUp)) && !enteringName)
                    moveUp();

                if ((checkKey(Keys.S) || checkPad(Buttons.DPadDown)) && !enteringName)
                    moveDown();

                if (checkKey(Keys.Enter) && mode == EditorMode.SetName) {
                    if (enteringName) {
                        theLevel.setName(levelName);
                    }
                    enteringName = !enteringName;
                }



            }

            oldKbState = kbState;
            oldGPstate = gpState;
        }


        void nextBlock() {

            if (blockType == Block.BlockType.FallingWall)
                blockType = Block.BlockType.Wall;
            else
                blockType++;

            blockFacing = GameObject.Direction.Up;
            updateBlock(blockFacing);
            blockSourceRegion = new Rectangle(0, 0, 101, 171);
        }

        void prevBlock() {

            if (blockType == Block.BlockType.Wall)
                blockType = Block.BlockType.FallingWall;
            else
                blockType--;

            blockFacing = GameObject.Direction.Up;
            updateBlock(blockFacing);
            blockSourceRegion = new Rectangle(0, 0, 101, 171);
        }

        void nextBlockFacing() {

            switch (blockType) {
                case Block.BlockType.Ice:
                    if ((int)blockFacing + 1 > 4) blockFacing = GameObject.Direction.Up;
                    else blockFacing++;
                    break;
                case Block.BlockType.Door:
                case Block.BlockType.Key:
                case Block.BlockType.Speed:
                case Block.BlockType.Item:
                case Block.BlockType.Bow:
                case Block.BlockType.Spawner:
                case Block.BlockType.Armor:
                    if ((int)blockFacing + 1 > 3) blockFacing = GameObject.Direction.Up;
                    else blockFacing++;
                    break;
                case Block.BlockType.Blue:
                case Block.BlockType.Green:
                case Block.BlockType.Red:
                case Block.BlockType.Trap:
                case Block.BlockType.Yellow:
                    if ((int)blockFacing + 1 > 1) blockFacing = GameObject.Direction.Up;
                    else blockFacing++;
                    break;
            }

            if (blockType == Block.BlockType.Door ||
                blockType == Block.BlockType.Key ||
                blockType == Block.BlockType.Item ||
                blockType == Block.BlockType.Speed ||
                blockType == Block.BlockType.Ice ||
                blockType == Block.BlockType.Bow ||
                blockType == Block.BlockType.Spawner ||
                blockType == Block.BlockType.Armor ||
                blockType == Block.BlockType.Blue ||
                blockType == Block.BlockType.Green ||
                blockType == Block.BlockType.Red ||
                blockType == Block.BlockType.Trap ||
                blockType == Block.BlockType.Yellow)
                updateBlock(blockFacing);

            else
                blockSourceRegion = new Rectangle(0, 0, 101, 171);
        }

        void prevBlockFacing() {

            switch (blockType) {
                case Block.BlockType.Ice:
                    if ((int)blockFacing - 1 < 0) blockFacing = GameObject.Direction.Neutral;
                    else blockFacing--;
                    break;

                case Block.BlockType.Door:
                case Block.BlockType.Key:
                case Block.BlockType.Speed:
                case Block.BlockType.Item:
                case Block.BlockType.Bow:
                case Block.BlockType.Spawner:
                case Block.BlockType.Armor:

                    if ((int)blockFacing - 1 < 0) blockFacing = GameObject.Direction.Left;
                    else blockFacing--;
                    break;

                case Block.BlockType.Blue:
                case Block.BlockType.Green:
                case Block.BlockType.Red:
                case Block.BlockType.Trap:
                case Block.BlockType.Yellow:

                    if ((int)blockFacing - 1 < 0) blockFacing = GameObject.Direction.Right;
                    else blockFacing--;
                    break;
            }


            if (blockType == Block.BlockType.Door ||
                blockType == Block.BlockType.Key ||
                blockType == Block.BlockType.Item ||
                blockType == Block.BlockType.Speed ||
                blockType == Block.BlockType.Ice ||
                blockType == Block.BlockType.Bow ||
                blockType == Block.BlockType.Spawner ||
                blockType == Block.BlockType.Armor ||
                blockType == Block.BlockType.Blue ||
                blockType == Block.BlockType.Green ||
                blockType == Block.BlockType.Red ||
                blockType == Block.BlockType.Trap ||
                blockType == Block.BlockType.Yellow)
                updateBlock(blockFacing);

            else
                blockSourceRegion = new Rectangle(0, 0, 101, 171);
        }

        //changes the sprite if required
        //i.e. different facing = different 
        //coloured keys and doors
        void updateBlock(GameObject.Direction facing) {

            switch (facing) {
                case GameObject.Direction.Up:
                    blockSourceRegion = new Rectangle(0, 0, 101, 171);
                    break;
                case GameObject.Direction.Right:
                    blockSourceRegion = new Rectangle(101, 0, 101, 171);
                    break;
                case GameObject.Direction.Down:
                    blockSourceRegion = new Rectangle(202, 0, 101, 171);
                    break;
                case GameObject.Direction.Left:
                    blockSourceRegion = new Rectangle(303, 0, 101, 171);
                    break;
                default:
                    blockSourceRegion = new Rectangle(0, 0, 101, 171);
                    break;
            }

            if (mode == EditorMode.Block && blockFacing == GameObject.Direction.Neutral)
            {
                blockSourceRegion = new Rectangle(404, 0, 101, 171);
            }

        }


        void nextMonster() {

            if (monsterType == Monster.MonsterType.Ice)
                monsterType = Monster.MonsterType.Armor;
            else
                monsterType++;

            if (monsterType == Monster.MonsterType.Moveable || monsterType == Monster.MonsterType.Ice)
            {
                monsterFacing = GameObject.Direction.Up;
                updateMonster(monsterFacing);
            }
        }

        void prevMonster() {

            if (monsterType == Monster.MonsterType.Armor)
                monsterType = Monster.MonsterType.Ice;
            else
                monsterType--;

            if (monsterType == Monster.MonsterType.Moveable || monsterType == Monster.MonsterType.Ice)
            {
                monsterFacing = GameObject.Direction.Up;
                updateMonster(monsterFacing);
            }
        }

        void nextMonsterFacing() {

            if (monsterFacing == GameObject.Direction.Left)
                monsterFacing = GameObject.Direction.Up;
            else
                monsterFacing++;

            if (monsterType == Monster.MonsterType.Moveable || monsterType == Monster.MonsterType.Ice)
                monsterFacing = GameObject.Direction.Up;

            updateMonster(monsterFacing);
        }

        void prevMonsterFacing() {

            if (monsterFacing == GameObject.Direction.Up)
                monsterFacing = GameObject.Direction.Left;
            else
                monsterFacing--;

            if (monsterType == Monster.MonsterType.Moveable || monsterType == Monster.MonsterType.Ice)
                monsterFacing = GameObject.Direction.Up;

            updateMonster(monsterFacing);
        }

        void updateMonster(GameObject.Direction facing) {

            switch (facing) {
                case GameObject.Direction.Up:
                    monsterSourceRegion = new Rectangle(0, 0, 96, 96);
                    break;
                case GameObject.Direction.Right:
                    monsterSourceRegion = new Rectangle(96, 0, 96, 96);
                    break;
                case GameObject.Direction.Down:
                    monsterSourceRegion = new Rectangle(192, 0, 96, 96);
                    break;
                case GameObject.Direction.Left:
                    monsterSourceRegion = new Rectangle(288, 0, 96, 96);
                    break;
                default:
                    monsterSourceRegion = new Rectangle(0, 0, 96, 96);
                    break;
            }

        }

        void moveDown() {

            if (mode == EditorMode.Block) {
                setLinePosition(0.255f);
                mode = EditorMode.Monster;
            } else if (mode == EditorMode.Monster) {
                setLinePosition(.43f);
                mode = EditorMode.SetTime;
            } else if (mode == EditorMode.SetTime) {
                setLinePosition(0.63f);
                mode = EditorMode.SetName;
            } else if (mode == EditorMode.SetName) {
                setLinePosition(0.035f);
                mode = EditorMode.Block;
                enteringName = false;
            }
        }

        void moveUp() {

            if (mode == EditorMode.Block) {
                setLinePosition(0.63f);
                mode = EditorMode.SetName;
            } else if (mode == EditorMode.Monster) {
                setLinePosition(0.035f);
                mode = EditorMode.Block;
            } else if (mode == EditorMode.SetTime) {
                setLinePosition(0.255f);
                mode = EditorMode.Monster;
            } else if (mode == EditorMode.SetName) {
                setLinePosition(0.43f);
                mode = EditorMode.SetTime;
                enteringName = false;
            }
        }

        //sets the y position of the dashed line
        void setLinePosition(float newY) {

            dashRegion = new Rectangle(

                (int)(screenRes.X * .76f),
                (int)(screenRes.Y * newY),
                (int)(screenRes.X * .2f),
                (int)(screenRes.Y * .06f)
                );
        }

        internal BlockSave getSelectedBlock() {

            return new BlockSave(blockType, new Vector2(0, 0), blockFacing);
        }

        internal MonsterSave getSelectedMonster() {

            return new MonsterSave(monsterType, new Vector2(0, 0), monsterFacing);
        }

        internal GameObject.Type getPlacementType() {

            switch (mode) {
                case EditorMode.Block:
                    return GameObject.Type.Block;
                case EditorMode.Monster:
                    return GameObject.Type.Monster;
                default: throw new FormatException("Spacebar pressed when not on block or monster?");
            }
        }

        //returns the time to complete the level 
        internal int getTime() {
            return time;
        }

        //returns the level name
        internal string getName() {
            return levelName;
        }

        void updateTime(int difference) {
            time += difference;
            if (time < 0)
                time = 0;

            theLevel.setCountDown(time);
        }

        float getTimeRegion() {

            if (time < 10)
                return .45f;
            else if (time < 100)
                return .42f;
            else
                return .39f;
        }

        float getNamePosition() {

            return .45f - (levelName.Length * .0185f);
        }

        void AddCharToName(Keys key) {

            string newChar = "";

            if (levelName.Length >= 20 && key != Keys.Back)
                return;

            switch (key) {
                case Keys.A:
                    newChar += "a";
                    break;
                case Keys.B:
                    newChar += "b";
                    break;
                case Keys.C:
                    newChar += "c";
                    break;
                case Keys.D:
                    newChar += "d";
                    break;
                case Keys.E:
                    newChar += "e";
                    break;
                case Keys.F:
                    newChar += "f";
                    break;
                case Keys.G:
                    newChar += "g";
                    break;
                case Keys.H:
                    newChar += "h";
                    break;
                case Keys.I:
                    newChar += "i";
                    break;
                case Keys.J:
                    newChar += "j";
                    break;
                case Keys.K:
                    newChar += "k";
                    break;
                case Keys.L:
                    newChar += "l";
                    break;
                case Keys.M:
                    newChar += "m";
                    break;
                case Keys.N:
                    newChar += "n";
                    break;
                case Keys.O:
                    newChar += "o";
                    break;
                case Keys.P:
                    newChar += "p";
                    break;
                case Keys.Q:
                    newChar += "q";
                    break;
                case Keys.R:
                    newChar += "r";
                    break;
                case Keys.S:
                    newChar += "s";
                    break;
                case Keys.T:
                    newChar += "t";
                    break;
                case Keys.U:
                    newChar += "u";
                    break;
                case Keys.V:
                    newChar += "v";
                    break;
                case Keys.W:
                    newChar += "w";
                    break;
                case Keys.X:
                    newChar += "x";
                    break;
                case Keys.Y:
                    newChar += "y";
                    break;
                case Keys.Z:
                    newChar += "z";
                    break;
                case Keys.Space:
                    newChar += " ";
                    break;
                case Keys.Back:
                    if (levelName.Length != 0)
                        levelName = levelName.Remove(levelName.Length - 1);
                    return;
            }

            if (kbState.IsKeyDown(Keys.RightShift) || kbState.IsKeyDown(Keys.LeftShift))
                newChar = newChar.ToUpper();

            levelName += newChar;
        }

        public bool allowCursorMove() {
            return (mode == EditorMode.Monster || mode == EditorMode.Block);
        }

        public bool allowCursorInput() {
            return (mode == EditorMode.Monster || mode == EditorMode.Block);
        }
    }
}
