using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;


namespace MediTap.Tests.Frontend
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class LoginAndRegisterTests : PageTest
    {
        private IConfiguration _config;

        [OneTimeSetUp]
        public void ClassInit()
        {
            _config = new ConfigurationBuilder()
                .AddUserSecrets<LoginAndRegisterTests>()
                .Build();
        } 
        [Test]
        public async Task UserJourney()
        {
            string adminPassword = _config["TestSettings:AdminPassword"];
            string adminUsername = _config["TestSettings:AdminUsername"];

            if (string.IsNullOrEmpty(adminPassword) || string.IsNullOrEmpty(adminUsername))
                NUnit.Framework.Assert.Fail("Admin Password or Username secret not found! Did you add it to secrets.json?");
           
            await Page.GotoAsync("https://localhost:7001");
            await Page.GetByRole(AriaRole.Link, new() { Name = "Login as Medic" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your username" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your username" }).FillAsync(adminUsername);
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).FillAsync(adminPassword);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Register Patient" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter first name" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter first name" }).FillAsync("luci");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter last name" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter last name" }).FillAsync("chel");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter email" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter email" }).FillAsync("palcau23@gmail.com");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter phone number" }).ClickAsync();
            await Page.GetByText("Register Patient First Name").ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter CNP" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter CNP" }).FillAsync("1870718260516");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter CNP" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter CNP" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter CNP" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter CNP" }).FillAsync("1870718267502");
            await Page.Locator("input[name=\"dateOfBirth\"]").FillAsync("2004-09-17");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter address" }).ClickAsync();
            await Page.GetByText("First Name Last Name Email (").ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter address" }).ClickAsync();
            await Page.Locator(".password-wrapper").ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).FillAsync("luci");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).PressAsync("CapsLock");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).FillAsync("luciPALCAU");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).PressAsync("CapsLock");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).PressAsync("CapsLock");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).PressAsync("CapsLock");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).FillAsync("luciPALCAU04*");
            await Page.Locator("#togglePassword").ClickAsync();
            await Page.Locator("#togglePassword").ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register Patient" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Back to Admin Dashboard" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Register Medic" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Back to Admin Dashboard" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "View Patients" }).ClickAsync();
            await Page.GoBackAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "View Medics" }).ClickAsync();
            await Page.GoBackAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Manage Accounts" }).ClickAsync();
            await Page.GoBackAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Logout" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Login as Patient" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your username" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your username" }).FillAsync("P-Timotei-Medi-91b1d0fe");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter password" }).FillAsync("Odobesti16E");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        }
    }
}
