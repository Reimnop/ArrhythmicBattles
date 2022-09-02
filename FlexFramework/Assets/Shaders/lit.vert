#version 150

#extension GL_ARB_explicit_attrib_location : enable
#extension GL_ARB_explicit_uniform_location : enable

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aUv;
layout(location = 3) in vec4 aColor;

layout(location = 0) uniform mat4 mvp;
layout(location = 1) uniform mat4 model;

out vec2 Uv;
out vec3 Normal;
out vec4 Color;

void main() {
    Uv = aUv;
    Normal = aNormal * mat3(transpose(inverse(model)));
    Color = aColor;
    gl_Position = vec4(aPos, 1.0) * mvp;
}
