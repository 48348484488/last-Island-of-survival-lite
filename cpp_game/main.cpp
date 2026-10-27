#include "raylib.h"
#include "raymath.h"
#include <vector>
#include <string>
#include <cmath>

struct TreeInstance {
    Vector3 position;
    float scale;
    float rotation;
};

int main() {
    const int screenWidth = 1280;
    const int screenHeight = 720;

    SetConfigFlags(FLAG_MSAA_4X_HINT | FLAG_WINDOW_RESIZABLE);
    InitWindow(screenWidth, screenHeight, "Last Day Rules: Survival - C++ Engine");
    SetTargetFPS(60);

    // Disable cursor for FPS look
    DisableCursor();

    // Setup Camera
    Camera3D camera = { 0 };
    camera.position = (Vector3){ 0.0f, 2.0f, 5.0f };
    camera.target = (Vector3){ 0.0f, 1.8f, 0.0f };
    camera.up = (Vector3){ 0.0f, 1.0f, 0.0f };
    camera.fovy = 65.0f;
    camera.projection = CAMERA_PERSPECTIVE;

    // Player state
    Vector3 playerPos = { 0.0f, 0.0f, 0.0f };
    float playerSpeed = 8.0f;
    float playerYaw = 0.0f;
    float playerPitch = 0.0f;
    float verticalVelocity = 0.0f;
    bool isGrounded = true;
    bool thirdPerson = true;

    // Load Game Models
    Model characterModel = LoadModel("assets/man_clothes_chushijuese_01.obj");
    Texture2D characterTex = LoadTexture("assets/man_clothes_chushijuese_01_ba.png");
    if (characterModel.meshCount > 0 && characterTex.id > 0) {
        SetMaterialTexture(&characterModel.materials[0], MATERIAL_MAP_DIFFUSE, characterTex);
    }

    Model houseModel = LoadModel("assets/Building_baopo_tiefang.obj");
    Texture2D houseTex = LoadTexture("assets/Building_baopo_tiefang_ba.png");
    if (houseModel.meshCount > 0 && houseTex.id > 0) {
        SetMaterialTexture(&houseModel.materials[0], MATERIAL_MAP_DIFFUSE, houseTex);
    }

    Model treeModel = LoadModel("assets/tree_baihuashu_03.obj");
    Texture2D treeTex = LoadTexture("assets/tree_baihuashu_03_d.png");
    if (treeModel.meshCount > 0 && treeTex.id > 0) {
        SetMaterialTexture(&treeModel.materials[0], MATERIAL_MAP_DIFFUSE, treeTex);
    }

    // Place trees around
    std::vector<TreeInstance> trees = {
        {{ -15.0f, 0.0f, -10.0f }, 1.2f, 45.0f },
        {{ -25.0f, 0.0f, 15.0f }, 1.0f, 90.0f },
        {{ 20.0f, 0.0f, -20.0f }, 1.4f, 120.0f },
        {{ 18.0f, 0.0f, 25.0f }, 1.1f, 30.0f },
        {{ -30.0f, 0.0f, -25.0f }, 1.3f, 75.0f },
        {{ 35.0f, 0.0f, 10.0f }, 1.2f, 210.0f }
    };

    bool cursorLocked = true;

    while (!WindowShouldClose()) {
        float dt = GetFrameTime();
        if (dt > 0.1f) dt = 0.1f; // Clamp delta time

        // Cursor toggle with ESC
        if (IsKeyPressed(KEY_ESCAPE)) {
            cursorLocked = !cursorLocked;
            if (cursorLocked) DisableCursor();
            else EnableCursor();
        }

        // Toggle 1st / 3rd person with TAB or V
        if (IsKeyPressed(KEY_TAB) || IsKeyPressed(KEY_V)) {
            thirdPerson = !thirdPerson;
        }

        if (cursorLocked) {
            // Mouse look
            Vector2 mouseDelta = GetMouseDelta();
            const float mouseSensitivity = 0.003f;
            playerYaw -= mouseDelta.x * mouseSensitivity;
            playerPitch -= mouseDelta.y * mouseSensitivity;

            // Clamp pitch
            if (playerPitch > 1.4f) playerPitch = 1.4f;
            if (playerPitch < -1.4f) playerPitch = -1.4f;

            // Direction vectors based on yaw
            Vector3 forward = { sinf(playerYaw), 0.0f, cosf(playerYaw) };
            Vector3 right = { cosf(playerYaw), 0.0f, -sinf(playerYaw) };

            // Movement Input
            Vector3 moveDir = { 0.0f, 0.0f, 0.0f };
            if (IsKeyDown(KEY_W)) moveDir = Vector3Add(moveDir, forward);
            if (IsKeyDown(KEY_S)) moveDir = Vector3Subtract(moveDir, forward);
            if (IsKeyDown(KEY_D)) moveDir = Vector3Subtract(moveDir, right);
            if (IsKeyDown(KEY_A)) moveDir = Vector3Add(moveDir, right);

            // Sprint
            float currentSpeed = playerSpeed;
            if (IsKeyDown(KEY_LEFT_SHIFT)) currentSpeed *= 1.8f;

            if (Vector3Length(moveDir) > 0.01f) {
                moveDir = Vector3Normalize(moveDir);
                playerPos = Vector3Add(playerPos, Vector3Scale(moveDir, currentSpeed * dt));
            }

            // Jump and Gravity
            const float gravity = 25.0f;
            if (isGrounded) {
                if (IsKeyPressed(KEY_SPACE)) {
                    verticalVelocity = 9.0f;
                    isGrounded = false;
                }
            } else {
                verticalVelocity -= gravity * dt;
                playerPos.y += verticalVelocity * dt;
                if (playerPos.y <= 0.0f) {
                    playerPos.y = 0.0f;
                    verticalVelocity = 0.0f;
                    isGrounded = true;
                }
            }
        }

        // Update Camera Position based on mode
        Vector3 lookDir = {
            sinf(playerYaw) * cosf(playerPitch),
            sinf(playerPitch),
            cosf(playerYaw) * cosf(playerPitch)
        };

        if (thirdPerson) {
            float camDistance = 4.5f;
            Vector3 camOffset = Vector3Scale(lookDir, -camDistance);
            camera.position = (Vector3){
                playerPos.x + camOffset.x,
                playerPos.y + 1.8f + camOffset.y,
                playerPos.z + camOffset.z
            };
            camera.target = (Vector3){ playerPos.x, playerPos.y + 1.6f, playerPos.z };
        } else {
            // First Person
            camera.position = (Vector3){ playerPos.x, playerPos.y + 1.7f, playerPos.z };
            camera.target = Vector3Add(camera.position, lookDir);
        }

        // DRAWING
        BeginDrawing();
        ClearBackground((Color){ 135, 206, 235, 255 }); // Sky blue

        BeginMode3D(camera);

            // Ground plane (Island Terrain grass)
            DrawPlane((Vector3){ 0.0f, 0.0f, 0.0f }, (Vector2){ 400.0f, 400.0f }, (Color){ 76, 120, 60, 255 });
            DrawGrid(40, 5.0f);

            // Draw Base Building (Metal House from the game)
            if (houseModel.meshCount > 0) {
                DrawModelEx(houseModel, (Vector3){ 10.0f, 0.0f, -15.0f }, (Vector3){ 0, 1, 0 }, 0.0f, (Vector3){ 1.0f, 1.0f, 1.0f }, WHITE);
            }

            // Draw Trees from the game
            if (treeModel.meshCount > 0) {
                for (const auto& t : trees) {
                    DrawModelEx(treeModel, t.position, (Vector3){ 0, 1, 0 }, t.rotation, (Vector3){ t.scale, t.scale, t.scale }, WHITE);
                }
            }

            // Draw Character (in 3rd person)
            if (characterModel.meshCount > 0) {
                if (thirdPerson) {
                    float charAngleDeg = (playerYaw * RAD2DEG) + 180.0f;
                    DrawModelEx(characterModel, playerPos, (Vector3){ 0, 1, 0 }, charAngleDeg, (Vector3){ 1.0f, 1.0f, 1.0f }, WHITE);
                }
            }

        EndMode3D();

        // 2D HUD / UI
        DrawRectangle(10, 10, 340, 110, Fade(BLACK, 0.6f));
        DrawText("LAST DAY RULES: SURVIVAL", 20, 18, 18, GOLD);
        DrawText("Engine: C++ / Raylib (Ultra-Leve)", 20, 42, 14, RAYWHITE);
        DrawText(TextFormat("FPS: %i  |  Pos: (%.1f, %.1f, %.1f)", GetFPS(), playerPos.x, playerPos.y, playerPos.z), 20, 64, 14, GREEN);
        DrawText(TextFormat("Visao: %s (Pressione TAB)", thirdPerson ? "3a Pessoa" : "1a Pessoa"), 20, 86, 14, YELLOW);

        // Survival Health / Stamina Bar (Bottom Left)
        int barY = screenHeight - 60;
        DrawRectangle(20, barY, 200, 18, Fade(DARKGRAY, 0.8f));
        DrawRectangle(20, barY, 180, 18, RED); // Health
        DrawText("VIDA: 100/100", 25, barY + 2, 12, WHITE);

        DrawRectangle(20, barY + 24, 200, 14, Fade(DARKGRAY, 0.8f));
        DrawRectangle(20, barY + 24, 150, 14, ORANGE); // Stamina
        DrawText("STAMINA", 25, barY + 25, 10, WHITE);

        // Controls reminder (Bottom Right)
        int infoW = 280;
        DrawRectangle(screenWidth - infoW - 10, screenHeight - 90, infoW, 80, Fade(BLACK, 0.6f));
        DrawText("CONTROLES:", screenWidth - infoW, screenHeight - 82, 12, GOLD);
        DrawText("WASD: Andar | SHIFT: Correr", screenWidth - infoW, screenHeight - 65, 11, RAYWHITE);
        DrawText("ESPACO: Pular | MOUSE: Olhar", screenWidth - infoW, screenHeight - 50, 11, RAYWHITE);
        DrawText("TAB: Alternar Camera | ESC: Liberar Mouse", screenWidth - infoW, screenHeight - 35, 11, RAYWHITE);

        EndDrawing();
    }

    // Unload resources
    UnloadTexture(characterTex);
    UnloadModel(characterModel);
    UnloadTexture(houseTex);
    UnloadModel(houseModel);
    UnloadTexture(treeTex);
    UnloadModel(treeModel);

    CloseWindow();
    return 0;
}
