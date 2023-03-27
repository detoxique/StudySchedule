using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;

namespace QuadroEngine.UI
{
    public class Page : UI_Element, Drawable
    {
        public List<UI_Element> Elements = new List<UI_Element>();
        public List<Drawable> BasicElements = new List<Drawable>();
    }
    public class Panel : UI_Element, Drawable
    {
        public List<Page> Pages;
        public int CurrentPage = 1;

        private Button CurrentPageText;
        private Button LeftButton;
        private Button RightButton;
        private RectangleShape Background;

        public Panel()
        {
            Pages = new List<Page>();
            CurrentPage = 1;
            CurrentPageText = new Button(new Vector2f(), new Text("1", Program.game.Fonts[0], 14), new Vector2f(20, 20));

            LeftButton = new Button(new Vector2f(), new Text("<", Program.game.Fonts[0], 14), new Vector2f(20, 20));
            RightButton = new Button(new Vector2f(), new Text(">", Program.game.Fonts[0], 14), new Vector2f(20, 20));

            Background = new RectangleShape();
            Background.FillColor = Color.White;
            Background.OutlineColor = Color.Black;
            Background.OutlineThickness = 1;
        }

        public Panel(Vector2f size, Font font)
        {
            Position = new Vector2f(0, 0);
            Size = size;
            Pages = new List<Page>();
            CurrentPage = 1;
            CurrentPageText = new Button(new Vector2f(0, 0), new Text("1", font, 14), new Vector2f(20, 20));

            LeftButton = new Button(new Vector2f(0, 0), new Text("<", font, 14), new Vector2f(20, 20));
            RightButton = new Button(new Vector2f(0, 0), new Text(">", font, 14), new Vector2f(20, 20));

            Background = new RectangleShape(size);
            Background.FillColor = Color.White;
            Background.OutlineColor = Color.Black;
            Background.OutlineThickness = 1;
        }

        public Panel(Vector2f pos, Vector2f size, Font font)
        {
            Position = pos;
            Size = size;
            Pages = new List<Page>();
            CurrentPage = 1;
            CurrentPageText = new Button(new Vector2f(0, 0), new Text("1", font, 14), new Vector2f(20, 20));

            LeftButton = new Button(new Vector2f(0, 0), new Text("<", font, 14), new Vector2f(20, 20), PreviousPage);
            RightButton = new Button(new Vector2f(0, 0), new Text(">", font, 14), new Vector2f(20, 20), NextPage);

            Background = new RectangleShape(size);
            Background.FillColor = Color.White;
            Background.OutlineColor = Color.Black;
            Background.OutlineThickness = 1;
        }

        public override void Update(float DeltaTime)
        {
            Background.Position = Position;
            Background.Size = Size;

            if (Pages != null)
                foreach (Page p in Pages)
                    if (p.Elements != null)
                        foreach (UI_Element e in p.Elements)
                            e.Update(DeltaTime);

            LeftButton.Update(DeltaTime);
            RightButton.Update(DeltaTime);
            CurrentPageText.Update(DeltaTime);

            LeftButton.Position = new Vector2f(Position.X, Position.Y + Size.Y - 20);
            CurrentPageText.Position = new Vector2f(Position.X + 20, Position.Y + Size.Y - 20);
            CurrentPageText.Label.DisplayedString = CurrentPage.ToString();

            if (CurrentPageText.Label.GetGlobalBounds().Width > CurrentPageText.Size.X)
                CurrentPageText.Size = new Vector2f(CurrentPageText.Label.GetGlobalBounds().Width * 1.5f, 20);

            RightButton.Position = new Vector2f(Position.X + 20 + CurrentPageText.Size.X, Position.Y + Size.Y - 20);
        }

        private void NextPage()
        {
            if (CurrentPage + 1 <= Pages.Count)
                CurrentPage++;
        }

        private void PreviousPage()
        {
            if (CurrentPage - 1 > 0)
                CurrentPage--;
        }

        public override void MouseCheck(MouseMoveEventArgs e)
        {
            if (Pages != null)
                if (Pages[CurrentPage - 1].Elements != null)
                    foreach (UI_Element el in Pages[CurrentPage - 1].Elements)
                        el.MouseCheck(e);
            LeftButton.MouseCheck(e);
            RightButton.MouseCheck(e);
        }

        public override void MouseClick(MouseButtonEventArgs e)
        {
            if (Pages != null)
                if (Pages[CurrentPage - 1].Elements != null)
                    foreach (UI_Element el in Pages[CurrentPage - 1].Elements)
                        el.MouseClick(e);
            LeftButton.MouseClick(e);
            RightButton.MouseClick(e);
        }

        public override void MouseRelease(MouseButtonEventArgs e)
        {
            if (Pages != null)
                if (Pages[CurrentPage - 1].Elements != null)
                    foreach (UI_Element el in Pages[CurrentPage - 1].Elements)
                        el.MouseRelease(e);
            LeftButton.MouseRelease(e);
            RightButton.MouseRelease(e);
        }

        public override void MouseWheel(float Delta)
        {
            if (Pages != null)
                if (Pages[CurrentPage - 1].Elements != null)
                    foreach (UI_Element el in Pages[CurrentPage - 1].Elements)
                        el.MouseWheel(Delta);
        }

        public override void Resized(SizeEventArgs e)
        {
            if (Pages != null)
                if (Pages[CurrentPage - 1].Elements != null)
                    foreach (UI_Element el in Pages[CurrentPage - 1].Elements)
                        el.Resized(e);
            LeftButton.Resized(e);
            RightButton.Resized(e);
        }

        public override void TextEntered(TextEventArgs e)
        {
            if (Pages != null)
                if (Pages[CurrentPage - 1].Elements != null)
                    foreach (UI_Element el in Pages[CurrentPage - 1].Elements)
                    el.TextEntered(e);
        }

        public override void KeyPressed(KeyEventArgs e)
        {
            
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Background, states);
            if (Pages != null)
            {
                if (Pages[CurrentPage - 1].Elements != null)
                    foreach (UI_Element element in Pages[CurrentPage - 1].Elements)
                        target.Draw(element, states);
                if (Pages[CurrentPage - 1].BasicElements != null)
                    foreach (Drawable element in Pages[CurrentPage - 1].BasicElements)
                        target.Draw(element, states);
            }
            LeftButton.Draw(target, states);
            RightButton.Draw(target, states);
            CurrentPageText.Draw(target, states);
        }
    }
}
