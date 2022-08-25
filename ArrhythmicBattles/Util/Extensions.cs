using ArrhythmicBattles.UI;
using FlexFramework.Core.EntitySystem.Default;
using FlexFramework.Rendering.Data;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Util;

public static class Extensions
{
    // ui element extensions
    public static T WithPosition<T>(this T element, Vector2i value) where T : UIElement
    {
        element.Position = value;
        return element;
    }
    
    public static T WithPosition<T>(this T element, int x, int y) where T : UIElement
    {
        return element.WithPosition(new Vector2i(x, y));
    }
    
    public static T WithSize<T>(this T element, Vector2i value) where T : UIElement
    {
        element.Size = value;
        return element;
    }
    
    public static T WithSize<T>(this T element, int x, int y) where T : UIElement
    {
        return element.WithSize(new Vector2i(x, y));
    }
    
    public static T WithOrigin<T>(this T element, Vector2d value) where T : UIElement
    {
        element.Origin = value;
        return element;
    }
    
    public static T WithOrigin<T>(this T element, double x, double y) where T : UIElement
    {
        return element.WithOrigin(new Vector2d(x, y));
    }
    
    // image extension
    public static ImageEntity WithTexture(this ImageEntity image, Texture2D texture)
    {
        image.Texture = texture;
        return image;
    }
    
    public static ImageEntity WithImageMode(this ImageEntity image, ImageMode imageMode)
    {
        image.ImageMode = imageMode;
        return image;
    }
    
    // button extensions
    public static ButtonEntity WithText(this ButtonEntity button, string text)
    {
        button.Text = text;
        return button;
    }
    
    public static ButtonEntity WithTextPosOffset(this ButtonEntity button, Vector2i value)
    {
        button.TextPosOffset = value;
        return button;
    }
    
    public static ButtonEntity WithTextPosOffset(this ButtonEntity button, int x, int y)
    {
        return button.WithTextPosOffset(new Vector2i(x, y));
    }
    
    public static ButtonEntity WithTextUnfocusedColor(this ButtonEntity button, Color4 value)
    {
        button.TextUnfocusedColor = value;
        return button;
    }

    public static ButtonEntity WithTextFocusedColor(this ButtonEntity button, Color4 value)
    {
        button.TextFocusedColor = value;
        return button;
    }

    public static ButtonEntity AddPressedCallback(this ButtonEntity button, Action callback)
    {
        button.PressedCallback += callback;
        return button;
    }
}