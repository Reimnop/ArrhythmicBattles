#version 430
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec4 aColor;
layout(location = 2) in vec2 aUv;
layout(location = 3) in int aColored;
layout(location = 4) in int aIndex;

out vec2 UV;
out vec4 Color;
flat out int Colored;
flat out int Index;

layout(location = 0) uniform mat4 mvp;

void main() {
    UV = aUv;
    Color = aColor;
    Colored = aColored;
    Index = aIndex;
    gl_Position = vec4(aPos, 0.0, 1.0) * mvp;
}