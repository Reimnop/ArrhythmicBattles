using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Drawables;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;
using Textwriter;

namespace ArrhythmicBattles.Menu;

public class MultiplayerScreen : Screen
{
    public override Vector2 Position { get; set; }

    private readonly FlexFrameworkMain engine;
    private List<Drawable> drawables;

    public MultiplayerScreen(FlexFrameworkMain engine, ABScene scene, InputInfo inputInfo)
    {
        this.engine = engine;
        Bounds bounds = new Bounds(0, 0, engine.Size.X, engine.Size.Y);
        drawables = RenderLayout(bounds);
    }

    private List<Drawable> RenderLayout(Bounds bounds)
    {
        Font font = engine.TextResources.GetFont("inconsolata-regular");
        
        Element root = new StackLayout(
            new RectElement(
                new TextElement("Lorem ipsum dolor sit amet,", Color4.Black, font)
                {
                    Width = Length.Full,
                    Height = Length.Full
                })
            {
                Radius = 8.0f,
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(12.0f, Unit.Pixel)
            },
            new RectElement(
                new TextElement("consectetur adipiscing elit.", Color4.Black, font)
                {
                    Width = Length.Full,
                    Height = Length.Full
                })
            {
                Radius = 8.0f,
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(12.0f, Unit.Pixel)
            },
            new RectElement(
                new TextElement("Nulla ut tincidunt quam.", Color4.Black, font)
                {
                    Width = Length.Full,
                    Height = Length.Full
                })
            {
                Radius = 8.0f,
                Width = Length.Full,
                Height = new Length(64.0f, Unit.Pixel),
                Padding = new Length(12.0f, Unit.Pixel)
            })
        {
            Width = new Length(512.0f, Unit.Pixel), 
            Height = new Length(240.0f, Unit.Pixel),
            Spacing = new Length(12.0f, Unit.Pixel),
            Padding = new Length(12.0f, Unit.Pixel)
        };

        return root.BuildDrawables(engine, bounds);
    }

    public override void Update(UpdateArgs args)
    {
        
    }

    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        foreach (Drawable drawable in drawables)
        {
            drawable.Render(renderer, layerId, matrixStack, cameraData);
        }
    }
}