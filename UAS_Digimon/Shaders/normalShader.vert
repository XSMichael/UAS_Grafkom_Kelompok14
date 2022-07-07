#version 430 core

layout (location = 0) in vec3 aPosition; //Vertex's Position
layout (location = 1) in vec3 aNormal; // Normals

uniform mat4 model;
uniform mat4 normalMat; //Normals Transformation Matrix
uniform mat4 view;
uniform mat4 projection;

out vec3 normal;
out vec3 FragPos;

void main() {
	gl_Position = vec4(aPosition, 1.0) * model * view * projection;

	FragPos = vec3(vec4(aPosition, 1.0) * model);
	normal = aNormal * mat3(normalMat);
}