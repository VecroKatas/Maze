int width = 10, height = 12;
int blockFrequency = 33;
int radiationFrequency = 70;
int doubleRadiationFrequency = 10;
Random random = new Random();
int radiationPoints = random.Next(15, 40);

bool isGameWon = false;
bool isGameLost = false;
bool isJetPackEquipped = false;
bool isBombEquipped = false;

char[,] maze = new char[height,width];

Dog dog = new Dog(0, 0);
Vector2 deltaInput;
Vector2 finish;
Vector2 jetPack;
Vector2 bomb;

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
            maze[i, j] = random.Next(0, 100) < blockFrequency ? '#' : ' ';
            if (maze[i, j] == ' ' && random.Next(0, 100) < radiationFrequency)
            {
                maze[i, j] = random.Next(0, 100) < doubleRadiationFrequency ? '_' : '.';
            }
        }
    }

    var dogCoords = GeneratePlacement();
    dog.x = dogCoords.x;
    dog.y = dogCoords.y;
    finish = GeneratePlacement();
    jetPack = GeneratePlacement();
    bomb = GeneratePlacement();
    
    maze[finish.y, finish.x] = 'F';
    maze[jetPack.y, jetPack.x] = 'J';
    maze[bomb.y, bomb.x] = '☺';
}

Vector2 GeneratePlacement()
{
    int x, y;
    do
    {
        x = random.Next(0, width);
        y = random.Next(0, height);
    } while (!IsTileEmpty(x, y));

    return new Vector2(x, y);
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

    (deltaInput.x, deltaInput.y) = input.Key switch
    {
        ConsoleKey.W or ConsoleKey.UpArrow => (0, -1),
        ConsoleKey.S or ConsoleKey.DownArrow => (0, 1),
        ConsoleKey.A or ConsoleKey.LeftArrow => (-1, 0),
        ConsoleKey.D or ConsoleKey.RightArrow => (1, 0),
        _ => (0, 0)
    };
}

void Logic()
{
    int newX = dog.x + deltaInput.x;
    int newY = dog.y + deltaInput.y;
    (newX, newY) = CheckBoundaries(newX, newY);

    if (CanFlyToTile(newX, newY))
    {
        Vector2 jetPackCoords = DetermineLandingTile(newX, newY);
        if (jetPackCoords.x == -1)
        {
            Notify("You cannot JetPack out of bounds(");
        }
        else
        {
            newX = jetPackCoords.x;
            newY = jetPackCoords.y;
            
            MoveDog(newX, newY);
            isJetPackEquipped = false;
        }
    }
    else if (CanGoToTile(newX, newY))
    {
        MoveDog(newX, newY);
    }
    else if (isBombEquipped)
    {
        DestroyWall(newX, newY);
    }
    else
    {
        DecreaseRadiation(dog.x, dog.y);
        return;
    }
    DecreaseRadiation(newX, newY);
}

void MoveDog(int newX, int newY)
{
    dog.Move(newX, newY);
    IsTileFinish(newX, newY);
    IsTileBomb(newX, newY);
    IsTileJetPack(newX, newY);
}

void DecreaseRadiation(int newX, int newY)
{
    radiationPoints -= maze[newY, newX] switch
    {
        '.' => 1,
        '_' => 2,
        _ => 0
    };
    if (radiationPoints <= 0)
    {
        isGameLost = true;
    }
}

void DestroyWall(int newX, int newY)
{
    maze[newY, newX] = ' ';
    isBombEquipped = false;
}

bool IsTileEmpty(int x, int y)
{
    return maze[y, x] == ' ' || maze[y, x] == '.' || maze[y, x] == '_';
}

bool CanGoToTile(int newX, int newY)
{
    return IsTileWalkable(newX, newY);
}

bool CanFlyToTile(int newX, int newY)
{
    return !IsTileWalkable(newX, newY) && isJetPackEquipped;
}

(int x, int y) CheckBoundaries(int newX, int newY)
{
    if (newX >= width)
        return (0, newY);
    
    if (newX < 0)
        return (width-1, newY);
    
    if (newY >= height)
        return (newX, 0);
    
    if (newY < 0)
        return (newX, height-1);
    
    return (newX, newY);
}

bool IsTileWalkable(int newX, int newY)
{
    if (maze[newY, newX] == '#')
        return false;
    return true;
}

bool IsGameEnded()
{
    return isGameWon || isGameLost;
}

void IsTileFinish(int newX, int newY)
{
    if (maze[newY, newX] == 'F')
        isGameWon = true;
}

void IsTileJetPack(int newX, int newY)
{
    if (maze[newY, newX] == 'J')
    {
        isJetPackEquipped = true;
        maze[newY, newX] = ' ';
    }
}

void IsTileBomb(int newX, int newY)
{
    if (maze[newY, newX] == '☺')
    {
        isBombEquipped = true;
        maze[newY, newX] = ' ';
    }
}

Vector2 DetermineLandingTile(int newX, int newY)
{
    do
    {
        newX += deltaInput.x;
        newY += deltaInput.y;
        (newX, newY) = CheckBoundaries(newX, newY);
    } while (maze[newY, newX] == '#');

    return new Vector2(newX, newY);
}

void Notify(string message)
{
    Console.WriteLine();
    Console.WriteLine(message);
}

void DetermineOutcome()
{
    if (isGameWon)
    {
        Notify("Yay you won champion");
        return;
    }
    if (isGameLost)
    {
        Notify("You died of cancer sorry");
        return;
    }
    Notify("Hello mister frog. You are not supposed to be here");
}

void Draw()
{
    Notify($"You have {radiationPoints} radiation resistance left");

    Notify(isJetPackEquipped ? "You have a JetPack!" : "You don't have a JetPack");
    Notify(isBombEquipped ? "You have a bomb :333333" : "You don't have a bomb((((");

    DisplayMaze();
}

void Main()
{
    Generate();
    Draw();

    while (!IsGameEnded())
    {
        GetInput();
        Logic();
        Draw();
    }
    DetermineOutcome();
}




class Dog
{
    public char symbol = '☻';
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
}