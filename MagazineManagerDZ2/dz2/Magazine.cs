class Magazine
{
    public int Id { get; set; }
    public int PublisherId { get; set; }
    public string Name { get; set; }
    
    private int _circulationK;
    
    public int CirculationK
    {
        get { return _circulationK; }
        set 
        {
            if (value < 0)
                throw new System.ArgumentException("Тираж не может быть отрицательным");
            _circulationK = value;
        }
    }
    
    public Magazine(int id, int publisherId, string name, int circulationK)
    {
        Id = id;
        PublisherId = publisherId;
        Name = name;
        CirculationK = circulationK;
    }
    
    public Magazine() : this(0, 0, "", 0) { }
    
    public override string ToString()
    {
        return $"[{Id}] {Name} (издательство #{PublisherId}, тираж: {CirculationK} тыс.)";
    }
}
