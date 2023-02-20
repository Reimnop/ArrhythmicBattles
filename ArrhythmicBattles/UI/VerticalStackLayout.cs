using FlexFramework;
using FlexFramework.Core.Entities;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.UI;

public class VerticalStackLayout : UIElement
{
    public override Vector2 Position { get; set; }
    public override Vector2 Size { get; set; }
    public override Vector2 Origin { get; set; }
    public override bool IsFocused { get; set; }

    public List<UIElement> Children { get; }

    private readonly FlexFrameworkMain engine;

    public VerticalStackLayout(FlexFrameworkMain engine) : base(engine)
    {
        this.engine = engine;
        
        Children = new List<UIElement>();
    }

    public override void Start()
    {
        ComputeLayout();
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        ComputeLayout();
    }

    private void ComputeLayout()
    {
        float offset = 0;
        foreach (UIElement child in Children)
        {
            Vector2 originOffset = new Vector2(child.Origin.X * child.Size.X, child.Origin.Y * child.Size.Y) + Position;
            child.Position = new Vector2(0.0f, offset) + originOffset;
            offset += child.Size.Y;
        }
    }

    public void AddChild(UIElement element)
    {
        Children.Add(element);
    }
}