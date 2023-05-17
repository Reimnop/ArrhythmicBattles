using FlexFramework.Core.Rendering.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace FlexFramework.Core.Rendering.Renderers;

public class DefaultRenderBuffer : IRenderBuffer, IGBuffer, IDisposable
{
    public Vector2i Size { get; private set; }

    public FrameBuffer WorldFrameBuffer { get; }
    public FrameBuffer GuiFrameBuffer { get; }

    // Texture attachments
    public Texture2D WorldFinal => worldFinal;
    public Texture2D WorldColor => worldColor;
    public Texture2D WorldNormal => worldNormal;
    public Texture2D WorldDepth => worldDepth;
    public Texture2D GuiColor => guiColor;
    
    private Texture2D worldFinal;
    private Texture2D worldColor;
    private Texture2D worldNormal;
    private Texture2D worldDepth;
    private Texture2D guiColor;

    public DefaultRenderBuffer(Vector2i size)
    {
        // Initialize framebuffers
        WorldFrameBuffer = new FrameBuffer("world");
        WorldFrameBuffer.DrawBuffers(DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1);
        
        GuiFrameBuffer = new FrameBuffer("gui");
        
        // Initialize textures
        CreateTextures(size, out worldFinal, out worldColor, out worldNormal, out worldDepth, out guiColor);
        
        // Attach textures to framebuffers
        WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, worldColor);
        WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment1, worldNormal);
        WorldFrameBuffer.Texture(FramebufferAttachment.DepthAttachment, worldDepth);
        GuiFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, guiColor);
    }

    public void Resize(Vector2i size)
    {
        if (Size != size)
        {
            Size = size;

            // Dispose old textures
            worldColor.Dispose();
            worldNormal.Dispose();
            worldDepth.Dispose();
            guiColor.Dispose();
            
            // Initialize textures
            CreateTextures(size, out worldFinal, out worldColor, out worldNormal, out worldDepth, out guiColor);
        
            // Attach textures to framebuffers
            WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, worldColor);
            WorldFrameBuffer.Texture(FramebufferAttachment.ColorAttachment1, worldNormal);
            WorldFrameBuffer.Texture(FramebufferAttachment.DepthAttachment, worldDepth);
            GuiFrameBuffer.Texture(FramebufferAttachment.ColorAttachment0, guiColor);
        }
    }

    private static void CreateTextures(Vector2i size, out Texture2D worldFinal, out Texture2D worldColor, out Texture2D worldNormal, out Texture2D worldDepth, out Texture2D guiColor)
    {
        worldFinal = new Texture2D("world_final", size.X, size.Y, SizedInternalFormat.Rgba16f);
        worldColor = new Texture2D("world_color", size.X, size.Y, SizedInternalFormat.Rgba16f);
        worldNormal = new Texture2D("world_normal", size.X, size.Y, SizedInternalFormat.Rgba16f);
        worldDepth = new Texture2D("world_depth", size.X, size.Y, SizedInternalFormat.DepthComponent32f);
        guiColor = new Texture2D("gui_color", size.X, size.Y, SizedInternalFormat.Rgba16f, samples: 4);
    }

    public void BlitToBackBuffer(Vector2i size)
    {
        GL.BlitNamedFramebuffer(GuiFrameBuffer.Handle, 0, 
            0, 0, Size.X, Size.Y, 
            0, 0, size.X, size.Y,
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
    }

    public void Dispose()
    {
        WorldFrameBuffer.Dispose();
        GuiFrameBuffer.Dispose();
        worldColor.Dispose();
        worldNormal.Dispose();
        worldDepth.Dispose();
        guiColor.Dispose();
    }
}