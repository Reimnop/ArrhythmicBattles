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

public class MySelectorElement<T> : Element, IUpdateable, IRenderable where T : INamed
{
    public T Selected => items[index];
    public event Action<T>? SelectionChanged;
    
    private readonly Interactivity leftInteractivity;
    private readonly Interactivity rightInteractivity;
    
    private readonly RectEntity border;
    private readonly ImageEntity leftIcon;
    private readonly ImageEntity rightIcon;
    private readonly TextEntity text;

    private readonly IReadOnlyList<T> items;
    private int index;
    
    private Vector2 leftIconPosition;
    private Vector2 rightIconPosition;

    public MySelectorElement(IEnumerable<T> items, IInputProvider inputProvider, ResourceManager resourceManager, string stylePath)
    {
        this.items = items.ToList();

        var style = resourceManager.Get<ResourceDictionary>(stylePath);
        var font = resourceManager.Get<Font>(Constants.BoldFontPath);
        var color = ColorUtil.ParseHex(style.GetRaw("Color"));
        var baseColor = ColorUtil.ParseHex(style.GetRaw("BaseColor"));
        var hoveredColor = ColorUtil.ParseHex(style.GetRaw("HoveredColor"));
        var icon = style.LoadResource<TextureSampler>("Icon", resourceManager);

        border = new RectEntity
        {
            Color = color,
            BorderThickness = 2.0f,
            Radius = 8.0f
        };
        
        leftIcon = new ImageEntity(icon);
        rightIcon = new ImageEntity(icon);
        
        text = new TextEntity(font)
        {
            Color = color,
            Text = Selected.Name.ToUpper(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        
        leftInteractivity = new Interactivity(inputProvider);
        leftInteractivity.MouseEnter += () => leftIcon.Color = hoveredColor;
        leftInteractivity.MouseLeave += () => leftIcon.Color = baseColor;
        leftInteractivity.MouseButtonUp += button =>
        {
            if (button == MouseButton.Left)
                Select((index - 1 + this.items.Count) % this.items.Count);
        };
        
        rightInteractivity = new Interactivity(inputProvider);
        rightInteractivity.MouseEnter += () => rightIcon.Color = hoveredColor;
        rightInteractivity.MouseLeave += () => rightIcon.Color = baseColor;
        rightInteractivity.MouseButtonUp += button =>
        {
            if (button == MouseButton.Left)
                Select((index + 1) % this.items.Count);
        };
    }
    
    public void Update(UpdateArgs args)
    {
        leftInteractivity.Update(args);
        rightInteractivity.Update(args);
    }

    private void Select(int index)
    {
        this.index = index;
        text.Text = items[index].Name.ToUpper();
        SelectionChanged?.Invoke(items[index]);
    }

    protected override void UpdateLayout(Box2 bounds, float dpiScale)
    {
        border.Bounds = bounds;
        text.Bounds = bounds;
        text.DpiScale = dpiScale;
        
        // Magic Figma numbers
        leftIconPosition = new Vector2(bounds.Min.X + 32.0f, bounds.Min.Y + bounds.Size.Y / 2.0f);
        rightIconPosition = new Vector2(bounds.Max.X - 32.0f, bounds.Min.Y + bounds.Size.Y / 2.0f);
        
        var leftIconBounds = new Box2(leftIconPosition - new Vector2(16.0f, 16.0f), leftIconPosition + new Vector2(16.0f, 16.0f));
        var rightIconBounds = new Box2(rightIconPosition - new Vector2(16.0f, 16.0f), rightIconPosition + new Vector2(16.0f, 16.0f));
        leftInteractivity.Bounds = leftIconBounds;
        leftInteractivity.DpiScale = dpiScale;
        rightInteractivity.Bounds = rightIconBounds;
        rightInteractivity.DpiScale = dpiScale;
    }

    public void Render(RenderArgs args)
    {
        var matrixStack = args.MatrixStack;
        
        border.Render(args);
        text.Render(args);
        
        matrixStack.Push();
        matrixStack.Scale(32.0f, 32.0f, 1.0f);
        matrixStack.Translate(leftIconPosition.X, leftIconPosition.Y, 0.0f);
        leftIcon.Render(args);
        matrixStack.Pop();
        
        matrixStack.Push();
        matrixStack.Scale(-32.0f, 32.0f, 1.0f);
        matrixStack.Translate(rightIconPosition.X, rightIconPosition.Y, 0.0f);
        rightIcon.Render(args);
        matrixStack.Pop();
    }
}