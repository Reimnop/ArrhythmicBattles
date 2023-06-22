using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.Util;
using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.UserInterface;

public class MyButtonElement : Element, IUpdateable, IRenderable
{
    public Action? Click { get; set; }
    
    private readonly Interactivity interactivity;
    
    private readonly RectEntity border;
    private readonly ImageEntity icon;
    private readonly TextEntity text;

    private Vector2 iconPosition;

    public MyButtonElement(IInputProvider inputProvider, ResourceManager resourceManager, string stylePath) 
    {
        var resourceDictionary = resourceManager.Get<ResourceDictionary>(stylePath);
        var defaultIcon = resourceDictionary.LoadResource<TextureSampler>("DefaultIcon", resourceManager);
        var hoveredIcon = resourceDictionary.LoadResource<TextureSampler>("HoveredIcon", resourceManager);
        
        var colorHex = resourceDictionary.GetRaw("Color");
        var color = ColorUtil.ParseHex(colorHex);

        border = new RectEntity()
        {
            Color = color,
            BorderThickness = 2.0f,
            Radius = 8.0f
        };
        
        icon = new ImageEntity(defaultIcon);
        
        var font = resourceManager.Get<Font>(Constants.BoldFontPath);
        text = new TextEntity(font)
        {
            Text = resourceDictionary.GetRaw("Text"),
            Color = color,
            VerticalAlignment = VerticalAlignment.Center
        };
        
        interactivity = new Interactivity(inputProvider);
        interactivity.MouseEnter += () =>
        {
            border.BorderThickness = float.PositiveInfinity;
            icon.Texture = hoveredIcon;
            text.Color = Color4.Black;
        };
        interactivity.MouseLeave += () =>
        {
            border.BorderThickness = 2.0f;
            icon.Texture = defaultIcon;
            text.Color = color;
        };
        interactivity.MouseButtonUp += button =>
        {
            if (button == MouseButton.Left)
                Click?.Invoke();
        };
    }

    protected override void UpdateLayout(Box2 bounds)
    {
        border.Bounds = bounds;
        interactivity.Bounds = bounds;
        iconPosition = new Vector2(bounds.Min.X + 32.0f, bounds.Center.Y);
        text.Bounds = new Box2(bounds.Min + new Vector2(80.0f, 16.0f), bounds.Max - new Vector2(16.0f, 16.0f));
    }
    
    public void Update(UpdateArgs args)
    {
        interactivity.Update(args);
    }

    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        border.Render(args);
        
        matrixStack.Push();
        matrixStack.Scale(32.0f, 32.0f, 1.0f);
        matrixStack.Translate(iconPosition.X, iconPosition.Y, 0.0f);
        icon.Render(args);
        matrixStack.Pop();
        
        text.Render(args);
    }
}
