using System.Drawing;
using FlexFramework;
using FlexFramework.Core.EntitySystem;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UI;

public abstract class UIElement : Entity
{
    public abstract Vector2i Position { get; set; }
    public abstract Vector2i Size { get; set; }
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
        Rectangle rectangle = GetBounds();
        return rectangle.Contains((int) mousePos.X, (int) mousePos.Y);
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(Position.X - (int) (Origin.X * Size.X), Position.Y - (int) (Origin.Y * Size.Y), Size.X, Size.Y);
    }

    protected virtual void OnUnfocused()
    {
    }

    protected virtual void OnFocused()
    {
    }
}