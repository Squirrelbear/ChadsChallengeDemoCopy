using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameDevMajor
{
    internal class ButtonListView : ButtonCollection
    {
        int maxInViewCount;
        Vector2 shiftAmount;
        Vector2 listTop;
        int top;

        internal ButtonListView(ContentManager theContentManager, int maxInViewCount, Vector2 listTop, Vector2 shiftAmount)
            : base(theContentManager)
        {
            this.maxInViewCount = maxInViewCount;
            this.shiftAmount = shiftAmount;
            this.listTop = listTop;
            top = 0;
        }

        internal override void add(Button button)
        {
            if (buttonList.Count >= maxInViewCount) button.setVisible(false);
            base.add(button);           
        }

        internal override void next()
        {
            base.next();

            if (selectedIndex > top + maxInViewCount - 1)
            {
                top++;
            }
            else if(selectedIndex == 0)
            {
                top = 0;
            }

            for (int i = 0; i < buttonList.Count; i++)
            {
                int mod = i - top;
                buttonList[i].setLocation(listTop + shiftAmount * mod);

                if (i >= top && i < top + maxInViewCount)
                {
                    buttonList[i].setVisible(true);
                }
                else
                {
                    buttonList[i].setVisible(false);
                }
            }
        }

        internal override void previous()
        {
            base.previous();

            if (selectedIndex < top && top > 0)
            {
                top--;
            }
            else if (top == 0)
            {
                top = buttonList.Count - maxInViewCount;

                if (top < 0) top = 0;
            }

            for (int i = 0; i < buttonList.Count; i++)
            {
                int mod = i - top;
                buttonList[i].setLocation(listTop + shiftAmount * mod);

                if (i >= top && i < top + maxInViewCount)
                {
                    buttonList[i].setVisible(true);
                }
                else
                {
                    buttonList[i].setVisible(false);
                }
            }
        }
    }
}
