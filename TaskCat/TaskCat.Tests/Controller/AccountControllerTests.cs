namespace TaskCat.Tests.Controller
{
    using Moq;
    using NUnit.Framework;
    using TaskCat.Controller.Auth;
    using Data.Entity.Identity;
    using TaskCat.Lib.Auth;
    using Data.Model.Identity.Registration;
    using Data.Model.Identity.Profile;
    using System.Net.Http;
    using TaskCat.Lib.Email;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http.Results;

    [TestFixture]
    public class AccountControllerTests
    {

        [Test]
        public async Task Test_ResendConfirmationEmail_With_SucessfulResult()
        {
            Mock<IAccountContext> accountContextMock = new Mock<IAccountContext>();
            accountContextMock.Setup(x => x.FindUser(It.IsAny<string>())).ReturnsAsync(
                new User(new UserRegistrationModel(), new UserProfile()));
            accountContextMock.Setup(x => x.NotifyUserCreationByMail(It.IsAny<User>(), It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new SendEmailResponse(HttpStatusCode.OK, null));

            AccountController accountController = new AccountController(accountContextMock.Object);
            var result = await accountController.ResendConfirmationEmail("123");

            Assert.IsInstanceOf<JsonResult<SendEmailResponse>>(result);
            Assert.IsNotNull(result);

            JsonResult<SendEmailResponse> convertedResult = result as JsonResult<SendEmailResponse>;
            Assert.IsNotNull(convertedResult.Content);

            Assert.IsTrue(convertedResult.Content.Success);
            Assert.AreEqual(HttpStatusCode.OK, convertedResult.Content.StatusCode);
        }

        [Test]
        public async Task Test_ResendConfirmationEmail_With_FailedResult()
        {
            Mock<IAccountContext> accountContextMock = new Mock<IAccountContext>();
            accountContextMock.Setup(x => x.FindUser(It.IsAny<string>())).ReturnsAsync(
                new User(new UserRegistrationModel(), new UserProfile()));
            accountContextMock.Setup(x => x.NotifyUserCreationByMail(It.IsAny<User>(), It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new SendEmailResponse(HttpStatusCode.InternalServerError, "Random mail error"));

            AccountController accountController = new AccountController(accountContextMock.Object);
            var result = await accountController.ResendConfirmationEmail("123");

            Assert.IsInstanceOf<FormattedContentResult<SendEmailResponse>>(result);
            Assert.IsNotNull(result);

            FormattedContentResult<SendEmailResponse> convertedResult = result as FormattedContentResult<SendEmailResponse>;
            Assert.IsNotNull(convertedResult.Content);

            Assert.IsFalse(convertedResult.Content.Success);
            Assert.AreEqual(HttpStatusCode.InternalServerError, convertedResult.Content.StatusCode);
        }
    }
}
