using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using LearnOpenTK.Common;

namespace UAS_Digimon
{
    static class Constants
    {
        public const string path_Shaders = "../../../Shaders/";
        public const string path_Assets = "../../../Assets/";
    }
    class Window : GameWindow
    {
        private readonly string path = "../../../";

        List<Asset3d> objectList = new List<Asset3d>();
        List<Asset3d> directLightList = new List<Asset3d>();
        List<Asset3d> pointLightList = new List<Asset3d>();
        List<Asset3d> spotLightList = new List<Asset3d>();

        private Cubemap cubemap;

        Camera camera;
        private bool tpv = true; //tpv
        private int face = 1; //0= +z, 1= -z, 2= +x,3= -x
        private int lastFace = 1;
        private Vector3 cameraDegree = new Vector3(0, 0, 0);
        private List<Vector3> collData = new List<Vector3>();
        private bool firstMove = true;
        private Vector3 startPosCamera = new Vector3(0, 8, 50);
        private float cameraSpeed = 6.0f;
        private float sensitivity = 0.2f;
        private Vector2 lastPos;

        private int renderSetting = 1;

        private float ambientStrengthSpotLight = 0.1f;
        private float specStrengthSpotLight = 0.1f;
        private Vector3 spotLightDirection = new Vector3(0, 0, -1);
        private float spotLightAngle = 30f;

        //Assets Path
        #region Assets Path
        //Castle
        //string cs= Constants.path_Assets + "Castle/wall3.obj";
        string castleWall = Constants.path_Assets + "Castle/wall.obj";
        string castleRoof = Constants.path_Assets + "Castle/roof.obj";
        string castlePillar = Constants.path_Assets + "Castle/pillar.obj";
        string castleWall2 = Constants.path_Assets + "Castle/wall2.obj";
        string castleWindow = Constants.path_Assets + "Castle/window.obj";

        string playerBody = Constants.path_Assets + "botamonwhitebody.obj";
        string playerEye = Constants.path_Assets + "botamonwhiteeyes.obj";

        string botamon = Constants.path_Assets + "Digimon/botamon/botamon";
        string budmon = Constants.path_Assets + "Digimon/budmon/budmon";
        string kapurimon = Constants.path_Assets + "Digimon/kapurimon/kapurimon";
        string koromon = Constants.path_Assets + "Digimon/koromon/koromon";

        string flagFlag = Constants.path_Assets + "Flag/flag.obj";
        string flagPole = Constants.path_Assets + "Flag/pole.obj";

        string flower = Constants.path_Assets + "Flowers/flower";

        string fountain = Constants.path_Assets + "Fountain/fountain.obj";

        string ground = Constants.path_Assets + "Ground/ground.obj";

        string houseV1Atap = Constants.path_Assets + "HouseV1/atap.obj";
        string houseV1Badan = Constants.path_Assets + "HouseV1/badan.obj";
        string houseV1Kayu1 = Constants.path_Assets + "HouseV1/kayu1.obj";
        string houseV1Kayu2 = Constants.path_Assets + "HouseV1/kayu2.obj";

        string houseV2Atap = Constants.path_Assets + "HouseV2/atap.obj";
        string houseV2Badan = Constants.path_Assets + "HouseV2/badan.obj";
        string houseV2Chimney = Constants.path_Assets + "HouseV2/chimney.obj";
        string houseV2Door = Constants.path_Assets + "HouseV2/door.obj";
        string houseV2DoorHandle = Constants.path_Assets + "HouseV2/doorhandle.obj";
        string houseV2Window = Constants.path_Assets + "HouseV2/window.obj";
        string houseV2WindowFrame = Constants.path_Assets + "HouseV2/windowframe.obj";

        string rocks = Constants.path_Assets + "Rocks/rock";

        string shop = Constants.path_Assets + "Shop/shop";

        string tower = Constants.path_Assets + "Tower/tower";

        string treeV1 = Constants.path_Assets + "TreeV1/treeV1_";

        string treeV2 = Constants.path_Assets + "TreeV2/treeV2_";

        string well = Constants.path_Assets + "Well/well";
        #endregion

        #region Object
        Asset3d Player;
        Asset3d Castle;
        Asset3d Flag;
        Asset3d Flowers;
        Asset3d Fountain;
        Asset3d Ground;
        Asset3d HouseV1;
        Asset3d HouseV2;
        Asset3d Rocks;
        Asset3d Shop;
        Asset3d Tower;
        Asset3d Tree1;
        Asset3d Tree2;
        Asset3d Well;
        Asset3d flash;
        #endregion

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        protected override void OnLoad()
        {
            base.OnLoad();

            camera = new Camera(startPosCamera, Size.X / (float)Size.Y);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);


