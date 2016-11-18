namespace TaskCat.Tests.Account
{
    using App.Settings;
    using Data.Entity.Identity;
    using Data.Model.Identity;
    using Data.Model.Identity.Profile;
    using Data.Model.Identity.Registration;
    using Its.Configuration;
    using Microsoft.AspNet.Identity;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Common.Email;
    using TaskCat.Lib.Auth;
    using TaskCat.Lib.Db;
    using TaskCat.Lib.Job;
    using Common.Storage;

    [TestFixture]
    public class AccountContextTest
    {
        Mock<IDbContext> dbContextMock = new Mock<IDbContext>();
        Mock<IEmailService> mailServiceMock = new Mock<IEmailService>();
        Mock<IUserStore<User>> userStoreMock = new Mock<IUserStore<User>>();
        Mock<AccountManager> accountManagerMock;
        Mock<IBlobService> blobServiceMock = new Mock<IBlobService>();
        Mock<IJobManager> jobManagerMock = new Mock<IJobManager>();

        [SetUp]
        public void SetUp()
        {
            Settings.Reset();
            Settings.Set<ClientSettings>(new ClientSettings()
            {
                WebCatUrl = "http://testwebcat.com/",
                ConfirmEmailPath = "confirm"
            });
            Settings.CertificatePassword = null;
        }

        [Test]
        public async Task Test_RegisterUser_With_Single_Interested_Locality()
        {
            accountManagerMock = new Mock<AccountManager>(userStoreMock.Object);

            IdentityResult identityResult = new IdentityResult(null);
            accountManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(identityResult);

            var accountContext = new AccountContext(
                dbContextMock.Object,
                mailServiceMock.Object,
                accountManagerMock.Object,
                blobServiceMock.Object,
                jobManagerMock.Object);

            var registerModel = new UserRegistrationModel()
            {
                UserName = "testUsername",
                Type = IdentityTypes.USER,
                Password = "testPassword",
                ConfirmPassword = "testPassword",
                Email = "testUsername@testmail.com",
                PhoneNumber = "+88017100000",
                InterestedLocalities = new List<string>() {
                    "TestLocality"
                }
            };

            var result = await accountContext.RegisterUser(registerModel);

            Assert.NotNull(result.User);
            Assert.AreEqual("testUsername", result.User.UserName);
            Assert.AreEqual(IdentityTypes.USER, result.User.Type);
            Assert.AreEqual("testUsername@testmail.com", result.User.Email);
            Assert.AreEqual(false, result.User.EmailConfirmed);
            Assert.AreEqual("+88017100000", result.User.PhoneNumber);
            Assert.AreEqual(false, result.User.PhoneNumberConfirmed);
            Assert.NotNull(result.User.Profile);
            Assert.AreEqual(typeof(UserProfile), result.User.Profile.GetType());
            Assert.NotNull((result.User.Profile as UserProfile).InterestedLocalities);
            Assert.AreEqual(1, (result.User.Profile as UserProfile).InterestedLocalities.Count);
            Assert.AreEqual("TestLocality", (result.User.Profile as UserProfile).InterestedLocalities.First());
        }

        [Test]
        public async Task Test_NotifyUserCreationByMail()
        {
            accountManagerMock = new Mock<AccountManager>(userStoreMock.Object);

            mailServiceMock.Setup(x => x.SendWelcomeMail(
                It.IsAny<SendWelcomeEmailRequest>())).ReturnsAsync(
                new SendEmailResponse(HttpStatusCode.OK));

            var accountContext = new AccountContext(
                dbContextMock.Object,
                mailServiceMock.Object,
                accountManagerMock.Object,
                blobServiceMock.Object,
                jobManagerMock.Object);

            Environment.SetEnvironmentVariable("Its.Configuration.Settings.Precedence", "local|production");

            var userMock = new Mock<User>(new RegistrationModelBase() { UserName = "test_username" });

            var result = await accountContext.NotifyUserCreationByMail(userMock.Object);
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
