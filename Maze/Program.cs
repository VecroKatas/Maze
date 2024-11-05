using System.Globalization;

int width = 10, height = 12;
int blockFrequency = 28;
Random random = new Random();
Dog dog = new Dog(random.Next(0, width), random.Next(0, height));
Vector2 deltaInput = new Vector2();
Vector2 finish = new Vector2(random.Next(0, width), random.Next(0, height));

bool isGameEnded = false;

char[,] maze = new char[height,width];

Main();

void Generate()
{
    GenerateMaze();
}

void GenerateMaze()
{
    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            maze[i, j] = random.Next(0, 100) < blockFrequency ? '#' : '.';
            if ((i, j) == (finish.x, finish.y)) maze[i, j] = '☻';
        }
    }
}

void DisplayMaze()
{
    Console.WriteLine();
    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            if (i == dog.y && j == dog.x)
                Console.Write(dog.symbol);
            else
                Console.Write(maze[i,j]);
        }
        Console.WriteLine();
    }
}

void GetInput()
{
    ConsoleKeyInfo input = Console.ReadKey();

    switch (input.Key)
    {
        case ConsoleKey.W or ConsoleKey.UpArrow:
        {
            (deltaInput.x, deltaInput.y) = (0, -1);
            break;
        }
        case ConsoleKey.S or ConsoleKey.DownArrow:
        {
            (deltaInput.x, deltaInput.y) = (0, 1);
            break;
        }
        case ConsoleKey.A or ConsoleKey.LeftArrow: {
            (deltaInput.x, deltaInput.y) = (-1, 0);
            break;
        }
        case ConsoleKey.D or ConsoleKey.RightArrow: {
            (deltaInput.x, deltaInput.y) = (1, 0);
            break;
        }
            default: break;
    }
}

void Logic()
{
    int newX = dog.x + deltaInput.x;
    int newY = dog.y + deltaInput.y;
    if (IsWithinBoundaries(newX, newY) && IsTargetWalkable(newX, newY))
    {
        dog.Move(newX, newY);
        IsTargetFinish(newX, newY);
    }
}

bool IsWithinBoundaries(int newX, int newY)
{
    if (newX >= width || newX < 0 || newY >= height || newY < 0)
        return false;
    return true;
}

bool IsTargetWalkable(int newX, int newY)
{
    if (maze[newY, newX] == '#')
        return false;
    return true;
}

void IsTargetFinish(int newX, int newY)
{
    if (maze[newY, newX] == '☻')
        isGameEnded = true;
}

void Draw()
{
    DisplayMaze();
}

void Main()
{
    Generate();
    Draw();

    while (!isGameEnded)
    {
        GetInput();
        Logic();
        Draw();
    }
    Console.WriteLine("Yay you won champion");
}

class Dog
{
    public char symbol = '@';
    public int x;
    public int y;

    public Dog(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public void Move(int newX, int newY)
    {
        x = newX;
        y = newY;
    }
}

struct Vector2
{
    public int x;
    public int y;
    
    public Vector2(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public void Set(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}