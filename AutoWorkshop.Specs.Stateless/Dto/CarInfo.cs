﻿namespace AutoWorkshop.Specs.Stateless.Dto
{
    public class CarInfo
    {
        public CarInfo(string registration, int customerId, string make, string model)
        {
            Registration = registration;
            CustomerId = customerId;
            Make = make;
            Model = model;
        }

        /// <summary>
        /// MySql constructor used by Dapper.
        /// </summary>
        private CarInfo(string registration, uint customerId, string make, string model)
            : this(registration, (int)customerId, make, model)
        {
        }

        public string Registration { get; }

        public int CustomerId { get; }

        public string Make { get; }

        public string Model { get; }
    }
}
