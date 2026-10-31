/*******************************************************************************************
*
*   Last Day Rules: Survival (Last Island of Survival LITE) - C++ Native Game Engine
*   Ready for PC (Windows/Linux) and Mobile (Android / iOS)
*   Powered by Raylib - Ultra-lightweight, zero overhead, native touch & gamepad support.
*
********************************************************************************************/

#include "raylib.h"
#include "raymath.h"
#include "rlgl.h"
#include "native_original_animation.hpp"
#include "mobile_original_hud_layout.hpp"
#include "mobile_hud_input.hpp"
#include "mobile_audio.hpp"
#include "native_build_craft.hpp"
#include "original_buildpart_v7.hpp"
#include "mobile_build_visual.hpp"
#include "mobile_craft_ui.hpp"
#include "mobile_real_craft_catalog.hpp"
#include "mobile_original_terrain.hpp"

#include <vector>
#include <string>
#include <cmath>
#include <cstdlib>
#include <cstring>
#include <set>
#include <array>
#include <algorithm>
#include <chrono>

// -----------------------------------------------------------------------------------------
// Global Definitions & Data Structures
// -----------------------------------------------------------------------------------------
enum WeaponType {
    WEAPON_AXE = 0,
    WEAPON_M4A1 = 1,
    WEAPON_COUNT = 2
};

enum ResourceType {
    RES_TREE,
    RES_SULFUR,
    RES_IRON,
    RES_STONE
};

struct ResourceNode {
    Vector3 position;
    ResourceType type;
    float health;
    float maxHealth;
    float scale;
    float rotation;
    bool active;
};

struct HitParticle {
    Vector3 position;
    Vector3 velocity;
    Color color;
    float life;
    float maxLife;
    float size;
};

struct FloatingNotification {
    std::string text;
    Vector2 screenPos;
    Color color;
    float life;
    float maxLife;
};

struct VirtualJoystick {
    Vector2 center;
    Vector2 knob;
    float radius;
    bool active;
    int touchId;
};

struct TouchButton {
    Rectangle bounds;
    const char* label;
    Color baseColor;
    Color activeColor;
    bool isPressed;
    int touchId;
};

// -----------------------------------------------------------------------------------------
// Game State
// -----------------------------------------------------------------------------------------
struct Player {
    Vector3 position;
    float yaw;
    float pitch;
    float speed;
    float verticalVelocity;
    bool isGrounded;
    bool isRunning;
    bool thirdPerson;

    // Vitals
    float health;
    float maxHealth;
    float stamina;
    float maxStamina;
    float hunger;
    float thirst;

    // Inventory / Weapon
    int equippedSlot;
    bool isAttacking;
    float attackTimer;
    float attackDuration;
    bool attackHitRegistered;

    // Resources collected
    int woodCount;
    int sulfurCount;
    int ironCount;
    int stoneCount;

    // Procedural animation
    float walkCycle;
};

