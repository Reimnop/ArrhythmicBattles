using ArrhythmicBattles.UI;
using ArrhythmicBattles.Util;
using FlexFramework;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.UserInterface;
using FlexFramework.Core.UserInterface.Elements;
using FlexFramework.Core.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Menu;

public class MultiplayerScreen : Screen
{
    public override Vector2 Position { get; set; }
    
    private Element rootElement;
    
    public MultiplayerScreen(FlexFrameworkMain engine, ABScene scene, InputInfo inputInfo)
    {
    }

    private Element CreateLayout(Bounds bounds)
    {
        Element root = new StackLayout()
        {
            Width = Length.Full, 
            Height = Length.Full
        };

        return root;
    }

    public override void Update(UpdateArgs args)
    {
        
    }

    public override void Render(Renderer renderer, int layerId, MatrixStack matrixStack, CameraData cameraData)
    {
        
    }
}