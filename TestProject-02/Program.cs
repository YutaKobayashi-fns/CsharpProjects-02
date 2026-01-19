// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System;
using System.Net.NetworkInformation;

string[,] corporate =
{
    {"Robert", "Bavin"}, {"Simon", "Bright"},
    {"Kim", "Sinclair"}, {"Aashrita", "Kamath"},
    {"Sarah", "Delucchi"}, {"Sinan", "Ali"}
};

string[,] external =
{
    {"Vinnie", "Ashton"}, {"Cody", "Dysart"},
    {"Shay", "Lawrence"}, {"Daren", "Valdes"}
};

string externalDomain = "hayworth.com";

DisplayEmailAddresses(corporate);
DisplayEmailAddresses(external, externalDomain);

void DisplayEmailAddresses(string[,] name, string domain = "contoso.com")
{
    for (int i = 0; i < name.GetLength(0); i++)
    {
        Console.WriteLine($"{name[i,0].Substring(0,2).ToLower()}{name[i,1].ToLower()}@{domain}");
    }
}