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

#include <vector>
#include <string>
#include <cmath>
#include <cstdlib>

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

    SetConfigFlags(FLAG_MSAA_4X_HINT | FLAG_WINDOW_RESIZABLE);
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
    player.speed = 5.5f;
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
    Texture2D texTorso = LoadTexture("assets/man_clothes_chushijuese_01_ba.png");
    if (modelTorso.meshCount > 0 && texTorso.id > 0)
        SetMaterialTexture(&modelTorso.materials[0], MATERIAL_MAP_DIFFUSE, texTorso);

    Model modelPants   = LoadModel("assets/man_trousers_chushijuese_01.obj");
    Texture2D texPants = LoadTexture("assets/man_trousers_chushijuese_01_ba.png");
    if (modelPants.meshCount > 0 && texPants.id > 0)
        SetMaterialTexture(&modelPants.materials[0], MATERIAL_MAP_DIFFUSE, texPants);

    Model modelHead    = LoadModel("assets/man_head_chushijuese_01.obj");
    Texture2D texHead  = LoadTexture("assets/man_head_chushijuese_01_ba.png");
    if (modelHead.meshCount > 0 && texHead.id > 0)
        SetMaterialTexture(&modelHead.materials[0], MATERIAL_MAP_DIFFUSE, texHead);

    Model modelHair    = LoadModel("assets/man_hair_chushijuese_01.obj");
    Texture2D texHair  = LoadTexture("assets/man_hair_chushijuese_01_ba.png");
    if (modelHair.meshCount > 0 && texHair.id > 0)
        SetMaterialTexture(&modelHair.materials[0], MATERIAL_MAP_DIFFUSE, texHair);

    Model modelShoes   = LoadModel("assets/man_shoes_chushijuese_01.obj");
    Texture2D texShoes = LoadTexture("assets/man_shoes_chushijuese_01_ba.png");
    if (modelShoes.meshCount > 0 && texShoes.id > 0)
        SetMaterialTexture(&modelShoes.materials[0], MATERIAL_MAP_DIFFUSE, texShoes);

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

    std::vector<HitParticle> particles;
    std::vector<FloatingNotification> notifications;

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
    while (!WindowShouldClose()) {
        float dt = GetFrameTime();
        if (dt > 0.05f) dt = 0.05f; // Clamp delta time

        // Desktop toggle cursor lock
        if (IsKeyPressed(KEY_ESCAPE)) {
            cursorLocked = !cursorLocked;
            if (cursorLocked) DisableCursor();
            else EnableCursor();
        }

        // Toggle camera with TAB or V
        if (IsKeyPressed(KEY_TAB) || IsKeyPressed(KEY_V)) {
            player.thirdPerson = !player.thirdPerson;
        }

        // Switch Weapon with Keys 1 and 2
        if (IsKeyPressed(KEY_ONE)) player.equippedSlot = 0;
        if (IsKeyPressed(KEY_TWO)) player.equippedSlot = 1;

        // ---------------------------------------------------------------------------------
        // Touch Input Processing (Multi-touch support for Mobile)
        // ---------------------------------------------------------------------------------
        int touchCount = GetTouchPointCount();
        bool hasTouchActive = (touchCount > 0);

        // Reset button states each frame
        btnAttack.isPressed = false;
        btnJump.isPressed = false;
        btnSprint.isPressed = false;
        btnCam.isPressed = false;
        btnSlot0.isPressed = false;
        btnSlot1.isPressed = false;

        Vector2 touchMoveDir = { 0.0f, 0.0f };

        // Process all active touches
        for (int i = 0; i < touchCount; i++) {
            Vector2 touchPos = GetTouchPosition(i);

            // Left Joystick interaction (left side of screen)
            if (touchPos.x < (float)screenWidth * 0.45f && touchPos.y > (float)screenHeight * 0.4f) {
                if (!leftStick.active) {
                    leftStick.active = true;
                    leftStick.center = touchPos;
                    leftStick.knob = touchPos;
                    leftStick.touchId = i;
                } else if (leftStick.touchId == i) {
                    Vector2 diff = Vector2Subtract(touchPos, leftStick.center);
                    float dist = Vector2Length(diff);
                    if (dist > leftStick.radius) {
                        diff = Vector2Scale(Vector2Normalize(diff), leftStick.radius);
                    }
                    leftStick.knob = Vector2Add(leftStick.center, diff);
                    touchMoveDir = Vector2Scale(diff, 1.0f / leftStick.radius);
                }
            }

            // Right side swipe camera look (if not pressing buttons)
            if (touchPos.x > (float)screenWidth * 0.5f && touchPos.y < (float)screenHeight - 160.0f) {
                Vector2 delta = GetMouseDelta();
                player.yaw -= delta.x * 0.005f;
                player.pitch -= delta.y * 0.005f;
            }

            // Buttons check
            if (CheckCollisionPointRec(touchPos, btnAttack.bounds)) btnAttack.isPressed = true;
            if (CheckCollisionPointRec(touchPos, btnJump.bounds)) btnJump.isPressed = true;
            if (CheckCollisionPointRec(touchPos, btnSprint.bounds)) btnSprint.isPressed = true;
            if (CheckCollisionPointRec(touchPos, btnCam.bounds)) btnCam.isPressed = true;
            if (CheckCollisionPointRec(touchPos, btnSlot0.bounds)) btnSlot0.isPressed = true;
            if (CheckCollisionPointRec(touchPos, btnSlot1.bounds)) btnSlot1.isPressed = true;
        }

        if (touchCount == 0) {
            leftStick.active = false;
            leftStick.knob = leftStick.center;
            leftStick.touchId = -1;
        }

        // Apply touch button clicks
        if (btnCam.isPressed) player.thirdPerson = !player.thirdPerson;
        if (btnSlot0.isPressed) player.equippedSlot = 0;
        if (btnSlot1.isPressed) player.equippedSlot = 1;

        // ---------------------------------------------------------------------------------
        // Desktop Mouse Look Processing
        // ---------------------------------------------------------------------------------
        if (cursorLocked && !hasTouchActive) {
            Vector2 mouseDelta = GetMouseDelta();
            const float mouseSensitivity = 0.003f;
            player.yaw -= mouseDelta.x * mouseSensitivity;
            player.pitch -= mouseDelta.y * mouseSensitivity;
        }

        // Clamp camera pitch
        if (player.pitch > 1.35f) player.pitch = 1.35f;
        if (player.pitch < -1.35f) player.pitch = -1.35f;

        // Calculate direction vectors from player yaw
        Vector3 forward = { sinf(player.yaw), 0.0f, cosf(player.yaw) };
        Vector3 right   = { cosf(player.yaw), 0.0f, -sinf(player.yaw) };

        // ---------------------------------------------------------------------------------
        // Player Movement Logic (Keyboard + Mobile Joystick)
        // ---------------------------------------------------------------------------------
        Vector3 moveDir = { 0.0f, 0.0f, 0.0f };

        // Keyboard input
        if (IsKeyDown(KEY_W)) moveDir = Vector3Add(moveDir, forward);
        if (IsKeyDown(KEY_S)) moveDir = Vector3Subtract(moveDir, forward);
        if (IsKeyDown(KEY_D)) moveDir = Vector3Subtract(moveDir, right);
        if (IsKeyDown(KEY_A)) moveDir = Vector3Add(moveDir, right);

        // Add Touch Joystick movement
        if (leftStick.active) {
            Vector3 stickMove = Vector3Add(
                Vector3Scale(forward, -touchMoveDir.y),
                Vector3Scale(right, -touchMoveDir.x)
            );
            moveDir = Vector3Add(moveDir, stickMove);
        }

        // Sprint logic
        player.isRunning = (IsKeyDown(KEY_LEFT_SHIFT) || btnSprint.isPressed) && (player.stamina > 5.0f);
        float currentSpeed = player.speed * (player.isRunning ? 1.75f : 1.0f);

        if (Vector3Length(moveDir) > 0.05f) {
            moveDir = Vector3Normalize(moveDir);
            player.position = Vector3Add(player.position, Vector3Scale(moveDir, currentSpeed * dt));
            player.walkCycle += dt * (player.isRunning ? 12.0f : 8.0f);

            if (player.isRunning) {
                player.stamina -= dt * 15.0f;
                if (player.stamina < 0.0f) player.stamina = 0.0f;
            }
        } else {
            // Regenerate stamina when not running
            if (player.stamina < player.maxStamina) {
                player.stamina += dt * 12.0f;
                if (player.stamina > player.maxStamina) player.stamina = player.maxStamina;
            }
        }

        // Jump and Gravity Physics
        const float gravity = 24.0f;
        bool wantJump = IsKeyPressed(KEY_SPACE) || btnJump.isPressed;
        if (player.isGrounded) {
            if (wantJump && player.stamina >= 10.0f) {
                player.verticalVelocity = 8.5f;
                player.isGrounded = false;
                player.stamina -= 10.0f;
            }
        } else {
            player.verticalVelocity -= gravity * dt;
            player.position.y += player.verticalVelocity * dt;
            if (player.position.y <= 0.0f) {
                player.position.y = 0.0f;
                player.verticalVelocity = 0.0f;
                player.isGrounded = true;
            }
        }

        // ---------------------------------------------------------------------------------
        // Attack & Resource Gathering System
        // ---------------------------------------------------------------------------------
        bool wantAttack = (IsMouseButtonPressed(MOUSE_BUTTON_LEFT) && cursorLocked) || btnAttack.isPressed;

        if (wantAttack && !player.isAttacking) {
            player.isAttacking = true;
            player.attackTimer = 0.0f;
            player.attackHitRegistered = false;
        }

        if (player.isAttacking) {
            player.attackTimer += dt;

            // Register hit at impact peak (halfway through swing: ~0.25s)
            if (player.attackTimer >= 0.22f && !player.attackHitRegistered) {
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

        if (player.thirdPerson) {
            float camDist = 3.6f;
            Vector3 camOffset = Vector3Scale(lookDir, -camDist);
            // Right shoulder offset for third-person perspective (like original Last Island)
            Vector3 shoulderOffset = Vector3Scale(right, 0.4f);
            camera.position = (Vector3){
                player.position.x + camOffset.x + shoulderOffset.x,
                player.position.y + 1.65f + camOffset.y,
                player.position.z + camOffset.z + shoulderOffset.z
            };
            camera.target = (Vector3){
                player.position.x + shoulderOffset.x,
                player.position.y + 1.45f,
                player.position.z + shoulderOffset.z
            };
        } else {
            // First Person mode
            camera.position = (Vector3){ player.position.x, player.position.y + 1.65f, player.position.z };
            camera.target = Vector3Add(camera.position, lookDir);
        }

        // ---------------------------------------------------------------------------------
        // 3D SCENE RENDERING
        // ---------------------------------------------------------------------------------
        BeginDrawing();
        ClearBackground((Color){ 130, 200, 230, 255 }); // Authentic Tropical Sky

        BeginMode3D(camera);

            // Ground Plane: Island Terrain Grass
            DrawPlane((Vector3){ 0.0f, 0.0f, 0.0f }, (Vector2){ 600.0f, 600.0f }, (Color){ 68, 115, 52, 255 });
            DrawGrid(60, 5.0f);

            // Draw Base Building (Metal Cabin from original APK)
            if (modelHouse.meshCount > 0) {
                DrawModelEx(modelHouse, (Vector3){ 10.0f, 0.0f, -15.0f }, (Vector3){ 0, 1, 0 }, 0.0f, (Vector3){ 1.0f, 1.0f, 1.0f }, WHITE);
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

            // Draw Hit Particles
            for (const auto& p : particles) {
                DrawCube(p.position, p.size, p.size, p.size, p.color);
            }

            // Draw Full Authentic Skinned Character in 3rd Person
            if (player.thirdPerson) {
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

        EndMode3D();

        // ---------------------------------------------------------------------------------
        // 2D HUD / SURVIVAL INTERFACE (Optimized for Mobile & Desktop)
        // ---------------------------------------------------------------------------------
        // Crosshair in center
        int cx = screenWidth / 2;
        int cy = screenHeight / 2;
        DrawRectangle(cx - 5, cy - 1, 10, 2, Fade(WHITE, 0.8f));
        DrawRectangle(cx - 1, cy - 5, 2, 10, Fade(WHITE, 0.8f));

        // Top Status Header
        DrawRectangle(10, 10, 360, 80, Fade(BLACK, 0.65f));
        DrawText("LAST ISLAND OF SURVIVAL", 20, 16, 16, GOLD);
        DrawText("Native C++ / Raylib Engine (Android/PC Ready)", 20, 36, 12, RAYWHITE);
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

        EndDrawing();
    }

    // -------------------------------------------------------------------------------------
    // Cleanup Resources
    // -------------------------------------------------------------------------------------
    UnloadModel(modelTorso);   UnloadTexture(texTorso);
    UnloadModel(modelPants);   UnloadTexture(texPants);
    UnloadModel(modelHead);    UnloadTexture(texHead);
    UnloadModel(modelHair);    UnloadTexture(texHair);
    UnloadModel(modelShoes);   UnloadTexture(texShoes);
    UnloadModel(modelAxe);     UnloadTexture(texAxe);
    UnloadModel(modelRifle);   UnloadTexture(texRifle);
    UnloadModel(modelHouse);   UnloadTexture(texHouse);
    UnloadModel(modelTree);    UnloadTexture(texTree);
    UnloadModel(modelSulfur);  UnloadTexture(texSulfur);
    UnloadModel(modelIron);    UnloadTexture(texIron);
    UnloadModel(modelStone);   UnloadTexture(texStone);

    CloseWindow();
    return 0;
}
