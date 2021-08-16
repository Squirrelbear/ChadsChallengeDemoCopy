using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace GameDevMajor.Level_Code.LevelWnds {

    class WindowCollection {

        List<WndBase> theWindows;


        internal WindowCollection() {

            theWindows = new List<WndBase>();
        }

        internal void add(WndBase win){

            theWindows.Add(win);
        }

        internal void remove(WndBase win) {

            win = null;
            GC.Collect();
            theWindows.Remove(win);
        }

        internal void Draw(SpriteBatch sb) {

            foreach (WndBase win in theWindows) {

                if (win != null && win.isActive())
                    win.draw(sb);
            }
        }

        internal void Update(GameTime gt) {

            if (theWindows.Count == 0) return;

            foreach (WndBase win in theWindows) {

                if (win != null && win.isActive())
                    win.update(gt);
                else 
                    remove(win);
            }
        }
    }
}
