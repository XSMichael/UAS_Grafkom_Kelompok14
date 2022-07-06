#version 430

out vec4 outputColor;

in vec3 TexCoords;

uniform samplerCube cubemap;

void main() {
	outputColor = texture(cubemap, TexCoords);
}
