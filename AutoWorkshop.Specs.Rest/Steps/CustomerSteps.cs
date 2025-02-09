﻿namespace AutoWorkshop.Specs.Rest.Steps
{
    using System.Linq;
    using System.Net;
    using Database;
    using Database.Questions;
    using Database.Tasks;
    using Dto;
    using Extensions;
    using FluentAssertions;
    using Rest;
    using Rest.Interactions;
    using RestSharp;
    using Screenplay;
    using TechTalk.SpecFlow;

    [Binding]
    public class CustomerSteps
    {
        private readonly RestResponseInfo _lastResponse;
        private readonly IActor _actor;
        private CustomerInfo _newCustomerInput;
        private CustomerInfo _updatedCustomerInput;
        private CustomerInfo _storedCustomer;

        public CustomerSteps(AppSettings appSettings, RestResponseInfo lastResponse)
        {
            _lastResponse = lastResponse;
            _actor = new Actor().WhoCan(
                UseMySqlDatabase.With(appSettings.MySqlConnectionString),
                CallRestApi.Using(new RestClient(appSettings.RestApiUrl)));
        }
        
        [Given(@"there are no customers named '(.*)'")]
        public void GivenThereAreNoCustomersNamed(string customerName)
        {
            _actor.AttemptsTo(
                DeleteCustomers.WithName(customerName));
        }

        [Given(@"there is no customer with ID (.*)")]
        public void GivenThereIsNoCustomerWithId(int customerId)
        {
            _actor.AttemptsTo(
                DeleteCustomer.WithId(customerId));
        }

        [Given(@"this existing customer")]
        [Given(@"the following existing customer")]
        public void GivenThisExistingCustomer(Table table)
        {
            var values = table.Rows.Single();

            _actor.AttemptsTo(DeleteCustomers.WithName(values["Name"]));

            _actor.AttemptsTo(
                InsertCustomer.Named(values["Name"])
                    .Titled(values.GetStringOrDefault("Title"))
                    .OfAddress(
                        values.GetStringOrDefault("Address Line 1"),
                        values.GetStringOrDefault("Address Line 2"),
                        values.GetStringOrDefault("Address Line 3"),
                        values.GetStringOrDefault("Postcode"))
                    .WithHomePhone(values.GetStringOrDefault("Home Phone"))
                    .WithMobile(values.GetStringOrDefault("Mobile"))
                    .WithAccountInvoicingSetTo(values.GetBoolOrDefault("Account Invoicing")));

            _storedCustomer = _actor.AsksFor(StoredCustomer.WithName(values["Name"]));
        }

        [When(@"I create a new customer resource with the following details")]
        public void WhenICreateANewCustomerResourceWithTheFollowingDetails(Table table)
        {
            var values = table.Rows.Single();

            _newCustomerInput = new CustomerInfo
            {
                Title = values["Title"],
                Name = values["Name"],
                AddressLine1 = values["Address Line 1"],
                AddressLine2 = values["Address Line 2"],
                AddressLine3 = values["Address Line 3"],
                Postcode = values["Postcode"],
                HomePhone = values["Home Phone"],
                Mobile = values["Mobile"],
                HasAccountInvoicing = values.GetBoolOrDefault("Account Invoicing")
            };

            _lastResponse.Response = _actor.Calls(Post.Resource(_newCustomerInput).To("api/customer"));
        }

        [When(@"I request the customer resource")]
        public void WhenIRequestTheCustomerResource()
        {
            _storedCustomer.Should().NotBeNull();

            _lastResponse.Response = _actor.Calls(Get.ResourceAt($"api/customer/{_storedCustomer.CustomerId}"));
        }

        [When(@"I request a customer resource with ID (.*)")]
        public void WhenIRequestACustomerResourceWithId(int customerId)
        {
            _lastResponse.Response = _actor.Calls(Get.ResourceAt($"api/customer/{customerId}"));
        }

        [When(@"I update the customer resource with the following changes")]
        public void WhenIUpdateTheCustomerResourceWithTheFollowingChanges(Table table)
        {
            _storedCustomer.Should().NotBeNull();
            
            var values = table.Rows.Single();

            _updatedCustomerInput = new CustomerInfo
            {
                Title = _storedCustomer.Title,
                Name = _storedCustomer.Name,
                AddressLine1 = _storedCustomer.AddressLine1,
                AddressLine2 = _storedCustomer.AddressLine2,
                AddressLine3 = _storedCustomer.AddressLine3,
                Postcode = _storedCustomer.Postcode,
                HomePhone = _storedCustomer.HomePhone,
                Mobile = values["Mobile"],
                HasAccountInvoicing = values.GetBoolOrDefault("Account Invoicing")
            };

            _lastResponse.Response = _actor.Calls(Put.Resource(_updatedCustomerInput).At($"api/customer/{_storedCustomer.CustomerId}"));
        }

        [When(@"I delete the customer resource")]
        public void WhenIDeleteTheCustomerResource()
        {
            _storedCustomer.Should().NotBeNull();

            _lastResponse.Response = _actor.Calls(Delete.ResourceAt($"api/customer/{_storedCustomer.CustomerId}"));
        }
        
        [Then(@"the customer should be added to the system with the details provided")]
        public void ThenTheCustomerShouldBeAddedToTheSystemWithTheDetailsProvided()
        {
            _newCustomerInput.Should().NotBeNull();

            CustomerInfo storedCustomer = _actor.AsksFor(StoredCustomer.WithName(_newCustomerInput.Name));

            storedCustomer.Title.Should().Be(_newCustomerInput.Title);
            storedCustomer.Name.Should().Be(_newCustomerInput.Name);
            storedCustomer.AddressLine1.Should().Be(_newCustomerInput.AddressLine1);
            storedCustomer.AddressLine2.Should().Be(_newCustomerInput.AddressLine2);
            storedCustomer.AddressLine3.Should().Be(_newCustomerInput.AddressLine3);
            storedCustomer.Postcode.Should().Be(_newCustomerInput.Postcode);
            storedCustomer.HomePhone.Should().Be(_newCustomerInput.HomePhone);
            storedCustomer.Mobile.Should().Be(_newCustomerInput.Mobile);
        }

        [Then(@"I should receive the full details of the customer")]
        public void ThenIShouldReceiveTheFullDetailsOfTheCustomer()
        {
            _lastResponse.Response.Should().NotBeNull();
            _storedCustomer.Should().NotBeNull();

            CustomerInfo responseCustomer = _lastResponse.Response.Content.FromJson<CustomerInfo>();

            responseCustomer.CustomerId.Should().Be(_storedCustomer.CustomerId);
            responseCustomer.Title.Should().Be(_storedCustomer.Title);
            responseCustomer.Name.Should().Be(_storedCustomer.Name);
            responseCustomer.AddressLine1.Should().Be(_storedCustomer.AddressLine1);
            responseCustomer.AddressLine2.Should().Be(_storedCustomer.AddressLine2);
            responseCustomer.AddressLine3.Should().Be(_storedCustomer.AddressLine3);
            responseCustomer.Postcode.Should().Be(_storedCustomer.Postcode);
            responseCustomer.HomePhone.Should().Be(_storedCustomer.HomePhone);
            responseCustomer.Mobile.Should().Be(_storedCustomer.Mobile);
        }

        [Then(@"the changes should be made to the customer in the system")]
        public void ThenTheChangesShouldBeMadeToTheCustomerInTheSystem()
        {
            _updatedCustomerInput.Should().NotBeNull();

            CustomerInfo storedCustomer = _actor.AsksFor(StoredCustomer.WithName(_updatedCustomerInput.Name));

            storedCustomer.Title.Should().Be(_updatedCustomerInput.Title);
            storedCustomer.Name.Should().Be(_updatedCustomerInput.Name);
            storedCustomer.AddressLine1.Should().Be(_updatedCustomerInput.AddressLine1);
            storedCustomer.AddressLine2.Should().Be(_updatedCustomerInput.AddressLine2);
            storedCustomer.AddressLine3.Should().Be(_updatedCustomerInput.AddressLine3);
            storedCustomer.Postcode.Should().Be(_updatedCustomerInput.Postcode);
            storedCustomer.HomePhone.Should().Be(_updatedCustomerInput.HomePhone);
            storedCustomer.Mobile.Should().Be(_updatedCustomerInput.Mobile);
        }

        [Then(@"the customer should be removed from the system")]
        public void ThenTheCustomerShouldBeRemovedFromTheSystem()
        {
            _storedCustomer.Should().NotBeNull();

            bool customerPresentInStorage = _actor.AsksFor(StoredCustomerExists.WithId(_storedCustomer.CustomerId));
            customerPresentInStorage.Should().BeFalse();
        }
    }
}
