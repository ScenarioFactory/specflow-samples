﻿namespace AutoWorkshop.Specs.Screenplay.Steps
{
    using System.Linq;
    using Database;
    using Database.Questions;
    using Database.Tasks;
    using Dto;
    using Extensions;
    using FluentAssertions;
    using Framework;
    using Pages;
    using Pattern;
    using TechTalk.SpecFlow;
    using WebDriver;
    using WebDriver.Questions;
    using WebDriver.Tasks;

    [Binding]
    public class CustomerUiSteps
    {
        private readonly IActor _actor;
        private CustomerUiViewInfo _uiViewInfo;
        private CustomerInfo _storedCustomer;

        public CustomerUiSteps(AppSettings appSettings, AutoWorkshopDriver driver)
        {
            _actor = new Actor().WhoCan(
                UseAutoWorkshop.With(driver),
                UseMySqlDatabase.With(appSettings.MySqlConnectionString));
        }

        [Given(@"this existing customer")]
        public void GivenThisExistingCustomer(Table table)
        {
            var values = table.Rows.Single();

            _actor.AttemptsTo(DeleteCustomers.WithName(values["Name"]));

            _storedCustomer = new CustomerInfo(
                values["Title"],
                values["Name"],
                values["Address Line 1"],
                values["Address Line 2"],
                values["Address Line 3"],
                values["Postcode"],
                values["Home Phone"],
                values["Mobile"],
                1);

            _actor.AttemptsTo(
                InsertCustomer.Named(values["Name"])
                    .Titled(values["Title"])
                    .OfAddress(
                        values["Address Line 1"],
                        values["Address Line 2"],
                        values["Address Line 3"],
                        values["Postcode"])
                    .WithHomePhone(values["Home Phone"])
                    .WithMobile(values["Mobile"]));
        }

        [When(@"I create a new customer with the following details")]
        public void WhenICreateANewCustomerWithTheFollowingDetails(Table table)
        {
            var values = table.Rows.Single();

            _uiViewInfo = new CustomerUiViewInfo(
                values["Title"],
                values["Name"],
                values["Address Line 1"],
                values["Address Line 2"],
                values["Address Line 3"],
                values["Postcode"],
                values["Home Phone"],
                values["Mobile"]);

            _actor.AttemptsTo(
                Navigate.To(CustomerMaintenancePage.Path),
                CreateCustomer.Named(values["Name"])
                    .Titled(values["Title"])
                    .OfAddress(
                        values["Address Line 1"],
                        values["Address Line 2"],
                        values["Address Line 3"],
                        values["Postcode"])
                    .WithHomePhone(values["Home Phone"])
                    .WithMobile(values["Mobile"]));
        }

        [When(@"I view the customer")]
        public void WhenIViewTheCustomer()
        {
            _storedCustomer.Should().NotBeNull();

            int customerId = _actor.AsksFor(StoredCustomerId.ForName(_storedCustomer.Name));

            _actor.AttemptsTo(ViewCustomer.WithId(customerId));
        }

        [When(@"I update the customer with a new mobile number of '(.*)'")]
        public void WhenIUpdateTheCustomerWithANewMobileNumberOf(string newMobileNumber)
        {
            _storedCustomer.Should().NotBeNull();

            int customerId = _actor.AsksFor(StoredCustomerId.ForName(_storedCustomer.Name));

            _actor.AttemptsTo(
                ViewCustomer.WithId(customerId),
                SendKeys.To(CustomerMaintenancePage.Mobile, newMobileNumber),
                Click.On(CustomerMaintenancePage.Save));
        }

        [When(@"I search for '(.*)'")]
        public void WhenISearchFor(string searchText)
        {
            _actor.AttemptsTo(
                Navigate.To(CustomerMaintenancePage.Path),
                SendKeys.To(CustomerMaintenancePage.Name, searchText).OneKeyAtATime());
        }

        [When(@"I select the option to create a new car for the customer")]
        public void WhenISelectTheOptionToCreateANewCarForTheCustomer()
        {
            _actor.AttemptsTo(ClickToolbarButton.WithAltText("Add a new car"));
        }

        [Then(@"the customer should be added to the system with the details provided")]
        public void ThenTheCustomerShouldBeAddedToTheSystemWithTheDetailsProvided()
        {
            _uiViewInfo.Should().NotBeNull();

            _storedCustomer = _actor.AsksFor(StoredCustomer.WithName(_uiViewInfo.Name));

            _storedCustomer.Should().NotBeNull();
            _storedCustomer.Title.Should().Be(_uiViewInfo.Title);
            _storedCustomer.Name.Should().Be(_uiViewInfo.Name);
            _storedCustomer.AddressLine1.Should().Be(_uiViewInfo.AddressLine1);
            _storedCustomer.AddressLine2.Should().Be(_uiViewInfo.AddressLine2);
            _storedCustomer.AddressLine3.Should().Be(_uiViewInfo.AddressLine3);
            _storedCustomer.Postcode.Should().Be(_uiViewInfo.Postcode);
            _storedCustomer.HomePhone.Should().Be(_uiViewInfo.HomePhone);
            _storedCustomer.Mobile.Should().Be(_uiViewInfo.Mobile);
        }

        [Then(@"the customer should be marked as manually invoiced")]
        public void ThenTheCustomerShouldBeMarkedAsManuallyInvoiced()
        {
            _storedCustomer.Should().NotBeNull();

            _storedCustomer.HasAccountInvoicing.Should().BeFalse();
        }

        [Then(@"I should see the stored customer details")]
        public void ThenIShouldSeeTheStoredCustomerDetails()
        {
            _storedCustomer.Should().NotBeNull();

            _actor.AsksFor(SelectedOptionText.Of(CustomerMaintenancePage.Title)).Should().Be(_storedCustomer.Title);
            _actor.AsksFor(Value.Of(CustomerMaintenancePage.Name)).Should().Be(_storedCustomer.Name);
            _actor.AsksFor(Value.Of(CustomerMaintenancePage.AddressLine1)).Should().Be(_storedCustomer.AddressLine1);
            _actor.AsksFor(Value.Of(CustomerMaintenancePage.AddressLine2)).Should().Be(_storedCustomer.AddressLine2);
            _actor.AsksFor(Value.Of(CustomerMaintenancePage.AddressLine3)).Should().Be(_storedCustomer.AddressLine3);
            _actor.AsksFor(Value.Of(CustomerMaintenancePage.Postcode)).Should().Be(_storedCustomer.Postcode);
            _actor.AsksFor(Value.Of(CustomerMaintenancePage.HomePhone)).Should().Be(_storedCustomer.HomePhone);
            _actor.AsksFor(Value.Of(CustomerMaintenancePage.Mobile)).Should().Be(_storedCustomer.Mobile);
        }

        [Then(@"I should see the following toolbar options")]
        public void ThenIShouldSeeTheFollowingToolbarOptions(Table table)
        {
            ToolbarButtonInfo[] toolbarButtons = _actor.AsksFor(ToolbarButtons.WhichAreVisible());

            table.Rows.ForEach(row =>
            {
                bool buttonIsVisible = toolbarButtons.Any(b => b.AltText == row["Option"]);
                buttonIsVisible.Should().BeTrue($"button for '{row["Option"]}' should be visible");
            });
        }

        [Then(@"the stored customer should be updated with mobile '(.*)'")]
        public void ThenTheStoredCustomerShouldBeUpdatedWithMobile(string expectedMobileNumber)
        {
            _storedCustomer.Should().NotBeNull();

            var latestStoredCustomer = _actor.AsksFor(StoredCustomer.WithName(_storedCustomer.Name));

            latestStoredCustomer.Mobile.Should().Be(expectedMobileNumber);
        }

        [Then(@"I should see the customer in the list of as-you-type results")]
        public void ThenIShouldSeeTheCustomerInTheListOfAsYouTypeResults()
        {
            _storedCustomer.Should().NotBeNull();

            bool foundExpectedCustomerInSearchResults = Poller.PollForSuccess(() =>
            {
                string[] searchResults = _actor.AsksFor(AsYouTypeSearchResults.Of(CustomerMaintenancePage.AsYouTypeSearchResults));
                return searchResults.Contains(_storedCustomer.Name);
            });

            foundExpectedCustomerInSearchResults.Should().BeTrue();
        }
    }
}
