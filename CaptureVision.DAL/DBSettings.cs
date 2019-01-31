namespace CaptureVision.DAL
{
    public class DBSettings
    {
        public Setting<string> ConnectionString = new Setting<string>
        {
            Value = "Server=127.0.0.1;SslMode=none;database=capturevision;uid=reads;Port=3306;Password=!Smb571;default command timeout=240;",
            Description = "Локальная строка подключения к БД"
        };
    }

    public class Setting<T>
    {

        public T Value { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return $"{Value}| {Description}";
        }
    }
}
