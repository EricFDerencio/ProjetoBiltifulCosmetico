public static class RawMaterialMenu
{
    public static void MaterialMenu()
    {
        // carrega uma vez ao entrar no menu; cada alteração é gravada na hora
        List<RawMaterial> materials = RawMaterialFile.LoadRawMaterials();

        while (true)
        {
            Console.WriteLine("\n=== MATÉRIA-PRIMA ===");
            Console.WriteLine("1 - Cria Material");
            Console.WriteLine("2 - Lista Materiais");
            Console.WriteLine("3 - Procurar Material por id");
            Console.WriteLine("4 - Definir como Inativo");
            Console.WriteLine("5 - Definir como Ativo");
            Console.WriteLine("6 - Editar nome");
            Console.WriteLine("0 - Voltar");
            Console.Write("Opção: ");

            if (!int.TryParse(Console.ReadLine(), out int opcao))
            {
                Console.WriteLine("Opção inválida.");
                continue;
            }

            switch (opcao)
            {
                case 0:
                    return;
                case 1:
                    CreateMaterial(materials);
                    break;
                case 2:
                    Navigate(materials);
                    break;
                case 3:
                    FindMaterial(materials);
                    break;
                case 4:
                    SetSituation(materials, ESituation.Inactive);
                    break;
                case 5:
                    SetSituation(materials, ESituation.Active);
                    break;
                case 6:
                    RenameMaterial(materials);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    private static void CreateMaterial(List<RawMaterial> materials)
    {
        if (materials.Count >= 10000)
        {
            Console.WriteLine("Limite de 10000 matérias-primas atingido.");
            return;
        }

        RawMaterial material = RawMaterialFile.Create();

        // o Id é sorteado; se já existir, sorteia outro (a chave não pode repetir)
        while (materials.Any(m => m.Id == material.Id))
            material = new RawMaterial(material.Name);

        materials.Add(material);
        Save(materials);

        Console.WriteLine($"Matéria-prima cadastrada com o Id {material.Id}.");
    }

    // impressão por registro: só ativas, com primeiro/anterior/próximo/último (conforme a spec)
    private static void Navigate(List<RawMaterial> materials)
    {
        List<RawMaterial> active = materials
            .Where(m => m.Situation == ESituation.Active)
            .ToList();

        if (active.Count == 0)
        {
            Console.WriteLine("Nenhuma matéria-prima ativa.");
            return;
        }

        int index = 0;

        while (true)
        {
            Console.WriteLine();
            Print(active[index]);
            Console.WriteLine($"\n[{index + 1}/{active.Count}]  P - Primeiro | A - Anterior | N - Próximo | U - Último | S - Sair");
            Console.Write("Opção: ");

            switch (Console.ReadLine()?.Trim().ToUpperInvariant())
            {
                case "P":
                    index = 0;
                    break;
                case "A":
                    if (index > 0) index--;
                    else Console.WriteLine("Já é o primeiro registro.");
                    break;
                case "N":
                    if (index < active.Count - 1) index++;
                    else Console.WriteLine("Já é o último registro.");
                    break;
                case "U":
                    index = active.Count - 1;
                    break;
                case "S":
                    return;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    private static void FindMaterial(List<RawMaterial> materials)
    {
        RawMaterial? material = AskForMaterial(materials);

        if (material != null)
            Print(material);
    }

    private static void SetSituation(List<RawMaterial> materials, ESituation target)
    {
        RawMaterial? material = AskForMaterial(materials);

        if (material == null)
            return;

        if (material.Situation == target)
        {
            Console.WriteLine(target == ESituation.Active
                ? "A matéria-prima já está ativa."
                : "A matéria-prima já está inativa.");
            return;
        }

        material.ChangeSituation();
        Save(materials);

        Console.WriteLine("Situação atualizada.");
    }

    private static void RenameMaterial(List<RawMaterial> materials)
    {
        RawMaterial? material = AskForMaterial(materials);

        if (material == null)
            return;

        Console.Write("Novo nome: ");

        try
        {
            material.ChangeName(Console.ReadLine() ?? string.Empty);
            Save(materials);
            Console.WriteLine("Nome atualizado.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    private static RawMaterial? AskForMaterial(List<RawMaterial> materials)
    {
        Console.Write("Digite o Id (ex.: MP0001): ");
        string id = Console.ReadLine() ?? string.Empty;

        RawMaterial? material = RawMaterialFile.FindById(materials, id);

        if (material == null)
            Console.WriteLine("Matéria-prima não encontrada.");

        return material;
    }

    private static void Save(List<RawMaterial> materials)
    {
        RawMaterialFile.SaveRawMaterials(materials, RawMaterialFile.FilePath);
    }

    private static void Print(RawMaterial material)
    {
        Console.WriteLine($"Id: {material.Id}");
        Console.WriteLine($"Nome: {material.Name}");
        Console.WriteLine($"Última compra: {material.LastBuy:dd/MM/yyyy}");
        Console.WriteLine($"Data de cadastro: {material.DateOfRegister:dd/MM/yyyy}");
        Console.WriteLine($"Situação: {(material.Situation == ESituation.Active ? "Ativa" : "Inativa")}");
    }
}