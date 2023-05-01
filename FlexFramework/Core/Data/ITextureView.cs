﻿namespace FlexFramework.Core.Data;

public interface ITextureView
{
    int Width { get; }
    int Height { get; }
    PixelFormat Format { get; }
    IBufferView Data { get; }
}