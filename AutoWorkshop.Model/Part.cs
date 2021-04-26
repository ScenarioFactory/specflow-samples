﻿namespace AutoWorkshop.Model
{
    public class Part
    {
        public Part(int jobId, string description, decimal cost)
        {
            JobId = jobId;
            Description = description;
            Cost = cost;
        }

        public int JobId { get; }

        public string Description { get; }

        public decimal Cost { get; }

        public string SupplierCode { get; set; }

        public string SupplierReference { get; set; }
    }
}