            //  Guide to selecting vertex and fragment shaders:
            //  CreateCuboid:
            //      if Instanced:
            //          if createCuboid(0, 0, 0, 1, false, false):
            //              instanced.vert & shader.frag
            //
            //          if createCuboid(0, 0, 0, 1, true, false):
            //              instancedNormal.vert & normalShader.frag
            //
            //          if createCuboid(0, 0, 0, 1, false, true):
            //              instancedTex.vert & texShader.frag
            //
            //      else:
            //          if createCuboid(0, 0, 0, 1, false, false):
            //              shader.vert & shader.frag
            //
            //          if createCuboid(0, 0, 0, 1, true, false):
            //              normalShader.vert & normalShader.frag
            //
            //          if createCuboid(0, 0, 0, 1, false, true):
            //              texShader.vert & texShader.frag

            //Player
            Player = new Asset3d(playerBody, "normalShader.vert", "normalShader.frag", new Vector3(255, 212, 71) / 255, new Vector3(204, 145, 67) / 255, new Vector3(0, 0, 0));
            var face = new Asset3d(playerEye, "normalShader.vert", "normalShader.frag", new Vector3(20, 20, 20) / 255, new Vector3(20, 20, 20) / 255, new Vector3(48, 46, 46) / 255);
            Player.child.Add(face);
            Player.translate(0, 0, 60);
            Player.rotate(Player.objectCenter, Player._euler[1], 180);
            rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, 0.1f, 180, 0.1f);
            objectList.Add(Player);

            //Castle
            Castle = new Asset3d(castleWall, "normalShader.vert", "normalShader.frag", new Vector3(255,255,255) / 255, new Vector3(255, 255, 255) / 255, new Vector3(0,0, 0) / 255);
            var child = new Asset3d(castleWall2, "normalShader.vert", "normalShader.frag", new Vector3(255, 255, 255) / 255, new Vector3(255, 255, 255) / 255, new Vector3(138, 135, 135) / 255);
            Castle.child.Add(child);
            child = new Asset3d(castlePillar, "normalShader.vert", "normalShader.frag", new Vector3(191, 191, 191) / 255, new Vector3(191, 191, 191) / 255, new Vector3(0, 0, 0));
            Castle.child.Add(child);
            child = new Asset3d(castleRoof, "normalShader.vert", "normalShader.frag", new Vector3(255, 0, 0) / 255, new Vector3(161, 24, 24) / 255, new Vector3(1, 1, 1));
            Castle.child.Add(child);
            child = new Asset3d(castleWindow, "normalShader.vert", "normalShader.frag", new Vector3(97, 97, 97) / 255, new Vector3(97, 97, 97) / 255, new Vector3(1, 1, 1));
            Castle.child.Add(child);

            objectList.Add(Castle);

