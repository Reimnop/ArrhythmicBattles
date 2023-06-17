using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.IO;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
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

public class GameScene : ABScene
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
    private readonly OrthographicCamera camera;
    private readonly ScreenManager screenManager;
    private Box2 currentScreenBounds;

    // Other things
    private readonly PhysicsWorld physicsWorld;
    private DebugScreen? debugScreen;

#if DEBUG
    private ScopedInputProvider? freeCamInputProvider;

    private readonly PerspectiveCamera freeCamCamera = new();
    private float freeCamYaw;
    private float freeCamPitch;
#endif

    public GameScene(ABContext context) : base(context)
    {
        Engine.CursorState = CursorState.Grabbed;
        currentScreenBounds = new Box2(Vector2.Zero, Engine.ClientSize);

        // Init resources
        physicsWorld = new PhysicsWorld();
        screenManager = new ScreenManager(currentScreenBounds, child => child);

        // Init entities
        var resourceManager = Context.ResourceManager;
        var mapMeta = resourceManager.Load<MapMeta>("Maps/Playground.json");

        mapEntity = EntityManager.Create(() => new MapEntity(resourceManager, mapMeta, physicsWorld, Context.Settings));
        inputProvider = Context.InputSystem.AcquireInputProvider();
        playerEntity = EntityManager.Create(() => new PlayerEntity(inputProvider, resourceManager, physicsWorld, 0.0f, 0.0f));
        playerEntity.Position = Vector3.UnitY * 4.0f;

        camera = new OrthographicCamera();
        camera.Size = 10.0f;
        camera.DepthNear = 0.1f;
        camera.DepthFar = 1000.0f;

        // Init post processing
        bloom = new Bloom();
        tonemapper = new Exposure();
        tonemapper.ExposureValue = 1.2f;
        edgeDetect = new EdgeDetect();
    }

    public override void Update(UpdateArgs args)
    {
        base.Update(args);
        
        // Update physics before everything else
        physicsWorld.Update(args);

        if (inputProvider.GetKeyDown(Keys.F3))
        {
            if (debugScreen == null)
            {
                debugScreen = new DebugScreen(Engine, this);
            }
            else
            {
                debugScreen = null;
            }
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
            Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, playerEntity.Yaw) * Quaternion.FromAxisAngle(Vector3.UnitX, playerEntity.Pitch);
            // Vector3 backward = Vector3.Transform(Vector3.UnitZ, rotation);
            // camera.Position = playerEntity.Position + new Vector3(0.0f, 0.75f, 0.0f) + backward * 3.5f;
            // camera.Rotation = rotation;

            Vector3 cameraPos = playerEntity.Position + new Vector3(0.0f, 0.0f, 500.0f);
            camera.Position = Vector3.Lerp(camera.Position, cameraPos, 2.5f * args.DeltaTime);
            camera.Rotation = rotation;
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
        
        screenManager.Update(args);
        
        // Update debug screen
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
        
        var alphaClipArgs = new RenderArgs(commandList, LayerType.AlphaClip, MatrixStack, cameraData);
        var opaqueArgs = new RenderArgs(commandList, LayerType.Opaque, MatrixStack, cameraData);
        
        // render player
        EntityManager.Invoke(playerEntity, entity => entity.Render(opaqueArgs));
        
        // render map
        EntityManager.Invoke(mapEntity, entity => entity.Render(opaqueArgs));

        // render gui
        var guiCameraData = GuiCamera.GetCameraData(Engine.ClientSize);
        var guiArgs = new RenderArgs(commandList, LayerType.Gui, MatrixStack, guiCameraData);
        
        screenManager.Render(guiArgs);
        
        // Render debug screen
        debugScreen?.Render(guiArgs);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        bloom.Dispose();
        tonemapper.Dispose();
        edgeDetect.Dispose();
        physicsWorld.Dispose();
        skyboxRenderer.Dispose();
        inputProvider.Dispose();
        screenManager.Dispose();
    }
}