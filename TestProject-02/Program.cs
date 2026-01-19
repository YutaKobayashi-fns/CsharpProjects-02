// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System;
using System.IO.Pipelines;
using System.Net.NetworkInformation;

Random random = new Random();

Console.WriteLine("Would you like to play? (Y/N)");
if (ShouldPlay())
{
    PlayGame();
}

bool ShouldPlay()
{
    string? result;

    result = Console.ReadLine();

    if (result != null)
    {
        return (result.Trim().ToLower() == "y") ? true : false;
    }
    else
    {
        return false;
    }
}

void PlayGame()
{
    var play = true;

    while (play)
    {
        var target = RandomTarget();
        var roll = RandomRoll();

        Console.WriteLine($"Roll a number greater than {target} to win!");
        Console.WriteLine($"You rolled a {roll}");
        Console.WriteLine(WinOrLose(target, roll));
        Console.WriteLine("\nPlay again? (Y/N)");

        play = ShouldPlay();
    }
}

int RandomTarget()
{
    return random.Next(1, 6);
}

int RandomRoll()
{
    return random.Next(1, 7);
}

string WinOrLose(int target, int roll)
{
    string result;

    if (roll >= target)
    {
        result = "You Win";
    }
    else
    {
        result = "You Lose";
    }

    return result;
}