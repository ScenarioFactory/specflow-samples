﻿namespace AutoWorkshop.Specs.Steps
{
    using System;
    using System.Linq;
    using Dto;
    using FluentAssertions;
    using Repositories;
    using TechTalk.SpecFlow;
    using UI;

    [Binding]
    public class CarSteps
    {
        private AutoWorkshopDriver _driver;
        private ChangeCarRegistrationPage _changeCarRegistrationPage;

        [Given(@"this existing car")]
        public void GivenThisExistingCar(Table table)
        {
            var values = table.Rows.Single();

            CarRepository.RemoveByRegistration(values["Registration"]);

            uint customerId = CustomerRepository.GetFirstCustomerId();

            var car = new CarInfo(
                values["Registration"],
                customerId,
                values["Make"],
                values["Model"]);

            CarRepository.Create(car);
        }

        [Given(@"there is no existing car with registration '(.*)'")]
        public void GivenThereIsNoExistingCarWithRegistration(string registration)
        {
            CarRepository.RemoveByRegistration(registration);
        }

        [When(@"I change the registration of '(.*)' to '(.*)'")]
        public void WhenIChangeTheRegistrationOfTo(string currentRegistration, string newRegistration)
        {
            _driver = AutoWorkshopDriver.CreateAuthenticatedInstance();
            _changeCarRegistrationPage = new ChangeCarRegistrationPage(_driver);

            _changeCarRegistrationPage.ChangeRegistration(currentRegistration, newRegistration);
        }

        [Then(@"I should see the message '(.*)'")]
        public void ThenIShouldSeeTheMessage(string expectedMessage)
        {
            _changeCarRegistrationPage.Should().NotBeNull();

            string successMessage = _changeCarRegistrationPage.GetSuccessMessage();

            successMessage.Should().Be(expectedMessage);
        }

        [Then(@"the following car should be present in the system")]
        public void ThenTheFollowingCarShouldBePresentInTheSystem(Table table)
        {
            var expectedValues = table.Rows.Single();

            CarInfo storedCar = CarRepository.GetInfoByRegistration(expectedValues["Registration"]);

            storedCar.Should().NotBeNull();
            storedCar.Registration.Should().Be(expectedValues["Registration"]);
            storedCar.Make.Should().Be(expectedValues["Make"]);
            storedCar.Model.Should().Be(expectedValues["Model"]);
        }

        [Then(@"there should be no car with registration '(.*)'")]
        public void ThenThereShouldBeNoCarWithRegistration(string registration)
        {
            CarInfo storedCar = CarRepository.GetInfoByRegistration(registration);

            storedCar.Should().BeNull();
        }
    }
}
