using System;
using System.Data;

Random random = new Random();
Console.CursorVisible = false;
int height = Console.WindowHeight - 1;
int width = Console.WindowWidth - 5;
bool shouldExit = false;

// Console position of the player
int playerX = 0;
int playerY = 0;

// Console position of the food
int foodX = 0;
int foodY = 0;

// Available player and food strings
string[] states = { "('-')", "(^-^)", "(X_X)" };
string[] foods = { "@@@@@", "$$$$$", "#####" };

// Current player string displayed in the Console
string player = states[0];

// Index of the current food
int food = 0;

// options(true = ON)
bool bootOption = true;
bool reinforceOption = false;

// Food eating check
int[] foodEatsIndexs = { 0, 0, 0, 0, 0 };
bool eatingCheck = false;

// Current player state check
const int STATE_ID_NORMAL = 0;
const int STATE_ID_GOOD = 1;
const int STATE_ID_BAD = 2;
// bool normalStateCheck = false;
// bool goodStateCheck = false;
bool badStateCheck = false;

// Current player reinforce
const int PLAYER_MOVEMENT_GOOD = 3;

InitializeGame();
while (!shouldExit)
{
    shouldExit = TerminalResized();
    if (shouldExit != true)
    {
        if (badStateCheck == false)
        {
            Move();
            eatingCheck = CheckEatingState();
            // プレイヤーは食べ物を食べたか？
            if (eatingCheck == true)
            {
                ChangePlayer();
                badStateCheck = CheckPlayerState(STATE_ID_BAD);
                // ステータスは不良か？
                if (badStateCheck == true)
                {
                    FreezePlayer();
                }
            }

            // 画面上の食べ物をすべて食べたか？
            if (foodEatsIndexs.Sum() == foodEatsIndexs.Length)
            {
                ShowFood();
                foodEatsIndexs = new int[] { 0, 0, 0, 0, 0 };
                eatingCheck = false;
            }
        }
        else
        {
            badStateCheck = CheckPlayerState(STATE_ID_BAD);
        }
    }
    else
    {
        Console.Clear();
        Console.WriteLine("Console was resized. Program exiting.");
        Console.WriteLine("Press Enter.");
        Console.ReadLine();

        break;
    }
}

// Returns true if the Terminal was resized 
bool TerminalResized()
{
    return height != Console.WindowHeight - 1 || width != Console.WindowWidth - 5;
}

// Displays random food at a random location
void ShowFood()
{
    // Update food to a random index
    food = random.Next(0, foods.Length);

    // Update food position to a random location
    foodX = random.Next(0, width - player.Length);
    foodY = random.Next(0, height - 1);

    // Display the food at the location
    Console.SetCursorPosition(foodX, foodY);
    Console.Write(foods[food]);
}

// Changes the player to match the food consumed
void ChangePlayer()
{
    player = states[food];
    Console.SetCursorPosition(playerX, playerY);
    Console.Write(player);
}

// Temporarily stops the player from moving
void FreezePlayer()
{
    System.Threading.Thread.Sleep(1000);
    player = states[0];
}

// Reads directional input from the Console and moves the player
void Move()
{
    int lastX = playerX;
    int lastY = playerY;
    bool goodStateCheck = false;

    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.UpArrow:
            playerY--;
            break;
        case ConsoleKey.DownArrow:
            playerY++;
            break;
        case ConsoleKey.LeftArrow:
            goodStateCheck = CheckPlayerState(STATE_ID_GOOD);
            // 移動速度の強化要か？
            if ((goodStateCheck == true) && (reinforceOption == true))
            {
                playerX -= PLAYER_MOVEMENT_GOOD;
            }
            else
            {
                playerX--;
            }

            break;
        case ConsoleKey.RightArrow:
            goodStateCheck = CheckPlayerState(STATE_ID_GOOD);
            // 移動速度の強化要か？
            if ((goodStateCheck == true) && (reinforceOption == true))
            {
                playerX += PLAYER_MOVEMENT_GOOD;
            }
            else
            {
                playerX++;
            }
            break;
        default:
            if (bootOption == false)
            {
                break;
            }
            else
            {
                shouldExit = true;

                Console.Clear();
                Console.WriteLine("Another key was entered. Program exiting.");
                Console.WriteLine("Press Enter.");
                Console.ReadLine();

                return;
            }
    }

    // Clear the characters at the previous position
    Console.SetCursorPosition(lastX, lastY);
    for (int i = 0; i < player.Length; i++)
    {
        Console.Write(" ");
    }

    // Keep player position within the bounds of the Terminal window
    playerX = (playerX < 0) ? 0 : (playerX >= width ? width : playerX);
    playerY = (playerY < 0) ? 0 : (playerY >= height ? height : playerY);

    // Draw the player at the new location
    Console.SetCursorPosition(playerX, playerY);
    Console.Write(player);
}

// Clears the console, displays the food and player
void InitializeGame()
{
    Console.Clear();
    ShowFood();
    Console.SetCursorPosition(0, 0);
    Console.Write(player);
}

// Check player eating state
bool CheckEatingState()
{
    int eatRangeX = 0;
    int eatRangeY = 0;
    int[] setIdx = { 0, 0, 0, 0, 0 };

    bool result = false;

    eatRangeX = playerX - foodX;
    eatRangeY = playerY - foodY;

    // 食べ物が表示されているY座標か？
    if (eatRangeY == 0)
    {
        // 食べ物が表示されているX座標範囲内か？
        switch (eatRangeX)
        {
            case -4:
                setIdx = new int[] { 1, 0, 0, 0, 0 };
                break;
            case -3:
                setIdx = new int[] { 1, 1, 0, 0, 0 };
                break;
            case -2:
                setIdx = new int[] { 1, 1, 1, 0, 0 };
                break;
            case -1:
                setIdx = new int[] { 1, 1, 1, 1, 0 };
                break;
            case 0:
                setIdx = new int[] { 1, 1, 1, 1, 1 };
                break;
            case 1:
                setIdx = new int[] { 0, 1, 1, 1, 1 };
                break;
            case 2:
                setIdx = new int[] { 0, 0, 1, 1, 1 };
                break;
            case 3:
                setIdx = new int[] { 0, 0, 0, 1, 1 };
                break;
            case 4:
                setIdx = new int[] { 0, 0, 0, 0, 1 };
                break;
            default:
                setIdx = new int[] { 0, 0, 0, 0, 0 };
                break;
        }

        for (int i = 0; i < setIdx.Length; i++)
        {
            // 食事状態設定の判定
            if (setIdx[i] == 1)
            {
                if (foodEatsIndexs[i] != 1)
                {
                    foodEatsIndexs[i] = setIdx[i];
                    result = true;
                }
            }
        }
    }

    return result;
}

// Player state check(Normal, Good, bad)
bool CheckPlayerState(int stateId)
{
    // stateId = 0:Noraml 1:Good 2:Bad
    // default case return False.
    bool result = false;

    switch (stateId)
    {
        case STATE_ID_NORMAL:   // 通常
            if (player == states[STATE_ID_NORMAL])
            {
                result = true;
            }
            break;

        case STATE_ID_GOOD:     // 良好
            if (player == states[STATE_ID_GOOD])
            {
                result = true;
            }
            break;

        case STATE_ID_BAD:      // 不良
            if (player == states[STATE_ID_BAD])
            {
                result = true;
            }
            break;
    }

    return result;
}