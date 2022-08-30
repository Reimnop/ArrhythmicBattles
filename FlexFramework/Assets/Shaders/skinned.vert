#version 150

#extension GL_ARB_explicit_attrib_location : enable
#extension GL_ARB_explicit_uniform_location : enable

const int MAX_BONES = 100;

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aUv;
layout(location = 2) in vec4 aColor;
layout(location = 3) in ivec4 aBoneIds;
layout(location = 4) in vec4 aWeights;

layout(location = 4) uniform mat4 bones[MAX_BONES];

layout(location = 0) uniform mat4 mvp;

out vec2 Uv;
out vec4 Color;

void main() {
    vec4 totalPos = vec4(0.0);
    for (int i = 0; i < 4; i++) {
        if (aBoneIds[i] == -1) {
            continue;
        }
        
        if (aBoneIds[i] >= MAX_BONES) {
            totalPos = vec4(aPos, 1.0);
            break;
        }
        
        totalPos += (vec4(aPos, 1.0) * bones[aBoneIds[i]]) * aWeights[i];
    }
    
    Uv = aUv;
    Color = aColor;
    gl_Position = totalPos * mvp;
}
