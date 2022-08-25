using FlexFramework;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UI;

public class VerticalStackLayout : UIElement
{
    public override Vector2i Position { get; set; }
    public override Vector2i Size { get; set; }
    public override Vector2d Origin { get; set; }
    public override bool IsFocused { get; set; }

    public List<UIElement> Children { get; }

    private readonly FlexFrameworkMain engine;

    public VerticalStackLayout(FlexFrameworkMain engine) : base(engine)
    {
        this.engine = engine;
        
        Children = new List<UIElement>();
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        int offset = 0;
        foreach (UIElement child in Children)
        {
            Vector2i originOffset = new Vector2i((int) (child.Origin.X * child.Size.X), (int) (child.Origin.Y * child.Size.Y)) + Position;
            child.Position = new Vector2i(0, offset) + originOffset;
            offset += child.Size.Y;
        }
    }

    public void AddChild(UIElement element)
    {
        Children.Add(element);
    }

    public override void Dispose()
    {
    }
}