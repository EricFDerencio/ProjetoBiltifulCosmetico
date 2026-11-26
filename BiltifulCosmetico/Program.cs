using BiltifulCosmetico;

Console.Write("Informe o CPF do cliente: ");
string cpf = Console.ReadLine()!;

string? registro = CustomerFile.FindCustomerByCPF(cpf);

if (registro == null)
{
    Console.WriteLine("Cliente não encontrado.");
}
else
{
    Customer customer = new Customer(registro);

    Console.WriteLine(customer);

    Console.Write("\nInforme o novo nome: ");
    string novoNome = Console.ReadLine()!;

    customer.SetName(novoNome);

    if (CustomerFile.UpdateCustomer(customer))
        Console.WriteLine("Cliente atualizado com sucesso.");
    else
        Console.WriteLine("Erro ao atualizar cliente.");
}