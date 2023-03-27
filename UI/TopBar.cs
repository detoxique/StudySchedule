using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;
using System.Xml.Linq;

namespace QuadroEngine.UI
{
    public class TopBar : UI_Element, Drawable
    {
        public RectangleShape Background;
        public List<MainButton> Elements;
        public float Height { get; set; }
        public TopBar(float ScreenWidth, float height)
        {
            Background = new RectangleShape(new Vector2f(ScreenWidth, 25));
            Elements = new List<MainButton>();
            Background.FillColor = new Color(0, 0, 0, 200);
            Background.Position = new Vector2f(0, 0);
            Height = height;
        }

        public void AddItem(Text Name, float Width, List<Button> Buttons)
        {
            float sizesX = 0;

            for (int i = 0; i < Elements.Count; i++)
                sizesX += Elements[i].Tray.Size.X;

            Button button = new Button(new Vector2f(sizesX, 0), Name, new Vector2f(Width, Height));

            List<Button> bs = new List<Button>();
            foreach (Button b in Buttons)
                bs.Add(b);

            for (int i = 0; i < bs.Count; i++)
            {
                bs[i].Position = new Vector2f(sizesX, (i + 1) * Height);
                //bs[i].Label.DisplayedString = sizesX.ToString();
                bs[i].Size = new Vector2f(Width, Height);
                bs[i].IsActive = false;
                bs[i].IsDraw = false;
            }

            Elements.Add(new MainButton(button, bs));
        }

        public override void Update(float DeltaTime)
        {
            foreach (MainButton b in Elements)
            {
                b.Tray.Update(DeltaTime);

                for (int i = 0; i < b.Buttons.Count; i++)
                {
                    //b.Buttons[i].Position = 
                    b.Buttons[i].Update(DeltaTime);
                }
            }

            foreach (MainButton b in Elements)
                foreach (Button b2 in b.Buttons)
                    b2.Update(DeltaTime);
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            
        }

        public override void MouseCheck(MouseMoveEventArgs e)
        {
            foreach (MainButton b in Elements)
                b.Tray.MouseCheck(e);

            foreach (MainButton b in Elements)
                foreach (Button b2 in b.Buttons)
                    b2.MouseCheck(e);

            foreach (MainButton b in Elements)
            {
                float sizesY = Height * b.Buttons.Count + Height;

                if (b.Tray.MouseHover)
                    b.Hover = true;

                foreach (Button button in b.Buttons)
                {
                    if (!b.Tray.MouseHover && button.MouseHover)
                    {
                        b.Hover = true;
                        break;
                    }
                    else if (!b.Tray.MouseHover && !button.MouseHover)
                    {
                        b.Hover = false;
                    }
                }

                if (b.Hover)
                {
                    foreach (Button button in b.Buttons)
                    {
                        button.IsActive = true;
                        button.IsDraw = true;
                    }
                }
                else
                {
                    foreach (Button button in b.Buttons)
                    {
                        button.IsActive = false;
                        button.IsDraw = false;
                    }
                }
            }
        }

        public override void MouseClick(MouseButtonEventArgs e)
        {
            foreach (MainButton b in Elements)
            {
                b.Tray.MouseClick(e);
                foreach (Button b2 in b.Buttons)
                    b2.MouseClick(e);
            }
        }

        public override void Resized(SizeEventArgs e)
        {
            Background.Size = new Vector2f(e.Width, Height);
        }

        public override void MouseRelease(MouseButtonEventArgs e)
        {
            foreach (MainButton b in Elements)
            {
                b.Tray.MouseRelease(e);
                foreach (Button b2 in b.Buttons)
                    b2.MouseRelease(e);
            }
        }

        public override void MouseWheel(float Delta)
        {
            foreach (MainButton b in Elements)
            {
                b.Tray.MouseWheel(Delta);
                foreach (Button b2 in b.Buttons)
                    b2.MouseWheel(Delta);
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Background, states);

            foreach (MainButton button in Elements)
            {
                target.Draw(button.Tray);
                foreach (Button b in button.Buttons)
                    target.Draw(b);
            }
        }
    }
    public class MainButton
    {
        public Button Tray;
        public List<Button> Buttons;
        public bool Hover = false;

        public MainButton(Button button, List<Button> buttons)
        {
            Tray = new Button(button.Position, button.Label, button.Size);
            Buttons = new List<Button>();
            foreach (Button b in buttons)
                Buttons.Add(b);
        }
    }
}