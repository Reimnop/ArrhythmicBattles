using FlexFramework;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UI;

public class VerticalStackLayout : UIElement
{
    public override Vector2d Position { get; set; }
    public override Vector2d Size { get; set; }
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
        
        double offset = 0;
        foreach (UIElement child in Children)
        {
            Vector2d originOffset = new Vector2d(child.Origin.X * child.Size.X, child.Origin.Y * child.Size.Y) + Position;
            child.Position = new Vector2d(0.0, offset) + originOffset;
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