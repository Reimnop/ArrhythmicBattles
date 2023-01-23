using FlexFramework.Logging;
using FlexFramework.Util.Exceptions;

namespace FlexFramework.Core;

public class SceneManager
{
    public Scene CurrentScene { get; private set; } = null!;

    private FlexFrameworkMain engine;
    
    internal SceneManager(FlexFrameworkMain engine)
    {
        this.engine = engine;
    }

    public Scene LoadScene(Scene scene)
    {
        engine.LogMessage(this, Severity.Info, null, $"Loading scene [{scene.GetType().Name}]");

        if (CurrentScene is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        scene.InitInternal(engine);
        scene.Init();
        CurrentScene = scene;

        return scene;
    }

    public T LoadScene<T>(params object?[]? args) where T : Scene
    {
        T? scene = (T?) Activator.CreateInstance(typeof(T), args);

        if (scene == null)
        {
            throw new LoadSceneException(typeof(T));
        }

        return (T) LoadScene(scene);
    }
}