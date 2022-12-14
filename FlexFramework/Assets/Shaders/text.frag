#version 430
layout(location = 0) out vec4 fragColor;

in vec2 UV;
in vec4 Color;
flat in int Colored;
flat in int Index;

layout(location = 1) uniform sampler2D atlas[16];
layout(location = 17) uniform vec4 overlayColor;

void main() {
    vec4 outCol = vec4(1.0);
    if (Index >= 0) {
        vec4 texCol = texture(atlas[Index], UV);
        outCol = Colored == 1 ? texCol : vec4(1.0, 1.0, 1.0, texCol.b);
    }

    fragColor = outCol * Color * overlayColor;
}