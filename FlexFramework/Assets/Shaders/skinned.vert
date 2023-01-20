#version 430
const int MAX_BONES = 100;

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aUv;
layout(location = 3) in vec4 aColor;
layout(location = 4) in ivec4 aBoneIds;
layout(location = 5) in vec4 aWeights;

layout(location = 0) uniform mat4 mvp;
layout(location = 1) uniform mat4 model;
layout(location = 9) uniform mat4 bones[MAX_BONES];

out vec2 Uv;
out vec3 Normal;
out vec4 Color;

void main() {
    mat4 boneTransform = mat4(0.0);
    for (int i = 0; i < 4; i++) {
        if (aBoneIds[i] == -1) {
            continue;
        }
        if (aBoneIds[i] >= MAX_BONES) {
            boneTransform = mat4(1.0);
            break;
        }

        boneTransform += bones[aBoneIds[i]] * aWeights[i];
    }
    
    Uv = aUv;
    Color = aColor;
    Normal = vec3(vec4(aNormal, 0.0) * boneTransform) * mat3(transpose(inverse(model)));
    gl_Position = vec4(aPos, 1.0) * boneTransform * mvp;
}
