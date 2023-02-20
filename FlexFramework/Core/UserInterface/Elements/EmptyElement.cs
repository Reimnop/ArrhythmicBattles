﻿using FlexFramework.Core.UserInterface.Renderables;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Elements;

public class EmptyElement : Element
{
    public override void BuildRenderables(List<IRenderable> renderables, FlexFrameworkMain engine, Bounds constraintBounds)
    {
        CalculateBounds(renderables, engine, constraintBounds, out _, out _, out Bounds contentBounds);

        float y = contentBounds.Y0;
        
        // Render children
        foreach (Element child in Children)
        {
            Bounds childConstraintBounds = new Bounds(contentBounds.X0, y, contentBounds.X1, contentBounds.Y1);
            Bounds childBounds = child.CalculateBoundingBox(childConstraintBounds);
            y += childBounds.Height;

            child.BuildRenderables(renderables, engine, childConstraintBounds);
        }
    }
}