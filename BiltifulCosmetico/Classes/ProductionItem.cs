using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BiltifulCosmetico.Services;

namespace BiltifulCosmetico.Classes
{
    internal class ProductionItem
    {
        public int Id { get; private set; }
        public DateOnly ProductionDate { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
        public RawMaterial RawMaterial { get; private set; }
        public int RawMaterialQuantity { get; private set; }

        public ProductionItem(RawMaterial rawMaterial, int rawMaterialQuantity)
        { 
            SetId();
            SetRawMaterial(rawMaterial);
            SetRawMaterialQuantity(rawMaterialQuantity);
        }

        private ProductionItem()
        {
        }

        private void SetId()
        {
            Id = ProductionItemFile.GetNextId();
        }

        private void SetRawMaterial(RawMaterial rawMaterial)
        {
            ArgumentNullException.ThrowIfNull(rawMaterial);

            string code = rawMaterial.Id;

            if (code is null ||
                code.Length != 6 ||
                !code.StartsWith("MP", StringComparison.Ordinal) ||
                !code.Substring(2, 4).All(char.IsAsciiDigit))
            {
                throw new ArgumentException(
                    "O código da matéria-prima deve seguir o formato MP0000.",
                    nameof(rawMaterial));
            }

            RawMaterial = rawMaterial;
        }

        private void SetRawMaterialQuantity(int quantity)
        {
            if (quantity < 1 || quantity > 999)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity), "Informe uma quantidade entre 1 e 999 unidades.");
            }

            RawMaterialQuantity = quantity;
        }

        public ProductionItem CreateRelatedItem(RawMaterial rawMaterial, int quantity)
        {
            var item = new ProductionItem
            {
                Id = Id,
                ProductionDate = ProductionDate
            };

            item.SetRawMaterial(rawMaterial);
            item.SetRawMaterialQuantity(quantity);

            return item;
        }

        public override string ToString()
        {
            return $"Produção {Id:D5} | {ProductionDate:dd/MM/yyyy} | " +
                   $"{RawMaterial.Id} - {RawMaterial.Name} | {RawMaterialQuantity} un.";
        }
    }
}