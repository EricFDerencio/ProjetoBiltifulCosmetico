public class SupplierMenu
{
    private SupplierFiles supplierFiles;

    public SupplierMenu()
    {
        supplierFiles = new SupplierFiles();
    }


    public void Menu()
    {
        do
        {
            Console.WriteLine("""
        =============== Menu - Fornecedores ===============
        1 - Cadastra Fornecedor.
        2 - Localizar cadastro de Fornecedor.
        3 - Editar Fornecedor.
        4 - Bloquear Fornecedor.
        5 - Desbloquear Fornecedor.
        6 - Verificar status do Fornecedor (bloq./desbloq.).
        7 - Ativar Fornecedor.
        8 - Inativar Fornecedor.
        9 - Verificar se o Fornecedor está ativo ou inativo.
        0 - Voltar ao Menu Principal.
        """);

            int option;

            Console.WriteLine("Escolha a opção desejada: ");
            if (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Entrada inválida. Digite um número: ");
                continue;
            }


            switch (option)
            {
                case 1:
                    RegisterSupplierMenu();
                    break;
                case 2:
                    FindSupplierMenu();
                    break;
                case 3:
                    EditSupplierMenu();
                    break;
                case 4:
                    BlockSupplierMenu();
                    break;
                case 5:
                    UnlockSupplierMenu();
                    break;
                case 6:
                    CheckSupplierStatusMenu();
                    break;
                case 7:
                    ActivateSupplierMenu();
                    break;
                case 8:
                    InativateSupplierMenu();
                    break;
                case 9:
                    CheckSituationMenu();
                    break;
                case 0: return;
                default:
                    Console.WriteLine("Opção inválida. Digite novamente: ");
                    break;


            }
        } while (true);
    }

    public void RegisterSupplierMenu() // opcao 1 - registrar fornecedor
    {
        Console.WriteLine("Digite o CNPJ: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Console.WriteLine("Digite a razão social: ");
        string corporateName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(corporateName))
        {
            Console.WriteLine("Razão Social não pode ficar vazia.");
            return;
        }

        Console.WriteLine("Digite a data de abertura do CNPJ (dd/MM/yyyy): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly openingDate))
        {
            Console.WriteLine("Data inválida, tente novamente.");
            return;
        }

        try
        {
            Supplier newsupplier = new Supplier(cnpj, corporateName, openingDate);

            if (supplierFiles.RegisterSupplier(newsupplier))
            {
                Console.WriteLine("Fornecedor cadastrado!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao cadastrar fornecedor: {ex.Message}");
        }
    }

    public void FindSupplierMenu() // opcao 2 - procurar fornecedor
    {
        Console.WriteLine("Digite o CNPJ que deseja localizar: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Supplier findsupplier = supplierFiles.FindCNPJ(cnpj);

        if (findsupplier != null)
        {
            Console.WriteLine($"CNPJ: {findsupplier.CNPJ}");
            Console.WriteLine($"Razão Social: {findsupplier.CorporateName}");
            Console.WriteLine($"Data de abertura: {findsupplier.OpeningDate}");
            Console.WriteLine($"Data de cadastro: {findsupplier.RegistrationDate}");
            Console.WriteLine($"Última compra: {findsupplier.LastPurchase}");
            Console.WriteLine($"Situação (ativo ou inativo): {findsupplier.Situation}");
        }
        else
        {
            Console.WriteLine("Fornecedor não foi encontrado.");
        }

    }

    public void EditSupplierMenu() // opcao 3 - editar fornecedor
    {
        Console.WriteLine("Digite o CNPJ do fornecedor que deseja editar: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Supplier editsupplier = supplierFiles.FindCNPJ(cnpj);

        if (editsupplier == null)
        {
            Console.WriteLine("Fornecedor não foi encontrado.");
            return;
        }

        Console.WriteLine("Qual a nova Razão Social?");
        string corporateName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(corporateName))
        {
            Console.WriteLine("Razão Social não pode ficar vazia.");
            return;
        }

        Console.WriteLine("Digite a nova data de abertura (dd/MM/yyyy): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly openingDate))
        {
            Console.WriteLine("Data inválida, tente novamente.");
            return;
        }

        try
        {
            Supplier updateSupplier = new Supplier(
                editsupplier.CNPJ,
                corporateName,
                openingDate,
                editsupplier.LastPurchase,
                editsupplier.RegistrationDate,
                editsupplier.Situation);

            if (supplierFiles.EditSupplier(cnpj, updateSupplier))
            {
                Console.WriteLine("Fornecedor atualizado.");
            }
            else
            {
                Console.WriteLine("Fornecedor não atualizado.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar fornecedor: {ex.Message}");
        }

    }

    public void BlockSupplierMenu() //opcao 4 - bloq fornecedor
    {
        Console.WriteLine("Digite o CNPJ que deseja bloquear: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Supplier supplier = supplierFiles.FindCNPJ(cnpj);

        if (supplier == null)
        {
            Console.WriteLine("Fornecedor não foi encontrado.");
            return;
        }
        if (supplierFiles.FindBlockedSupplier(cnpj))
        {
            Console.WriteLine("Este fornecedor já foi bloqueado anteriormente.");
            return;
        }
        if (supplierFiles.BlockSupplier(cnpj))
        {
            Console.WriteLine("Fornecedor bloqueado com sucesso!");
        }
        else
        {
            Console.WriteLine("Não foi possível bloquear o fornecedor.");
        }
    }

    public void UnlockSupplierMenu() // opcao 5 - desbloq fornecedor
    {
        Console.WriteLine("Digite o CNPJ que deseja desbloquear: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Supplier supplier = supplierFiles.FindCNPJ(cnpj);

        if (supplier == null)
        {
            Console.WriteLine("Fornecedor não foi encontrado.");
            return;
        }
        if (!supplierFiles.FindBlockedSupplier(cnpj))
        {
            Console.WriteLine("Este fornecedor não está bloqueado.");
            return;
        }
        if (supplierFiles.UnlockSupplier(cnpj))
        {
            Console.WriteLine("Fornecedor desbloqueado!");
        }
        else
        {
            Console.WriteLine("Não foi possível desbloquear o fornecedor.");
        }
    }

    public void CheckSupplierStatusMenu() // opcao 6 - verificar bloq/desbloq
    {
        Console.WriteLine("Digite o CNPJ que deseja consultar: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Supplier supplier = supplierFiles.FindCNPJ(cnpj);

        if (supplier == null)
        {
            Console.WriteLine("Fornecedor não encontrado");
            return;
        }
        if (supplierFiles.FindBlockedSupplier(cnpj))
        {
            Console.WriteLine($"O fornecedor referente ao CNPJ {cnpj} está bloqueado!");
        }
        else
        {
            Console.WriteLine($"O fornecedor referente ao CNPJ {cnpj} está desbloqueado.");
        }
    }

    public void ActivateSupplierMenu() // opcao 7 - ativar fornecedor
    {
        Console.WriteLine("Digite o CNPJ que deseja ativar: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Supplier supplier = supplierFiles.FindCNPJ(cnpj);

        if (supplier == null)
        {
            Console.WriteLine("Fornecedor não foi encontrado.");
            return;
        }
        if(supplier.Situation == ESituation.Active)
        {
            Console.WriteLine("Este fornecedor já está ativo.");
            return;
        }
        try
        {
            Supplier updateSupplier = new Supplier(
                supplier.CNPJ,
                supplier.CorporateName,
                supplier.OpeningDate,
                supplier.LastPurchase,
                supplier.RegistrationDate,
                ESituation.Active);
            if (supplierFiles.EditSupplier(cnpj, updateSupplier))
            {
                Console.WriteLine("Fornecedor ativado.");
            }
            else
            {
                Console.WriteLine("Não foi possível ativar o fornecedor.");
            }
        }catch(Exception ex)
        {
            Console.WriteLine($"Erro ao ativar o fornecedor: {ex.Message}");
        }
    }

    public void InativateSupplierMenu() // opcao 8 - inativar fornecedor
    {
        Console.WriteLine("Digite o CNPJ que deseja inativar: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Supplier supplier = supplierFiles.FindCNPJ(cnpj);

        if (supplier == null)
        {
            Console.WriteLine("Fornecedor não foi encontrado.");
            return;
        }
        if (supplier.Situation == ESituation.Inactive)
        {
            Console.WriteLine("Este fornecedor já está inativo.");
            return;
        }
        try
        {
            Supplier updateSupplier = new Supplier(
                supplier.CNPJ,
                supplier.CorporateName,
                supplier.OpeningDate,
                supplier.LastPurchase,
                supplier.RegistrationDate,
                ESituation.Inactive);

            if(supplierFiles.EditSupplier(cnpj, updateSupplier))
            {
                Console.WriteLine("Fornecedor inativado.");
            }
            else
            {
                Console.WriteLine("Não foi possível inativar o fornecedor.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inativar fornecedor: {ex.Message}");
        }
    }

    public void CheckSituationMenu() // opcao 9 - verificar ativo/inativo
    {
        Console.WriteLine("Digite o CNPJ que deseja consultar: ");
        string cnpj = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cnpj))
        {
            Console.WriteLine("CNPJ não pode ficar vazio.");
            return;
        }

        cnpj = cnpj.ToUpperInvariant();

        Supplier supplier = supplierFiles.FindCNPJ(cnpj);

        if (supplier == null)
        {
            Console.WriteLine("Fornecedor não encontrado.");
            return;
        }
        if (supplier.Situation == ESituation.Active)
        {
            Console.WriteLine("O fornecedor está ativo.");
        }
        else if (supplier.Situation == ESituation.Inactive)
        {
            Console.WriteLine("O fornecedor está inativo.");
        }
    }
}
