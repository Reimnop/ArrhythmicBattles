using FlexFramework.Core.Util;
using FlexFramework.Rendering.Data;
using FlexFramework.Rendering.PostProcessing;
using OpenTK.Mathematics;

namespace FlexFramework.Rendering;

public abstract class Renderer : IDisposable
{
    public Color4 ClearColor { get; set; } = Color4.Black;

    protected FlexFrameworkMain Engine { get; private set; }

    internal void SetEngine(FlexFrameworkMain engine)
    {
        Engine = engine;
    }

    public abstract void Init();
    public abstract int GetLayerId(string name);
    public abstract void EnqueueDrawData(int layerId, IDrawData drawData);
    public abstract void UsePostProcessor(PostProcessor postProcessor);
    public abstract void UseSkybox(Texture2D skyboxTexture, CameraData cameraData);
    public abstract void Update(UpdateArgs args);
    public abstract void Render();
    public abstract void Dispose();
}