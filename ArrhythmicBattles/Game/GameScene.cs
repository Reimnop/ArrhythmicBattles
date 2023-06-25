using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Game.Content;
using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.Rendering;
using FlexFramework.Core.Rendering.BackgroundRenderers;
using FlexFramework.Core.Rendering.PostProcessing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ArrhythmicBattles.Game;

public class GameScene : ABScene, IDisposable
{
    // Entities
    private readonly MapEntity mapEntity;
    private readonly PlayerEntity playerEntity;

    // Post processing
    private readonly Bloom bloom;
    private readonly Exposure tonemapper;
    private readonly EdgeDetect edgeDetect;
    
    // Rendering stuff
    private readonly ProceduralSkyboxRenderer skyboxRenderer = new();
    private readonly GameLighting lighting = new();
    private readonly ScopedInputProvider inputProvider;
    private readonly GuiCamera guiCamera = new();
    private readonly OrthographicCamera camera = new()
    {
        Size = 10.0f,
        DepthNear = 0.1f,
        DepthFar = 1000.0f
    };
    private readonly ScreenManager screenManager;
    private Box2 currentScreenBounds;
    
    // Game content
    private readonly Character character;

    // Other things
    private readonly EntityManager entityManager = new();
    private readonly PhysicsWorld physicsWorld = new();
    private readonly MatrixStack matrixStack = new();
    private DebugScreen? debugScreen;

#if DEBUG
    private ScopedInputProvider? freeCamInputProvider;

    private readonly PerspectiveCamera freeCamCamera = new();
    private float freeCamYaw;
    private float freeCamPitch;
#endif

    public GameScene(ABContext context, Character character) : base(context)
    {
        this.character = character;
        
        Engine.CursorState = CursorState.Grabbed;
        currentScreenBounds = new Box2(Vector2.Zero, Engine.ClientSize);

        // Init resources
        screenManager = new ScreenManager(currentScreenBounds, child => child);

        // Init entities
        var resourceManager = Context.ResourceManager;
        var mapMeta = resourceManager.Get<MapMeta>("Maps/Playground.json");
        
        inputProvider = Context.InputSystem.AcquireInputProvider();
        var inputMethod = new KeyboardInputMethod(inputProvider);

        mapEntity = entityManager.Create(() => new MapEntity(resourceManager, mapMeta, physicsWorld, Context.Settings));
        playerEntity = entityManager.Create(() => new PlayerEntity(character, inputMethod, resourceManager, physicsWorld));
        playerEntity.Position = Vector3.UnitY * 4.0f;
        
        // Init post processing
        bloom = new Bloom();
        tonemapper = new Exposure();
        tonemapper.ExposureValue = 1.2f;
        edgeDetect = new EdgeDetect();
    }

    public override void Update(UpdateArgs args)
    {
        // Update entities and physics before everything else
        entityManager.Update(args);
        physicsWorld.Update(args);

        if (inputProvider.GetKeyDown(Keys.F3))
        {
            debugScreen = debugScreen == null ? new DebugScreen(Engine, this) : null;
        }
        
        if (inputProvider.GetKeyDown(Keys.Escape))
        {
            screenManager.Open(new PauseScreen(Engine, screenManager, Context));
            Engine.CursorState = CursorState.Normal;
        }
        
        // Teleport player to origin if they fall off the map
        if (playerEntity.Position.Y < -10.0f)
        {
            playerEntity.Position = Vector3.UnitY * 4.0f;
        }

#if DEBUG
        if (inputProvider.GetKeyDown(Keys.F1))
        {
            // Toggle free cam
            freeCamInputProvider = Context.InputSystem.AcquireInputProvider();
        }
        
        if (freeCamInputProvider != null && freeCamInputProvider.GetKeyDown(Keys.F1))
        {
            freeCamInputProvider.Dispose();
            freeCamInputProvider = null;
        }
#endif
        
#if DEBUG
        // Update camera pos
        if (freeCamInputProvider == null)
        {
#endif
            var cameraPos = playerEntity.Position + new Vector3(0.0f, 0.0f, 500.0f);
            camera.Position = Vector3.Lerp(camera.Position, cameraPos, 2.5f * args.DeltaTime);
#if DEBUG
        }
        else
        {
            // Free cam
            Vector3 forward = Vector3.Transform(-Vector3.UnitZ, freeCamCamera.Rotation);
            Vector3 right = Vector3.Transform(Vector3.UnitX, freeCamCamera.Rotation);
            Vector2 move = freeCamInputProvider.Movement;
            
            Vector3 moveDir = (forward * move.Y + right * move.X) * 5.0f;
            freeCamCamera.Position += moveDir * args.DeltaTime;
            
            freeCamYaw -= freeCamInputProvider.MouseDelta.X / 480.0f;
            freeCamPitch -= freeCamInputProvider.MouseDelta.Y / 480.0f;
            freeCamPitch = Math.Clamp(freeCamPitch, -MathF.PI * 0.5f, MathF.PI * 0.5f);
            freeCamCamera.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, freeCamYaw) * Quaternion.FromAxisAngle(Vector3.UnitX, freeCamPitch);
        }
#endif
        
        var screenBounds = new Box2(Vector2.Zero, Engine.ClientSize);
        if (screenBounds != currentScreenBounds)
        {
            currentScreenBounds = screenBounds;
            screenManager.Resize(currentScreenBounds);
        }
        
        // Update screens
        screenManager.Update(args);
        debugScreen?.Update(args);
    }

    protected override void RenderScene(CommandList commandList)
    {
        commandList.AddPostProcessor(bloom);
        commandList.AddPostProcessor(tonemapper);
        commandList.AddPostProcessor(edgeDetect);
        
#if DEBUG
        var cameraData = freeCamInputProvider != null ? freeCamCamera.GetCameraData(Engine.ClientSize) : camera.GetCameraData(Engine.ClientSize);
#else
        var cameraData = camera.GetCameraData(Engine.ClientSize);
#endif

        commandList.UseBackgroundRenderer(skyboxRenderer, cameraData);
        commandList.UseLighting(lighting);
        
        var alphaClipArgs = new RenderArgs(commandList, LayerType.AlphaClip, matrixStack, cameraData);
        var opaqueArgs = new RenderArgs(commandList, LayerType.Opaque, matrixStack, cameraData);
        
        // render player
        entityManager.Invoke(playerEntity, entity => entity.Render(opaqueArgs));
        
        // render map
        entityManager.Invoke(mapEntity, entity => entity.Render(opaqueArgs));

        // render gui
        var guiCameraData = guiCamera.GetCameraData(Engine.ClientSize);
        var guiArgs = new RenderArgs(commandList, LayerType.Gui, matrixStack, guiCameraData);
        
        screenManager.Render(guiArgs);
        
        // Render debug screen
        debugScreen?.Render(guiArgs);
    }

    public void Dispose()
    {
        entityManager.Dispose();
        bloom.Dispose();
        tonemapper.Dispose();
        edgeDetect.Dispose();
        physicsWorld.Dispose();
        skyboxRenderer.Dispose();
        inputProvider.Dispose();
        screenManager.Dispose();
    }
}