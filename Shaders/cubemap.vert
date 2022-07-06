#version 430 core

layout (location = 0) in vec3 aPosition;

uniform mat4 view;
uniform mat4 projection;

out vec3 TexCoords;

void main() {
	vec4 pos = vec4(aPosition, 1.0) * view * projection;
	gl_Position = pos.xyww;

	TexCoords = aPosition;
}