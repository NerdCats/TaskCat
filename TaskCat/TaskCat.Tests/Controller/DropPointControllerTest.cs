namespace TaskCat.Tests.Controller
{
    using Data.Entity;
    using Data.Model.Geocoding;
    using Data.Model.GeoJson;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using TaskCat.Controller;
    using TaskCat.Lib.DropPoint;
    using System.Web.Http.Results;
    using System.Security.Claims;

    [TestFixture]
    public class DropPointControllerTest
    {
        [Test]
        public async Task Test_Post_DropPoint()
        {
            DropPoint dropPoint = new DropPoint(
                "123456",
                "test_name",
                new DefaultAddress(
                    "test_formatted_address",
                    new Point(2, 1)));

            Mock<IDropPointService> dropPointServiceMock = new Mock<IDropPointService>();
            dropPointServiceMock.Setup(x => x.Insert(It.IsAny<DropPoint>())).ReturnsAsync(dropPoint);

            var IIPrincipalMock = new Mock<IPrincipal>();
            var ClaimsIdentityMock = new Mock<ClaimsIdentity>();
            ClaimsIdentityMock.As<IIdentity>();
            ClaimsIdentityMock.SetupGet(x => x.AuthenticationType).Returns("test_auth_type");
            ClaimsIdentityMock.SetupGet(x => x.IsAuthenticated).Returns(true);
            ClaimsIdentityMock.SetupGet(x => x.Name).Returns("test_username");
            ClaimsIdentityMock.Setup(x => x.FindFirst(It.IsAny<string>())).Returns(new Claim("test_auth_type", "123456"));

            IIPrincipalMock.SetupGet(x => x.Identity).Returns(ClaimsIdentityMock.Object);
            IIPrincipalMock.Setup(x => x.IsInRole("Administrator")).Returns(false);
            IIPrincipalMock.Setup(x => x.IsInRole("BackOfficeAdmin")).Returns(false);

            DropPointController controller = new DropPointController(dropPointServiceMock.Object);
            controller.User = IIPrincipalMock.Object;

            var result = await controller.Post(dropPoint);
            Assert.IsInstanceOf<JsonResult<DropPoint>>(result);

            var convertedResult = result as JsonResult<DropPoint>;
            Assert.AreEqual(dropPoint.Address, convertedResult.Content.Address);
            Assert.AreEqual(dropPoint.Name, convertedResult.Content.Name);
            Assert.AreEqual(dropPoint.UserId, convertedResult.Content.UserId);
        }
    }
}
