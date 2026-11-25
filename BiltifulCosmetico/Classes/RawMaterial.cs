public class RawMaterial
{
    public string Id { get; private set; } = $"MP{Random.Shared.Next(0, 10000):D4}";
    public string Name { get; private set; }
    public DateTime LastBuy { get; private set; } = DateTime.Today;
    public DateTime DateOfRegister { get; private set; } = DateTime.Today;
    public ESituation Situation { get; private set; } = ESituation.Active;

    public RawMaterial(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome é obrigatório.");

        if (name.Length > 20)
            throw new ArgumentException("O nome deve ter no máximo 20 caracteres.");

        Name = name;
    }
    public RawMaterial(string id, string name, DateTime lastBuy, DateTime dateOfRegister, ESituation situation)
    {
        Id = id;
        Name = name;
        LastBuy = lastBuy;
        DateOfRegister = dateOfRegister;
        Situation = situation;
    }

    public void ChangeSituation()
    {
        Situation = Situation == ESituation.Active
            ? ESituation.Inactive
            : ESituation.Active;
    }
    public string ToFileString()
    {
        return Id +
               $"{Name.PadLeft(20)}" +
               LastBuy.ToString("ddMMyyyy") +
               DateOfRegister.ToString("ddMMyyyy") +
               (char)Situation;
    }


}