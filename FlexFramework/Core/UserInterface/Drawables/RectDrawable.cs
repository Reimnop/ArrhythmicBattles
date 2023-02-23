﻿using FlexFramework.Core.Data;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.Data;
using FlexFramework.Core;
using FlexFramework.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.UserInterface.Drawables;

public class RectDrawable : Drawable, IDisposable
{
    private readonly Mesh<Vertex> mesh;
    private readonly Bounds bounds;
    private readonly Color4 color;
    private readonly float radius;

    public RectDrawable(FlexFrameworkMain engine, Color4 color, float radius = 0.0f)
    {
        this.bounds = bounds;
        this.color = color;
        this.radius = radius;

        if (radius == 0.0f)
        {
            EngineResources resources = engine.Resources;
            mesh = engine.ResourceManager.GetResource<Mesh<Vertex>>(resources.QuadMesh);
        }
        else
        {
            Vector2[] vertexPositions = MeshGenerator.GenerateRoundedRectangle(bounds.Min, bounds.Max, radius);
            Vertex[] vertices = vertexPositions
                .Select(pos =>
                {
                    Vector2 relativePos = pos - bounds.Min;
                    Vector2 uv = new Vector2(relativePos.X / bounds.Width, relativePos.Y / bounds.Height);
                    return new Vertex(new Vector3(pos), uv);
                })
                .ToArray();
            mesh = new Mesh<Vertex>($"quad-rounded-{this.radius}", vertices);
        }
    }
    
    public override void Render(RenderArgs args)
    {
        Renderer renderer = args.Renderer;
        int layerId = args.LayerId;
        MatrixStack matrixStack = args.MatrixStack;
        CameraData cameraData = args.CameraData;

        matrixStack.Push();
        Transform.ApplyToMatrixStack(matrixStack);
        
        matrixStack.Push();

        if (radius == 0.0f)
        {
            matrixStack.Translate(0.5f, 0.5f, 0.0f);
            matrixStack.Scale(bounds.Width, bounds.Height, 1.0f);
            matrixStack.Translate(bounds.X0, bounds.Y0, 0.0f);
        }
        
        VertexDrawData vertexDrawData = new VertexDrawData(mesh.VertexArray, mesh.Count, matrixStack.GlobalTransformation * cameraData.View * cameraData.Projection, null, color, PrimitiveType.Triangles);
        renderer.EnqueueDrawData(layerId, vertexDrawData);
        matrixStack.Pop();
        matrixStack.Pop();
    }

    public void Dispose()
    {
        if (radius != 0.0f)
            mesh.Dispose();
    }
}