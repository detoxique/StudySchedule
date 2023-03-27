using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace QuadroEngine.UI
{
    public enum SelectorType
    {
        Horizontal,
        Vertical
    };

    public enum TextPosition
    {
        OverScroll,
        RightOfSlider,
        LeftOfSlider
    };

    public class SliderOld : UI_Element, Drawable
    {
        private Vector2f DragPos = new Vector2f();
        private Vector2f MousePosition = new Vector2f();
        private bool Drag = false;

        public SelectorType SelectorType = SelectorType.Horizontal;
        public TextPosition TextPosition = TextPosition.OverScroll;

        public RectangleShape Rectangle = new RectangleShape();
        public RectangleShape Selector = new RectangleShape();

        public Font font;
        public Text ValueText = new Text();
        public string AddText = "";

        public Animator TextAnimator = new Animator(0, 255, 1);

        //public Color BackColor = Color.White;
        public Color SelectorColor = new Color(0, 0, 0, 220);
        public Color SelectorMHColor = new Color(0, 0, 0, 160);

        public float Value, ValueFrom, ValueTo, dtime;
        public bool MouseHover, Click, MouseHoverSlider, DrawValue = false;

        public SliderOld()
        {
            
        }

        public override void Update(float DeltaTime)
        {
            //Selector.Position = Position;
            Rectangle.Position = Position;

            if (SelectorType == SelectorType.Horizontal)
                Rectangle.Size = new Vector2f(Size.X, 15);
            else
                Rectangle.Size = new Vector2f(15, Size.Y);
            //Rectangle.FillColor = BackColor;

            if (SelectorType == SelectorType.Horizontal)
            {
                if (Drag)
                {
                    //if (MousePosition.X - DragPos.X >= Position.X && MousePosition.X - DragPos.X < Position.X + Size.X)
                    //    Selector.Position = new Vector2f(MousePosition.X - DragPos.X, Position.Y);
                    if (MousePosition.X + DragPos.X >= Position.X && MousePosition.X - DragPos.X - 15 <= Position.X + Size.X)
                        Selector.Position = new Vector2f(MousePosition.X - DragPos.X, Position.Y);
                    else if (MousePosition.X + DragPos.X < Position.X)
                        Selector.Position = new Vector2f(Position.X, Position.Y);
                    else if (MousePosition.X + DragPos.X > Position.X + Size.X - 15)
                        Selector.Position = new Vector2f(Position.X + Size.X - 15, Position.Y);

                    Value = Map(Selector.Position.X - Position.X, 0, Position.X + Size.X - 15, ValueFrom, ValueTo);
                }

                //Selector.Position = new Vector2f(Map(Value, ValueFrom, ValueTo, Position.X, Rectangle.Position.X + Rectangle.Size.X - 15), Position.Y);
                Selector.Size = new Vector2f(15, 15);
                if (MouseHoverSlider)
                {
                    Selector.FillColor = SelectorMHColor;
                }
                else
                {
                    Selector.FillColor = SelectorColor;
                }

                if (DrawValue)
                {
                    ValueText.Font = font;
                    ValueText.CharacterSize = 14;
                    ValueText.DisplayedString = ((int)Value).ToString() + AddText;

                    if (TextPosition == TextPosition.OverScroll)
                    {
                        ValueText.Position = new Vector2f(Selector.Position.X + Selector.Size.X / 2 - ValueText.GetLocalBounds().Width / 2, Selector.Position.Y - ValueText.GetLocalBounds().Height - 8);
                        ValueText.FillColor = Color.White;
                    }
                    else if (TextPosition == TextPosition.RightOfSlider)
                    {
                        ValueText.Position = new Vector2f(Position.X + Size.X + 5, Position.Y + Rectangle.Size.Y / 2 - ValueText.GetLocalBounds().Height / 2);
                    }
                    else if (TextPosition == TextPosition.LeftOfSlider)
                    {
                        ValueText.Position = new Vector2f(Position.X - ValueText.GetLocalBounds().Width - 5, Position.Y + Rectangle.Size.Y / 2 - ValueText.GetLocalBounds().Height / 2);
                    }
                }
            }
            else
            {
                //Selector.Position = new Vector2f(Position.X, Map(Value, ValueFrom, ValueTo, Position.Y, Rectangle.Position.Y + Rectangle.Size.Y - 15));
                Selector.Size = new Vector2f(15, 15);
                if (MouseHoverSlider)
                {
                    Selector.FillColor = SelectorMHColor;
                }
                else
                {
                    Selector.FillColor = SelectorColor;
                }

                if (DrawValue)
                {
                    ValueText.Font = font;
                    ValueText.CharacterSize = 14;
                    ValueText.DisplayedString = ((int)Value).ToString() + AddText;
                    ValueText.Position = new Vector2f(Selector.Position.X + Selector.Size.X + 5, Selector.Position.Y + Selector.Size.Y / 2 - ValueText.GetLocalBounds().Height / 2);
                    ValueText.FillColor = Color.White;
                }
            }
        }

        public override void MouseCheck(MouseMoveEventArgs e)
        {
            MousePosition = new Vector2f(e.X, e.Y);

            if (e.X >= Position.X && e.X <= Position.X + Size.X &&
                e.Y >= Position.Y && e.Y <= Position.Y + Size.Y)
            {
                MouseHover = true;
            }
            else
            {
                MouseHover = false;
            }

            if (e.X >= Selector.Position.X && e.X <= Selector.Position.X + Selector.Size.X &&
                e.Y >= Selector.Position.Y && e.Y <= Selector.Position.Y + Selector.Size.Y)
            {
                MouseHoverSlider = true;
                Selector.FillColor = SelectorMHColor;
            }
            else
            {
                MouseHoverSlider = false;
                Selector.FillColor = SelectorColor;
            }
        }

        public override void MouseClick(MouseButtonEventArgs e)
        {
            if (MouseHoverSlider && e.Button == Mouse.Button.Left)
            {
                Click = true;
                Drag = true;
                DragPos = new Vector2f(e.X - Position.X, e.Y - Position.Y);
            }
        }

        public override void MouseRelease(MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                Click = false;
                DrawValue = false;

                Drag = false;
            }
        }

        public override void MouseWheel(float Delta)
        {
            if (MouseHover && Value + Delta <= ValueTo && Value + Delta >= ValueFrom)
                Value += Delta;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Rectangle);
            target.Draw(Selector);
            if (DrawValue)
                target.Draw(ValueText);
        }

        private float Map(float value, float From1, float From2, float To1, float To2)
        {
            return (value - From1) / (From2 - From1) * (To2 - To1) + To1;
        }
    }
}
