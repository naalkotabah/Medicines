namespace Medicines.Data.dto
{
    public class SearchMedicinesDto
    {
        public int id  { get; set; }
        public string? ScientificName { get; set; }

        public string? TradeName { get; set; }

        public decimal Price { get; set; }
    }
}
