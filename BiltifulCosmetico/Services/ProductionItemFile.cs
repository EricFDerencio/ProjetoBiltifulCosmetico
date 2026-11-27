
using BiltifulCosmetico.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BiltifulCosmetico.Services
{
    internal static class ProductionItemFile
    {
        private static readonly string directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "biltiful"
        );

        private static readonly string filePath =
            Path.Combine(directory, "ItemProducao.dat");

        private const int RecordLength = 22;

        public static string EnsureFileExists()
        {
            Directory.CreateDirectory(directory);

            if (!File.Exists(filePath))
            {
                using FileStream file = File.Open(
                    filePath,
                    FileMode.OpenOrCreate
                );
            }

            return filePath;
        }

        internal static int GetNextId()
        {
            EnsureFileExists();

            string? lastRecord = File.ReadLines(filePath).LastOrDefault();

            if (lastRecord is null)
                return 1;

            int lastId = ReadId(lastRecord);

            if (lastId >= 99999)
                throw new InvalidOperationException(
                    "O limite de IDs de produção foi atingido."
                );

            return lastId + 1;
        }

        private static int ReadId(string record)
        {
            if (record.Length != RecordLength ||
                !int.TryParse(
                    record.Substring(0, 5),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out int id) ||
                id < 1)
            {
                throw new InvalidDataException(
                    "Registro inválido em ItemProducao.dat."
                );
            }

            return id;
        }

        public static void Save(ProductionItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            EnsureFileExists();

            File.AppendAllText(
                filePath,
                ToFileString(item) + Environment.NewLine
            );
        }

        private static string ToFileString(ProductionItem item)
        {
            string record =
                item.Id.ToString("D5", CultureInfo.InvariantCulture) +
                item.ProductionDate.ToString("ddMMyyyy", CultureInfo.InvariantCulture) +
                item.RawMaterial.Id +
                item.RawMaterialQuantity.ToString("D3", CultureInfo.InvariantCulture);

            if (record.Length != RecordLength)
                throw new InvalidDataException(
                    "O registro do item de produção possui tamanho incorreto."
                );

            return record;
        }

        public static List<ProductionItem> FindByProductionId(int productionId)
        {
            EnsureFileExists();

            List<RawMaterial> rawMaterials =
                RawMaterialFile.LoadRawMaterials();

            List<ProductionItem> items = new();

            foreach (string record in File.ReadLines(filePath))
            {
                int id = ReadId(record);

                if (id == productionId)
                    items.Add(FromFileString(record, id, rawMaterials));
            }

            return items;
        }

        private static ProductionItem FromFileString(
            string record,
            int id,
            List<RawMaterial> rawMaterials)
        {
            DateOnly date = DateOnly.ParseExact(
                record.Substring(5, 8),
                "ddMMyyyy",
                CultureInfo.InvariantCulture
            );

            string materialCode = record.Substring(13, 6);

            int quantity = int.Parse(
                record.Substring(19, 3),
                CultureInfo.InvariantCulture
            );

            RawMaterial? material =
                rawMaterials.Find(raw => raw.Id == materialCode);

            if (material is null)
                throw new InvalidDataException(
                    $"Matéria-prima {materialCode} não cadastrada."
                );

            return ProductionItem.LoadFromRecord(
                id,
                date,
                material,
                quantity
            );
        }

        public static int DeleteByProductionId(int productionId)
        {
            EnsureFileExists();

            string[] records = File.ReadAllLines(filePath);

            string[] remaining = records
                .Where(record => ReadId(record) != productionId)
                .ToArray();

            int deletedCount = records.Length - remaining.Length;

            if (deletedCount > 0)
                File.WriteAllLines(filePath, remaining);

            return deletedCount;
        }
    }
}
