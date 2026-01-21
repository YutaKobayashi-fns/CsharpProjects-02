/*
This code instantiates a value and then calls the ChangeValue method
to update the value. The code then prints the updated value to the console.
*/
int x = 5;

x = ChangeValue();

Console.WriteLine(x);

int ChangeValue()
{
    return 10;
}