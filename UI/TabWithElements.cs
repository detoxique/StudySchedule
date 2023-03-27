using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;
using Text = SFML.Graphics.Text;

namespace QuadroEngine.UI
{
    public struct ColumnRowInfo
    {
        public string Name;
        public Vector2f Size;

        public ColumnRowInfo(string name, Vector2f size)
        {
            Name = name;
            Size = size;
        }
    }

    public class TabWithElements : UI_Element, Drawable
    {
        private Vector2f DeltaPosition = new Vector2f(0, 0);
        private Vector2f OldPosition;

        public List<Element> Elements;
        public List<ColumnRowInfo> Columns;
        public List<ColumnRowInfo> Rows;

        public float LinesWidth = 1;

        // Labels
        private List<Text> ColumnTitles;
        private List<Text> RowsTitles;

        // Table
        private List<RectangleShape> Lines;
        private RectangleShape background;

        public TabWithElements()
        {
            Elements = new List<Element>();
            Columns = new List<ColumnRowInfo>();
            Rows = new List<ColumnRowInfo>();
            Lines = new List<RectangleShape>();
        }

        public TabWithElements(List<ColumnRowInfo> columns, List<ColumnRowInfo> rows, Vector2f pos, List<Element> elements)
        {
            Position = pos;
            if (elements != null)
                Elements = elements;
            Columns = columns;
            Rows = rows;
            OldPosition = pos;

            ColumnTitles = new List<Text>();
            RowsTitles = new List<Text>();
            Lines = new List<RectangleShape>();

            #region Size Calculation
            float tabx = 0, taby = 0;

            foreach (ColumnRowInfo cri in columns) /////// X
            {
                tabx += cri.Size.X;
            }
            foreach (ColumnRowInfo cri in rows)    /////// Y
            {
                taby += cri.Size.Y;
            }

            Size = new Vector2f(rows[0].Size.X + tabx, columns[0].Size.Y + taby);
            #endregion

            #region first lines
            RectangleShape firstVerticalLine = new RectangleShape(new Vector2f(LinesWidth, Size.Y)); // vertical firsts
            firstVerticalLine.FillColor = Color.Black;
            firstVerticalLine.Position = Position;
            Lines.Add(firstVerticalLine);
            RectangleShape secondVerticalLine = new RectangleShape(new Vector2f(LinesWidth, Size.Y));
            secondVerticalLine.FillColor = Color.Black;
            secondVerticalLine.Position = new Vector2f(Position.X + rows[0].Size.X, Position.Y);
            Lines.Add(secondVerticalLine);

            RectangleShape firstHorizontalLine = new RectangleShape(new Vector2f(Size.X, LinesWidth)); // horizontal firsts
            firstHorizontalLine.FillColor = Color.Black;
            firstHorizontalLine.Position = Position;
            Lines.Add(firstHorizontalLine);
            RectangleShape secondHorizontalLine = new RectangleShape(new Vector2f(Size.X, LinesWidth));
            secondHorizontalLine.FillColor = Color.Black;
            secondHorizontalLine.Position = new Vector2f(Position.X, Position.Y + columns[0].Size.Y);
            Lines.Add(secondHorizontalLine);
            #endregion

            float sizesXSum = Position.X + rows[0].Size.X;
            foreach (ColumnRowInfo cri in columns)
            {
                Text title = new Text(cri.Name, new Font("font.ttf"), 14);
                title.FillColor = Color.Black;
                title.Position = new Vector2f(sizesXSum + cri.Size.X / 2 - title.GetGlobalBounds().Width / 2, Position.Y + cri.Size.Y / 2 - title.GetGlobalBounds().Height / 1.5f);
                ColumnTitles.Add(title);

                // adding a vertical line
                RectangleShape line = new RectangleShape(new Vector2f(LinesWidth, Size.Y));
                sizesXSum += cri.Size.X;
                line.Position = new Vector2f(sizesXSum, Position.Y);
                line.FillColor = Color.Black;

                Lines.Add(line);
            }

            float sizesYSum = Position.Y + columns[0].Size.Y;
            foreach (ColumnRowInfo cri in rows)
            {
                Text title = new Text(cri.Name, new Font("font.ttf"), 14);
                title.FillColor = Color.Black;
                title.Position = new Vector2f(Position.X + cri.Size.X / 2 - title.GetGlobalBounds().Width / 2f, sizesYSum + cri.Size.Y / 2 - title.GetGlobalBounds().Height / 1.5f);
                RowsTitles.Add(title);

                // adding a horizontal line
                RectangleShape line = new RectangleShape(new Vector2f(Size.X + 1, LinesWidth));
                sizesYSum += cri.Size.Y;
                line.Position = new Vector2f(Position.X, sizesYSum);
                line.FillColor = Color.Black;

                Lines.Add(line);
            }

            background = new RectangleShape(Size);
            background.Position = Position;
            background.FillColor = new Color(0, 0, 0, 25);

            IsDraw = true;
        }

