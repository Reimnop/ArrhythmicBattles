﻿using FlexFramework.Core.UserInterface.Drawables;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class RectElement : EmptyElement
{
    public float Radius { get; set; } = 0.0f;
    public Color4 Color { get; set; } = Color4.White;
    
    public RectElement(params Element[] children) : base(children)
    {
    }

    public override void BuildDrawables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds)
    {
        DrawDebugBounds(renderables, engine, constraintBounds);
        CalculateBounds(constraintBounds, out _, out Bounds elementBounds, out _);
        renderables.Add(new RectDrawable(engine, elementBounds, Color, Radius));
        
        base.BuildDrawables(renderables, engine, constraintBounds);
    }
}