            Asset3d character;
            //Botamon
            for (int i = 1; i <= 9; i++)
            {
                character = new Asset3d(botamon + i + "/body" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(54, 54, 54) / 255, new Vector3(54, 54, 54) / 255, new Vector3(1, 1, 1));
                child = new Asset3d(botamon + i + "/lefteye" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(247, 235, 64) / 255, new Vector3(247, 235, 64) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(botamon + i + "/righteye" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(247, 235, 64) / 255, new Vector3(247, 235, 64) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                objectList.Add(character);
            }
            //Budmon
            for (int i = 1; i <= 5; i++)
            {
                character = new Asset3d(budmon + i + "/body" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(140, 232, 74) / 255, new Vector3(140, 232, 74) / 255, new Vector3(1, 1, 1));
                child = new Asset3d(budmon + i + "/frontcone" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(245, 150, 250) / 255, new Vector3(245, 150, 250) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(budmon + i + "/leftcone" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(245, 150, 250) / 255, new Vector3(245, 150, 250) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(budmon + i + "/lefteye" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(budmon + i + "/rightcone" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(245, 150, 250) / 255, new Vector3(245, 150, 250) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(budmon + i + "/righteye" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(budmon + i + "/tail" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(47, 173, 40) / 255, new Vector3(47, 173, 40) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                objectList.Add(character);
            }
            //kapurimon
            for (int i = 1; i <= 3; i++)
            {
                character = new Asset3d(kapurimon + i + "/body" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(95, 122, 163) / 255, new Vector3(95, 122, 163) / 255, new Vector3(1, 1, 1));
                child = new Asset3d(kapurimon + i + "/leftear" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(207, 207, 207) / 255, new Vector3(207, 207, 207) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(kapurimon + i + "/lefteye" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(255, 255, 255) / 255, new Vector3(255, 255, 255) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(kapurimon + i + "/leftpupil" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(89, 30, 1) / 255, new Vector3(89, 30, 1) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(kapurimon + i + "/mouth" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(150, 15, 15) / 255, new Vector3(150, 15, 15) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(kapurimon + i + "/nose" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(33, 33, 33) / 255, new Vector3(33, 33, 33) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(kapurimon + i + "/rightear" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(207, 207, 207) / 255, new Vector3(207, 207, 207) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(kapurimon + i + "/righteye" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(255, 255, 255) / 255, new Vector3(255, 255, 255) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(kapurimon + i + "/rightpupil" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(89, 30, 1) / 255, new Vector3(89, 30, 1) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(kapurimon + i + "/tail" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(95, 122, 163) / 255, new Vector3(95, 122, 163) / 255, new Vector3(1, 1, 1));
                character.child.Add(child);
                objectList.Add(character);
            }
            //koromon
            for (int i = 1; i <= 3; i++)
            {
                character = new Asset3d(koromon + i + "/body" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(255, 181, 181) / 255, new Vector3(255, 181, 181) / 255, new Vector3(0, 0, 0));
                child = new Asset3d(koromon + i + "/eye" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(koromon + i + "/leftpupil" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(koromon + i + "/rightpupil" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1));
                character.child.Add(child);
                child = new Asset3d(koromon + i + "/teeth" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1));
                character.child.Add(child);

                objectList.Add(character);
            }

            //Flag
            Flag = new Asset3d(flagFlag, "normalShader.vert", "normalShader.frag", new Vector3(168, 15, 15) / 255, new Vector3(168, 15, 15) / 255, new Vector3(1, 1, 1));
            child = new Asset3d(flagPole, "normalShader.vert", "normalShader.frag", new Vector3(82, 32, 11) / 255, new Vector3(82, 32, 11) / 255, new Vector3(1, 1, 1));
            Flag.child.Add(child);
            objectList.Add(Flag);


            //Flowers
            for (int i = 1; i <= 2; i++)
            {
                Flowers = new Asset3d(flower + i + "/batang" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(48, 255, 0) / 255, new Vector3(48, 255, 0) / 255, new Vector3(1, 1, 1));
                child = new Asset3d(flower + i + "/petal" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(255, 21, 0) / 255, new Vector3(255, 21, 0) / 255, new Vector3(1, 1, 1));
                Flowers.child.Add(child);

                objectList.Add(Flowers);
            }

            //Fountain
            Fountain = new Asset3d(fountain, "normalShader.vert", "normalShader.frag", new Vector3(116, 117, 117) / 255, new Vector3(116, 117, 117) / 255, new Vector3(0, 0, 0));
            objectList.Add(Fountain);
            
            //Ground
            Ground = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(0, 130, 0) / 255, new Vector3(0, 130, 0) / 255, new Vector3(0, 0, 0));
            Ground.createCuboid(0, -100, 0, 200, true, false);
            objectList.Add(Ground);


            //HouseV1
            HouseV1 = new Asset3d(houseV1Atap, "normalShader.vert", "normalShader.frag", new Vector3(122, 58, 31) / 255, new Vector3(122, 58, 31) / 255, new Vector3(0, 0, 0));
            child = new Asset3d(houseV1Badan, "normalShader.vert", "normalShader.frag", new Vector3(237, 162, 130) / 255, new Vector3(237, 162, 130) / 255, new Vector3(0, 0, 0));
            HouseV1.child.Add(child);
            child = new Asset3d(houseV1Kayu1, "normalShader.vert", "normalShader.frag", new Vector3(1, 0.5f, 0.25f), new Vector3(122, 58, 31) / 255, new Vector3(0, 0, 0));
            HouseV1.child.Add(child);
            child = new Asset3d(houseV1Kayu2, "normalShader.vert", "normalShader.frag", new Vector3(1, 0.5f, 0.25f), new Vector3(122, 58, 31) / 255, new Vector3(0, 0, 0));
            HouseV1.child.Add(child);

            objectList.Add(HouseV1);

            //HouseV2
            HouseV2 = new Asset3d(houseV2Atap, "normalShader.vert", "normalShader.frag", new Vector3(122, 58, 31) / 255, new Vector3(122, 58, 31) / 255, new Vector3(0, 0, 0));
            child = new Asset3d(houseV2Badan, "normalShader.vert", "normalShader.frag", new Vector3(201, 143, 85) / 255, new Vector3(201, 143, 85) / 255, new Vector3(0, 0, 0));
            HouseV2.child.Add(child);
            child = new Asset3d(houseV2Chimney, "normalShader.vert", "normalShader.frag", new Vector3(79, 79, 79) / 255, new Vector3(79, 79, 79) / 255, new Vector3(0, 0, 0));
            HouseV2.child.Add(child);
            child = new Asset3d(houseV2Door, "normalShader.vert", "normalShader.frag", new Vector3(138, 84, 45) / 255, new Vector3(138, 84, 45) / 255, new Vector3(0, 0, 0));
            HouseV2.child.Add(child);
            child = new Asset3d(houseV2DoorHandle, "normalShader.vert", "normalShader.frag", new Vector3(250, 192, 67) / 255, new Vector3(250, 192, 67) / 255, new Vector3(0, 0, 0));
            HouseV2.child.Add(child);
            child = new Asset3d(houseV2Window, "normalShader.vert", "normalShader.frag", new Vector3(125, 231, 255) / 255, new Vector3(125, 231, 255) / 255, new Vector3(0, 0, 0));
            HouseV2.child.Add(child);
            child = new Asset3d(houseV2WindowFrame, "normalShader.vert", "normalShader.frag", new Vector3(99, 58, 17) / 255, new Vector3(99, 58, 17) / 255, new Vector3(0, 0, 0));
            HouseV2.child.Add(child);

            objectList.Add(HouseV2);


            string rocks = Constants.path_Assets + "Rocks/rock";

            string shop = Constants.path_Assets + "Shop/shop";

            string tower = Constants.path_Assets + "Tower/tower";

            string treeV1 = Constants.path_Assets + "TreeV1/treeV1_";

            string treeV2 = Constants.path_Assets + "TreeV2/treeV2_";

            string well = Constants.path_Assets + "Well/well";

            //Rocks
            for (int i = 1; i <= 4; i++)
            {
                Rocks = new Asset3d(rocks + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(77, 77, 77) / 255, new Vector3(77, 77, 77) / 255, new Vector3(1, 1, 1));

                objectList.Add(Rocks);
            }

            //Shop
            for (int i = 1; i <= 2; i++)
            {
                Shop = new Asset3d(shop + i + "/atap" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(128, 70, 19) / 255, new Vector3(128, 70, 19) / 255, new Vector3(0,0,0));
                child = new Asset3d(shop + i + "/barrel" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(66, 36, 13) / 255, new Vector3(66, 36, 13) / 255, new Vector3(0, 0, 0));
                Shop.child.Add(child);
                child = new Asset3d(shop + i + "/kain" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(222, 138, 64) / 255, new Vector3(222, 138, 64) / 255, new Vector3(0, 0, 0));
                Shop.child.Add(child);
                child = new Asset3d(shop + i + "/kayu" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(232, 151, 79) / 255, new Vector3(232, 151, 79) / 255, new Vector3(0, 0, 0));
                Shop.child.Add(child);
                objectList.Add(Shop);
            }

            //Tower
            for (int i = 1; i <= 2; i++)
            {
                Tower = new Asset3d(tower + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(117, 115, 115) / 255, new Vector3(117, 115, 115) / 255, new Vector3(1, 1, 1));

                objectList.Add(Tower);
            }
            //TreeV1
            for (int i = 1; i <= 7; i++)
            {
                Tree1 = new Asset3d(treeV1 + i + "/batang" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(94, 46, 12) / 255, new Vector3(94, 46, 12) / 255, new Vector3(1, 1, 1));
                child = new Asset3d(treeV1 + i + "/daun" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(66, 128, 18) / 255, new Vector3(66, 128, 18) / 255, new Vector3(1, 1, 1));
                Tree1.child.Add(child);

                objectList.Add(Tree1);
            }
            //TreeV2
            for (int i = 1; i <= 5; i++)
            {
                Tree2 = new Asset3d(treeV2 + i + "/batang" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(94, 46, 12) / 255, new Vector3(94, 46, 12) / 255, new Vector3(1, 1, 1));
                child = new Asset3d(treeV2 + i + "/daun" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(66, 128, 18) / 255, new Vector3(66, 128, 18) / 255, new Vector3(1, 1, 1));
                Tree2.child.Add(child);

                objectList.Add(Tree2);
            }
            //Well
            for (int i = 1; i <= 2; i++)
            {
                Well = new Asset3d(well + i + "/alas" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(117, 115, 115) / 255, new Vector3(117, 115, 115) / 255, new Vector3(1, 1, 1));
                child = new Asset3d(well + i + "/atap" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(79, 42, 9) / 255, new Vector3(79, 42, 9) / 255, new Vector3(1, 1, 1));
                Well.child.Add(child);
                child = new Asset3d(well + i + "/badan" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(117, 115, 115) / 255, new Vector3(117, 115, 115) / 255, new Vector3(1, 1, 1));
                Well.child.Add(child);
                child = new Asset3d(well + i + "/kayu" + i + ".obj", "normalShader.vert", "normalShader.frag", new Vector3(135, 79, 28) / 255, new Vector3(135, 79, 28) / 255, new Vector3(1, 1, 1));
                Well.child.Add(child);

                objectList.Add(Well);
            }


            /*var cube1 = new Asset3d("normalShader.vert", "normalShader.frag", new Vector3(1, 0.5f, 0.25f), new Vector3(1, 0.5f, 0.25f), new Vector3(0, 0, 0));
            cube1.createCuboid(0, 0, 0, 5, true, false);
            cube1.rotate(Vector3.Zero, Vector3.UnitX, 45);
            cube1.resetEuler();
            cube1.rotate(Vector3.Zero, Vector3.UnitZ, MathHelper.RadiansToDegrees(MathF.Acos(MathF.Sqrt(2) / MathF.Sqrt(3))));
            cube1.resetEuler();
            objectList.Add(cube1);*/


            //CAHAYA            
            var light1 = new Asset3d("shader.vert", "shader.frag", new Vector3(245, 220, 144)/255);
            light1.createCuboid(7, 0, 26, 0.5f, false, false);
            light1.setPointLight(0.001f, 0.1f);
            pointLightList.Add(light1);

            var light2 = new Asset3d("shader.vert", "shader.frag", new Vector3(245, 220, 144) / 255);
            light2.createCuboid(-9, 0, 26, 0.5f, false, false);
            light2.setPointLight(0.001f, 0.1f);
            pointLightList.Add(light2);

            var light3 = new Asset3d("shader.vert", "shader.frag", new Vector3(245, 220, 144) / 255);
            light3.createCuboid(7, 0, 34, 0.5f, false, false);
            light3.setPointLight(0.001f, 0.1f);
            pointLightList.Add(light3);

            var light4 = new Asset3d("shader.vert", "shader.frag", new Vector3(245, 220, 144) / 255);
            light4.createCuboid(7, 0, 42, 0.5f, false, false);
            light4.setPointLight(0.001f, 0.1f);
            pointLightList.Add(light4);

            var light5 = new Asset3d("shader.vert", "shader.frag", new Vector3(245, 220, 144) / 255);
            light5.createCuboid(-9, 0, 34, 0.5f, false, false);
            light5.setPointLight(0.001f, 0.1f);
            pointLightList.Add(light5);

            var light6 = new Asset3d("shader.vert", "shader.frag", new Vector3(245, 220, 144) / 255);
            light6.createCuboid(-9, 0, 42, 0.5f, false, false);
            light6.setPointLight(0.001f, 0.1f);
            pointLightList.Add(light6);

            //Inside Castle
            var light7 = new Asset3d("shader.vert", "shader.frag", new Vector3(245, 220, 144) / 255);
            light7.createCuboid(7, 8, 8.5f, 0.5f, false, false);
            light7.setPointLight(0.01f, 0.1f);
            pointLightList.Add(light7);

            var light8 = new Asset3d("shader.vert", "shader.frag", new Vector3(245, 220, 144) / 255);
            light8.createCuboid(-9, 8, 8.5f, 0.5f, false, false);
            light8.setPointLight(0.01f, 0.1f);
            pointLightList.Add(light8);

            //ATAS
            var floor = new Asset3d("shader.vert", "shader.frag", new Vector3(2, 112, 186) / 255);
            floor.createCuboid(30, 80, 20, 0.5f, false, false);
            floor.setDirectLight(0.1f, 0.05f, new Vector3(0, -1,0 ));
            directLightList.Add(floor);

            //BAWAH KAMERA
            flash = new Asset3d("shader.vert", "shader.frag", new Vector3(1, 1, 1));
            //flash.createCuboid(-4, 20, -6, 0.5f, false, false);
            flash.setSpotLight(ambientStrengthSpotLight, specStrengthSpotLight, spotLightDirection, spotLightAngle);
            spotLightList.Add(flash);

            //cubemap
            var cubemapPaths = new List<string>();
            cubemapPaths.Add(path + "Textures/Cubemap/bluecloud_ft.jpg");
            cubemapPaths.Add(path + "Textures/Cubemap/bluecloud_bk.jpg");
            cubemapPaths.Add(path + "Textures/Cubemap/bluecloud_up.jpg");
            cubemapPaths.Add(path + "Textures/Cubemap/bluecloud_dn.jpg");
            cubemapPaths.Add(path + "Textures/Cubemap/bluecloud_rt.jpg");
            cubemapPaths.Add(path + "Textures/Cubemap/bluecloud_lf.jpg");

            cubemap = new Cubemap("cubemap.vert", "cubemap.frag", cubemapPaths);
            cubemap.load();


            //LOAD
            foreach (Asset3d i in directLightList)
            {
                i.load();
            }
            foreach (Asset3d i in pointLightList)
            {
                i.load();
            }
            foreach (Asset3d i in spotLightList)
            {
                i.load();
            }

            foreach (Asset3d i in objectList)
            {
                i.load();
            }

            CursorGrabbed = true;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            float time = (float)args.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            for (int i = 0; i < directLightList.Count; i++)
            {
                directLightList[i].render(renderSetting, camera.GetViewMatrix(), camera.GetProjectionMatrix(), camera.Position, directLightList, pointLightList, spotLightList);
            }
            for (int i = 0; i < pointLightList.Count; i++)
            {
                pointLightList[i].render(renderSetting, camera.GetViewMatrix(), camera.GetProjectionMatrix(), camera.Position, directLightList, pointLightList, spotLightList);
            }
            for (int i = 0; i < spotLightList.Count; i++)
            {
                spotLightList[i].render(renderSetting, camera.GetViewMatrix(), camera.GetProjectionMatrix(), camera.Position, directLightList, pointLightList, spotLightList);
            }
            /////////////////////////////////
            //render per object list

            foreach (Asset3d i in objectList)
            {
                i.render(renderSetting, camera.GetViewMatrix(), camera.GetProjectionMatrix(), camera.Position, directLightList, pointLightList, spotLightList);
                //i.rotate(i.objectCenter, i._euler[1], 45 * time);
            }

            //////////////////////
            GL.Disable(EnableCap.CullFace);
            GL.DepthFunc(DepthFunction.Lequal);
            cubemap.render(camera.GetViewMatrix(), camera.GetProjectionMatrix());
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            float time = (float)args.Time;

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            #region Camera & Viewer Transformation
            if (!tpv)
            {
                if (input.IsKeyDown(Keys.W))
                {
                    camera.Position += Vector3.Normalize(Vector3.Cross(camera.Up, camera.Right)) * cameraSpeed * time;
                }
                if (input.IsKeyDown(Keys.S))
                {
                    camera.Position -= Vector3.Normalize(Vector3.Cross(camera.Up, camera.Right)) * cameraSpeed * time;
                }

                if (input.IsKeyDown(Keys.A))
                {
                    camera.Position -= camera.Right * cameraSpeed * time;
                }

                if (input.IsKeyDown(Keys.D))
                {
                    camera.Position += camera.Right * cameraSpeed * time;
                }

                if (input.IsKeyDown(Keys.Space))
                {
                    camera.Position += camera.Up * cameraSpeed * time;
                }

                if (input.IsKeyDown(Keys.LeftShift))
                {
                    camera.Position -= camera.Up * cameraSpeed * time;
                }

                if (input.IsKeyPressed(Keys.LeftControl))
                {
                    cameraSpeed += 5;
                    camera.Fov += 10;
                }

                if (input.IsKeyReleased(Keys.LeftControl))
                {
                    cameraSpeed -= 5;
                    camera.Fov -= 10;
                }
            }
            else
            {
                if (input.IsKeyDown(Keys.W))
                {
                    if (!collCheck())
                    {
                        rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, -cameraDegree.Y, time);
                        cameraDegree.Y = 0;
                        rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, -cameraDegree.X, time);
                        cameraDegree.X = 0;
                        rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, -cameraDegree.Z, time);
                        cameraDegree.Z = 0;
                        Vector3 tempVec = -Vector3.Normalize(Vector3.Cross(camera.Up, camera.Right)) * cameraSpeed * time;
                        camera.Position += tempVec;
                        Player.translate(tempVec.X, tempVec.Y, tempVec.Z);
                    }

                }
                if (input.IsKeyPressed(Keys.S))
                {
                    rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, -cameraDegree.Y, time);
                    cameraDegree.Y = 0;
                    rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, -cameraDegree.X, time);
                    cameraDegree.X = 0;
                    rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, -cameraDegree.Z, time);
                    cameraDegree.Z = 0;

                    if (lastFace == 3)
                    {
                        face = 1;
                    }
                    else if (lastFace == 2)
                    {
                        face = 0;
                    }
                    else if (lastFace == 1)
                    {
                        face = 3;
                    }
                    else if (lastFace == 0)
                    {
                        face = 2;
                    }
                    Player.rotate(Player.objectCenter, Player._euler[1], 180);
                    rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, 180, time);

                    lastFace = face;
                }

                if (input.IsKeyPressed(Keys.A))
                {
                    rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, -cameraDegree.Y, time);
                    cameraDegree.Y = 0;
                    rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, -cameraDegree.X, time);
                    cameraDegree.X = 0;
                    rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, -cameraDegree.Z, time);
                    cameraDegree.Z = 0;
                    if (lastFace == 3)
                    {
                        face = 2;
                    }
                    else if (lastFace == 2)
                    {
                        face = 1;
                    }
                    else if (lastFace == 1)
                    {
                        face = 0;
                    }
                    else if (lastFace == 0)
                    {
                        face = 3;
                    }

                    Player.rotate(Player.objectCenter, Player._euler[1], 90);
                    rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, -90, time);
                    lastFace = face;
                }

                if (input.IsKeyPressed(Keys.D))
                {
                    rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, -cameraDegree.Y, time);
                    cameraDegree.Y = 0;
                    rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, -cameraDegree.X, time);
                    cameraDegree.X = 0;
                    rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, -cameraDegree.Z, time);
                    cameraDegree.Z = 0;
                    if (lastFace == 3)
                    {
                        face = 0;
                    }
                    else if (lastFace == 2)
                    {
                        face = 3;
                    }
                    else if (lastFace == 1)
                    {
                        face = 2;
                    }
                    else if (lastFace == 0)
                    {
                        face = 1;
                    }
                    Player.rotate(Player.objectCenter, Player._euler[1], -90);
                    rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, 90, time);

                    lastFace = face;
                }
                if (input.IsKeyDown(Keys.Down))
                {
                    if (face == 0)
                    {
                        float speed = -15 * cameraSpeed * time;
                        if (cameraDegree.Z > -15)
                        {
                            rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, speed, time);
                            cameraDegree.Z += speed;
                        }
                    }
                    else if (face == 1)
                    {
                        float speed = 15 * cameraSpeed * time;
                        if (cameraDegree.X < 15)
                        {
                            rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, speed, time);
                            cameraDegree.X += speed;
                        }
                    }
                    else if (face == 2)
                    {
                        float speed = 15 * cameraSpeed * time;
                        if (cameraDegree.Z < 15)
                        {
                            rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, speed, time);
                            cameraDegree.Z += speed;
                        }
                    }
                    else if (face == 3)
                    {
                        float speed = -15 * cameraSpeed * time;
                        if (cameraDegree.X > -15)
                        {
                            rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, speed, time);
                            cameraDegree.X += speed;
                        }
                    }
                }
                if (input.IsKeyDown(Keys.Up))
                {
                    if (face == 0)
                    {
                        float speed = 15 * cameraSpeed * time;
                        if (cameraDegree.Z < 15)
                        {
                            rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, speed, time);
                            cameraDegree.Z += speed;
                        }
                    }
                    else if (face == 1)
                    {
                        float speed = -15 * cameraSpeed * time;
                        if (cameraDegree.X > -15)
                        {
                            rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, speed, time);
                            cameraDegree.X += speed;
                        }
                    }
                    else if (face == 2)
                    {
                        float speed = -15 * cameraSpeed * time;
                        if (cameraDegree.Z > -15)
                        {
                            rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, speed, time);
                            cameraDegree.Z += speed;
                        }
                    }
                    else if (face == 3)
                    {
                        float speed = 15 * cameraSpeed * time;
                        if (cameraDegree.X < 15)
                        {
                            rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, speed, time);
                            cameraDegree.X += speed;
                        }
                    }

                }
                if (input.IsKeyDown(Keys.Left))
                {
                    float speed = -15 * cameraSpeed * time;
                    if (cameraDegree.Y > -60)
                    {
                        rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, speed, time);
                        cameraDegree.Y += speed;
                    }

                }
                if (input.IsKeyDown(Keys.Right))
                {
                    float speed = 15 * cameraSpeed * time;
                    if (cameraDegree.Y < 60)
                    {
                        rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, speed, time);
                        cameraDegree.Y += speed;
                    }
                }
                if (input.IsKeyPressed(Keys.LeftControl))
                {
                    cameraSpeed += 5;
                    camera.Fov += 10;
                }

                if (input.IsKeyReleased(Keys.LeftControl))
                {
                    cameraSpeed -= 5;
                    camera.Fov -= 10;
                }
            }

            if (input.IsKeyPressed(Keys.GraveAccent))
            {
                renderSetting *= -1;
            }
            //tpv on/off
            if (input.IsKeyPressed(Keys.LeftAlt))
            {
                tpv = !tpv;
                if (tpv)
                {
                    rotateCamera(Vector3.UnitX, Player.objectCenter, Player.objectCenter, time, 0, time);
                    rotateCamera(Vector3.UnitY, Player.objectCenter, Player.objectCenter, time, 0, time);
                    rotateCamera(Vector3.UnitZ, Player.objectCenter, Player.objectCenter, time, 0, time);
                    lastPos = new Vector2(camera.Yaw, camera.Pitch);
                    firstMove = true;
                }
                else
                {
/*                    rotateCamera(Vector3.UnitX, camera.Position, camera.Front, time, 0, time);
                    rotateCamera(Vector3.UnitY, camera.Position, camera.Front, time, 0, time);
                    rotateCamera(Vector3.UnitZ, camera.Position, camera.Front, time, 0, time);*/
                }
            }

            var mouseState = MouseState;
            if (mouseState.IsButtonDown(MouseButton.Left))
            {
                if (firstMove)
                {
                    lastPos = new Vector2(mouseState.X, mouseState.Y);
                    firstMove = false;
                }
                else
                {
                    var deltaX = mouseState.X - lastPos.X;
                    var deltaY = mouseState.Y - lastPos.Y;
                    lastPos = new Vector2(mouseState.X, mouseState.Y);

                    camera.Yaw += deltaX * sensitivity;
                    camera.Pitch -= deltaY * sensitivity;
                }
            }
            if (tpv)
            {
                if (face == 0)
                {
                    spotLightDirection = new Vector3(-1, 0, 0);
                }
                else if (face == 1)
                {
                    spotLightDirection = new Vector3(0, 0, -1);
                }
                else if (face == 2)
                {
                    spotLightDirection = new Vector3(1, 0, 0);
                }
                else if (face == 3)
                {
                    spotLightDirection = new Vector3(0, 0, 1);
                }
                flash.setSpotLight(ambientStrengthSpotLight, specStrengthSpotLight, spotLightDirection, spotLightAngle);
                foreach (Asset3d i in spotLightList)
                {
                    i.load();
                }
            }
            else
            {
                spotLightDirection = new Vector3(0, 0, 1);
            }
            #endregion

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);

            camera.AspectRatio = Size.X / (float)Size.Y;
        }
        public Matrix4 generateArbRotationMatrix(Vector3 axis, float angle)
        {
            angle = MathHelper.DegreesToRadians(angle);

            var arbRotationMatrix = new Matrix4(
                (float)Math.Cos(angle) + (float)Math.Pow(axis.X, 2) * (1 - (float)Math.Cos(angle)), axis.X * axis.Y * (1 - (float)Math.Cos(angle)) - axis.Z * (float)Math.Sin(angle), axis.X * axis.Z * (1 - (float)Math.Cos(angle)) + axis.Y * (float)Math.Sin(angle), 0,
                axis.Y * axis.X * (1 - (float)Math.Cos(angle)) + axis.Z * (float)Math.Sin(angle), (float)Math.Cos(angle) + (float)Math.Pow(axis.Y, 2) * (1 - (float)Math.Cos(angle)), axis.Y * axis.Z * (1 - (float)Math.Cos(angle)) - axis.X * (float)Math.Sin(angle), 0,
                axis.Z * axis.X * (1 - (float)Math.Cos(angle)) - axis.Y * (float)Math.Sin(angle), axis.Z * axis.Y * (1 - (float)Math.Cos(angle)) + axis.X * (float)Math.Sin(angle), (float)Math.Cos(angle) + (float)Math.Pow(axis.Z, 2) * (1 - (float)Math.Cos(angle)), 0,
                0, 0, 0, 1
                );

            return arbRotationMatrix;
        }
        public void rotateCamera(Vector3 axis, Vector3 camRotationCenter, Vector3 lookAt, float delta, float angle, float duration)
        {
            float rotationSpeed = angle * delta / duration;
            camera.Position -= camRotationCenter;
            if (axis == Vector3.UnitY)
            {
                camera.Yaw += rotationSpeed;
            }
            camera.Position = Vector3.Transform(camera.Position, generateArbRotationMatrix(axis, rotationSpeed).ExtractRotation());
            camera.Position += camRotationCenter;
            camera._front = Vector3.Normalize(lookAt - camera.Position);
        }
        public bool collCheck()
        {
            bool result = false;

            return result;
        }

    }
}
