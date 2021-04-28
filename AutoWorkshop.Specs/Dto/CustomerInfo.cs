﻿namespace AutoWorkshop.Specs.Dto
{
    public class CustomerInfo
    {
        public CustomerInfo(
            string title,
            string name,
            string addressLine1,
            string addressLine2,
            string addressLine3,
            string postcode,
            string homePhone,
            string mobile,
            byte accountInvoicing)
        {
            Title = title;
            Name = name;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            AddressLine3 = addressLine3;
            Postcode = postcode;
            HomePhone = homePhone;
            Mobile = mobile;
            AccountInvoicing = accountInvoicing;
        }

        public string Title { get; }

        public string Name { get; }

        public string AddressLine1 { get; }

        public string AddressLine2 { get; }

        public string AddressLine3 { get; }

        public string Postcode { get; }

        public string HomePhone { get; }

        public string Mobile { get; }

        public byte AccountInvoicing { get; }

        public bool HasAccountInvoicing => AccountInvoicing == 1;
    }
}
