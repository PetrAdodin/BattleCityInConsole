using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

// Класс Game управляет всей логикой игры, включая танки, стены, пули и взаимодействие между ними.
public class Game
{
    [JsonInclude]
    private List<Wall> walls = new List<Wall>(); //список стен на уровне
    [JsonInclude]
    private List<Tank> enemy_tanks = new List<Tank> { new Tank(0, 0), new Tank(5, 0), new Tank(12, 0) }; //список врагов на уровне
    [JsonInclude]
    private Tank playerTank; //танк игрока
    [JsonInclude]
    private string filename = "SaveProgress.json"; //имя файла для сохранения прогресса
    private char[,] _field = new char[13, 13];
    private int _consoleWidth = 13;
    private int _consoleHeight = 13;




    //свойство для доступа к танку игрока
    public Tank PlayerTank => playerTank;

    //свойство для доступа к вражеским танкам
    public List<Tank> EnemyTanks => enemy_tanks;

    //свойство для доступа к стенам
    public List<Wall> Walls => walls;

    // Инициализация стен и танков
    public Game()
    {
        playerTank = new Tank(5, 6);
        walls = new List<Wall> { new Wall(3, 0), new Wall(7, 0),
            new Wall(1, 1), new Wall(3, 1), new Wall(7, 1), new Wall(9, 1), new Wall(12, 1),
            new Wall(1, 2), new Wall(6, 2), new Wall(7, 2), new Wall(9, 2), new Wall(10, 2), new Wall(11, 2),
            new Wall(3, 3), new Wall(9, 3),
            new Wall(3, 4), new Wall(6, 4), new Wall(9, 4), new Wall(11, 4), new Wall(12, 4),
            new Wall(5, 5), new Wall(8, 5),
            new Wall(1, 6), new Wall(2, 6), new Wall(3, 6), new Wall(7, 6), new Wall(11, 6),
            new Wall(3, 7), new Wall(5, 7), new Wall(7, 7), new Wall(9, 7), new Wall(11, 7),
            new Wall(0, 8), new Wall(1, 8), new Wall(3, 8), new Wall(5, 8), new Wall(7, 8), new Wall(11, 8),
            new Wall(1, 9), new Wall(3, 9), new Wall(5, 9), new Wall(6, 9), new Wall(7, 9), new Wall(9, 9), new Wall(10, 9), new Wall(11, 9),
            new Wall(1, 10), new Wall(3, 10), new Wall(5, 10), new Wall(6, 10), new Wall(7, 10),
            new Wall(1, 11), new Wall(5, 11), new Wall(6, 11), new Wall(7, 11), new Wall(9, 11), new Wall(11, 11),
            new Wall(1, 12), new Wall(3, 12), new Wall(5, 12), new Wall(7, 12), new Wall(9, 12), new Wall(10, 12), new Wall(11, 12) };
    }

    //метод для отрисовки поля
    public void Show()
    {
        for (int j = 0; j < _consoleWidth; j++)
        {
            for (int i = 0; i < _consoleHeight; i++)
            {
                Console.Write(_field[i, j]);
            }
        }
    }

    //метод для очистки поля
    public void Clear_field()
    {
        for (int j = 0; j < _consoleWidth; j++)
        {
            for (int i = 0; i < _consoleHeight; i++)
            {
                _field[i, j] = ' ';
            }
        }
    }

    //обновление позиций танка игрока
    public void UpdateTank(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.W:
                PlayerMove('U');
                break;
            case ConsoleKey.S:
                PlayerMove('D');
                break;
            case ConsoleKey.A:
                PlayerMove('L');
                break;
            case ConsoleKey.D:
                PlayerMove('R');
                break;
            case ConsoleKey.Spacebar:
                if (!playerTank.IsDestroyed) //если танк игрока не уничтожен
                {
                    PlayerShoot(); //стреляем
                }
                break;
        }
        //перебор пуль вражеских танков
        foreach (var enemy_tank in enemy_tanks)
        {
            foreach (var bullet in enemy_tank.GetBullets)
                if (!bullet.IsDestroyed)
                    CheckCollisions(bullet, true);
        }
        foreach (var enemy_tank in enemy_tanks)
            enemy_tank.RemoveBullets(); //удаление уничтоженных пуль из списка пуль врага

