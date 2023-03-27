using SFML.Graphics;
using SFML.System;
using SFML.Window;
namespace QuadroEngine
{
    public enum ShapeType
    {
        Rectangle,
        Circle,
        Polygon
    };

    public class GameObject : Drawable
    {
        public Vector2f Position { get; set; }
        public Vector2f WorldPosition { get; set; }
        public Vector2f Velocity { get; set; }
        public Vector2f Size { get; set; }
        public Vector2f Gravity { get; set; }
        public Sprite sprite { get; set; }
        public Shape shape { get; set; }

        public float Restiution, Radius, Mass;

        public bool IsStatic { get; set; }
        public bool IsOnGround { get; set; }

        public delegate void IsCollides();

        public event IsCollides OnCollision;

        public bool IsEditable { get; set; }

        public bool Clicked { get; set; }

        public GameObject()
        {
            
        }

        public virtual void Update(float DeltaTime, Entity Camera)
        {
            
            if (shape != null)
            {
                shape.Position = Position;
            }
        }

        public void CallOnCollision()
        {
            OnCollision?.Invoke();
        }

        //public void Click(MouseButtonEventArgs e, Entity Camera)
        //{
        //    if (IsEditable)
        //    {
        //        if (sprite != null)
        //        {
        //            if (e.Button == Mouse.Button.Left && e.X > sprite.Position.X - sprite.Origin.X && e.X < sprite.Position.X + sprite.Origin.X &&
        //            e.Y > sprite.Position.Y - sprite.Origin.Y && e.Y < sprite.Position.Y + sprite.Origin.Y)
        //            {
        //                Clicked = true;
        //            }
        //        }
        //    }
        //}

        public void MoveToMouse(MouseMoveEventArgs e, Entity Camera)
        {
            //if (IsEditable && Clicked)
            //{
                
            //}
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            if (sprite != null)
            {
                target.Draw(sprite, states);
            }
            if (shape != null)
            {
                target.Draw(shape, states);
            }
        }
    }
}