        public override void Update(float dtime)
        {
            background.Position = Position;

            //Lines[0].Position = Position;
            //Lines[1].Position = new Vector2f(Position.X + Rows[0].Size.X, Position.Y);
            //Lines[2].Position = Position;
            //Lines[3].Position = new Vector2f(Position.X, Position.Y + Columns[0].Size.Y);

            if (OldPosition != Position)
            {
                DeltaPosition = new Vector2f(Position.X - OldPosition.X, Position.Y - OldPosition.Y);

                foreach (RectangleShape line in Lines)
                {
                    line.Position = new Vector2f(line.Position.X + DeltaPosition.X, line.Position.Y + DeltaPosition.Y);
                }

                foreach (Text t in ColumnTitles)
                {
                    t.Position = new Vector2f(t.Position.X + DeltaPosition.X, t.Position.Y + DeltaPosition.Y);
                }

                foreach (Element e in Elements)
                {
                    e.Position = new Vector2f(e.Position.X + DeltaPosition.X, e.Position.Y + DeltaPosition.Y);
                }

                OldPosition = Position;
            }

            if (Elements != null)
            {
                foreach (Element e in Elements)
                {
                    e.Update(dtime);

                    if (e.Drag && e.Dragable)
                    {
                        e.Position = new Vector2f(e.MousePosition.X - e.DragPos.X, e.MousePosition.Y - e.DragPos.Y);
                    }
                    else if (!e.Drag && e.Dragable)
                    {
                        if (!e.AlignedX) ///////////////////////////// Align
                        {
                            float allcolumnssizes = 0;
                            for (int i = 0; i < Columns.Count; i++)
                            {
                                allcolumnssizes += Columns[i].Size.X;

                                if (i + 1 == Columns.Count)
                                {
                                    e.Position = new Vector2f(Position.X + Rows[0].Size.X + allcolumnssizes - Columns[0].Size.X, e.Position.Y);

                                    e.AlignedX = true;
                                    break;
                                }

                                if ((e.MousePosition.X - e.DragPos.X - Columns[i].Size.X / 2) - (Rows[0].Size.X + allcolumnssizes - Columns[i].Size.X) < Columns[i].Size.X)
                                {
                                    e.Position = new Vector2f(Position.X + Rows[0].Size.X + allcolumnssizes - Columns[i].Size.X, e.Position.Y);
                                    e.AlignedX = true;
                                    break;
                                }
                            }
                        }

                        if (!e.AlignedY)
                        {
                            float allrowssizes = 0;
                            for (int i = 0; i < Rows.Count; i++)
                            {
                                allrowssizes += Rows[i].Size.Y;

                                if (i + 1 == Rows.Count)
                                {
                                    e.Position = new Vector2f(e.Position.X, Position.Y + Rows[0].Size.Y + allrowssizes - Rows[0].Size.Y);
                                    e.AlignedY = true;
                                    break;
                                }

                                if ((e.MousePosition.Y - e.DragPos.Y - Rows[i].Size.Y / 2) - (Columns[0].Size.Y + allrowssizes - Rows[i].Size.Y) < Rows[i].Size.Y)
                                {
                                    e.Position = new Vector2f(e.Position.X, Position.Y + Columns[0].Size.Y + allrowssizes - Rows[i].Size.Y);
                                    e.AlignedY = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void MouseCheck(MouseMoveEventArgs e)
        {
            if (Elements != null)
            {
                foreach (Element el in Elements)
                {
                    el.MouseCheck(e);
                }
            }
        }

        public override void MouseClick(MouseButtonEventArgs e)
        {
            if (Elements != null)
            {
                foreach (Element el in Elements)
                {
                    el.MouseClick(e);
                }
            }
        }

        public override void MouseRelease(MouseButtonEventArgs e)
        {
            if (Elements != null)
            {
                foreach (Element el in Elements)
                {
                    el.MouseRelease(e);
                }
            }
        }

        public override void Resized(SizeEventArgs e)
        {
            if (Elements != null)
            {
                foreach (Element el in Elements)
                {
                    el.Resized(e);
                }
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (IsDraw)
            {
                target.Draw(background);

                foreach (RectangleShape line in Lines)
                {
                    target.Draw(line);
                }

                foreach (Text t in ColumnTitles)
                {
                    target.Draw(t);
                }

                foreach (Text t in RowsTitles)
                {
                    target.Draw(t);
                }

                if (Elements != null)
                {
                    foreach (Element e in Elements)
                    {
                        target.Draw(e);
                    }
                }
            }
        }
    }

    public class Element : Button
    {
        public Vector2f MousePosition = new Vector2f();
        public Vector2f DragPos = new Vector2f();
        public Vector2f OldPosition = new Vector2f();

        public bool Drag = false, AlignedX = true, AlignedY = true, Dragable = true;

        public Text SubLabel;

        public Element(Vector2f position, Text label, Vector2f size)
        {
            Position = position;
            OldPosition = position;
            Label = label;
            Size = size;
            IsActive = true;
            doubleClickAction += Delete;
        }

        public Element(Vector2f position, Text label, Text subLabel, Vector2f size)
        {
            Position = position;
            OldPosition = position;
            Label = label;
            Size = size;
            IsActive = true;
            doubleClickAction += Delete;
            SubLabel = subLabel;
        }

        public override void Update(float DeltaTime)
        {
            if (IsActive)
            {
                Background.Position = Position;
                Background.Size = Size;
                if (Icon != null)
                    Icon.Position = new Vector2f(Position.X + Size.X / 2 - Icon.TextureRect.Width / 2, Position.Y + Size.Y / 2 - Icon.TextureRect.Height / 2);

                Label.Position = new Vector2f(Background.Position.X + (Size.X / 2) - Label.GetGlobalBounds().Width / 2,
                    Background.Position.Y + Size.Y / 2 - Label.GetGlobalBounds().Height / 1.5f);

                if (SubLabel != null)
                    SubLabel.Position = new Vector2f(Position.X + 2, Position.Y + Size.Y - SubLabel.GetGlobalBounds().Height - 3);

                if (MouseHover && !Click)
                {
                    Background.FillColor = new Color((byte)Fade.Lerp(Background.FillColor.R, 255, DeltaTime),
                        (byte)Fade.Lerp(Background.FillColor.G, 255, DeltaTime),
                        (byte)Fade.Lerp(Background.FillColor.B, 255, DeltaTime),
                        (byte)Fade.Lerp(Background.FillColor.A, 100, DeltaTime)); // 100
                    Label.FillColor = new Color(20, 20, 20, 255);

                    if (SubLabel != null)
                        SubLabel.FillColor = new Color(20, 20, 20, 255);
                }
                else if (!MouseHover && !Click)
                {
                    Background.FillColor = new Color((byte)Fade.Lerp(Background.FillColor.R, 0, DeltaTime),
                        (byte)Fade.Lerp(Background.FillColor.G, 0, DeltaTime),
                        (byte)Fade.Lerp(Background.FillColor.B, 0, DeltaTime),
                        (byte)Fade.Lerp(Background.FillColor.A, 200, DeltaTime)); // 0 0 0 200
                    Label.FillColor = Color.White;

                    if (SubLabel != null)
                        SubLabel.FillColor = Color.White;
                }
                if (Click && MouseHover)
                {
                    Background.FillColor = new Color(255, 255, 255, 200);
                    Label.FillColor = Color.Black;

                    if (SubLabel != null)
                        SubLabel.FillColor = Color.Black;

                    if (action != null && !ActionDone && IsDraw)
                    {
                        action.Invoke();
                        ActionDone = true;
                    }
                }
                
            }
            
        }

        private void Delete()
        {
            IsDraw = false;
            IsActive = false;
        }

        public override void MouseCheck(MouseMoveEventArgs e)
        {
            if (IsActive)
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
                    Click = false;
                }
            }
        }

        public override void MouseClick(MouseButtonEventArgs e)
        {
            if (IsActive)
            {
                if (MouseHover && e.Button == Mouse.Button.Left)
                {
                    Click = true;
                    Drag = true;
                    AlignedX = false;
                    AlignedY = false;
                    DragPos = new Vector2f(e.X - Position.X, e.Y - Position.Y);
                }
                else
                {
                    Click = false;
                    Drag = false;
                }
            }
        }

        public override void MouseRelease(MouseButtonEventArgs e)
        {
            Click = false;
            Drag = false;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (IsDraw && IsActive)
            {
                target.Draw(Background);
                target.Draw(Label);
                if (Icon != null)
                    target.Draw(Icon);
                if (SubLabel != null)
                    target.Draw(SubLabel);
            }
        }
    }
}
