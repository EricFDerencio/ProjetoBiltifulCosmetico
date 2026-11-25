public class Product
{
    public string Barcode { get; set; }

    public Product(List<Product> products)
    {
        Barcode = SetGenerateBarcode(products);

    }

    string SetGenerateBarcode(List<Product> products)
    {
        int[] vector = new int[13];
        //requisito começar com 789
        vector[0] = 7;
        vector[1] = 8;
        vector[2] = 9;

        bool invalidCode = true;
        int sumWeights = 0;
        int remainderOfDivisionSum = 0;
        string readyBarcode = string.Empty;
        Random random = new Random();


        while (invalidCode)
        {
            sumWeights = 0;
            remainderOfDivisionSum = 0;

            //percorre o vetor do código de barras e preenche
            for (int i = 3; i <= 11; i++)
            {

                vector[i] = random.Next(0, 10);
            }

            //percorre o vetor do código somando à variável de acordo com o peso
            for (int i = 1; i <= 12; i++)
            {
                if (i % 2 != 0)
                {
                    sumWeights += (vector[i - 1] * 1);
                }
                else
                {
                    sumWeights += (vector[i - 1] * 3);
                }
            }

            //cálculo do último digito verificador
            remainderOfDivisionSum = sumWeights % 10;
            if (remainderOfDivisionSum == 0)
            {
                vector[12] = 0;
            }
            else
            {
                vector[12] = 10 - remainderOfDivisionSum;
            }

            readyBarcode = string.Concat(vector);

            //verificação de duplicidade de códigos de barra
            if (products != null)
            {
                invalidCode = false;
                foreach (var item in products) //verifica todos os códigos dos produtos, encontrando um igual ele repete o processo do while
                {
                    if (item.Barcode == readyBarcode)
                    {
                        invalidCode = true;
                        break;
                    }

                }

            }
            else //se não tiver produtos na lista, este vai ser o primeiro, não tendo com quem comparar
            {
                invalidCode = false;
            }

        }

        return readyBarcode;


    }

}