﻿namespace AutoWorkshop.Specs.Screenplay.WebDriver.Tasks
{
    using Pages;
    using Pattern;

    public class CreateCustomer : WebTask
    {
        private readonly string _name;
        private string _title;
        private string _addressLine1;
        private string _addressLine2;
        private string _addressLine3;
        private string _postcode;
        private string _homePhone;
        private string _mobile;

        private CreateCustomer(string name)
        {
            _name = name;
        }

        public static CreateCustomer Named(string name)
        {
            return new CreateCustomer(name);
        }

        public CreateCustomer Titled(string title)
        {
            _title = title;
            return this;
        }

        public CreateCustomer OfAddress(string addressLine1, string addressLine2, string addressLine3, string postcode)
        {
            _addressLine1 = addressLine1;
            _addressLine2 = addressLine2;
            _addressLine3 = addressLine3;
            _postcode = postcode;
            return this;
        }

        public CreateCustomer WithHomePhone(string homePhone)
        {
            _homePhone = homePhone;
            return this;
        }

        public CreateCustomer WithMobile(string mobile)
        {
            _mobile = mobile;
            return this;
        }

        protected override void PerformAs(IActor actor, AutoWorkshopDriver driver)
        {
            actor.AttemptsTo(
                Select.ByText(CustomerMaintenancePage.Title, _title),
                SendKeys.To(CustomerMaintenancePage.Name, _name),
                SendKeys.To(CustomerMaintenancePage.AddressLine1, _addressLine1),
                SendKeys.To(CustomerMaintenancePage.AddressLine2, _addressLine2),
                SendKeys.To(CustomerMaintenancePage.AddressLine3, _addressLine3),
                SendKeys.To(CustomerMaintenancePage.Postcode, _postcode),
                SendKeys.To(CustomerMaintenancePage.HomePhone, _homePhone),
                SendKeys.To(CustomerMaintenancePage.Mobile, _mobile),
                Click.On(CustomerMaintenancePage.Save));
        }
    }
}
