using System.Drawing;
using FlexFramework;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.EntitySystem.Default;

public abstract class UIElement : Entity
{
    public abstract Vector2d Position { get; set; }
    public abstract Vector2d Size { get; set; }
    public abstract Vector2d Origin { get; set; }
    public abstract bool IsFocused { get; set; }

    private bool wasFocused = false;

    protected FlexFrameworkMain Engine { get; }

    public UIElement(FlexFrameworkMain engine)
    {
        Engine = engine;
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        bool currentlyFocused = IsFocused;

        if (!wasFocused && currentlyFocused)
        {
            OnFocused();
        }
        else if (wasFocused && !currentlyFocused)
        {
            OnUnfocused();
        }
        
        wasFocused = currentlyFocused;
    }

    private Vector2d GetMousePos()
    {
        Vector2d mousePos = Engine.Input.MousePosition;
        return new Vector2d(mousePos.X, Engine.ClientSize.Y - mousePos.Y);
    }

    public bool IsMouseOver()
    {
        Vector2d mousePos = GetMousePos();
        RectangleF rectangle = GetBounds();
        return rectangle.Contains((float) mousePos.X, (float) mousePos.Y);
    }

    public RectangleF GetBounds()
    {
        return new RectangleF((float) (Position.X - Origin.X * Size.X), (float) (Position.Y - Origin.Y * Size.Y), (float) Size.X, (float) Size.Y);
    }

    protected virtual void OnUnfocused()
    {
    }

    protected virtual void OnFocused()
    {
    }
}