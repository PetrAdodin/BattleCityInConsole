public class Wall
{
    private int _x; // Координата X стены
    private int _y; // Координата Y стены
    private bool _isDestroyed; // Флаг: true, если стена разрушена

    public char Sybol { get; } = '#'; // Символ стены, отображаемый в консоли

    // Координата X стены
    public int X
    {
        get => _x;
        set => _x = value;
    }

    // Координата Y стены
    public int Y
    {
        get => _y;
        set => _y = value;
    }

    // Статус "разрушена/нет"
    public bool IsDestroyed
    {
        get => _isDestroyed;
        set => _isDestroyed = value;
    }

    // Конструктор для создания стены
    public Wall(int x, int y)
    {
        X = x;
        Y = y;
        IsDestroyed = false;
    }
}
