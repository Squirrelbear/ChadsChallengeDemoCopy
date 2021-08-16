using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevMajor {

    internal class ButtonCollection {

        protected List<Button> buttonList;
        SoundEffect clicked, changed;
        SoundEffectInstance iChanged, iClicked;
        protected int selectedIndex;

        internal ButtonCollection(ContentManager theContentManager) {

            buttonList = new List<Button>();

            changed = theContentManager.Load<SoundEffect>("./WndBaseContent/Buttons/sound1");
            iChanged = changed.CreateInstance();
            clicked = theContentManager.Load<SoundEffect>("./WndBaseContent/Buttons/sound2");
            iClicked = clicked.CreateInstance();
            selectedIndex = 0;
        }

        internal virtual void add(Button button) {

            buttonList.Add(button);
        }

        internal void remove(int x) {

            buttonList.RemoveAt(x);
        }

        internal virtual void next() {

            //do nothing if list only contains 1, or is empty
            if (buttonList.Count > 1 ) {
    
                iChanged.Play();

                for (int i = 0; i < buttonList.Count; i++) {

                    //find currently selected
                    if (buttonList[i].isSelected) {

                        //reset currently selected
                        buttonList[i].isSelected = false;

                        //select next in line
                        if (i + 1 < buttonList.Count) {

                            buttonList[i + 1].isSelected = true;
                            selectedIndex = i + 1;
                            return;

                            //if no next in line, start at beginning   
                        } else {

                            buttonList[0].isSelected = true;
                            selectedIndex = 0;
                            return;

                        }
                    }
                }
            }
        }

        internal virtual void previous() {


            if (buttonList.Count > 1) {

                iChanged.Play();

                for (int i = 0; i < buttonList.Count; i++) {

                    if (buttonList[i].isSelected) {

                        buttonList[i].isSelected = false;

                        if ((i - 1) >= 0) {

                            buttonList[i - 1].isSelected = true;
                            selectedIndex = i - 1;
                            return;

                        } else {

                            buttonList[buttonList.Count - 1].isSelected = true;
                            selectedIndex = buttonList.Count - 1;
                            return;

                        }
                    }
                }
            }
        }

        internal Button getSelected() {

            foreach (Button b in buttonList) {

                if (b.isSelected)

                    return b;
            }

            return null;
        }

        internal void draw(SpriteBatch spritebatch){

            foreach (Button b in buttonList) {

                b.Draw(spritebatch);
            }
        }

        //played when the button is selected..
        internal void playSelectedSound() {

            iClicked.Play();
        }
    }
}
