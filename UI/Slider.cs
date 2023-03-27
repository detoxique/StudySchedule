using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadroEngine.UI
{
    public class Slider : UI_Element, Drawable
    {
        private Vector2f DragPos = new Vector2f();
        private Vector2f MousePosition = new Vector2f();
        private bool Drag = false, mouseHover, mouseHoverSlider, drawValue = false;

        public SelectorType selectorType = SelectorType.Horizontal;
        public TextPosition TextPosition = TextPosition.OverScroll;

        public RectangleShape Background = new RectangleShape();
        public RectangleShape Selector = new RectangleShape();

        public Font font;
        public Text ValueText = new Text();
        public string AddText = "";

        public float Value, ValueFrom, ValueTo;

        public Slider()
        {
            Background.Position = Position;
            Selector.Position = Position;
        }

        public Slider(SelectorType selector, Vector2f position, Vector2f size)
        {
            selectorType = selector;

            Position = position;
            Size = size;

            Background.Position = Position;
            Background.FillColor = new Color(0, 0, 0, 50);

            Selector.Position = Position;
            Selector.Size = new Vector2f(15, 15);
            Selector.FillColor = new Color(0, 0, 0, 220);

            if (selectorType == SelectorType.Horizontal)
                Background.Size = new Vector2f(Size.X, 15);
            else
                Background.Size = new Vector2f(15, Size.Y);

        }

        public override void Update(float DeltaTime)
        {
            if (selectorType == SelectorType.Horizontal)
            {
                if (Drag)
                {
                    if (MousePosition.X - DragPos.X >= Background.Position.X && MousePosition.X - DragPos.X - 15 <= Background.Position.X + Size.X)
                        Selector.Position = new Vector2f(MousePosition.X - DragPos.X, Position.Y);
                    else if (MousePosition.X - DragPos.X < Background.Position.X)
                        Selector.Position = new Vector2f(Position.X, Position.Y);
                    else if (MousePosition.X - DragPos.X > Background.Position.X + Background.Size.X - 15)
                        Selector.Position = new Vector2f(Position.X + Size.X - 15, Position.Y);
                }
                else
                {
                    DragPos = new Vector2f(0, 0);
                }

                Value = Map(Selector.Position.X - Position.X, 0, Position.X + Size.X - 15, ValueFrom, ValueTo);
            }
            else
            {
                if (Drag)
                {
                    if (MousePosition.Y + DragPos.Y >= Position.Y && MousePosition.Y - DragPos.Y - 15 <= Position.Y + Size.Y)
                        Selector.Position = new Vector2f(MousePosition.X, Position.Y - DragPos.Y);
                    else if (MousePosition.Y + DragPos.Y < Position.Y)
                        Selector.Position = new Vector2f(Position.X, Position.Y);
                    else if (MousePosition.Y + DragPos.Y > Position.Y + Size.Y - 15)
                        Selector.Position = new Vector2f(Position.X, Position.Y + Size.Y - 15);
                }
                else
                {
                    DragPos = new Vector2f(0, 0);
                }

                Value = Map(Selector.Position.Y - Position.Y, 0, Position.Y + Size.Y - 15, ValueFrom, ValueTo);
            }
        }

        public override void MouseCheck(MouseMoveEventArgs e)
        {
            MousePosition = new Vector2f(e.X, e.Y);

            if (e.X >= Position.X && e.X <= Position.X + Size.X &&
                e.Y >= Position.Y && e.Y <= Position.Y + Size.Y)
            {
                mouseHover = true;
            }
            else
            {
                mouseHover = false;
            }

            if (e.X >= Selector.Position.X && e.X <= Selector.Position.X + Selector.Size.X &&
                e.Y >= Selector.Position.Y && e.Y <= Selector.Position.Y + Selector.Size.Y)
            {
                mouseHoverSlider = true;
                Selector.FillColor = new Color(0, 0, 0, 160);
            }
            else
            {
                mouseHoverSlider = false;
                Selector.FillColor = new Color(0, 0, 0, 220);
            }
        }

        public override void MouseClick(MouseButtonEventArgs e)
        {
            if (mouseHoverSlider && e.Button == Mouse.Button.Left)
            {
                Drag = true;
                DragPos = new Vector2f(e.X - Position.X, e.Y - Position.Y);
            }
        }

        public override void MouseRelease(MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                drawValue = false;
                Drag = false;
            }
        }

        public override void MouseWheel(float Delta)
        {
            if (mouseHover && Value + Delta <= ValueTo && Value + Delta >= ValueFrom)
                Value += Delta;
        }

        public override void Resized(SizeEventArgs e)
        {
            
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Background);
            target.Draw(Selector);
            if (drawValue)
                target.Draw(ValueText);
        }

        private float Map(float value, float From1, float From2, float To1, float To2)
        {
            return (value - From1) / (From2 - From1) * (To2 - To1) + To1;
        }
    }
}
