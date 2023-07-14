using ArrhythmicBattles.Core;
using ArrhythmicBattles.Core.Input;
using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Game.Content;
using ArrhythmicBattles.UserInterface;
using FlexFramework.Core;
using FlexFramework.Core.Data;
using FlexFramework.Core.Entities;
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
    private readonly IInputMethod inputMethod;
    private readonly ImageEntity inputIndicator;
    private Vector2 movement;

    // Other things
    private readonly EntityManager entityManager = new();
    private readonly PhysicsWorld physicsWorld = new(-19.62f, 0.15f);
    private readonly MatrixStack matrixStack = new();
    private DebugScreen? debugScreen;

#if DEBUG
    private ScopedInputProvider? freeCamInputProvider;

    private readonly PerspectiveCamera freeCamCamera = new();
    private float freeCamYaw;
    private float freeCamPitch;
#endif

    public GameScene(ABContext context, IInputMethod inputMethod, Character character) : base(context)
    {
        this.inputMethod = inputMethod;
        
        Engine.CursorState = CursorState.Grabbed;
        currentScreenBounds = new Box2(Vector2.Zero, Engine.ClientSize);

        // Init resources
        screenManager = new ScreenManager(currentScreenBounds, Engine.DpiScale, child => child);

        // Init entities
        var resourceManager = Context.ResourceManager;
        var mapMeta = resourceManager.Get<MapMeta>("Maps/Playground.json");
        var inputIndicatorTexture = resourceManager.Get<TextureSampler>("Textures/InputIndicator.png");
        inputIndicator = new ImageEntity(inputIndicatorTexture);
        
        inputProvider = Context.InputSystem.AcquireInputProvider();

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
        
        // Update movement
        movement = inputMethod.GetMovement();

        // Open debug screen
        if (inputProvider.GetKeyDown(Keys.F3))
        {
            debugScreen = debugScreen == null ? new DebugScreen(Engine, this) : null;
        }
        
        // Pause game
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
            camera.Position = Vector3.Lerp(camera.Position, cameraPos, 5.0f * args.DeltaTime);
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
        
        // Update screens
        var screenBounds = new Box2(Vector2.Zero, Engine.ClientSize);
        if (screenBounds != currentScreenBounds)
        {
            currentScreenBounds = screenBounds;
            screenManager.Resize(currentScreenBounds, Engine.DpiScale);
        }

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
        
        // var alphaClipArgs = new RenderArgs(commandList, LayerType.AlphaClip, matrixStack, cameraData);
        var opaqueArgs = new RenderArgs(commandList, LayerType.Opaque, matrixStack, cameraData);
        
        // Render player and map
        entityManager.Invoke(playerEntity, entity => entity.Render(opaqueArgs));
        entityManager.Invoke(mapEntity, entity => entity.Render(opaqueArgs));

        // Render gui
        var guiCameraData = guiCamera.GetCameraData((Vector2i) screenManager.ComputedBounds.Size);
        var guiArgs = new RenderArgs(commandList, LayerType.Gui, matrixStack, guiCameraData);

        // Render input indicator
        if (movement != Vector2.Zero)
        {
            // Project player center onto screen
            var playerPosition = playerEntity.Position;
            var playerPositionProjected = new Vector4(playerPosition, 1.0f) * cameraData.View * cameraData.Projection;
            playerPositionProjected /= playerPositionProjected.W;
            
            // Unproject player center onto gui
            var playerPositionUnprojected = new Vector4(playerPositionProjected.X, playerPositionProjected.Y, 0.0f, 1.0f) * guiCameraData.Projection.Inverted() * guiCameraData.View.Inverted();
            playerPositionUnprojected /= playerPositionUnprojected.W;
            var playerPositionGui = playerPositionUnprojected.Xy;
            
            // Calculate indicator angle
            var indicatorAngle = MathF.Atan2(movement.Y, movement.X);
            
            // Render indicator
            matrixStack.Push();
            matrixStack.Rotate(-Vector3.UnitZ, indicatorAngle);
            matrixStack.Scale(384.0f, 384.0f, 1.0f);
            matrixStack.Translate(playerPositionGui.X, playerPositionGui.Y, 0.0f);
            inputIndicator.Render(guiArgs);
            matrixStack.Pop();
        }
        
        screenManager.Render(guiArgs);
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