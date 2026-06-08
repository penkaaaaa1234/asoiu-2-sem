class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public Publisher(int id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public Publisher() : this(0, "") { }
    
    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }
}
