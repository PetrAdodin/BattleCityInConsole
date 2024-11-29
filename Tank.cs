public class Tank
{
    private int _x; // Координата X танка
    private int _y; // Координата Y танка
    private char _direction; // Направление движения танка
    private bool _isDestroyed; // Флаг: true, если танк уничтожен
    private List<Bullet> _bullets = new List<Bullet>(); // Список пуль, выпущенных этим танком
    private int _consoleWidth = 13;
    private int _consoleHeight = 13;

    public char Sybol { get; } = 'T'; // Символ танка, отображаемый в консоли

    // Координата X танка
    public int X
    {
        get => _x;
        set => _x = value;
    }

    // Координата Y танка
    public int Y
    {
        get => _y;
        set => _y = value;
    }

    // Направление движения танка
    public char Direction
    {
        get => _direction;
        set => _direction = value;
    }

    // Статус "уничтожен/нет"
    public bool IsDestroyed
    {
        get => _isDestroyed;
        set => _isDestroyed = value;
    }

    // Возвращает список всех активных пуль танка (только для чтения)
    public IEnumerable<Bullet> GetBullets => _bullets.AsReadOnly();

    // Удаляет из списка все уничтоженные пули
    public void RemoveBullets()
    {
        _bullets.RemoveAll(b => b.IsDestroyed);
    }

    // Добавляет пулю в список пуль танка
    public void AddBullet(Bullet bullet)
    {
        _bullets.Add(bullet);
    }

    // Конструктор для создания танка с начальными координатами
    public Tank(int x, int y)
    {
        X = x;
        Y = y;
        Direction = 'U'; // Танки по умолчанию смотрят вверх
        IsDestroyed = false;
    }

    // Метод для перемещения танка
    public void Move(char direction, List<Wall> walls, List<Tank> enemy_tanks)
    {
        Direction = direction; // Устанавливаем новое направление
        int newX = X, newY = Y;

        // Рассчитываем новые координаты на основе направления
        switch (direction)
        {
            case 'U': newY--; break;
            case 'D': newY++; break;
            case 'L': newX--; break;
            case 'R': newX++; break;
        }

        // Проверяем выход за границы экрана
        if (newX < 0 || newY < 0 || newX >= _consoleWidth || newY >= _consoleHeight)
        {
            return; // Не двигаемся, если танк выходит за пределы
        }

        // Проверяем столкновение с другими танками
        if (enemy_tanks.Count(t => t.X == newX && t.Y == newY && !t.IsDestroyed) >= 1)
        {
            return; // Не двигаемся, если впереди враг
        }

        // Проверяем столкновение со стеной
        if (!walls.Exists(w => w.X == newX && w.Y == newY && !w.IsDestroyed))
        {
            X = newX; // Двигаемся только если впереди нет препятствий
            Y = newY;
        }
    }

    // Метод для стрельбы: создаёт новую пулю
    public Bullet Shoot()
    {
        return new Bullet(X, Y, Direction);
    }
}
