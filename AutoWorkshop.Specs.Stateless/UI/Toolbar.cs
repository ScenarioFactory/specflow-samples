﻿namespace AutoWorkshop.Specs.Stateless.UI
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using OpenQA.Selenium;

    public class Toolbar
    {
        private readonly AutoWorkshopDriver _driver;

        public Toolbar(AutoWorkshopDriver driver)
        {
            _driver = driver;
        }

        public ToolbarButton[] Buttons
        {
            get
            {
                ReadOnlyCollection<IWebElement> anchors = _driver.FindElements(By.XPath("//fieldset//a"));

                return anchors
                    .Select(anchor => new ToolbarButton(anchor))
                    .ToArray();
            }
        }
    }
}
