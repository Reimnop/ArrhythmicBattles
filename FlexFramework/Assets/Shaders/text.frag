#version 430
layout(location = 0) out vec4 fragColor;

in vec2 UV;
in vec4 Color;
flat in int Index;

layout(location = 1) uniform sampler2D atlas[16];
layout(location = 17) uniform vec4 overlayColor;

// layout(location = 18) uniform float width = 0.45;
// layout(location = 19) uniform float edge = 0.25;

const float pxRange = 4.0; // TODO: Don't hardcode this

void main() {
    vec4 outCol = vec4(1.0);
    if (Index >= 0) {
        vec3 msdf = texture(atlas[Index], UV).rgb;
        float dist = max(min(msdf.r, msdf.g), min(max(msdf.r, msdf.g), msdf.b));
        float alpha = clamp((dist - 0.5) * pxRange + 0.5, 0.0, 1.0);
        outCol = vec4(vec3(1.0), alpha);
    }

    fragColor = outCol * Color * overlayColor;
}