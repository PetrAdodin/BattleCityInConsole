public class Bullet
{
    private int _x;
    private int _y;
    private char _direction; // Направление движения пули: 'U' (вверх), 'D' (вниз), 'L' (влево), 'R' (вправо)
    private bool _isDestroyed; // Флаг: true, если пуля уничтожена

    public char Sybol { get; } = '*'; // Символ пули, отображаемый в консоли

    // Координата X пули
    public int X
    {
        get => _x;
        set => _x = value;
    }

    // Координата Y пули
    public int Y
    {
        get => _y;
        set => _y = value;
    }

    // Направление движения
    public char Direction
    {
        get => _direction;
        set => _direction = value;
    }

    // Статус "уничтожена/нет"
    public bool IsDestroyed
    {
        get => _isDestroyed;
        set => _isDestroyed = value;
    }

    // Конструктор для создания пули с начальными координатами и направлением
    public Bullet(int x, int y, char direction)
    {
        X = x;
        Y = y;
        Direction = direction;
        IsDestroyed = false;
    }

    // Метод для перемещения пули в зависимости от её направления
    public void Move()
    {
        switch (Direction)
        {
            case 'U': Y--; break; // Вверх
            case 'D': Y++; break; // Вниз
            case 'L': X--; break; // Влево
            case 'R': X++; break; // Вправо
        }
    }
}
