#version 430

out vec4 outputColor;

in vec3 normal; // Transformed normals
in vec3 FragPos; // Transformed vertex position

//uniform vec4 objColor;

uniform vec3 viewPos; // Posisi Kamera

struct Material{
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform Material material;
struct DirectLight{
    vec3 lightColor;
    float ambientStre;
    float specStre;

    vec3 lightDir;
};

struct PointLight{
    vec3 lightPos;
    vec3 lightColor;
    float ambientStre;
    float specStre;

};

struct SpotLight{
    vec3 lightPos;
    vec3 lightColor;
    float ambientStre;
    float specStre;
    
    vec3 spotDir;
    float spotAngleCos;
};

# define directNumber 10
uniform DirectLight directList [directNumber];

# define pointNumber 10
uniform PointLight pointList [pointNumber];

# define spotNumber 5
uniform SpotLight spotList [spotNumber];

vec3 CalcDirect(DirectLight light, vec3 norm) {
//Ambient
    vec3 ambient = light.ambientStre * light.lightColor * material.ambient;

    // Normalize
    //vec3 norm = normalize(normal);
    //vec3 lightDir = normalize(lightPos - FragPos);
    //vec3 spotDir = normalize(vec3(1,0,1));
    vec3 lightDir = light.lightDir;

    //Diffuse
    float diff = max(dot(norm, lightDir), 0);
    vec3 diffuse = diff * light.lightColor * material.diffuse;
    //    diffuse = diff * lightColor;


    //Specular
    //float specularStrength = 1;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0), 32);
    vec3 specular = light.specStre * spec * light.lightColor * material.specular;
    //specular = specularStrength * spec * lightColor;

    vec3 result = (ambient + diffuse + specular);
    return result;
}

vec3 CalcPoint(PointLight light, vec3 norm) {
//Ambient
    vec3 ambient = light.ambientStre * light.lightColor * material.ambient;

    // Normalize
    vec3 lightDir = normalize(light.lightPos - FragPos);


    //Diffuse
    float diff = max(dot(norm, lightDir), 0);
    vec3 diffuse = diff * light.lightColor  * material.diffuse;


    //Specular
    float specularStrength = 1;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0), 32);
    vec3 specular = light.specStre * spec * light.lightColor * material.specular;

    vec3 result = (ambient + diffuse + specular);
    return result;

}

vec3 CalcSpot(SpotLight light, vec3 norm) {
    //Ambient
    //float ambientStrength = 0.1;
    vec3 ambient = light.ambientStre * light.lightColor* material.ambient;

    // Normalize
    //vec3 norm = normalize(normal);
    vec3 lightDir = normalize(light.lightPos - FragPos);
    //vec3 spotDir = light.spotDir;
    //vec3 lightDir = vec3(0,-1,0);

    vec3 diffuse;
    vec3 specular;
    if (dot(lightDir, light.spotDir) >= light.spotAngleCos)
    {
        //Diffuse
        float diff = max(dot(norm, lightDir), 0);
        //vec3 diffuse = diff * lightColor;
        diffuse = diff * light.lightColor * material.diffuse;


        //Specular
        //float specularStrength = 1;
        vec3 viewDir = normalize(viewPos - FragPos);
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0), 32);
        //vec3 specular = specularStrength * spec * lightColor;
        specular = light.specStre * spec * light.lightColor * material.specular;
    }
    else
    {
        vec3 diffuse = vec3(0,0,0);
        vec3 specular = vec3(0,0,0);
    }

    vec3 result = (ambient + diffuse + specular);
    return result;

}

void main() {
    // Normalize
    vec3 norm = normalize(normal);

    vec3 result = vec3(0,0,0);
    for(int i=0;i<directNumber;i++){
        result+=CalcDirect(directList[i], norm);
    }
    for(int i=0;i<pointNumber;i++){
        result+=CalcPoint(pointList[i], norm);
    }
    for(int i=0;i<spotNumber;i++){
        result+=CalcSpot(spotList[i], norm);
    }

    //vec3 result = (ambient + diffuse + specular) * vec3(material.diffuse);

    outputColor = vec4(result, 1.0);
}


