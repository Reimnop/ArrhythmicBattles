﻿using System.Drawing;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexFramework.Core.UserInterface.Elements;

public class ButtonElement : VisualElement, IUpdateable, IDisposable
{
    public event Action? Click;
    
    private readonly Interactivity interactivity;
    private readonly RectEntity rectEntity = new RectEntity();

    public ButtonElement(IInputProvider inputProvider, params Element[] children) : base(children)
    {
        interactivity = new Interactivity(inputProvider);
        interactivity.MouseButtonDown += OnMouseButtonDown;
    }

    private void OnMouseButtonDown(MouseButton button)
    {
        if (button == MouseButton.Left)
        {
            Click?.Invoke();
        }
    }

    public void Update(UpdateArgs args)
    {
        interactivity.Update();

        rectEntity.Color = new Color4(0.9f, 0.9f, 0.9f, 1.0f);
        if (interactivity.MouseOver)
        {
            rectEntity.Color = new Color4(0.8f, 0.8f, 0.8f, 1.0f);
        }
        if (interactivity.MouseButtons[(int) MouseButton.Left])
        {
            rectEntity.Color = new Color4(0.6f, 0.6f, 0.6f, 1.0f);
        }
    }

    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
        
        interactivity.Bounds = ElementBounds;
        rectEntity.Min = ElementBounds.Min;
        rectEntity.Max = ElementBounds.Max;
    }

    public override void Render(RenderArgs args)
    {
        rectEntity.Render(args);
    }

    public void Dispose()
    {
        rectEntity.Dispose();
    }
}