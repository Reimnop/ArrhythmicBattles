namespace FlexFramework.UserInterface.Elements;

public abstract class Element
{
    public List<Element> Children { get; } = new List<Element>();

    public Length Width { get; set; } = Length.Zero;
    public Length Height { get; set; } = Length.Zero;
    public Length MarginLeft { get; set; } = Length.Zero;
    public Length MarginRight { get; set; } = Length.Zero;
    public Length MarginTop { get; set; } = Length.Zero;
    public Length MarginBottom { get; set; } = Length.Zero;
    public Length PaddingLeft { get; set; } = Length.Zero;
    public Length PaddingRight { get; set; } = Length.Zero;
    public Length PaddingTop { get; set; } = Length.Zero;
    public Length PaddingBottom { get; set; } = Length.Zero;

    public Bounds CalculateBounds(Bounds parentBounds)
    {
        Bounds marginBounds = new Bounds(
            parentBounds.X0 + MarginLeft.Calculate(parentBounds.Width),
            parentBounds.Y0 + MarginTop.Calculate(parentBounds.Height),
            parentBounds.X1 - MarginRight.Calculate(parentBounds.Width),
            parentBounds.Y1 - MarginBottom.Calculate(parentBounds.Height)
        );

        return new Bounds(
                marginBounds.X0,
                marginBounds.Y0,
                marginBounds.X0 + Width.Calculate(parentBounds.Width),
                marginBounds.Y0 + Height.Calculate(parentBounds.Height)
            );
    }
    
    public abstract void BuildDrawables(List<Drawable> drawables, Bounds parentBounds);
        
    public abstract Drawable? CreateDrawable(Bounds bounds);
}