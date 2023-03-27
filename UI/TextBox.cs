using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace QuadroEngine.UI
{
    public class TextBox : UI_Element, Drawable
    {
        private RectangleShape Background = new RectangleShape();
        private RectangleShape PointerRect = new RectangleShape();
        public Text Label = new Text();
        public Vector2f LabelPos = new Vector2f();

        private bool MouseChecked = false, MouseClicked = false, pointrect = true, Applied = true;
        public string text = "", TextEnter = "";

        private int Pointer = 0;

        private Clock RectTimer = new Clock();

        public delegate void Apply();
        public event Apply OnApply;

        public TextBox()
        {
            Label.FillColor = Color.White;
            Label.Font = Program.game.Fonts[0];
            Label.CharacterSize = 14;
            Label.DisplayedString = "Введите текст";
            LabelPos = new Vector2f(Position.X + 3, Position.Y + Size.Y / 2 - Label.GetGlobalBounds().Height / 2 - 4);
            Label.Position = LabelPos;

            PointerRect.Size = new Vector2f(1, Size.Y);
            PointerRect.FillColor = Color.White;

            Background.FillColor = new Color(0, 0, 0, 180);
            Background.Size = Size;

            Pointer = text.Length;

            IsActive = true;
            IsDraw = true;
        }

        public TextBox(Vector2f pos, Vector2f size)
        {
            Position = pos;
            Size = size;

            Label.FillColor = Color.White;
            Label.Font = Program.game.Fonts[0];
            Label.CharacterSize = 14;
            Label.DisplayedString = "Введите текст";
            LabelPos = new Vector2f(Position.X + 3, Position.Y + Size.Y / 2 - Label.GetGlobalBounds().Height / 2 - 4);
            Label.Position = LabelPos;

            PointerRect.Size = new Vector2f(1, Size.Y);
            PointerRect.FillColor = Color.White;

            Background.FillColor = new Color(0, 0, 0, 180);
            Background.Size = Size;

            Pointer = text.Length;

            IsActive = true;
            IsDraw = true;
        }

        public override void Update(float DeltaTime)
        {
            Label.DisplayedString = text;
            Label.Position = LabelPos;

            Background.Size = Size;
            Background.Position = Position;

            //Label.DisplayedString = "IsFocused = " + IsFocused.ToString();

            if (IsFocused)
            {
                Text temp;
                if (text.Length - Pointer > 0)
                    temp = new Text(text.Remove(Pointer, text.Length - Pointer), Label.Font, 14);
                else
                    temp = new Text(text, Label.Font, 14);

                PointerRect.Position = new Vector2f(Position.X + temp.GetGlobalBounds().Width + 5, Position.Y + 2);
                PointerRect.Size = new Vector2f(1, Size.Y - 4);

                if (pointrect)
                    PointerRect.FillColor = Color.White;
                else
                    PointerRect.FillColor = new Color(0, 0, 0, 0);

                if (RectTimer.ElapsedTime.AsSeconds() > 0.75f)
                {
                    if (pointrect)
                        pointrect = false;
                    else
                        pointrect = true;
                    RectTimer.Restart();
                }
            }
        }

        public override void MouseCheck(MouseMoveEventArgs e)
        {
            if (e.X >= Background.Position.X && e.X <= Background.Position.X + Background.Size.X &&
                e.Y >= Background.Position.Y && e.Y <= Background.Position.Y + Background.Size.Y)
            {
                MouseChecked = true;
            }
            else
            {
                MouseChecked = false;
            }
        }

        public override void MouseClick(MouseButtonEventArgs e)
        {
            if (MouseChecked && e.Button == Mouse.Button.Left)
            {
                MouseClicked = true;
                IsFocused = true;
                Background.OutlineColor = Color.Black;
                Background.OutlineThickness = 1;

                Pointer = text.Length;
                Applied = false;
            }
            else if (!MouseChecked && e.Button == Mouse.Button.Left)
            {
                MouseClicked = false;
                IsFocused = false;
                Background.OutlineThickness = 0;

                if (OnApply != null)
                {
                    OnApply.Invoke();
                    Applied = true;
                }
            }
        }

        public override void TextEntered(TextEventArgs e)
        {
            //text = ((int)e.Unicode.ToCharArray()[0]).ToString();
            if (IsFocused)
            {
                switch ((int)e.Unicode.ToCharArray()[0])
                {
                    case 13: //////// ENTER
                             //text += "\n";
                        break;
                    case 8:  //////// BACKSPACE
                        if (Pointer - 1 >= 0)
                        {
                            text = text.Remove(Pointer - 1, 1);
                            Pointer--;
                        }
                        break;
                    default:
                        if (text != "")
                            text = text.Insert(Pointer, e.Unicode);
                        else
                            text += e.Unicode;
                        Pointer++;
                        break;
                }
            }
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Left && Pointer - 1 >= 0)
                Pointer--;

            if (e.Code == Keyboard.Key.Right && Pointer + 1 <= text.Length)
                Pointer++;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (IsDraw)
            {
                target.Draw(Background);
                target.Draw(Label);
                if (IsFocused)
                    target.Draw(PointerRect);
            }
        }
    }
}
