public class Supplier
{
    public string CNPJ { get; private set; }
    public string CorporateName { get; private set; }
    public DateOnly OpeningDate { get; private set; }
    public DateOnly LastPurchase { get; private set; }
    public DateOnly RegistrationDate { get; private set; }

    public ESituation Situation { get; private set; } = ESituation.Active;


    public Supplier(string cnpj, string name, DateOnly openingDate)
    {
        ValidateCNPJ(cnpj);

        if (ValidateCNPJ(cnpj))
        {
            CNPJ = cnpj;
        }
        else
        {
            throw new ArgumentException("CNPJ inválido!");
        }


        if (name.Length > 50) // valida se a razao social informada possui 50 caracteres ou mais
        {
            throw new ArgumentException("Razão social inválida. Deve ter no máximo 50 caracteres.");
        }

        CorporateName = name.PadLeft(50, '0'); // preenche com 0 os caracteres restantes da razão social (pra completar 50 caracteres)



        if (openingDate > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ArgumentException("A data de abertura não pode ser futura.");
        }
        if (openingDate.AddMonths(6) > DateOnly.FromDateTime(DateTime.Today)) // compara a data de abertura + 6 meses com a data atual
        {
            throw new ArgumentException("Fornecedor abriu a empresa há menos de 6 meses.");
        }

        OpeningDate = openingDate;


        RegistrationDate = DateOnly.FromDateTime(DateTime.Today); // preenche automaticamente a data de registro
        LastPurchase = RegistrationDate; // no momento do cadastro, a data de ultima venda fica = data de registro
    }

    public static bool ValidateCNPJ(string cnpj)
    {
        int[] weightsConversion = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }; //pesos que serão utilizados para o 1° verificador do cnpj
        int[] weightsSecondConversion = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }; //pesos que serão utilizados para o 2° verificador do cnpj

        if (cnpj == null)
        {
            return false;
        }

        cnpj = cnpj.ToUpperInvariant();

        if (cnpj.Length != 14)
        {
            return false;                    // verifica se tem 14 caracteres
        }

        for (int i = 0; i < 12; i++)   // verifica se sao letras || numeros
        {
            if (!((cnpj[i] >= '0' && cnpj[i] <= '9') || (cnpj[i] >= 'A' && cnpj[i] <= 'Z')))
            {
                return false;
            }
        }

        for (int i = 12; i < 14; i++)     // verifica se os ultimos 2 sao numeros
        {
            if (!(cnpj[i] >= '0' && cnpj[i] <= '9'))
            {
                return false;
            }
        }

        int sumResults = 0;
        for (int i = 0; i < 12; i++)    // pega o caracter de cada posição, converte para o ascii 
        {                              // e multiplica pelo peso de cada item com base no vetor
            int value = cnpj[i] - 48;
            int result = value * weightsConversion[i];

            sumResults += result;
        }

        int firstDigit;                 // com base no calculo que fizemos, precisamos calcular qual
                                        // deve ser o primeiro verificador
        int remainder = sumResults % 11;
        if (remainder < 2)
        {
            firstDigit = 0;
        }
        else
        {
            firstDigit = 11 - remainder;
        }


        int sumSecondResults = 0;         //com base nos caracteres + primeiro verificador, 
                                          // calculamos o segundo verificador
        for (int i = 0; i < 13; i++)
        {
            int value;

            if (i < 12)
            {
                value = cnpj[i] - 48;
            }
            else
            {
                value = firstDigit;
            }

            int secondResult = value * weightsSecondConversion[i];
            sumSecondResults += secondResult;
        }

        int secondDigit;                 // com base no calculo que fizemos, precisamos calcular qual
                                         // deve ser o segundo verificador
        int secondRemainder = sumSecondResults % 11;
        if (secondRemainder < 2)
        {
            secondDigit = 0;
        }
        else
        {
            secondDigit = 11 - secondRemainder;
        }


        int informedFirstDigit = cnpj[12] - 48;   // compara digitos passados pelo usuario
        int informedSecondDigit = cnpj[13] - 48;  // com os digitos calculados

        return firstDigit == informedFirstDigit &&
            secondDigit == informedSecondDigit;
    }

    public void UpdateLastPurchase()
    {
        LastPurchase = DateOnly.FromDateTime(DateTime.Today); //metodo para atualizar a data de ultima compra
    }

    public void Activate()
    {
        Situation = ESituation.Active;
    }

    public void Desactivate()
    {
        Situation = ESituation.Inactive;
    }
}




