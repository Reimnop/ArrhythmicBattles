﻿using FlexFramework.Core.Entities;
using OpenTK.Mathematics;
using Textwriter;

namespace FlexFramework.Core.UserInterface.Elements;

public class TextElement : VisualElement, IRenderable, IDisposable
{
    public string Text
    {
        get => textEntity.Text;
        set => textEntity.Text = value;
    }

    public Color4 Color
    {
        get => textEntity.Color;
        set => textEntity.Color = value;
    }

    private readonly TextEntity textEntity;

    public TextElement(FlexFrameworkMain engine, Font font, params Element[] children) : base(children)
    {
        textEntity = new TextEntity(engine, font);
        textEntity.BaselineOffset = font.Height;
    }
    
    public override void UpdateLayout(Bounds constraintBounds)
    {
        base.UpdateLayout(constraintBounds);
        UpdateChildrenLayout(ContentBounds);
    }

    public override void Render(RenderArgs args)
    {
        MatrixStack matrixStack = args.MatrixStack;
        
        matrixStack.Push();
        matrixStack.Translate(ElementBounds.X0, ElementBounds.Y0, 0.0f);
        textEntity.Render(args);
        matrixStack.Pop();
        
        DrawDebugBoxes(args);
    }

    public void Dispose()
    {
        textEntity.Dispose();
    }
}