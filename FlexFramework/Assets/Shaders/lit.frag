#version 430
layout(location = 0) out vec4 fragColor;

layout(location = 2) uniform bool hasTexture;
layout(location = 3) uniform sampler2D _texture;
layout(location = 4) uniform vec4 color;
layout(location = 5) uniform vec3 ambientColor;
layout(location = 6) uniform vec3 lightDir; // should be normalized
layout(location = 7) uniform vec3 lightColor;
layout(location = 8) uniform float lightIntensity;

in vec2 Uv;
in vec3 Normal;
in vec4 Color;

void main() {
    float light = max(min(dot(Normal, -lightDir), 1.0), 0.0);
    vec3 lightCol = lightColor * light * lightIntensity + ambientColor;
    
    vec4 outColor = hasTexture ? texture(_texture, Uv) : vec4(1.0);
    outColor *= Color;
    
    if (outColor.a < 0.01)
        discard;
    
    fragColor = outColor * color * vec4(lightCol, 1.0);
}