        //перебор пуль танка игрока
        foreach (var bullet in playerTank.GetBullets)
            CheckCollisions(bullet, false);
        playerTank.RemoveBullets(); //удаление уничтоженных пуль из списка пуль игрока
    }

    //обновление позиций врагов
    public void UpdateEnemy()
    {
        EnemyMove(); //перемещение всех врагов на 1 клетку в случайном напрвлении
        Random rand = new Random();
        var aliveEnemyTanks = enemy_tanks.FindAll(t => !t.IsDestroyed); //фильтруем все живые вражесские танки
        if (aliveEnemyTanks.Count > 0)
        {
            var enemy_tank_shooting = aliveEnemyTanks[rand.Next(aliveEnemyTanks.Count)];
            var enemy_bullet = enemy_tank_shooting.Shoot(); //случайный вражеский танк стреляет
            enemy_tank_shooting.AddBullet(enemy_bullet);
        }


        //перебор пуль вражеских танков
        foreach (var enemy_tank in enemy_tanks)
        {
            foreach (var bullet in enemy_tank.GetBullets)
                if (!bullet.IsDestroyed)
                    CheckCollisions(bullet, true); //проверка на столкновение вражеских пуль
        }
        foreach (var enemy_tank in enemy_tanks)
            enemy_tank.RemoveBullets();

        //перебор пуль танка игрока
        foreach (var bullet in playerTank.GetBullets)
            CheckCollisions(bullet, false); //проверка на столкновение пуль игрока
        playerTank.RemoveBullets();
    }

    //обновление позиций пуль
    public void UpdateBullets()
    {
        //перебор пуль вражеских танков
        foreach (var enemy_tank in enemy_tanks)
        {
            foreach (var bullet in enemy_tank.GetBullets)
            {
                if (!bullet.IsDestroyed)
                {
                    bullet.Move();
                    CheckCollisions(bullet, true);
                }
            }
        }
        foreach (var enemy_tank in enemy_tanks)
        {
            enemy_tank.RemoveBullets();
        }

        //перебор пуль танка игрока
        foreach (var bullet in playerTank.GetBullets)
        {
            bullet.Move();
            CheckCollisions(bullet, false);
        }
        playerTank.RemoveBullets();
    }

    //метод для перемещения танка игрока
    public void PlayerMove(char direction)
    {
        playerTank.Move(direction, walls, enemy_tanks);
    }

    //метод для стрельбы танка игрока
    public void PlayerShoot()
    {
        var bullet = playerTank.Shoot();
        playerTank.AddBullet(bullet);

    }

    //метод для перемещения вражеского танка
    public void EnemyMove()
    {
        char[] directions = { 'U', 'D', 'L', 'R' };
        Random rand = new Random();
        foreach (var enemy_tank in enemy_tanks)
        {
            enemy_tank.Move(directions[rand.Next(directions.Length)], walls, enemy_tanks);
        }
    }

    //метод для проверки столкновений пуль со стенами и границами экрана
    private void CheckCollisions(Bullet bullet, bool IsEnemy)
    {
        if (bullet.X < 0 || bullet.Y < 0 || bullet.X >= _consoleWidth || bullet.Y >= _consoleHeight)
        {
            bullet.IsDestroyed = true;
            return;
        }

        foreach (var wall in walls)
        {
            if (!wall.IsDestroyed && wall.X == bullet.X && wall.Y == bullet.Y)
            {
                bullet.IsDestroyed = true;
                wall.IsDestroyed = true;
                break;
            }
        }

        if (!IsEnemy)
        {
            foreach (var enemy_tank in enemy_tanks)
            {
                if (bullet.X == enemy_tank.X && bullet.Y == enemy_tank.Y)
                {
                    enemy_tank.IsDestroyed = true;
                    bullet.IsDestroyed = true;
                }
            }
        }

        if (IsEnemy)
        {
            if (bullet.X == playerTank.X && bullet.Y == playerTank.Y)
            {
                bullet.IsDestroyed = true;
                playerTank.IsDestroyed = true;
            }
        }
    }

    //Метод для обновления кадра
    public void Draw()
    {
        Console.Clear();
        Clear_field();

        //стены
        foreach (var wall in walls)
        {
            if (!wall.IsDestroyed)
            {
                _field[wall.X, wall.Y] = wall.Sybol;
            }
        }

        //танк игрока
        if (!playerTank.IsDestroyed)
        {
            _field[playerTank.X, playerTank.Y] = playerTank.Sybol;
        }

        //вражеские танки
        foreach (var enemy_tank in enemy_tanks)
        {
            if (!enemy_tank.IsDestroyed)
            {
                _field[enemy_tank.X, enemy_tank.Y] = '@';
            }
        }


        //вражеские пули
        foreach (var enemy_tank in enemy_tanks)
        {
            foreach (var enemy_bullet in enemy_tank.GetBullets)
            {
                if (!enemy_bullet.IsDestroyed)
                {
                    _field[enemy_bullet.X, enemy_bullet.Y] = '"';
                }
            }
        }

        //пули
        foreach (var bullet in playerTank.GetBullets)
        {
            if (!bullet.IsDestroyed)
            {
                _field[bullet.X, bullet.Y] = bullet.Sybol;
            }
        }
        Show();
    }

    //метод для проверки конца игры
    public int IsGameEnd()
    {
        if (playerTank.IsDestroyed)
        {
            Console.Clear();
            var new_game = new Game();
            new_game.SaveGame();
            Console.WriteLine("Defeat!");
            Console.ReadKey();
            return 0; //игрок проиграл
        }


        var count = 0;
        foreach (var enemy_tank in enemy_tanks)
        {
            if (enemy_tank.IsDestroyed)
                count += 1;
        }
        if (count == enemy_tanks.Count)
        {
            Console.Clear();
            var new_game = new Game();
            new_game.SaveGame();
            Console.WriteLine("Victory!");
            Console.ReadKey();
            return 1; //игрок выйграл
        }

        return 2; //никто не победил
    }

    //метод для сохранения игры в файл
    public void SaveGame()
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(this);
            File.WriteAllText(filename, jsonString); //создаем файл с именем <filename> и записываем все данные объекта jsonString
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении игры: {ex.Message}");
        }
    }

    //метод для загрузки игры из файла
    public Game LoadGame()
    {
        try
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("Сохранение не найдено, будет начата новая игра.");
                return new Game();
            }

            string jsonString = File.ReadAllText(filename);
            return JsonSerializer.Deserialize<Game>(jsonString) ?? new Game();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке игры: {ex.Message}");
            return new Game();
        }
    }

    //главный игровой цикл
    public void Run()
    {
        int tankDelay = 200;    //задержка кадра (перемещение танка) для оптимизации игры на всех пк
        int bulletDelay = 200; //задержка кадра (перемещение пули) для оптимизации игры на всех пк
        int enemyDelay = 1500;

        DateTime lastTankMove = DateTime.Now;
        DateTime lastBulletMove = DateTime.Now;
        DateTime lastEnemyMove = DateTime.Now;

        var temp = true;
        while (temp)
        {

            if ((DateTime.Now - lastTankMove).TotalMilliseconds >= tankDelay)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    UpdateTank(key);
                    lastTankMove = DateTime.Now;

                    if (key == ConsoleKey.Escape)
                    {
                        this.SaveGame();
                        temp = false;
                    }
                }
            }

            if ((DateTime.Now - lastBulletMove).TotalMilliseconds >= bulletDelay)
            {
                UpdateBullets();
                lastBulletMove = DateTime.Now;
            }

            if ((DateTime.Now - lastEnemyMove).TotalMilliseconds >= enemyDelay)
            {
                UpdateEnemy();
                lastEnemyMove = DateTime.Now;
            }
            Draw();
            Thread.Sleep(63);

            switch (IsGameEnd())
            {
                case 0:
                    temp = false;
                    break;
                case 1:
                    temp = false;
                    break;
                case 2:
                    break;
            }
        }
        Console.Clear();
        Console.WriteLine("Esc - leave\nN - new game\nE - continue");
    }
}