// -----------------------------------------------------------------------------------------
// Main Entry Point
// -----------------------------------------------------------------------------------------
int main() {
    const int screenWidth = 1280;
    const int screenHeight = 720;

    SetConfigFlags(FLAG_WINDOW_RESIZABLE);
    InitWindow(screenWidth, screenHeight, "Last Island of Survival - C++ Engine (Mobile Ready)");
    SetTargetFPS(60);

    // Initial cursor state: locked for desktop, unlocked if touch
    bool cursorLocked = true;
    DisableCursor();

    // Setup 3D Camera
    Camera3D camera = { 0 };
    camera.position = (Vector3){ 0.0f, 2.0f, 4.0f };
    camera.target = (Vector3){ 0.0f, 1.5f, 0.0f };
    camera.up = (Vector3){ 0.0f, 1.0f, 0.0f };
    camera.fovy = 65.0f;
    camera.projection = CAMERA_PERSPECTIVE;

    // Player Initialization
    Player player = { 0 };
    player.position = (Vector3){ 0.0f, 0.0f, 0.0f };
    player.yaw = 0.0f;
    player.pitch = 0.1f;
    // Audited from original source LIOSSKN1 foot plant motion (30 fps).
    // zhan_run_front01 planted toes travel ~4.35 units/s, not 5.5.
    player.speed = 4.35f;
    player.verticalVelocity = 0.0f;
    player.isGrounded = true;
    player.isRunning = false;
    player.thirdPerson = true;

    player.health = 100.0f;
    player.maxHealth = 100.0f;
    player.stamina = 100.0f;
    player.maxStamina = 100.0f;
    player.hunger = 95.0f;
    player.thirst = 90.0f;

    player.equippedSlot = 0; // 0 = Axe, 1 = M4A1
    player.isAttacking = false;
    player.attackTimer = 0.0f;
    player.attackDuration = 0.6f;
    player.attackHitRegistered = false;

    player.woodCount = 0;
    player.sulfurCount = 0;
    player.ironCount = 0;
    player.stoneCount = 0;
    player.walkCycle = 0.0f;

    // -------------------------------------------------------------------------------------
    // Load Authentic 3D Models & Textures from assets/
    // -------------------------------------------------------------------------------------
    // Character Parts
    Model modelTorso   = LoadModel("assets/man_clothes_chushijuese_01.obj");
    Texture2D texTorso = LoadTexture("assets/native_man_clothes_chushijuese_01_ba.png");
    if (modelTorso.meshCount > 0 && texTorso.id > 0)
        SetMaterialTexture(&modelTorso.materials[0], MATERIAL_MAP_DIFFUSE, texTorso);

    Model modelPants   = LoadModel("assets/man_trousers_chushijuese_01.obj");
    Texture2D texPants = LoadTexture("assets/native_man_trousers_chushijuese_01_ba.png");
    if (modelPants.meshCount > 0 && texPants.id > 0)
        SetMaterialTexture(&modelPants.materials[0], MATERIAL_MAP_DIFFUSE, texPants);

    Model modelHead    = LoadModel("assets/man_head_chushijuese_01.obj");
    Texture2D texHead  = LoadTexture("assets/native_man_head_chushijuese_01_ba.png");
    if (modelHead.meshCount > 0 && texHead.id > 0)
        SetMaterialTexture(&modelHead.materials[0], MATERIAL_MAP_DIFFUSE, texHead);

    Model modelHair    = LoadModel("assets/man_hair_chushijuese_01.obj");
    Texture2D texHair  = LoadTexture("assets/native_man_hair_chushijuese_01_ba.png");
    if (modelHair.meshCount > 0 && texHair.id > 0)
        SetMaterialTexture(&modelHair.materials[0], MATERIAL_MAP_DIFFUSE, texHair);

    Model modelShoes   = LoadModel("assets/man_shoes_chushijuese_01.obj");
    Texture2D texShoes = LoadTexture("assets/native_man_shoes_chushijuese_01_ba.png");
    // The desktop original rig uses this DIFFERENT skin atlas for its
    // exposed skin and gloves. v0.13 incorrectly substituted the head atlas.
    Texture2D texSkin = LoadTexture("assets/native_man_clothes_yf_0_ba.png");
    TraceLog(LOG_INFO,"LIOS_ORIGINAL_CHARACTER_ATLASES skin=%u torso=%u pants=%u head=%u hair=%u shoes=%u",
             texSkin.id,texTorso.id,texPants.id,texHead.id,texHair.id,texShoes.id);
    if (modelShoes.meshCount > 0 && texShoes.id > 0)
        SetMaterialTexture(&modelShoes.materials[0], MATERIAL_MAP_DIFFUSE, texShoes);

    // Original 64-bone, 33-clip Unity rig imported from PC data, not fake
    // procedural movement. A missing/corrupt asset falls back to legacy meshes.
    LIOS::OriginalAnimation unityRig;
    bool hasUnityRig = unityRig.load("assets/lios_skeleton_v1.bin");
    if (hasUnityRig) {
        hasUnityRig = unityRig.upload();
        if (hasUnityRig) {
            unityRig.setTextures(texPants, texTorso, texSkin, texShoes, texHead, texHair);
            TraceLog(LOG_INFO, "LIOS_NATIVE_UNITY_RIG_READY bones=%d clips=%d meshes=%d",
                     (int)unityRig.boneNames.size(),(int)unityRig.clips.size(),(int)unityRig.parts.size());
        }
    }
    float animationIdleTime=0, animationWalkTime=0, animationJumpTime=0;
    float animationCrossFade=1.f;
    // Original assets contain packed smoothness in albedo alpha, not body opacity.
    // The native_* copies are generated at build time, leaving Unity originals intact.

    std::string animationCurrent="zhan_stand01", animationPrevious="zhan_stand01";

    // Weapons
    Model modelAxe   = LoadModel("assets/wq_shifu.obj");
    Texture2D texAxe = LoadTexture("assets/wq_shifu_ba.png");
    if (modelAxe.meshCount > 0 && texAxe.id > 0)
        SetMaterialTexture(&modelAxe.materials[0], MATERIAL_MAP_DIFFUSE, texAxe);

    Model modelRifle   = LoadModel("assets/wq_m4_1.obj");
    Texture2D texRifle = LoadTexture("assets/wq_m4_1_ba.png");
    if (modelRifle.meshCount > 0 && texRifle.id > 0)
        SetMaterialTexture(&modelRifle.materials[0], MATERIAL_MAP_DIFFUSE, texRifle);

    // World Objects
    Model modelHouse   = LoadModel("assets/Building_baopo_tiefang.obj");
    Texture2D texHouse = LoadTexture("assets/Building_baopo_tiefang_ba.png");
    if (modelHouse.meshCount > 0 && texHouse.id > 0)
        SetMaterialTexture(&modelHouse.materials[0], MATERIAL_MAP_DIFFUSE, texHouse);

    Model modelTree    = LoadModel("assets/tree_baihuashu_03.obj");
    Texture2D texTree  = LoadTexture("assets/tree_baihuashu_03_d.png");
    if (modelTree.meshCount > 0 && texTree.id > 0)
        SetMaterialTexture(&modelTree.materials[0], MATERIAL_MAP_DIFFUSE, texTree);

    Model modelSulfur   = LoadModel("assets/ore_liuhuangkuang_01.obj");
    Texture2D texSulfur = LoadTexture("assets/ore_liuhuangkuang_01_d.png");
    if (modelSulfur.meshCount > 0 && texSulfur.id > 0)
        SetMaterialTexture(&modelSulfur.materials[0], MATERIAL_MAP_DIFFUSE, texSulfur);

    Model modelIron   = LoadModel("assets/ore_tiekuang_01.obj");
    Texture2D texIron = LoadTexture("assets/ore_tiekuang_01_d.png");
    if (modelIron.meshCount > 0 && texIron.id > 0)
        SetMaterialTexture(&modelIron.materials[0], MATERIAL_MAP_DIFFUSE, texIron);

    // Transparent original Unity battlepanel sprites, not the handmade HUD.
    // Both full on-foot panels are regenerated from ORIGINAL Unity sprites,
    // per-mode sibling draw order and RectTransforms (no static fake selection).
    Texture2D originalHudMelee=LoadTexture("assets/hud_original_melee.png");
    Texture2D originalHudRifle=LoadTexture("assets/hud_original_rifle.png");
    // Sprite pixels and rects from ORIGINAL battlepanel.ab build profile.
    Texture2D originalHudBuild=LoadTexture("assets/hud_original_build.png");
    Texture2D originalBuildCategories=LoadTexture("assets/hud_original_build_categories.png");
    Texture2D originalBuildSelected=LoadTexture("assets/hud_original_build_selected.png");
    Texture2D originalBuildRotate=LoadTexture("assets/hud_original_build_rotate.png");
    Texture2D originalBuildChildren=LoadTexture("assets/hud_original_build_children.png");
    // All 35 original source BuildPart.icon sprites, in original cfg order.
    Texture2D originalBuildItemAtlases[6]={
        LoadTexture("assets/hud_original_build_items_type_2.png"),
        LoadTexture("assets/hud_original_build_items_type_3.png"),
        LoadTexture("assets/hud_original_build_items_type_8.png"),
        LoadTexture("assets/hud_original_build_items_type_4.png"),
        LoadTexture("assets/hud_original_build_items_type_6.png"),
        LoadTexture("assets/hud_original_build_items_type_7.png")
    };
    LIOS_CRAFT_UI::Art originalCraftArt;
    originalCraftArt.load();
    LIOS_REAL_CRAFT::Art realCraftArt;
    realCraftArt.load();
    Texture2D originalThumb=LoadTexture("assets/hud_original_joystick_knob.png");
    // Exact-source Unity m_heightLignt for all seven hotbar slots.
    Texture2D originalHotbarSelection=LoadTexture("assets/hud_original_hotbar_selection.png");
    // 72 original-source compass frames, 5 degrees per frame.
    Texture2D originalCompass=LoadTexture("assets/hud_original_compass_strip.png");
    Texture2D sourceResources[4]={
        LoadTexture("assets/icon_resource_wood.png"),
        LoadTexture("assets/icon_resource_metal.png"),
        LoadTexture("assets/icon_resource_stone.png"),
        LoadTexture("assets/icon_resource_sulfur.png"),
    };
    bool hasOriginalHud=originalHudMelee.id>0&&originalHudRifle.id>0&&originalHotbarSelection.id>0;
    if(hasOriginalHud)TraceLog(LOG_INFO,"LIOS_ORIGINAL_UNITY_HUD_PROFILES source=melee+rifle; slots=7; touch=11");

    Model modelStone   = LoadModel("assets/ore_yanshikuang_01.obj");
    Texture2D texStone = LoadTexture("assets/ore_yanshikuang_01_d.png");
    if (modelStone.meshCount > 0 && texStone.id > 0)
        SetMaterialTexture(&modelStone.materials[0], MATERIAL_MAP_DIFFUSE, texStone);

    // -------------------------------------------------------------------------------------
    // Spawn Resource Nodes in the World
    // -------------------------------------------------------------------------------------
    std::vector<ResourceNode> nodes = {
        // Trees
        { { -14.0f, 0.0f,  -8.0f }, RES_TREE,   100.0f, 100.0f, 1.1f,  45.0f, true },
        { { -22.0f, 0.0f,  12.0f }, RES_TREE,   100.0f, 100.0f, 1.0f,  90.0f, true },
        { {  18.0f, 0.0f, -18.0f }, RES_TREE,   100.0f, 100.0f, 1.3f, 120.0f, true },
        { {  16.0f, 0.0f,  20.0f }, RES_TREE,   100.0f, 100.0f, 1.1f,  30.0f, true },
        { { -28.0f, 0.0f, -22.0f }, RES_TREE,   100.0f, 100.0f, 1.2f,  75.0f, true },
        { {  30.0f, 0.0f,   8.0f }, RES_TREE,   100.0f, 100.0f, 1.2f, 210.0f, true },
        { {   0.0f, 0.0f,  28.0f }, RES_TREE,   100.0f, 100.0f, 1.1f, 150.0f, true },
        // Ores
        { {  -8.0f, 0.0f,  -5.0f }, RES_SULFUR,  80.0f,  80.0f, 1.0f,  15.0f, true },
        { {  10.0f, 0.0f,  12.0f }, RES_SULFUR,  80.0f,  80.0f, 0.9f,  60.0f, true },
        { {  -6.0f, 0.0f,  16.0f }, RES_IRON,    90.0f,  90.0f, 1.1f,  45.0f, true },
        { {  24.0f, 0.0f,  -6.0f }, RES_IRON,    90.0f,  90.0f, 1.0f,  80.0f, true },
        { {   8.0f, 0.0f,  -8.0f }, RES_STONE,  100.0f, 100.0f, 1.2f, 110.0f, true },
        { { -18.0f, 0.0f,  -4.0f }, RES_STONE,  100.0f, 100.0f, 1.0f,  35.0f, true }
    };

    // Source tile around the original world origin. Preserve a flat fallback
    // only when the authenticated OBJ/texture cannot load.
    LIOS_TERRAIN::Tile terrain;
    terrain.load();
    player.position.y=terrain.ground(player.position.x,player.position.z);
    // Probe both sides of each 400m source-tile seam in the real Android
    // runtime. Log gaps/height discontinuities without claiming Unity physics.
    const float seamCoords[4][4]={
        {0.f,199.5f,0.f,200.5f},
        {0.f,-199.5f,0.f,-200.5f},
        {199.5f,0.f,200.5f,0.f},
        {-199.5f,0.f,-200.5f,0.f}
    };
    const char* seamNames[4]={"north","south","east","west"};
    for(int seam=0;seam<4;seam++){
        float innerHeight=0.f,outerHeight=0.f;
        const bool inner=terrain.sample(seamCoords[seam][0],seamCoords[seam][1],innerHeight);
        const bool outer=terrain.sample(seamCoords[seam][2],seamCoords[seam][3],outerHeight);
        TraceLog(LOG_INFO,"LIOS_TERRAIN_SEAM_PROBE %s inner=%d outer=%d height_in=%.4f height_out=%.4f",
                 seamNames[seam],int(inner),int(outer),innerHeight,outerHeight);
    }

    // Five offsets on every tile edge. These are independent runtime
    // collision samples, not evidence of player-controlled traversal.
    const float seamGridOffsets[5]={-150.f,-75.f,0.f,75.f,150.f};
    for(int seam=0;seam<4;seam++){
        for(float offset:seamGridOffsets){
            const float xIn=(seam<2)?offset:seamCoords[seam][0];
            const float zIn=(seam<2)?seamCoords[seam][1]:offset;
            const float xOut=(seam<2)?offset:seamCoords[seam][2];
            const float zOut=(seam<2)?seamCoords[seam][3]:offset;
            float hIn=0.f,hOut=0.f;
            const bool in=terrain.sample(xIn,zIn,hIn);
            const bool out=terrain.sample(xOut,zOut,hOut);
            TraceLog(LOG_INFO,"LIOS_TERRAIN_SEAM_GRID_PROBE %s offset=%.0f inner=%d outer=%d height_in=%.4f height_out=%.4f",
                     seamNames[seam],offset,int(in),int(out),hIn,hOut);
        }
    }

    for(auto& node:nodes)
        node.position.y=terrain.ground(node.position.x,node.position.z);
    const float houseGround=terrain.ground(10.f,-15.f);
    TraceLog(LOG_INFO,"LIOS_TERRAIN_SPAWN_GROUND y=%.5f source=%d",
             player.position.y,int(terrain.ready));

    std::vector<HitParticle> particles;
    std::vector<FloatingNotification> notifications;
    LIOS_BUILD::System building;
    LIOS_BUILD::OriginalBuildTextures buildSourceMaterials;
    buildSourceMaterials.load();
    if(!buildSourceMaterials.complete())
        TraceLog(LOG_ERROR,"LIOS_SOURCE_BUILD_MATERIALS_MISSING: do not treat fallback as original");
    else
        TraceLog(LOG_INFO,"LIOS_SOURCE_BUILD_MATERIALS_LOADED original_texture2d=6");
    std::string buildStatus="Colete recursos para fabricar.";
    std::set<int> previousUiTouches;
    bool buildMode=false;
    int buildCategory=-1; // No category active; source m_buildChilds hidden.
    int buildSubitem=-1;
    int sourceBuildPartId=-1,sourceBuildOnceAngle=0;
    bool buildSubmenuOpen=false,buildBagOpen=false;
    int craftSelectedRecipe=0,craftQuantity=1;
    int craftBigTab=1,craftSmallTab=1,craftSelectedOrder=0;
    LIOS_REAL_CRAFT::State realCraft;
    bool showOfflineCraftPrototype=false;
    std::string craftStatus="Selecione uma receita e fabrique.";

    // -------------------------------------------------------------------------------------
    // Mobile On-Screen Virtual Controls Setup
    // -------------------------------------------------------------------------------------
    VirtualJoystick leftStick;
    leftStick.center = (Vector2){ 140.0f, (float)screenHeight - 140.0f };
    leftStick.knob = leftStick.center;
    leftStick.radius = 70.0f;
    leftStick.active = false;
    leftStick.touchId = -1;

    // Touch Action Buttons (Mobile HUD)
    TouchButton btnAttack  = { { (float)screenWidth - 130.0f, (float)screenHeight - 130.0f, 90.0f, 90.0f }, "GOLPE",  (Color){ 200, 50, 50, 180 },  (Color){ 255, 80, 80, 240 }, false, -1 };
    TouchButton btnJump    = { { (float)screenWidth - 210.0f, (float)screenHeight - 80.0f,  65.0f, 65.0f }, "PULO",   (Color){ 60, 140, 200, 180 }, (Color){ 90, 180, 255, 240 }, false, -1 };
    TouchButton btnSprint  = { { (float)screenWidth - 80.0f,  (float)screenHeight - 210.0f, 65.0f, 65.0f }, "CORRER", (Color){ 220, 160, 30, 180 }, (Color){ 255, 200, 50, 240 }, false, -1 };
    TouchButton btnCam     = { { (float)screenWidth - 90.0f,  20.0f,                         70.0f, 40.0f }, "1P/3P",  (Color){ 70, 70, 80, 180 },   (Color){ 120, 120, 140, 240 }, false, -1 };
    TouchButton btnSlot0   = { { (float)screenWidth/2 - 70,   (float)screenHeight - 65.0f,   60.0f, 55.0f }, "[1] MACH", (Color){ 60, 60, 70, 200 }, (Color){ 200, 160, 50, 240 }, false, -1 };
    TouchButton btnSlot1   = { { (float)screenWidth/2 + 10,   (float)screenHeight - 65.0f,   60.0f, 55.0f }, "[2] M4A1", (Color){ 60, 60, 70, 200 }, (Color){ 200, 160, 50, 240 }, false, -1 };

    // -------------------------------------------------------------------------------------
    // Main Game Loop
    // -------------------------------------------------------------------------------------
    LIOS_INPUT::Controller hudInput;
    LIOS_AUDIO::Audio sound;
    sound.init();
    bool autoRun=false,aim=false,crouch=false,prone=false;
    // Exact original Unity source FSM: Stand -> Crouch -> Pa (prone).
    int lastStance=0;
    bool stanceTransitionActive=false;
    std::string stanceTransitionClip;
    float stanceTransitionTime=0.f;
    float cameraEyeHeight=1.65f,cameraTargetHeight=1.45f;
    int activePanel=0; // 0=game, 1=bag, 2=craft, 3=settings, 4=chat
    int selectedHotbarSlot=0,magazine=30,reserveAmmo=90;
    float shotCooldown=0.f,stepTimer=0.f;
    bool wasOnGround=true;
    // Fixed-size frame-time window: no per-frame allocation or log spam.
    // Samples measure observed frame pacing, not GPU-only rendering time.
    std::array<float,60> framePacingMs{};
    unsigned int framePacingCount=0;
    // Per-frame CPU wall time, not a GPU timer query. Each window uses
    // exactly 60 completed frames and is emitted once (no log spam).
    unsigned int phaseFrames=0;
    unsigned int phaseVisibleFrames=0,phasePoseCalls=0;
    bool firstPersonPoseSkipReported=false;
    double phasePoseTotal=0.0,phaseVboTotal=0.0;
    double phaseEndTotal=0.0,phaseWorkTotal=0.0;
    while (!WindowShouldClose()) {
        const auto frameWorkStart=std::chrono::steady_clock::now();
        unityRig.lastPoseCpuMs=0.0;
        unityRig.lastVboSubmitMs=0.0;
        const int panelAtFrameStart=activePanel;
        // Snapshot modal ownership before processing any input this frame.
        // Keyboard shortcuts must obey the same UI capture rules as touch.
        const bool overlayWasOpen=activePanel!=0;
        // Cap only physics/movement steps. Offline crafting must consume
        // actual elapsed frame time, otherwise a slow Android frame makes
        // every crafting order take longer than its stated duration.
        float elapsedFrame = GetFrameTime();
        if (!std::isfinite(elapsedFrame) || elapsedFrame < 0.f)
            elapsedFrame = 0.f;
        const float dt = std::min(elapsedFrame, 0.05f);
        if(elapsedFrame>0.f && elapsedFrame<10.f){
            framePacingMs[framePacingCount++]=elapsedFrame*1000.f;
            if(framePacingCount==framePacingMs.size()){
                std::array<float,60> sorted=framePacingMs;
                std::sort(sorted.begin(),sorted.end());
                float total=0.f;
                unsigned int slow=0;
                for(float ms:sorted){total+=ms;if(ms>33.333f)++slow;}
                const float mean=total/float(sorted.size());
                TraceLog(LOG_INFO,
                    "LIOS_FRAME_TIMING frames=60 mean_ms=%.3f p95_ms=%.3f max_ms=%.3f slow_33ms=%u mean_fps=%.2f",
                    mean,sorted[56],sorted.back(),slow,1000.f/mean);
                framePacingCount=0;
            }
        }
        building.tickCraft(elapsedFrame);
        realCraft.tick(elapsedFrame);
        if(building.blueprint&&craftStatus=="Fabricando..."){
            craftStatus="Item pronto. Equipe pelo ultimo slot da barra.";
        }

        // Desktop toggle cursor lock
        if (IsKeyPressed(KEY_ESCAPE)) {
            cursorLocked = !cursorLocked;
            if (cursorLocked) DisableCursor();
            else EnableCursor();
        }

        // Toggle camera with TAB or V
        if (!overlayWasOpen &&
            (IsKeyPressed(KEY_TAB) || (!buildMode&&IsKeyPressed(KEY_V)))) {
            player.thirdPerson = !player.thirdPerson;
            TraceLog(LOG_INFO,"LIOS_CAMERA_MODE third_person=%d",int(player.thirdPerson));
        }

        // Switch Weapon with Keys 1 and 2
        if (!overlayWasOpen&&IsKeyPressed(KEY_ONE)){player.equippedSlot=0;selectedHotbarSlot=0;aim=false;}
        if (!overlayWasOpen&&IsKeyPressed(KEY_TWO)){player.equippedSlot=1;selectedHotbarSlot=1;}
        // Resource/empty quick slots have no native item-use port yet.
        if (!overlayWasOpen&&IsKeyPressed(KEY_THREE)){player.equippedSlot=-1;selectedHotbarSlot=2;aim=false;}
        if (!overlayWasOpen&&IsKeyPressed(KEY_FOUR)){player.equippedSlot=-1;selectedHotbarSlot=3;aim=false;}
        if (!overlayWasOpen&&IsKeyPressed(KEY_FIVE)){player.equippedSlot=-1;selectedHotbarSlot=4;aim=false;}
        if (!overlayWasOpen&&IsKeyPressed(KEY_SIX)){player.equippedSlot=-1;selectedHotbarSlot=5;aim=false;}
        if (!overlayWasOpen&&IsKeyPressed(KEY_SEVEN)){player.equippedSlot=-1;selectedHotbarSlot=6;aim=false;}

        // ---------------------------------------------------------------------------------
        // Touch Input Processing (Multi-touch support for Mobile)
        // ---------------------------------------------------------------------------------
        int touchCount = GetTouchPointCount();
        bool hasTouchActive = (touchCount > 0);

        // The original battlepanel.ab hitboxes are consumed by ONE action per
        // stable Android finger. Raylib synthesizes a mouse click for touchscreen
        // activity; it must NEVER be interpreted as a separate global attack.
        hudInput.frame(player.equippedSlot==1,(float)GetScreenWidth(),(float)GetScreenHeight(),overlayWasOpen,buildMode);
        // UI overlay consumes only touch DOWN edges, never a continuous drag.
        // Keeps construction buttons from placing 60 pieces while held.
        std::vector<Vector2> uiClicks;
        std::set<int> touchesNow;
        for(int n=0;n<touchCount;n++){
            int id=GetTouchPointId(n);
            touchesNow.insert(id);
            if(!previousUiTouches.count(id))uiClicks.push_back(GetTouchPosition(n));
        }
        previousUiTouches.swap(touchesNow);
#if !defined(__ANDROID__)
        if(!hasTouchActive&&IsMouseButtonPressed(MOUSE_BUTTON_LEFT))
            uiClicks.push_back(GetMousePosition());
#endif
        using HA=LIOS_INPUT::Action;
        btnAttack.isPressed=hudInput.isHeld(HA::Attack);
        btnJump.isPressed=hudInput.isHeld(HA::Jump);
        btnSprint.isPressed=hudInput.isHeld(HA::AutoRun);
        btnCam.isPressed=false;
        btnSlot0.isPressed=hudInput.isHeld(HA::Slot0);
        btnSlot1.isPressed=hudInput.isHeld(HA::Slot1);
        const Vector2 touchMoveDir=hudInput.stick;
        float canvasScale=std::min(float(GetScreenWidth())/1920.f,float(GetScreenHeight())/1080.f);
        float canvasLeft=(GetScreenWidth()-1920.f*canvasScale)/2;
        float canvasTop=(GetScreenHeight()-1080.f*canvasScale)/2;
        auto unityRect=[&](float l,float t,float r,float b)->Rectangle{
            return {canvasLeft+l*canvasScale,canvasTop+t*canvasScale,
                    (r-l)*canvasScale,(b-t)*canvasScale};
        };
        auto sourceRect=[&](LIOS_HUD::Rect bounds)->Rectangle {
            // Use the same measured HUD artwork offset as touch hit-testing.
            const float dy=LIOS_INPUT::HudVisualYOffset;
            return unityRect(bounds.left,bounds.top+dy,
                             bounds.right,bounds.bottom+dy);
        };
        if(hasOriginalHud){
            leftStick.center={
                canvasLeft+LIOS_HUD::JoystickCenterX*canvasScale,
                canvasTop+(LIOS_HUD::JoystickCenterY+LIOS_INPUT::HudVisualYOffset)*canvasScale
            };
            leftStick.radius=LIOS_HUD::JoystickRadius*canvasScale;
            leftStick.knob=hudInput.stickPoint;
            leftStick.active=hudInput.owner.size()>0&&
                (std::fabs(hudInput.stick.x)+std::fabs(hudInput.stick.y)>0.002f);
        }
        // A BagAndBuildPanel captures all touches. Without modal ownership,
        // tapping a recipe also fires hidden HUD/weapon buttons underneath.
        // Touch events (edge-triggered unless the action is explicitly held).
        if(!overlayWasOpen&&!buildMode&&hudInput.justPressed(HA::Slot0)){
            player.equippedSlot=0;selectedHotbarSlot=0;aim=false;
            TraceLog(LOG_INFO,"LIOS_HOTBAR_TOUCH_SELECTED slot=0 equipped=0");
        }
        if(!overlayWasOpen&&!buildMode&&hudInput.justPressed(HA::Slot1)){
            player.equippedSlot=1;selectedHotbarSlot=1;
            TraceLog(LOG_INFO,"LIOS_HOTBAR_TOUCH_SELECTED slot=1 equipped=1");
        }
        for(int i=0;i<5;i++)if(!overlayWasOpen&&!buildMode&&hudInput.justPressed(
            static_cast<HA>(int(HA::Slot2)+i))){
            // Unity hotbar selects the item; it does NOT open the inventory.
            // Native resource item use is not ported; never fire the old gun.
            selectedHotbarSlot=i+2;player.equippedSlot=-1;aim=false;
            TraceLog(LOG_INFO,"LIOS_HOTBAR_TOUCH_SELECTED slot=%d equipped=-1",i+2);
        }
        if(!overlayWasOpen&&hudInput.justPressed(HA::AutoRun))autoRun=!autoRun;
        if(!overlayWasOpen&&(hudInput.justPressed(HA::Crouch)||IsKeyPressed(KEY_C))){
            const bool wasProne=prone;
            crouch=wasProne||!crouch;prone=false;
            // Runtime evidence: a HUD hitbox DOWN alone does not prove the
            // crouch command changed gameplay state. Log the actual transition.
            if(hudInput.justPressed(HA::Crouch))
                TraceLog(LOG_INFO,"LIOS_CROUCH_TOUCH_TRANSITION crouch=%d prone=%d",
                         int(crouch),int(prone));
        }
        if(!overlayWasOpen&&(hudInput.justPressed(HA::Prone)||IsKeyPressed(KEY_Z))){
            prone=!prone;crouch=false;
        }
        const int stance=prone?2:crouch?1:0;
        if(stance!=lastStance){
            stanceTransitionClip.clear();
            if(lastStance==1&&stance==2)
                stanceTransitionClip=player.equippedSlot==1?
                    "chongfengqiang_hengwo_dun_qiepa01":"kongshou_dun_qiepa01";
            else if(lastStance==2&&stance==1)
                stanceTransitionClip=player.equippedSlot==1?
                    "chongfengqiang_hengwo_pa_qiedun01":"kongshou_pa_qiedun01";
            else if(lastStance==2&&stance==0)
                stanceTransitionClip=player.equippedSlot==1?
                    "chiqiang_pa_qiezhan01":"kongshou_pa_qiezhan01";
            else if(lastStance==0&&stance==2)
                stanceTransitionClip=player.equippedSlot==1?
                    "chongfengqiang_hengwo_dun_qiepa01":"kongshou_dun_qiepa01";
            stanceTransitionTime=0.f;
            stanceTransitionActive=hasUnityRig&&!stanceTransitionClip.empty()&&
                unityRig.clipIndex(stanceTransitionClip.c_str())>=0;
            lastStance=stance;
            if(stance>0){player.isAttacking=false;aim=false;}
        }
        if(!overlayWasOpen&&!buildMode&&hudInput.justPressed(HA::Aim)&&player.equippedSlot==1){
            aim=!aim;
            TraceLog(LOG_INFO,"LIOS_AIM_TOUCH_TRANSITION aim=%d slot=%d",int(aim),player.equippedSlot);
        }
        if(!overlayWasOpen&&hudInput.justPressed(HA::Reload)&&player.equippedSlot==1&&magazine<30&&reserveAmmo>0){
            int n=std::min(30-magazine,reserveAmmo);
            magazine+=n;reserveAmmo-=n;
        }
        // Modal HUD buttons must remain able to close their OWN panels.
        // The previous !overlayWasOpen guard made the advertised "tap again
        // to close" action unreachable for inventory/settings/quick chat.
        // Other HUD actions stay blocked while an overlay owns input.
        if((!overlayWasOpen||activePanel==1)&&hudInput.justPressed(HA::Inventory)){
            activePanel=(activePanel==1?0:1);
            if(buildMode){buildMode=false;building.selected=-1;}
            TraceLog(LOG_INFO,activePanel==1?
                "LIOS_INVENTORY_PANEL_OPEN":"LIOS_INVENTORY_PANEL_CLOSE");
        }
        if(!overlayWasOpen&&hudInput.justPressed(HA::Craft)){
            activePanel=(activePanel==2?0:2);
            if(buildMode){buildMode=false;building.selected=-1;}
            if(activePanel==2)
                TraceLog(LOG_INFO,"LIOS_CRAFT_PANEL_OPEN");
        }
        if((!overlayWasOpen||activePanel==3)&&hudInput.justPressed(HA::Settings)){
            activePanel=(activePanel==3?0:3);
            TraceLog(LOG_INFO,activePanel==3?
                "LIOS_SETTINGS_PANEL_OPEN":"LIOS_SETTINGS_PANEL_CLOSE");
        }
        if((!overlayWasOpen||activePanel==4)&&hudInput.justPressed(HA::QuickChat))
            activePanel=(activePanel==4?0:4);
        // Original BuildState is entered from equipped/quick-use builder
        // equipment, NOT from an invented "ENTER BUILD MODE" craft button.
        // Offline native mapping: building item occupies the last hotbar slot.
        // A keyboard shortcut must not bypass modal input ownership:
        // previously KEY_B could close inventory/settings and enter build
        // mode while a panel was open, even though the HUD slot was gated.
        if(!overlayWasOpen&&building.blueprint&&
            (IsKeyPressed(KEY_B)||hudInput.justPressed(HA::Slot6))){
            buildMode=!buildMode;
            TraceLog(LOG_INFO,"LIOS_BUILD_MODE_TRANSITION enabled=%d blueprint=%d",
                     int(buildMode),int(building.blueprint));
            if(buildMode){
                buildCategory=-1;buildSubitem=-1;buildSubmenuOpen=false;
                sourceBuildPartId=-1;sourceBuildOnceAngle=0;
                buildBagOpen=false;building.selected=-1;
                selectedHotbarSlot=6;player.equippedSlot=-1;aim=false;
                buildStatus="CONSTRUCAO: categoria > peca > colocar.";
            }else{
                buildSubmenuOpen=false;buildBagOpen=false;
                buildCategory=-1;buildSubitem=-1;building.selected=-1;
                sourceBuildPartId=-1;sourceBuildOnceAngle=0;
                buildStatus="Construcao encerrada.";
            }
            activePanel=0;
        }
        auto panelBox=[&](float x,float y,float w,float h)->Rectangle{
            return {canvasLeft+x*canvasScale,canvasTop+y*canvasScale,
                    w*canvasScale,h*canvasScale};
        };
        auto hitUi=[&](Vector2 p,Rectangle b)->bool{
            return CheckCollisionPointRec(p,b);
        };
        // A touch opening a modal belongs to the HUD for this frame. Do not
        // dispatch that same DOWN event into the newly opened panel: it can
        // otherwise select a recipe or close a menu behind the HUD icon.
        if(activePanel!=panelAtFrameStart){
            uiClicks.clear();
            if(activePanel!=0){
                hudInput.captureModal();
                TraceLog(LOG_INFO,"LIOS_MODAL_TOUCH_CAPTURE panel=%d",activePanel);
            }
        }
        for(const Vector2 &click:uiClicks){
            if(activePanel==2){
                // Source-derived 519 recipes and original Sprite atlases are
                // the default screen. Keep the legacy THREE-ITEM sandbox
                // accessible separately, not presented as the real catalog.
                if(LIOS_REAL_CRAFT::hit(click,1685,1011,225,60,
                    canvasScale,canvasLeft,canvasTop)){
                    showOfflineCraftPrototype=!showOfflineCraftPrototype;
                    continue;
                }
                if(!showOfflineCraftPrototype){
                    if(LIOS_REAL_CRAFT::hit(click,1654,0,316,86,
                        canvasScale,canvasLeft,canvasTop)){
                        activePanel=0;
                        TraceLog(LOG_INFO,"LIOS_CRAFT_PANEL_CLOSE");
                        continue;
                    }
                    LIOS_BUILD::Resources nativeResources{
                        player.woodCount,player.stoneCount,
                        player.ironCount,player.sulfurCount};
                    LIOS_REAL_CRAFT::touch(realCraft,click,canvasScale,
                        canvasLeft,canvasTop,nativeResources);
                    player.woodCount=nativeResources.wood;
                    player.stoneCount=nativeResources.stone;
                    player.ironCount=nativeResources.metal;
                    continue;
                }
                const auto touch=[&](float x,float y,float w,float h){
                    return LIOS_CRAFT_UI::hit(click,x,y,w,h,canvasScale,canvasLeft,canvasTop);
                };
                if(LIOS_CRAFT_UI::hit(click,LIOS_CRAFT_SOURCE::Back,
                    canvasScale,canvasLeft,canvasTop)){
                    activePanel=0;
                    TraceLog(LOG_INFO,"LIOS_CRAFT_PANEL_CLOSE");
                    continue;
                }
                // Unity BagProducePage: main tabs -> sub-tabs -> recipe cells.
                bool navigationTouched=false;
                for(int i=0;i<5;i++){
                    if(touch(LIOS_CRAFT_SOURCE::MainTabOnSprite.x,
                             LIOS_CRAFT_SOURCE::MainTabOnSprite.y+
                                 LIOS_CRAFT_SOURCE::MainTabStepY*i,
                             LIOS_CRAFT_SOURCE::MainTabOnSprite.w,
                             LIOS_CRAFT_SOURCE::MainTabOnSprite.h)){
                        craftBigTab=LIOS_CRAFT_UI::BigTabs[i].id;
                        craftSmallTab=LIOS_CRAFT_UI::firstSubTab(craftBigTab);
                        craftSelectedRecipe=LIOS_CRAFT_UI::visibleRecipe(
                            craftBigTab,craftSmallTab,0);
                        craftQuantity=1;navigationTouched=true;break;
                    }
                }
                if(navigationTouched)continue;
                for(int i=0;i<LIOS_CRAFT_UI::subTabCount(craftBigTab);i++){
                    if(touch(LIOS_CRAFT_SOURCE::SubTabOnSprite.x+
                             LIOS_CRAFT_SOURCE::SubTabStepX*i,
                             LIOS_CRAFT_SOURCE::SubTabOnSprite.y,120,35)){
                        craftSmallTab=LIOS_CRAFT_UI::subTab(craftBigTab,i).id;
                        craftSelectedRecipe=LIOS_CRAFT_UI::visibleRecipe(
                            craftBigTab,craftSmallTab,0);
                        craftQuantity=1;navigationTouched=true;break;
                    }
                }
                if(navigationTouched)continue;
                bool recipeTouched=false;
                for(int slot=0;slot<3;slot++){
                    int recipe=LIOS_CRAFT_UI::visibleRecipe(
                        craftBigTab,craftSmallTab,slot);
                    if(recipe>=0&&touch(
                        LIOS_CRAFT_SOURCE::RecipeTemplateCell.x+180.f*slot,
                        LIOS_CRAFT_SOURCE::RecipeTemplateCell.y+12.f,149.5f,220.f)){
                        craftSelectedRecipe=recipe;craftQuantity=1;
                        recipeTouched=true;break;
                    }
                }
                if(recipeTouched)continue;
                LIOS_BUILD::Resources available={
                    player.woodCount,player.stoneCount,player.ironCount,player.sulfurCount};
                const int maxAvailable=craftSelectedRecipe>=0?
                    building.maxCraftableQuantity(
                        static_cast<LIOS_BUILD::RecipeType>(craftSelectedRecipe),available):0;
                const auto sourceTouch=[&](LIOS_CRAFT_SOURCE::Bounds r){
                    return LIOS_CRAFT_UI::hit(click,r,canvasScale,canvasLeft,canvasTop);
                };
                if(sourceTouch(LIOS_CRAFT_SOURCE::QuantityMinus)&&craftSelectedRecipe>=0){
                    craftQuantity=std::max(1,craftQuantity-1);
                }else if(sourceTouch(LIOS_CRAFT_SOURCE::QuantityPlus)&&craftSelectedRecipe>=0){
                    craftQuantity=std::min(std::max(1,maxAvailable),craftQuantity+1);
                }else if(sourceTouch(LIOS_CRAFT_SOURCE::QuantityMax)&&craftSelectedRecipe>=0){
                    craftQuantity=std::max(1,maxAvailable);
                }else if(touch(1780,627,60,60)&&!building.queue.empty()){
                    if(building.cancelCraft(
                        unsigned(std::max(0,std::min(craftSelectedOrder,
                            int(building.queue.size())-1))),available)){
                        player.woodCount=available.wood;player.stoneCount=available.stone;
                        player.ironCount=available.metal;
                        craftSelectedOrder=std::max(0,std::min(craftSelectedOrder,
                            int(building.queue.size())-1));
                        craftStatus="Fabricacao cancelada; recursos devolvidos.";
                    }
                }else if(touch(1510,674,340,25.f*float(building.queue.size()))
                          &&!building.queue.empty()){
                    int idx=int((click.y-(canvasTop+674.f*canvasScale))/
                                (25.f*canvasScale));
                    craftSelectedOrder=std::max(0,std::min(idx,
                        int(building.queue.size())-1));
                }else if(touch(1510,951,200,75)&&craftSelectedRecipe>=0){
                    const auto type=static_cast<LIOS_BUILD::RecipeType>(craftSelectedRecipe);
                    if(building.requestCraft(type,available,craftQuantity)){
                        player.woodCount=available.wood;player.stoneCount=available.stone;
                        player.ironCount=available.metal;
                        craftSelectedOrder=int(building.queue.size())-1;
                        craftStatus="Fabricando...";
                    }else craftStatus="Materiais insuficientes ou fila cheia.";
                }
            }else if(buildMode&&activePanel==0){
                // Original source SC.UI.BattlePanel.cs:
                // click a category -> CheckBuildChildsClose -> FillBuildChildsIcons,
                // then click a populated toggle -> ChangePrefab. btn_null
                // toggles scp_bagBuilds; btn_changeButton exits BuildState.
                const LIOS_HUD::Rect categories[]={
                    LIOS_HUD::Build_btn_null,LIOS_HUD::Build_btn_lantu,
                    LIOS_HUD::Build_btn_fundation,LIOS_HUD::Build_btn_wall,
                    LIOS_HUD::Build_btn_floor,LIOS_HUD::Build_btn_stair,
                    LIOS_HUD::Build_btn_window,LIOS_HUD::Build_btn_obstacle
                };
                static constexpr int sourceTypeByCategory[8]={0,0,2,3,8,4,6,7};
                const LIOS_HUD::Rect childRects[]={
                    LIOS_HUD::BuildChildBox0,LIOS_HUD::BuildChildBox1,
                    LIOS_HUD::BuildChildBox2,LIOS_HUD::BuildChildBox3,
                    LIOS_HUD::BuildChildBox4,LIOS_HUD::BuildChildBox5,
                    LIOS_HUD::BuildChildBox6,LIOS_HUD::BuildChildBox7,
                    LIOS_HUD::BuildChildBox8,LIOS_HUD::BuildChildBox9,
                    LIOS_HUD::BuildChildBox10
                };
                if(hitUi(click,sourceRect(LIOS_HUD::Build_btn_changeButton))){
                    buildMode=false;buildSubmenuOpen=false;buildBagOpen=false;
                    buildCategory=-1;buildSubitem=-1;building.selected=-1;
                    sourceBuildPartId=-1;sourceBuildOnceAngle=0;
                    buildStatus="Construcao encerrada.";
                    continue;
                }
                bool tabTouched=false;
                for(int category=0;category<8;category++){
                    if(!hitUi(click,sourceRect(categories[category])))continue;
                    tabTouched=true;
                    if(category==0){
                        buildBagOpen=!buildBagOpen;
                        buildSubmenuOpen=false;building.selected=-1;
                        buildCategory=0;buildSubitem=-1;
                        sourceBuildPartId=-1;sourceBuildOnceAngle=0;
                        buildStatus=buildBagOpen?"Itens de construcao no inventario.":
                                                "Inventario fechado.";
                    }else if(category==1){
                        // EdgeAI original callback audit: btn_lantu is present
                        // in the prefab but is NOT wired with ClickListener
                        // in the decompiled BattlePanel. Never invent a
                        // blueprint-saving action here.
                        buildStatus="Funcao do projeto nao comprovada no Unity.";
                    }else{
                        // Source toggles same category on a second click.
                        const bool close=buildSubmenuOpen&&buildCategory==category;
                        buildCategory=close?-1:category;
                        buildSubmenuOpen=!close;
                        buildBagOpen=false;buildSubitem=-1;building.selected=-1;
                        sourceBuildPartId=-1;sourceBuildOnceAngle=0;
                        buildStatus=close?"Categoria fechada.":"Selecione uma peca abaixo.";
                    }
                    break;
                }
                if(tabTouched)continue;
                bool childTouched=false;
                if(buildSubmenuOpen&&buildCategory>=2){
                    const int origType=sourceTypeByCategory[buildCategory];
                    const int sourceCount=LIOS_SOURCE_BUILD::count(origType);
                    for(int n=0;n<LIOS_HUD::BuildChildCount&&n<sourceCount;n++){
                        if(!hitUi(click,sourceRect(childRects[n])))continue;
                        const auto *source=LIOS_SOURCE_BUILD::find(origType,n);
                        if(!source)continue;
                        sourceBuildPartId=source->id;
                        sourceBuildOnceAngle=source->onceAngle;
                        buildSubitem=n;
                        const int which=LIOS_BUILD::partForOriginalCategory(buildCategory,n);
                        if(which<0){
                            building.selected=-1;
                            buildStatus=TextFormat(
                                "Peça Unity ID %d: modelo 3D ainda nao portado.",
                                sourceBuildPartId);
                        }else{
                            building.selected=which;
                            buildStatus=TextFormat(
                                "Peça Unity ID %d selecionada (rotacao %d).",
                                sourceBuildPartId,sourceBuildOnceAngle);
                        }
                        childTouched=true;break;
                    }
                }
                if(childTouched)continue;
                if(buildBagOpen){
                    if(hitUi(click,panelBox(720.f,680.f,105.f,105.f))){
                        building.selected=LIOS_BUILD::Storage;buildBagOpen=false;
                        buildStatus="Bau selecionado do inventario.";
                    }else if(hitUi(click,panelBox(840.f,680.f,105.f,105.f))){
                        building.selected=LIOS_BUILD::Campfire;buildBagOpen=false;
                        buildStatus="Fogueira selecionada do inventario.";
                    }
                    continue;
                }
                if(building.selected>=0&&building.selected<LIOS_BUILD::PieceCount&&
                    hitUi(click,sourceRect(LIOS_HUD::Build_btn_opRotate))){
                    if(sourceBuildOnceAngle>0&&sourceBuildOnceAngle%90==0){
                        building.orientation=(building.orientation+
                            (sourceBuildOnceAngle/90))%4;
                    }else{
                        buildStatus="Angulo original nao suportado pela malha nativa.";
                    }
                    continue;
                }
                if(hitUi(click,sourceRect(LIOS_HUD::Build_btn_freeBuildModel))){
                    building.freePlacement=!building.freePlacement;
                    buildStatus=building.freePlacement?"Posicionamento livre.":"Encaixe ativado.";
                    continue;
                }
                if(hitUi(click,sourceRect(LIOS_HUD::Build_btn_build))){
                    LIOS_BUILD::Resources available={
                        player.woodCount,player.stoneCount,player.ironCount,player.sulfurCount};
                    const Vector3 dir={sinf(player.yaw),0.f,cosf(player.yaw)};
                    const int gx=LIOS_BUILD::grid(player.position.x+dir.x*4.3f);
                    const int gz=LIOS_BUILD::grid(player.position.z+dir.z*4.3f);
                    if(building.selected<0||building.selected>=LIOS_BUILD::PieceCount){
                        buildStatus="Selecione primeiro uma peca do submenu.";
                    }else if(building.place(static_cast<LIOS_BUILD::PieceType>(building.selected),
                          gx,gz,building.orientation,player.position.x,player.position.z,available)){
                        player.woodCount=available.wood;player.stoneCount=available.stone;
                        player.ironCount=available.metal;
                        buildStatus="Peca construida.";
                    }else buildStatus="Posicao invalida ou materiais insuficientes.";
                }
            }
        }
        if(buildMode&&activePanel==0){
            // Keyboard fallback does not alter camera mode in build state.
            if(IsKeyPressed(KEY_V)){
                buildCategory=buildCategory<2?2:2+(buildCategory-1)%6;
                buildSubmenuOpen=true;buildSubitem=-1;building.selected=-1;
            }
            if(IsKeyPressed(KEY_R)&&building.selected>=0){
                if(sourceBuildOnceAngle>0&&sourceBuildOnceAngle%90==0)
                    building.orientation=(building.orientation+
                        (sourceBuildOnceAngle/90))%4;
                else buildStatus="Angulo original nao suportado.";
            }
            if(IsKeyPressed(KEY_ENTER)&&building.selected>=0&&
               building.selected<LIOS_BUILD::PieceCount){
                LIOS_BUILD::Resources available={
                    player.woodCount,player.stoneCount,player.ironCount,player.sulfurCount};
                Vector3 dir={sinf(player.yaw),0.f,cosf(player.yaw)};
                int gx=LIOS_BUILD::grid(player.position.x+dir.x*4.3f);
                int gz=LIOS_BUILD::grid(player.position.z+dir.z*4.3f);
                if(building.place(static_cast<LIOS_BUILD::PieceType>(building.selected),
                      gx,gz,building.orientation,player.position.x,player.position.z,available)){
                    player.woodCount=available.wood;player.stoneCount=available.stone;
                    player.ironCount=available.metal;buildStatus="Peca construida.";
                }else buildStatus="Construcao invalida.";
            }
        }
        if(activePanel!=0&&hudInput.justPressed(HA::Attack)){
            // A gameplay command outside a panel does not implicitly close it
            // or fire the weapon. Menus have explicit source touch owners.
        }
        if(activePanel==0){
            player.yaw-=hudInput.lookDelta.x*0.005f;
            player.pitch-=hudInput.lookDelta.y*0.005f;
        }

        // ---------------------------------------------------------------------------------
        // Desktop Mouse Look Processing
        // ---------------------------------------------------------------------------------
        if (activePanel==0&&cursorLocked && !hasTouchActive) {
            Vector2 mouseDelta = GetMouseDelta();
            const float mouseSensitivity = 0.003f;
            player.yaw -= mouseDelta.x * mouseSensitivity;
            player.pitch -= mouseDelta.y * mouseSensitivity;
        }

        // Clamp camera pitch
        if (player.pitch > 1.35f) player.pitch = 1.35f;
        if (player.pitch < -1.35f) player.pitch = -1.35f;

        // Unity character's forward is +Z. IMPORTANT: with this camera
        // looking toward +Z, the SCREEN-RIGHT vector is forward x world-up
        // = (-cos(yaw), 0, sin(yaw)). Previous Android code used +cos here,
        // so strafing physically opposed the original desktop controller and
        // its signed left/right animation selection. The camera shoulder uses
        // a separate geometric offset and is intentionally NOT flipped.
        Vector3 forward = { sinf(player.yaw), 0.0f, cosf(player.yaw) };
        Vector3 right   = { cosf(player.yaw), 0.0f, -sinf(player.yaw) };
        const Vector3 screenRight={-right.x,0.0f,-right.z};

        // ---------------------------------------------------------------------------------
        // Player Movement Logic (Keyboard + Mobile Joystick)
        // ---------------------------------------------------------------------------------
        Vector3 moveDir = { 0.0f, 0.0f, 0.0f };

        // Keyboard input
        if (IsKeyDown(KEY_W)) moveDir = Vector3Add(moveDir, forward);
        if (IsKeyDown(KEY_S)) moveDir = Vector3Subtract(moveDir, forward);
        if (IsKeyDown(KEY_D)) moveDir = Vector3Add(moveDir, screenRight);
        if (IsKeyDown(KEY_A)) moveDir = Vector3Subtract(moveDir, screenRight);

        // Add Touch Joystick movement
        if (leftStick.active) {
            // A nearly vertical thumb drag should not add an unintended
            // sideways component. Preserve deliberate diagonal movement.
            float inputSide=touchMoveDir.x;
            float inputFwd=-touchMoveDir.y;
            if(std::fabs(inputSide)<std::fabs(inputFwd)*0.20f)inputSide=0.0f;
            if(std::fabs(inputFwd)<std::fabs(inputSide)*0.12f)inputFwd=0.0f;
            Vector3 stickMove = Vector3Add(
                Vector3Scale(forward, inputFwd),
                Vector3Scale(screenRight, inputSide)
            );
            moveDir = Vector3Add(moveDir, stickMove);
        }

        // Modal production captures movement/look and all underlying attacks.
        if(activePanel!=0||stanceTransitionActive)moveDir={0.f,0.f,0.f};
        // Crouched locomotion can accelerate; prone cannot sprint.
        player.isRunning = !prone&&(IsKeyDown(KEY_LEFT_SHIFT) || autoRun) &&
            (player.stamina > 5.0f);
        // Measure the original 30fps clips at the ground-contact frames:
        // front ~4.35, back ~2.15, source-left ~2.25, source-right ~3.0.
        // These are foot-plant treadmill velocities, not frame translations
        // (the Bip001 root was intentionally locked to prevent double motion).
        // The raw Unity clips are oriented toward +Z; our camera screen-right
        // points -X. This makes source 'left' travel toward SCREEN RIGHT.
        const float toward=Vector3DotProduct(moveDir,forward);
        const float lateral=Vector3DotProduct(moveDir,screenRight);
        const float absToward=std::fabs(toward),absLateral=std::fabs(lateral);
        const float nativeFrontSpeed=toward>=0.0f?player.speed:2.15f;
        const float nativeSideSpeed=lateral>=0.0f?2.25f:3.00f;
        const float strideSpeed=(absToward*nativeFrontSpeed+
                                absLateral*nativeSideSpeed)/
                                std::max(0.0001f,absToward+absLateral);
        const float currentSpeed=strideSpeed*(player.isRunning?1.75f:1.0f)*
            (prone?.35f:(crouch?.65f:1.f));

        // Keep the analog speed and the gait phase tied to actual distance.
        // Previously any tiny joystick deflection instantly moved at FULL
        // velocity while the run clip advanced at a fixed time rate.
        const float moveAmount=Vector3Length(moveDir);
        const bool moving=moveAmount>0.09f;
        const float driveScale=std::min(1.0f,moveAmount);
        if (moving) {
            if(player.isGrounded){
                stepTimer+=dt;
                if(stepTimer>=(player.isRunning?.28f:.46f)){
                    sound.play(sound.step,player.isRunning?.57f:.42f);
                    stepTimer=0;
                }
            }
            moveDir = Vector3Normalize(moveDir);
            Vector3 nextPosition=Vector3Add(player.position,
                Vector3Scale(moveDir,currentSpeed*driveScale*dt));
            // Walls are solid; reject implausibly tall uphill steps and
            // follow the measured source mesh instead of a fixed Y=0 plane.
            const float nextGround=terrain.ground(nextPosition.x,nextPosition.z);
            if(!building.blocks(nextPosition.x,nextPosition.z) &&
               (!player.isGrounded || nextGround-player.position.y<=0.55f)){
                player.position=nextPosition;
                if(player.isGrounded)player.position.y=nextGround;
            }
            player.walkCycle += dt * (player.isRunning ? 12.0f : 8.0f);

            if (player.isRunning) {
                player.stamina -= dt * 15.0f;
                if (player.stamina < 0.0f) player.stamina = 0.0f;
            }
        } else {
            stepTimer=0;
            // Regenerate stamina when not running
            if (player.stamina < player.maxStamina) {
                player.stamina += dt * 12.0f;
                if (player.stamina > player.maxStamina) player.stamina = player.maxStamina;
            }
        }

        // Jump and Gravity Physics
        const float gravity = 24.0f;
        bool wantJump = !overlayWasOpen&&activePanel==0&&!buildMode&&!prone&&!crouch&&!stanceTransitionActive&&
            (IsKeyPressed(KEY_SPACE) || hudInput.justPressed(HA::Jump));
        if (player.isGrounded) {
            if (wantJump && player.stamina >= 10.0f) {
                player.verticalVelocity = 8.5f;
                player.isGrounded = false;
                player.stamina -= 10.0f;
                sound.play(sound.jump,.7f);
                TraceLog(LOG_INFO,"LIOS_JUMP_TRIGGERED y=%.3f velocity=%.3f",
                         player.position.y,player.verticalVelocity);
            }
        } else {
            player.verticalVelocity -= gravity * dt;
            player.position.y += player.verticalVelocity * dt;
            const float groundY=terrain.ground(player.position.x,player.position.z);
            if (player.position.y <= groundY) {
                player.position.y = groundY;
                player.verticalVelocity = 0.0f;
                player.isGrounded = true;
                sound.play(sound.land,.7f);
            }
        }

        // ---------------------------------------------------------------------------------
        // Attack & Resource Gathering System
        // ---------------------------------------------------------------------------------
        // Touchscreen events are also exposed as mouse clicks by raylib on
        // Android. Do not make EVERY button / drag fire the weapon.
        bool wantAttack=btnAttack.isPressed&&!buildMode;
#if !defined(__ANDROID__)
        if(!hasTouchActive && cursorLocked && IsMouseButtonPressed(MOUSE_BUTTON_LEFT))
            wantAttack=true;
#endif
        if(activePanel!=0||buildMode)wantAttack=false;
        shotCooldown=std::max(0.f,shotCooldown-dt);
        if(wantAttack && !player.isAttacking && shotCooldown<=0.f &&
            (player.equippedSlot==0||player.equippedSlot==1)){
            TraceLog(LOG_INFO,"LIOS_ATTACK_TRIGGERED slot=%d",player.equippedSlot);
            if(player.equippedSlot==0){
                player.attackDuration=.6f;
                sound.play(sound.axe,.8f);
            }else if(player.equippedSlot==1&&magazine>0){
                player.attackDuration=.125f;
                magazine--;
                shotCooldown=.13f;
                // Play the original wq_m4_1 outdoor loop as one continuous
                // burst while the trigger remains pressed, not one 2.62s
                // sample layered for every 0.13-second shot.
                sound.fireM4();
            }
            if(player.equippedSlot==0||(player.equippedSlot==1&&shotCooldown>0.f)){
                player.isAttacking=true;
                player.attackTimer=0.0f;
                player.attackHitRegistered=false;
            }
        }

        if(player.equippedSlot!=1 || !wantAttack || magazine<=0)
            sound.releaseM4();

        if (player.isAttacking) {
            player.attackTimer += dt;

            // Register hit at impact peak (halfway through swing: ~0.25s)
            if (player.attackTimer >= (player.equippedSlot==1?.07f:.22f) && !player.attackHitRegistered) {
                player.attackHitRegistered = true;

                // Hit check in front of player
                Vector3 hitPoint = Vector3Add(player.position, Vector3Scale(forward, 2.2f));
                hitPoint.y += 1.2f;

                for (auto& node : nodes) {
                    if (!node.active) continue;
                    float dist = Vector3Distance(player.position, node.position);

                    if (dist < 3.2f) { // Within harvest range
                        node.health -= 25.0f;

                        // Spawn impact hit particles
                        for (int p = 0; p < 8; p++) {
                            HitParticle part;
                            part.position = hitPoint;
                            part.velocity = (Vector3){
                                ((float)rand() / RAND_MAX - 0.5f) * 4.0f,
                                ((float)rand() / RAND_MAX) * 3.0f + 1.0f,
                                ((float)rand() / RAND_MAX - 0.5f) * 4.0f
                            };
                            part.maxLife = 0.5f;
                            part.life = part.maxLife;
                            part.size = 0.08f;

                            if (node.type == RES_TREE) part.color = (Color){ 160, 110, 60, 255 };
                            else if (node.type == RES_SULFUR) part.color = (Color){ 230, 220, 50, 255 };
                            else if (node.type == RES_IRON) part.color = (Color){ 180, 120, 80, 255 };
                            else part.color = (Color){ 150, 150, 150, 255 };

                            particles.push_back(part);
                        }

                        if(player.equippedSlot==0)
                            sound.play(node.type==RES_TREE?sound.wood:sound.ore,.65f);
                        // Harvest resources
                        FloatingNotification notif;
                        notif.screenPos = (Vector2){ (float)screenWidth / 2.0f + 50.0f, (float)screenHeight / 2.0f - 20.0f };
                        notif.maxLife = 1.2f;
                        notif.life = notif.maxLife;

                        if (node.type == RES_TREE) {
                            player.woodCount += 15;
                            notif.text = "+15 Madeira";
                            notif.color = (Color){ 100, 220, 100, 255 };
                        } else if (node.type == RES_SULFUR) {
                            player.sulfurCount += 10;
                            notif.text = "+10 Enxofre";
                            notif.color = (Color){ 255, 230, 40, 255 };
                        } else if (node.type == RES_IRON) {
                            player.ironCount += 10;
                            notif.text = "+10 Ferro";
                            notif.color = (Color){ 220, 140, 60, 255 };
                        } else {
                            player.stoneCount += 20;
                            notif.text = "+20 Pedra";
                            notif.color = (Color){ 200, 200, 200, 255 };
                        }
                        notifications.push_back(notif);

                        if (node.health <= 0.0f) {
                            node.active = false;
                        }
                        break;
                    }
                }
            }

            if (player.attackTimer >= player.attackDuration) {
                player.isAttacking = false;
                player.attackTimer = 0.0f;
                player.attackHitRegistered = false;
            }
        }

        // Extracted original Unity man AnimatorController Base Layer:
        // Crouch: kongshou_dun_stand01 + dun_run_* blend.
        // Pa.Pa: pa_stand01 + pa_walk_* crawling blend.
        // CrouchDown / PaUp: real (weapon-specific) transition clips.
        if (hasUnityRig) {
            animationIdleTime+=dt;
            if(stanceTransitionActive){
                stanceTransitionTime+=dt;
                int transition=unityRig.clipIndex(stanceTransitionClip.c_str());
                if(transition<0||stanceTransitionTime>=
                    float(unityRig.clips[transition].frames-1)/
                    unityRig.clips[transition].fps)
                    stanceTransitionActive=false;
            }
            // Source locomotion is an in-place 64-bone clip. Advance it by
            // the same speed ratio as the physically traveled distance, so
            // sprint, slow analog input and crouch do not visibly over/under
            // drive the feet compared with the avatar world displacement.
            if(moving)animationWalkTime+=dt*(currentSpeed/strideSpeed)*driveScale;
            if(!player.isGrounded)animationJumpTime+=dt;
            else animationJumpTime=0;
            float localFwd=Vector3DotProduct(moveDir,forward);
            // Select the same signed local locomotion clip as viewer3d.py:
            // joystick left -> zhan_run_left01, right -> zhan_run_right01.
            float localRight=Vector3DotProduct(moveDir,screenRight);
            std::string target;
            if(stanceTransitionActive)
                target=stanceTransitionClip;
            else if(player.isAttacking){
                if(prone)
                    target=player.equippedSlot==1?
                        "chongfengqiang_hengwo_pa_miaozhun_attack01":"pa_stand01";
                else if(crouch)
                    target=player.equippedSlot==1?
                        "chongfengqiang_hengwo_dun_miaozhun_attack01":"kongshou_dun_stand01";
                else target=player.equippedSlot==1?
                    "wq_m4_1_zhan_miaozhun_attack01":"fu_zhan_attack_01";
            }else if(!player.isGrounded)
                target=animationJumpTime<.25f?"kongshou_zhan_jump_up02":
                       animationJumpTime<.55f?"kongshou_zhan_jump_idle02":"kongshou_zhan_jump_down02";
            else if(prone){
                if(moving){
                    if(std::fabs(localFwd)>=std::fabs(localRight))
                        target=localFwd>=0?"pa_walk_front01":"pa_walk_back01";
                    else target=localRight>=0?"pa_walk_left01":"pa_walk_right01";
                }else target=player.equippedSlot==1?
                    "chongfengqiang_hengwo_pa_miaozhun_stand01":"pa_stand01";
            }else if(crouch){
                if(moving){
                    if(std::fabs(localFwd)>=std::fabs(localRight))
                        target=localFwd>=0?
                            (player.equippedSlot==1?"chongfengqiang_hengwo_dun_miaozhun_run_front01":
                             player.isRunning?"dun_rush_front01":"dun_run_front01"):
                            "dun_run_back01";
                    else target=localRight>=0?"dun_run_left01":"dun_run_right01";
                }else target=player.equippedSlot==1?
                    "chongfengqiang_hengwo_dun_miaozhun_stand01":"kongshou_dun_stand01";
            }else if(moving){
                if(std::fabs(localFwd)>=std::fabs(localRight))
                    target=localFwd>=0?
                        (player.equippedSlot==1?"wq_m4_1_zhan_miaozhun_run_front01":"zhan_run_front01"):
                        "zhan_run_back01";
                else target=localRight>=0?"zhan_run_left01":"zhan_run_right01";
            }else target=player.equippedSlot==1?
                "wq_m4_1_zhan_miaozhun_stand01":"zhan_stand01";
            if(target!=animationCurrent){
                animationPrevious=animationCurrent;animationCurrent=target;
                animationCrossFade=0;
            }
            animationCrossFade=std::min(1.f,animationCrossFade+dt/.15f);
            auto clipTime=[&](const std::string& name)->float{
                if(name=="fu_zhan_attack_01"||name=="wq_m4_1_zhan_miaozhun_attack01")
                    return player.attackTimer*(player.equippedSlot==1?3.5f:1.3f);
                if(stanceTransitionActive&&name==stanceTransitionClip)
                    return stanceTransitionTime;
                if(name.find("_run_")!=std::string::npos||
                   name.find("_walk_")!=std::string::npos||
                   name.find("_rush_")!=std::string::npos)return animationWalkTime;
                return animationIdleTime;
            };
            int from=unityRig.clipIndex(animationPrevious.c_str());
            int to=unityRig.clipIndex(animationCurrent.c_str());
            // The avatar is not drawn in first-person. Keep advancing clip
            // clocks and transitions, but do not skin/upload seven invisible
            // meshes every frame. Returning to third-person poses immediately.
            if(to>=0 && player.thirdPerson){
                if(from<0)from=to;
                unityRig.pose(from,clipTime(animationPrevious),to,clipTime(animationCurrent),animationCrossFade);
                ++phasePoseCalls;
            }else if(!player.thirdPerson && !firstPersonPoseSkipReported){
                TraceLog(LOG_INFO,"LIOS_FIRST_PERSON_POSE_SKIPPED");
                firstPersonPoseSkipReported=true;
            }
            if(player.thirdPerson)firstPersonPoseSkipReported=false;
        }

        // Update Particles
        for (size_t i = 0; i < particles.size(); ) {
            particles[i].life -= dt;
            if (particles[i].life <= 0.0f) {
                particles.erase(particles.begin() + i);
            } else {
                particles[i].position = Vector3Add(particles[i].position, Vector3Scale(particles[i].velocity, dt));
                particles[i].velocity.y -= 9.8f * dt; // Gravity
                i++;
            }
        }

        // Update Floating Notifications
        for (size_t i = 0; i < notifications.size(); ) {
            notifications[i].life -= dt;
            notifications[i].screenPos.y -= dt * 35.0f; // Float upwards
            if (notifications[i].life <= 0.0f) {
                notifications.erase(notifications.begin() + i);
            } else {
                i++;
            }
        }

        // ---------------------------------------------------------------------------------
        // Update Camera Position & Target
        // ---------------------------------------------------------------------------------
        Vector3 lookDir = {
            sinf(player.yaw) * cosf(player.pitch),
            sinf(player.pitch),
            cosf(player.yaw) * cosf(player.pitch)
        };

        // Follow Unity stance change with smoothly moving camera anchor.
        const float eyeGoal=prone?.65f:crouch?1.20f:1.65f;
        const float targetGoal=prone?.50f:crouch?1.00f:1.45f;
        const float eyeAlpha=std::min(1.f,dt*7.f);
        cameraEyeHeight+=(eyeGoal-cameraEyeHeight)*eyeAlpha;
        cameraTargetHeight+=(targetGoal-cameraTargetHeight)*eyeAlpha;
        camera.fovy=aim&&player.equippedSlot==1?48.0f:65.0f;
        if (player.thirdPerson) {
            // Match the working desktop viewer3d.py gluLookAt exactly.
            // At yaw=0 the avatar walks towards +Z and the camera MUST be
            // at negative Z, showing the BACK of the correctly skinned model.
            const float camDist=aim&&player.equippedSlot==1?2.3f:3.8f;
            Vector3 shoulderOffset=Vector3Scale(right,0.35f);
            Vector3 shoulderTarget=Vector3Add(player.position,shoulderOffset);
            camera.position={
                shoulderTarget.x-lookDir.x*camDist,
                player.position.y+cameraEyeHeight-lookDir.y*camDist,
                shoulderTarget.z-lookDir.z*camDist
            };
            camera.target={
                shoulderTarget.x+lookDir.x*2.0f,
                player.position.y+cameraTargetHeight+lookDir.y*2.0f,
                shoulderTarget.z+lookDir.z*2.0f
            };
        } else {
            // First Person mode
            camera.position = (Vector3){ player.position.x, player.position.y + cameraEyeHeight, player.position.z };
            camera.target = Vector3Add(camera.position, lookDir);
        }

        // ---------------------------------------------------------------------------------
        // 3D SCENE RENDERING
        // ---------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground((Color){ 130, 200, 230, 255 }); // Authentic Tropical Sky

        BeginMode3D(camera);

            // Unity LITE 7.3 exported source tile, with measured mesh heights.
            // Retain a clearly identified fallback if source assets fail.
            if(terrain.ready) terrain.draw(player.position);
            else DrawPlane((Vector3){ 0.0f, 0.0f, 0.0f },
                           (Vector2){ 600.0f, 600.0f }, (Color){ 68, 115, 52, 255 });
            // Do not draw raylib's debug grid over the terrain in release gameplay.
            // The real Android screenshot showed prominent parallel lines at the horizon.

            // Draw Base Building (Metal Cabin from original APK)
            if (modelHouse.meshCount > 0) {
                DrawModelEx(modelHouse, (Vector3){ 10.0f, houseGround, -15.0f }, (Vector3){ 0, 1, 0 }, 0.0f, (Vector3){ 1.0f, 1.0f, 1.0f }, WHITE);
            }

            // Draw Resource Nodes (Trees & Ores)
            for (const auto& node : nodes) {
                if (!node.active) continue;

                if (node.type == RES_TREE && modelTree.meshCount > 0) {
                    DrawModelEx(modelTree, node.position, (Vector3){ 0, 1, 0 }, node.rotation, (Vector3){ node.scale, node.scale, node.scale }, WHITE);
                } else if (node.type == RES_SULFUR && modelSulfur.meshCount > 0) {
                    DrawModelEx(modelSulfur, node.position, (Vector3){ 0, 1, 0 }, node.rotation, (Vector3){ node.scale, node.scale, node.scale }, WHITE);
                } else if (node.type == RES_IRON && modelIron.meshCount > 0) {
                    DrawModelEx(modelIron, node.position, (Vector3){ 0, 1, 0 }, node.rotation, (Vector3){ node.scale, node.scale, node.scale }, WHITE);
                } else if (node.type == RES_STONE && modelStone.meshCount > 0) {
                    DrawModelEx(modelStone, node.position, (Vector3){ 0, 1, 0 }, node.rotation, (Vector3){ node.scale, node.scale, node.scale }, WHITE);
                }
            }

            // Player-built, grid-snapped offline structures. Geometry is a
            // temporary placeholder while source map/build prefab extraction
            // is audited. Gameplay/costs/collision are implemented natively.
            for(const auto& part:building.placed){
                rlPushMatrix();
                rlTranslatef(0.f,terrain.ground(LIOS_BUILD::world(part.gx),
                                              LIOS_BUILD::world(part.gz)),0.f);
                LIOS_BUILD::drawPiece(part,buildSourceMaterials);
                rlPopMatrix();
            }
            if(buildMode&&activePanel==0&&building.selected>=0&&
               building.selected<LIOS_BUILD::PieceCount){
                const Vector3 dir={sinf(player.yaw),0.f,cosf(player.yaw)};
                const int gx=LIOS_BUILD::grid(player.position.x+dir.x*4.3f);
                const int gz=LIOS_BUILD::grid(player.position.z+dir.z*4.3f);
                const auto type=static_cast<LIOS_BUILD::PieceType>(building.selected);
                const LIOS_BUILD::Piece ghost={type,gx,gz,building.orientation};
                const bool ok=building.valid(type,gx,gz,building.orientation,
                                      player.position.x,player.position.z);
                rlPushMatrix();
                rlTranslatef(0.f,terrain.ground(LIOS_BUILD::world(gx),
                                              LIOS_BUILD::world(gz)),0.f);
                LIOS_BUILD::drawPiece(ghost,buildSourceMaterials,true,ok);
                rlPopMatrix();
            }

            // Draw Hit Particles
            for (const auto& p : particles) {
                DrawCube(p.position, p.size, p.size, p.size, p.color);
            }

            // Original 64-bone per-vertex deformation from PC, now native C++.
            if (player.thirdPerson && hasUnityRig) {
                rlPushMatrix();
                rlTranslatef(player.position.x,player.position.y,player.position.z);
                rlRotatef(player.yaw*RAD2DEG,0,1,0);
                // Unity's original renderer never culls the exported skinned
                // vertices by their Blender/OpenGL winding. Raylib defaults
                // to backface culling and could discard the full avatar.
                rlDisableBackfaceCulling();
                unityRig.draw();
                rlEnableBackfaceCulling();
                rlPopMatrix();
            }
            // Explicit fallback for a missing or corrupt native asset.
            if (player.thirdPerson && !hasUnityRig) {
                float charYawDeg = (player.yaw * RAD2DEG) + 180.0f;

                // Procedural Step Bobbing & Breathing
                float bobY = sinf(player.walkCycle) * 0.04f;
                float tiltAngle = sinf(player.walkCycle * 0.5f) * 2.5f;

                Vector3 charPos = { player.position.x, player.position.y + bobY, player.position.z };

                // Draw Head, Hair, Torso, Pants, Shoes
                // Unity OBJ coordinate fix: rotate -90 around X to stand upright, then rotate to face yaw
                rlPushMatrix();
                    rlTranslatef(charPos.x, charPos.y, charPos.z);
                    rlRotatef(charYawDeg, 0.0f, 1.0f, 0.0f);
                    rlRotatef(-90.0f, 1.0f, 0.0f, 0.0f); // Make OBJ upright

                    // Draw Character Mesh Parts
                    if (modelTorso.meshCount > 0) DrawModel(modelTorso, (Vector3){0,0,0}, 1.0f, WHITE);
                    if (modelPants.meshCount > 0) DrawModel(modelPants, (Vector3){0,0,0}, 1.0f, WHITE);
                    if (modelHead.meshCount > 0)  DrawModel(modelHead,  (Vector3){0,0,0}, 1.0f, WHITE);
                    if (modelHair.meshCount > 0)  DrawModel(modelHair,  (Vector3){0,0,0}, 1.0f, WHITE);
                    if (modelShoes.meshCount > 0) DrawModel(modelShoes, (Vector3){0,0,0}, 1.0f, WHITE);

                rlPopMatrix();

                // Draw Equipped Weapon attached to Right Hand Socket
                rlPushMatrix();
                    rlTranslatef(charPos.x, charPos.y, charPos.z);
                    rlRotatef(charYawDeg, 0.0f, 1.0f, 0.0f);

                    // Right Hand Socket position in local character space
                    float handX = 0.38f;
                    float handY = 0.85f;
                    float handZ = 0.15f;

                    // Animate weapon swing on attack
                    float swingAngle = 0.0f;
                    if (player.isAttacking) {
                        float prog = player.attackTimer / player.attackDuration;
                        swingAngle = sinf(prog * PI) * -75.0f; // Powerful downward chop
                    }

                    rlTranslatef(handX, handY, handZ);
                    rlRotatef(swingAngle, 1.0f, 0.0f, 0.0f);

                    if (player.equippedSlot == 0 && modelAxe.meshCount > 0) {
                        // Stone Axe (wq_shifu)
                        rlRotatef(-90.0f, 0.0f, 1.0f, 0.0f);
                        rlScalef(1.0f, 1.0f, 1.0f);
                        DrawModel(modelAxe, (Vector3){0, 0, 0}, 1.0f, WHITE);
                    } else if (player.equippedSlot == 1 && modelRifle.meshCount > 0) {
                        // M4A1 Assault Rifle (wq_m4_1)
                        rlRotatef(90.0f, 0.0f, 1.0f, 0.0f);
                        rlScalef(1.0f, 1.0f, 1.0f);
                        DrawModel(modelRifle, (Vector3){0, 0, 0}, 1.0f, WHITE);

                        // Muzzle flash on attack
                        if (player.isAttacking && player.attackTimer < 0.15f) {
                            DrawSphere((Vector3){0.0f, 0.05f, 0.8f}, 0.12f, GOLD);
                        }
                    }
                rlPopMatrix();
            }

            if(player.thirdPerson && hasUnityRig){
                // Weapon follows the real Unity wuqi_guadian bone (not a fixed
                // world-space fake hand or the older procedural swing).
                int socket=-1;
                for(size_t k=0;k<unityRig.boneNames.size();k++)
                    if(unityRig.boneNames[k]=="wuqi_guadian"){socket=int(k);break;}
                if(socket>=0){
                    // Same M_U2GL @ RX90 = identity as original PC rig.
                    LIOS::Mat LH=LIOS::ident();
                    LIOS::Mat m=LIOS::multiply(LH,unityRig.world[socket]);
                    float columnMajor[16];
                    for(int row=0;row<4;row++)for(int col=0;col<4;col++)
                        columnMajor[col*4+row]=m[row*4+col];
                    rlPushMatrix();
                    rlTranslatef(player.position.x,player.position.y,player.position.z);
                    rlRotatef(player.yaw*RAD2DEG,0,1,0);
                    rlMultMatrixf(columnMajor);
                    if(player.equippedSlot==0 && modelAxe.meshCount>0){
                        // Source wq_shifu socket conversion M_gl. Includes local
                        // translation and rotation (see skinned_character.py).
                        float originalAxeSocket[16]={
                            0,-1,0,0,
                            .3420201f,0,-.9396927f,0,
                            .9396927f,0,.3420201f,0,
                            .01589137f,.000635996f,-.1284298f,1
                        };
                        rlMultMatrixf(originalAxeSocket);
                        DrawModel(modelAxe,{0,0,0},1,WHITE);
                    }else if(player.equippedSlot==1 && modelRifle.meshCount>0){
                        DrawModel(modelRifle,{0,0,0},1,WHITE);
                    }
                    rlPopMatrix();
                }
            }
        EndMode3D();

        // ---------------------------------------------------------------------------------
        // 2D HUD / SURVIVAL INTERFACE (Optimized for Mobile & Desktop)
        // ---------------------------------------------------------------------------------
        if (hasOriginalHud) {
            // Both layers are generated directly from the Unity UGUI renderer:
            // includes source-selected slot highlight, weapon icons, controls,
            // compass, minimap and the correct rifle-only aim/reload buttons.
            const Texture2D &base=(buildMode&&originalHudBuild.id)?
                originalHudBuild:(player.equippedSlot==1)?originalHudRifle:originalHudMelee;
            DrawTexturePro(base,{0,0,float(base.width),float(base.height)},
                {canvasLeft,canvasTop,1920.f*canvasScale,1080.f*canvasScale},
                {0,0},0,WHITE);
            // Dynamic original Unity selected-state sprite (m_heightLignt).
            // No duplicate highlight when equipment changes or resources are tapped.
            const LIOS_HUD::Rect selectionRects[7]={
                LIOS_HUD::SlotHighlight0,LIOS_HUD::SlotHighlight1,
                LIOS_HUD::SlotHighlight2,LIOS_HUD::SlotHighlight3,
                LIOS_HUD::SlotHighlight4,LIOS_HUD::SlotHighlight5,
                LIOS_HUD::SlotHighlight6
            };
            if(!buildMode&&originalHotbarSelection.id&&selectedHotbarSlot>=0&&selectedHotbarSlot<7){
                const Rectangle sample={
                    float(selectedHotbarSlot*LIOS_HUD::HotbarHighlightTileW),
                    0,float(LIOS_HUD::HotbarHighlightTileW),
                    float(LIOS_HUD::HotbarHighlightTileH)
                };
                DrawTexturePro(originalHotbarSelection,sample,
                    sourceRect(selectionRects[selectedHotbarSlot]),{0,0},0,WHITE);
            }
            // Source BattlePanel_kedu_zw (PathID 7430988158235635878).
            // Previously compass on both HUD profiles was a static 0-degree
            // preview. Use original masked texture + decorated strip frames.
            if(originalCompass.id){
                float heading=fmodf(player.yaw*RAD2DEG,360.f);
                if(heading<0)heading+=360.f;
                int frame=int(floorf((heading+2.5f)/5.f))%LIOS_HUD::CompassFrames;
                Rectangle sample={
                    float((frame%4)*LIOS_HUD::CompassTileW),
                    float((frame/4)*LIOS_HUD::CompassTileH),
                    float(LIOS_HUD::CompassTileW),
                    float(LIOS_HUD::CompassTileH)
                };
                DrawTexturePro(originalCompass,sample,sourceRect(LIOS_HUD::Compass),
                               {0,0},0,WHITE);
            }
            auto sourceText=[&](const char* value,LIOS_HUD::Text origin){
                const Rectangle rect=sourceRect(origin.bounds);
                const int size=std::max(8,int(origin.fontSize*canvasScale));
                const int length=MeasureText(value,size);
                const int hGroup=origin.align%3;
                const int vGroup=origin.align/3;
                float x=rect.x+(hGroup==0?0.0f:hGroup==1?
                    (rect.width-length)/2.0f:rect.width-length);
                float y=rect.y+(vGroup==0?0.0f:vGroup==1?
                    (rect.height-size)/2.0f:rect.height-size);
                DrawText(value,int(x),int(y),size,
                    {origin.r,origin.g,origin.b,origin.a});
            };
            sourceText(TextFormat("%d",int(player.health)),LIOS_HUD::Health);
            sourceText(TextFormat("%d",int(player.hunger)),LIOS_HUD::Food);
            sourceText(TextFormat("%d",int(player.thirst)),LIOS_HUD::Water);
            // Original equipped icons/selection are baked into correct
            // per-weapon source UGUI profile. Draw ONLY runtime resource slots.
            const int counts[4]={player.woodCount,player.ironCount,
                                 player.stoneCount,player.sulfurCount};
            const LIOS_HUD::Rect sourceIcons[4]={
                LIOS_HUD::Slot2,LIOS_HUD::Slot3,
                LIOS_HUD::Slot4,LIOS_HUD::Slot5};
            const LIOS_HUD::Text sourceCountRects[4]={
                LIOS_HUD::SlotCount2,LIOS_HUD::SlotCount3,
                LIOS_HUD::SlotCount4,LIOS_HUD::SlotCount5};
            for(int i=0;i<4;i++){
                if(buildMode||counts[i]<=0)continue;
                const Texture2D icon=sourceResources[i];
                if(icon.id){
                    Rectangle b=sourceRect(sourceIcons[i]);
                    DrawTexturePro(icon,{0,0,float(icon.width),float(icon.height)},
                                   b,{0,0},0,WHITE);
                }
                sourceText(TextFormat("%d",counts[i]),sourceCountRects[i]);
            }
            // After producing the actual builder item, use the final
            // quickslot to enter BuildState. Source construction-hammer
            // icon cj_jzc extracted from original tubiao1 bundle.
            if(building.blueprint&&!buildMode&&originalCraftArt.hammer.id){
                const Rectangle b=sourceRect(LIOS_HUD::Slot6);
                DrawTexturePro(originalCraftArt.hammer,
                    {0,0,float(originalCraftArt.hammer.width),
                     float(originalCraftArt.hammer.height)},b,{0,0},0,WHITE);
            }
            // Live states on source buttons. The visual base is the real
            // original UI; functional panels remain native provisional UI.
            if(autoRun)DrawRectangleLinesEx(sourceRect(LIOS_HUD::Sprint),2*canvasScale,GREEN);
            if(crouch)DrawRectangleLinesEx(sourceRect(LIOS_HUD::Crouch),2*canvasScale,SKYBLUE);
            if(prone)DrawRectangleLinesEx(sourceRect(LIOS_HUD::Prone),2*canvasScale,SKYBLUE);
            if(aim&&player.equippedSlot==1)DrawRectangleLinesEx(sourceRect(LIOS_HUD::Aim),2*canvasScale,GREEN);
            if(player.equippedSlot==1){
                sourceText(TextFormat("%d / %d",magazine,reserveAmmo),
                    LIOS_HUD::SlotCount1);
            }
            if(activePanel!=0&&activePanel!=2){
                Rectangle panel={canvasLeft+620*canvasScale,
                                 canvasTop+180*canvasScale,680*canvasScale,510*canvasScale};
                DrawRectangleRec(panel,Fade(BLACK,.87f));
                DrawRectangleLinesEx(panel,2*canvasScale,RAYWHITE);
                int fs=std::max(12,int(33*canvasScale));
                const char* titles[]={"","MOCHILA","FABRICAR","CONFIGURACOES","CHAT RAPIDO"};
                DrawText(titles[activePanel],int(panel.x+35*canvasScale),
                         int(panel.y+32*canvasScale),fs,WHITE);
                int bs=std::max(10,int(24*canvasScale));
                if(activePanel==1){
                    DrawText(TextFormat("Madeira: %d   Metal: %d",player.woodCount,player.ironCount),
                        int(panel.x+35*canvasScale),int(panel.y+120*canvasScale),bs,RAYWHITE);
                    DrawText(TextFormat("Pedra: %d   Enxofre: %d",player.stoneCount,player.sulfurCount),
                        int(panel.x+35*canvasScale),int(panel.y+170*canvasScale),bs,RAYWHITE);
                }else if(activePanel==2){
                    // Actual craft actions are handled on touch-down above.
                    // Costs are prototype values, not proven original CFG.
                    const char* names[]={
                        "PLANTA DE CONSTRUCAO",
                        "MARTELO",
                        "PICARETA DE PEDRA"
                    };
                    const bool owned[]={
                        building.blueprint,building.hammer,building.pickaxe
                    };
                    for(int i=0;i<3;i++){
                        Rectangle option=panelBox(675.f,300.f+i*82.f,550.f,65.f);
                        DrawRectangleRec(option,Fade(owned[i]?DARKGREEN:DARKGRAY,.87f));
                        DrawRectangleLinesEx(option,2.f*canvasScale,RAYWHITE);
                        DrawText(names[i],int(option.x+12*canvasScale),
                                 int(option.y+8*canvasScale),bs,WHITE);
                        const auto cost=LIOS_BUILD::CRAFT_COSTS[i];
                        DrawText(TextFormat("Madeira %d  Pedra %d  Metal %d%s",
                            cost.wood,cost.stone,cost.metal,owned[i]?"  [PRONTO]":""),
                            int(option.x+12*canvasScale),int(option.y+35*canvasScale),
                            std::max(9,int(18*canvasScale)),LIGHTGRAY);
                    }
                    Rectangle open=panelBox(675.f,552.f,550.f,73.f);
                    DrawRectangleRec(open,Fade(DARKGREEN,.88f));
                    DrawText("ENTRAR NO MODO CONSTRUCAO",
                        int(open.x+12*canvasScale),int(open.y+22*canvasScale),bs,WHITE);
                    DrawText(buildStatus.c_str(),int(panel.x+24*canvasScale),
                             int(panel.y+450*canvasScale),
                             std::max(9,int(18*canvasScale)),YELLOW);
                }else if(activePanel==3){
                    DrawText("Som: ativado (audio original M4, machado, coleta)",
                        int(panel.x+35*canvasScale),int(panel.y+140*canvasScale),bs,RAYWHITE);
                }else if(activePanel==4){
                    DrawText("Chat multiplayer indisponivel no modo offline",
                        int(panel.x+35*canvasScale),int(panel.y+140*canvasScale),bs,RAYWHITE);
                }
                DrawText("Toque no botao novamente para fechar",
                    int(panel.x+30*canvasScale),int(panel.y+435*canvasScale),bs,GRAY);
            }
            // The PNGs intentionally omit m_littleYaogan: render original
            // Unity thumb Sprite once at its actual touch-controlled position.
            if(originalThumb.id){
                Rectangle b=sourceRect(LIOS_HUD::JoystickThumb);
                b.x=leftStick.knob.x-b.width/2.f;
                b.y=leftStick.knob.y-b.height/2.f;
                DrawTexturePro(originalThumb,
                    {0,0,float(originalThumb.width),float(originalThumb.height)},
                    b,{0,0},0,WHITE);
            }
        } else {
        // Crosshair in center
        int cx = screenWidth / 2;
        int cy = screenHeight / 2;
        DrawRectangle(cx - 5, cy - 1, 10, 2, Fade(WHITE, 0.8f));
        DrawRectangle(cx - 1, cy - 5, 2, 10, Fade(WHITE, 0.8f));

        // Top Status Header
        DrawRectangle(10, 10, 360, 80, Fade(BLACK, 0.65f));
        DrawText("LAST ISLAND OF SURVIVAL", 20, 16, 16, GOLD);
        DrawText(hasUnityRig ? "Original 33 Unity Animations / Native Android" :
                  "Original skinning unavailable - simplified fallback",
                  20, 36, 12, RAYWHITE);
        DrawText(TextFormat("FPS: %i | Coletados: Made=%i Enx=%i Fer=%i Ped=%i",
            GetFPS(), player.woodCount, player.sulfurCount, player.ironCount, player.stoneCount),
            20, 56, 12, GREEN);

        // Survival Vitals Bar (Bottom Left)
        int vitalsY = screenHeight - 90;
        int barW = 160;
        int barH = 12;

        // Health Bar (Red)
        DrawRectangle(20, vitalsY, barW, barH, Fade(DARKGRAY, 0.8f));
        DrawRectangle(20, vitalsY, (int)(barW * (player.health / player.maxHealth)), barH, (Color){ 220, 40, 40, 255 });
        DrawText("VIDA 100%", 25, vitalsY + 1, 10, WHITE);

        // Stamina Bar (Yellow/Orange)
        DrawRectangle(20, vitalsY + 16, barW, barH, Fade(DARKGRAY, 0.8f));
        DrawRectangle(20, vitalsY + 16, (int)(barW * (player.stamina / player.maxStamina)), barH, (Color){ 230, 170, 30, 255 });
        DrawText("ENERGIA", 25, vitalsY + 17, 10, WHITE);

        // Hunger Bar (Orange)
        DrawRectangle(20, vitalsY + 32, barW, barH, Fade(DARKGRAY, 0.8f));
        DrawRectangle(20, vitalsY + 32, (int)(barW * (player.hunger / 100.0f)), barH, (Color){ 200, 100, 30, 255 });
        DrawText("FOME", 25, vitalsY + 33, 10, WHITE);

        // Thirst Bar (Blue)
        DrawRectangle(20, vitalsY + 48, barW, barH, Fade(DARKGRAY, 0.8f));
        DrawRectangle(20, vitalsY + 48, (int)(barW * (player.thirst / 100.0f)), barH, (Color){ 40, 140, 220, 255 });
        DrawText("SEDE", 25, vitalsY + 49, 10, WHITE);

        // Minimap / Compass (Top Right)
        int mapSize = 100;
        int mapX = screenWidth - mapSize - 20;
        int mapY = 20;
        DrawCircle(mapX + mapSize/2, mapY + mapSize/2, mapSize/2, Fade(BLACK, 0.6f));
        DrawCircleLines(mapX + mapSize/2, mapY + mapSize/2, mapSize/2, GOLD);
        DrawCircle(mapX + mapSize/2, mapY + mapSize/2, 4, RED); // Player position
        DrawText("N", mapX + mapSize/2 - 4, mapY + 6, 12, RAYWHITE);

        // Bottom Hotbar (Weapon Slots)
        int slotW = 60;
        int slotH = 55;
        int hotbarTotalW = slotW * 2 + 10;
        int hotbarX = (screenWidth - hotbarTotalW) / 2;
        int hotbarY = screenHeight - 65;

        // Slot 0: Axe
        Color slot0Color = (player.equippedSlot == 0) ? GOLD : Fade(BLACK, 0.7f);
        DrawRectangle(hotbarX, hotbarY, slotW, slotH, slot0Color);
        DrawRectangleLines(hotbarX, hotbarY, slotW, slotH, (player.equippedSlot == 0) ? YELLOW : GRAY);
        DrawText("1: MACH", hotbarX + 8, hotbarY + 18, 11, WHITE);

        // Slot 1: Rifle
        int slot1X = hotbarX + slotW + 10;
        Color slot1Color = (player.equippedSlot == 1) ? GOLD : Fade(BLACK, 0.7f);
        DrawRectangle(slot1X, hotbarY, slotW, slotH, slot1Color);
        DrawRectangleLines(slot1X, hotbarY, slotW, slotH, (player.equippedSlot == 1) ? YELLOW : GRAY);
        DrawText("2: M4A1", slot1X + 10, hotbarY + 18, 11, WHITE);

        // Render Floating Resource Notifications
        for (const auto& notif : notifications) {
            float alpha = notif.life / notif.maxLife;
            DrawText(notif.text.c_str(), (int)notif.screenPos.x, (int)notif.screenPos.y, 18, Fade(notif.color, alpha));
        }

        // ---------------------------------------------------------------------------------
        // Mobile Virtual Controls Rendering (Touch Joysticks & Buttons)
        // ---------------------------------------------------------------------------------
        // Left Virtual Joystick
        DrawCircleV(leftStick.center, leftStick.radius, Fade(GRAY, 0.25f));
        DrawCircleLines((int)leftStick.center.x, (int)leftStick.center.y, leftStick.radius, Fade(WHITE, 0.4f));
        DrawCircleV(leftStick.knob, 28.0f, Fade(LIGHTGRAY, 0.6f));

        // Touch Action Buttons (Right side)
        auto DrawBtn = [](const TouchButton& btn) {
            Color c = btn.isPressed ? btn.activeColor : btn.baseColor;
            DrawRectangleRounded(btn.bounds, 0.4f, 8, c);
            DrawRectangleRoundedLines(btn.bounds, 0.4f, 8, WHITE);
            int tw = MeasureText(btn.label, 14);
            DrawText(btn.label, (int)(btn.bounds.x + (btn.bounds.width - tw)/2), (int)(btn.bounds.y + (btn.bounds.height - 14)/2), 14, WHITE);
        };

        DrawBtn(btnAttack);
        DrawBtn(btnJump);
        DrawBtn(btnSprint);
        DrawBtn(btnCam);
        } // Original Unity HUD versus simplified desktop fallback

        if(activePanel==2){
            const LIOS_BUILD::Resources available={
                player.woodCount,player.stoneCount,player.ironCount,player.sulfurCount};
            if(showOfflineCraftPrototype){
                LIOS_CRAFT_UI::render(originalCraftArt,building,available,
                    craftSelectedRecipe,craftQuantity,craftBigTab,craftSmallTab,
                    craftSelectedOrder,craftStatus,canvasScale,canvasLeft,canvasTop);
            }else{
                LIOS_REAL_CRAFT::render(realCraftArt,realCraft,available,
                    canvasScale,canvasLeft,canvasTop);
            }
            // Only native developer fallback: allows already-implemented
            // builder test items to remain obtainable on Android.
            DrawRectangleRec(panelBox(1685,1011,225,60),
                Color{30,40,44,235});
            DrawRectangleLinesEx(panelBox(1685,1011,225,60),
                1.5f*canvasScale,GRAY);
            DrawText(showOfflineCraftPrototype?"CATALOGO ORIGINAL":"TESTE OFFLINE",
                int(canvasLeft+1697*canvasScale),
                int(canvasTop+1031*canvasScale),
                std::max(11,int(18*canvasScale)),LIGHTGRAY);
        }
        if(buildMode&&activePanel==0&&originalBuildCategories.id){
            // Exact ORIGINAL Unity UGUI sprites, generated from Image.m_Sprite
            // with FileID+PathID provenance. Original HorizontalLayoutGroup
            // distributes category tabs across m_buildPartTypes RectTransform.
            const LIOS_HUD::Rect tabs[]={
                LIOS_HUD::Build_btn_null,LIOS_HUD::Build_btn_lantu,
                LIOS_HUD::Build_btn_fundation,LIOS_HUD::Build_btn_wall,
                LIOS_HUD::Build_btn_floor,LIOS_HUD::Build_btn_stair,
                LIOS_HUD::Build_btn_window,LIOS_HUD::Build_btn_obstacle
            };
            for(int i=0;i<LIOS_HUD::BuildCategoryCount;i++){
                DrawTexturePro(originalBuildCategories,
                    {float(i*LIOS_HUD::BuildCategoryTileW),0.f,
                     float(LIOS_HUD::BuildCategoryTileW),
                     float(LIOS_HUD::BuildCategoryTileH)},
                    sourceRect(tabs[i]),{0,0},0,WHITE);
            }
            if(originalBuildSelected.id&&buildCategory>=0&&buildCategory<8&&buildCategory!=1){
                Rectangle target=sourceRect(tabs[buildCategory]);
                // Source Checkmark nested inside the source 95px Toggle.
                target.x+=target.width*.10f;target.y+=target.height*.10f;
                target.width*=.80f;target.height*=.80f;
                DrawTexturePro(originalBuildSelected,
                    {0,0,float(originalBuildSelected.width),
                     float(originalBuildSelected.height)},target,{0,0},0,WHITE);
            }
            if(building.selected>=0&&sourceBuildOnceAngle>0&&originalBuildRotate.id)
                DrawTexturePro(originalBuildRotate,
                    {0,0,float(originalBuildRotate.width),
                     float(originalBuildRotate.height)},
                    sourceRect(LIOS_HUD::Build_btn_opRotate),
                    {0,0},0,WHITE);
            if(buildSubmenuOpen&&buildCategory>=2&&originalBuildChildren.id){
                // The original scene contains 11 Toggle children in
                // m_buildChilds. Draw only slots that have an implemented
                // native BuildPart selection; the Unity runtime hides the
                // unused ones after FillBuildChildsIcons(type).
                const LIOS_HUD::Rect partSlots[]={
                    LIOS_HUD::BuildChildBox0,LIOS_HUD::BuildChildBox1,
                    LIOS_HUD::BuildChildBox2,LIOS_HUD::BuildChildBox3,
                    LIOS_HUD::BuildChildBox4,LIOS_HUD::BuildChildBox5,
                    LIOS_HUD::BuildChildBox6,LIOS_HUD::BuildChildBox7,
                    LIOS_HUD::BuildChildBox8,LIOS_HUD::BuildChildBox9,
                    LIOS_HUD::BuildChildBox10
                };
                static constexpr int sourceTypes[8]={0,0,2,3,8,4,6,7};
                const int typ=sourceTypes[buildCategory];
                const int origCount=LIOS_SOURCE_BUILD::count(typ);
                const Texture2D &atlas=originalBuildItemAtlases[buildCategory-2];
                for(int n=0;n<LIOS_HUD::BuildChildCount&&n<origCount;n++){
                    const LIOS_HUD::Rect &slot=partSlots[n];
                    Rectangle target=sourceRect(slot);
                    DrawTexturePro(originalBuildChildren,
                        {slot.left,slot.top,slot.right-slot.left,slot.bottom-slot.top},
                        target,{0,0},0,WHITE);
                    if(atlas.id){
                        Rectangle iconRect=target;
                        iconRect.x+=target.width*.11f;iconRect.y+=target.height*.11f;
                        iconRect.width*=.78f;iconRect.height*=.78f;
                        DrawTexturePro(atlas,{float(n*120),0.f,120.f,120.f},
                            iconRect,{0,0},0,WHITE);
                    }
                    if(buildSubitem==n&&originalBuildSelected.id)
                        DrawTexturePro(originalBuildSelected,
                            {0,0,float(originalBuildSelected.width),
                             float(originalBuildSelected.height)},target,{0,0},0,WHITE);
                }
            }
            if(buildBagOpen){
                // The original btn_null toggles scp_bagBuilds (inventory
                // building items). Only two offline crafted options exist.
                const Rectangle box=sourceRect(LIOS_HUD::Build_scp_bagBuilds);
                DrawRectangleRec(box,Fade(BLACK,.72f));
                DrawRectangleLinesEx(box,2.f,RAYWHITE);
                DrawText("BAU",int(canvasLeft+725.f*canvasScale),
                    int(canvasTop+688.f*canvasScale),
                    std::max(12,int(24.f*canvasScale)),RAYWHITE);
                DrawText("FOGUEIRA",int(canvasLeft+846.f*canvasScale),
                    int(canvasTop+688.f*canvasScale),
                    std::max(12,int(24.f*canvasScale)),RAYWHITE);
            }
            // The original btn_opRotate Image is a dark 9-slice background;
            // its actual caption is a Unity UGUI Text component. In this
            // renderer a readable font fallback preserves button usability.
            if(building.selected>=0&&sourceBuildOnceAngle>0){
                const Rectangle rotate=sourceRect(LIOS_HUD::Build_btn_opRotate);
                DrawText("GIRAR",int(rotate.x+rotate.width*.28f),
                    int(rotate.y+rotate.height*.28f),
                    std::max(12,int(31.f*canvasScale)),WHITE);
            }
            if(building.freePlacement&&originalBuildSelected.id){
                const Rectangle t=sourceRect(LIOS_HUD::Build_btn_freeBuildModel);
                DrawTexturePro(originalBuildSelected,
                    {0,0,float(originalBuildSelected.width),
                     float(originalBuildSelected.height)},t,{0,0},0,WHITE);
            }
            // Dynamic status is not a fabricated clickable button.
            DrawText(TextFormat("%s  /  %d",
                buildStatus.c_str(),int(building.placed.size())),
                int(canvasLeft+360.f*canvasScale),
                int(canvasTop+700.f*canvasScale),
                std::max(12,int(21.f*canvasScale)),YELLOW);
        }
        const auto endDrawingStart=std::chrono::steady_clock::now();
        EndDrawing();
        const auto frameWorkEnd=std::chrono::steady_clock::now();
        const double endDrawingMs=std::chrono::duration<double,std::milli>(
            frameWorkEnd-endDrawingStart).count();
        const double frameWorkMs=std::chrono::duration<double,std::milli>(
            frameWorkEnd-frameWorkStart).count();
        if(player.thirdPerson)++phaseVisibleFrames;
        phasePoseTotal+=unityRig.lastPoseCpuMs;
        phaseVboTotal+=unityRig.lastVboSubmitMs;
        phaseEndTotal+=endDrawingMs;
        phaseWorkTotal+=frameWorkMs;
        if(++phaseFrames==60){
            TraceLog(LOG_INFO,
                "LIOS_PHASE_TIMING frames=60 pose_cpu_ms=%.3f vbo_submit_ms=%.3f end_drawing_ms=%.3f frame_work_ms=%.3f visible_frames=%u pose_calls=%u",
                phasePoseTotal/60.0,phaseVboTotal/60.0,
                phaseEndTotal/60.0,phaseWorkTotal/60.0,
                phaseVisibleFrames,phasePoseCalls);
            phaseVisibleFrames=phasePoseCalls=0;
            phaseFrames=0;
            phasePoseTotal=phaseVboTotal=phaseEndTotal=phaseWorkTotal=0.0;
        }
        // A live PID and one completed swap do not prove sustained rendering:
        // some emulator runs display a uniform grass frame indefinitely.
        // This heartbeat distinguishes a continuing native render loop from
        // a stall after the first frame without fabricating visual success.
        static unsigned int completedFrames = 0;
        ++completedFrames;
        if (completedFrames == 1 || completedFrames % 60 == 0) {
            TraceLog(LOG_INFO,
                     "LIOS_RENDER_HEARTBEAT frame=%u screen=%dx%d canvas=%.3f hud=%d panel=%d camera_y=%.3f",
                     completedFrames,GetScreenWidth(),GetScreenHeight(),
                     canvasScale,int(hasOriginalHud),activePanel,camera.position.y);
        }
        // The Android smoke runner must wait for a completed render/swap, not
        // merely a live process while Unity-sourced assets are still loading.
        static bool firstFrameReported = false;
        if (!firstFrameReported) {
            TraceLog(LOG_INFO, "LIOS_FIRST_FRAME_ENDDRAWING");
            firstFrameReported = true;
        }
    }

    // -------------------------------------------------------------------------------------
    // Cleanup Resources
    // -------------------------------------------------------------------------------------
    sound.shutdown();
    unityRig.unload();
    if(originalHudMelee.id)UnloadTexture(originalHudMelee);
    if(originalHudRifle.id)UnloadTexture(originalHudRifle);
    if(originalThumb.id)UnloadTexture(originalThumb);
    if(originalHotbarSelection.id)UnloadTexture(originalHotbarSelection);
    if(originalCompass.id)UnloadTexture(originalCompass);
    for(auto &icon:sourceResources)if(icon.id)UnloadTexture(icon);
    UnloadModel(modelTorso);   UnloadTexture(texTorso);
    UnloadModel(modelPants);   UnloadTexture(texPants);
    UnloadModel(modelHead);    UnloadTexture(texHead);
    UnloadModel(modelHair);    UnloadTexture(texHair);
    UnloadModel(modelShoes);   UnloadTexture(texShoes);
    if(texSkin.id)UnloadTexture(texSkin);
    UnloadModel(modelAxe);     UnloadTexture(texAxe);
    UnloadModel(modelRifle);   UnloadTexture(texRifle);
    UnloadModel(modelHouse);   UnloadTexture(texHouse);
    UnloadModel(modelTree);    UnloadTexture(texTree);
    UnloadModel(modelSulfur);  UnloadTexture(texSulfur);
    UnloadModel(modelIron);    UnloadTexture(texIron);
    UnloadModel(modelStone);   UnloadTexture(texStone);
    terrain.unload();

    CloseWindow();
    return 0;
}
