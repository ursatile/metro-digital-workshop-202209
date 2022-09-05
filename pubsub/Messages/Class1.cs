namespace Messages;
public class Greeting
{
    public string Name { get; set; } = "METRO";
    public int Number { get; set; }
    public override string ToString()
    {
        return $"Hello {Name} (Greeting #{Number}";
    }

}
