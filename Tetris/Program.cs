using System;
using System.Threading;

namespace MyTetris
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            TetrisGame game = new TetrisGame();
            game.Run();
        }
    }

    // ДОДАНИЙ МОДУЛЬ 1: Відповідає тільки за відмальовування екрану
    class Renderer
    {
        public void Draw(int[,] playField, int[,] currentShape, int currentX, int currentY, int score, int width, int height)
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < height; y++)
            {
                Console.Write("|");
                for (int x = 0; x < width; x++)
                {
                    if (IsShapePoint(x, y, currentShape, currentX, currentY) || playField[x, y] == 1)
                        Console.Write("[]");
                    else
                        Console.Write(" .");
                }
                Console.WriteLine("|");
            }
            Console.WriteLine($"Score: {score}");
        }

        // Винесена логіка для зменшення цикломатичної складності методу Draw
        private bool IsShapePoint(int x, int y, int[,] shape, int shapeX, int shapeY)
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1 && shapeX + i == x && shapeY + j == y) return true;
                }
            }
            return false;
        }
    }

    // ДОДАНИЙ МОДУЛЬ 2: Відповідає за перевірку зіткнень
    class CollisionDetector
    {
        public bool Check(int nextX, int nextY, int[,] shape, int[,] playField, int width, int height)
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 0) continue;
                    int x = nextX + i;
                    int y = nextY + j;
                    if (x < 0 || x >= width || y >= height || (y >= 0 && playField[x, y] == 1)) return true;
                }
            }
            return false;
        }
    }

    // Головний клас став набагато меншим і чистішим
    class TetrisGame
    {
        private const int Width = 10;
        private const int Height = 20;
        private int[,] playField = new int[Width, Height];
        private int currentX = 4, currentY = 0, score = 0;
        private int[,] currentShape;

        // Зменшення зчеплення: делегування задач новим модулям
        private Renderer renderer = new Renderer();
        private CollisionDetector collider = new CollisionDetector();
        private Random random = new Random();

        private readonly int[][,] shapes = new int[][,] {
            new int[,] { { 1, 1, 1, 1 } },
            new int[,] { { 1, 1 }, { 1, 1 } },
            new int[,] { { 0, 1, 0 }, { 1, 1, 1 } },
            new int[,] { { 1, 0, 0 }, { 1, 1, 1 } }
        };

        public TetrisGame() { SpawnShape(); }

        public void Run()
        {
            while (true)
            {
                Input();
                Logic();
                renderer.Draw(playField, currentShape, currentX, currentY, score, Width, Height);
                Thread.Sleep(50);
            }
        }

        private void SpawnShape()
        {
            currentShape = shapes[random.Next(shapes.Length)];
            currentX = Width / 2 - currentShape.GetLength(0) / 2;
            currentY = 0;
            if (collider.Check(currentX, currentY, currentShape, playField, Width, Height)) Environment.Exit(0);
        }

        private void Input()
        {
            if (!Console.KeyAvailable) return;
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.LeftArrow && !collider.Check(currentX - 1, currentY, currentShape, playField, Width, Height)) currentX--;
            if (key == ConsoleKey.RightArrow && !collider.Check(currentX + 1, currentY, currentShape, playField, Width, Height)) currentX++;
            if (key == ConsoleKey.DownArrow && !collider.Check(currentX, currentY + 1, currentShape, playField, Width, Height)) currentY++;
        }

        private void Logic()
        {
            if (DateTime.Now.Millisecond % 10 != 0) return;
            if (!collider.Check(currentX, currentY + 1, currentShape, playField, Width, Height)) currentY++;
            else { MergeShape(); ClearLines(); SpawnShape(); }
        }

        private void MergeShape()
        {
            for (int i = 0; i < currentShape.GetLength(0); i++)
                for (int j = 0; j < currentShape.GetLength(1); j++)
                    if (currentShape[i, j] == 1 && currentY + j >= 0) playField[currentX + i, currentY + j] = 1;
        }

        private void ClearLines()
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                bool full = true;
                for (int x = 0; x < Width; x++) if (playField[x, y] == 0) full = false;
                if (full) { score += 100; MoveLinesDown(y); y++; }
            }
        }

        // Метод скорочено шляхом винесення логіки зсуву
        private void MoveLinesDown(int fromY)
        {
            for (int y = fromY; y > 0; y--)
                for (int x = 0; x < Width; x++) playField[x, y] = playField[x, y - 1];
        }
    }
}