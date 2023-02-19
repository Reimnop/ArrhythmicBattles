using System.Drawing;
using FlexFramework;
using FlexFramework.Core.System;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace FlexFramework.Core.System.Entities;

public abstract class UIElement : Entity
{
    public abstract Vector2 Position { get; set; }
    public abstract Vector2 Size { get; set; }
    public abstract Vector2 Origin { get; set; }
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

    private Vector2 GetMousePos()
    {
        Vector2 mousePos = Engine.Input.MousePosition;
        return new Vector2(mousePos.X, Engine.ClientSize.Y - mousePos.Y);
    }

    public bool IsMouseOver()
    {
        Vector2 mousePos = GetMousePos();
        RectangleF rectangle = GetBounds();
        return rectangle.Contains(mousePos.X, mousePos.Y);
    }

    public RectangleF GetBounds()
    {
        return new RectangleF(Position.X - Origin.X * Size.X, Position.Y - Origin.Y * Size.Y, Size.X, Size.Y);
    }

    protected virtual void OnUnfocused()
    {
    }

    protected virtual void OnFocused()
    {
    }
}