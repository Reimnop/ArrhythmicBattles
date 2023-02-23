using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Drawables;
using FlexFramework.Core.UserInterface.Elements;
using OpenTK.Mathematics;
using Textwriter;

namespace ArrhythmicBattles.Menu;

public class MultiplayerScreen : Screen
{
    public override Vector2 Position { get; set; }

    private readonly FlexFrameworkMain engine;
    private readonly Element root;

    public MultiplayerScreen(FlexFrameworkMain engine, ABScene scene, InputInfo inputInfo)
    {
        this.engine = engine;
        root = InitUI();
    }

    private Element InitUI()
    {
        Font font = engine.TextResources.GetFont("inconsolata-regular");

        return new StackLayout(
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
    }

    public override void Update(UpdateArgs args)
    {
        root.UpdateRecursive(args);
    }

    public override void Render(RenderArgs args)
    {
        root.RenderRecursive(args);
    }
}