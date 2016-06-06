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
        private Mock<ClaimsIdentity> ClaimsIdentityMock;
        private DropPointController Controller;
        private DropPoint DropPoint;
        private Mock<IDropPointService> DropPointServiceMock;
        private Mock<IPrincipal> IIPrincipalMock;

        [SetUp]
        public void Setup()
        {
            this.DropPoint = new DropPoint(
                "123456",
                "test_name",
                new DefaultAddress(
                    "test_formatted_address",
                    new Point(2, 1)));

            this.DropPointServiceMock = new Mock<IDropPointService>();
            this.IIPrincipalMock = new Mock<IPrincipal>();
            this.ClaimsIdentityMock = new Mock<ClaimsIdentity>();

            this.Controller = new DropPointController(DropPointServiceMock.Object);
        }

        [Test]
        public async Task Test_Post_DropPoint_As_User()
        {
            DropPointServiceMock.Setup(x => x.Insert(It.IsAny<DropPoint>())).ReturnsAsync(DropPoint);

            ClaimsIdentityMock.As<IIdentity>();
            ClaimsIdentityMock.SetupGet(x => x.AuthenticationType).Returns("test_auth_type");
            ClaimsIdentityMock.SetupGet(x => x.IsAuthenticated).Returns(true);
            ClaimsIdentityMock.SetupGet(x => x.Name).Returns("test_username");
            ClaimsIdentityMock.Setup(x => x.FindFirst(It.IsAny<string>())).Returns(new Claim("test_auth_type", "123456"));

            IIPrincipalMock.SetupGet(x => x.Identity).Returns(ClaimsIdentityMock.Object);
            IIPrincipalMock.Setup(x => x.IsInRole("Administrator")).Returns(false);
            IIPrincipalMock.Setup(x => x.IsInRole("BackOfficeAdmin")).Returns(false);

            Controller.User = IIPrincipalMock.Object;

            var result = await Controller.Post(DropPoint);
            Assert.IsInstanceOf<JsonResult<DropPoint>>(result);

            var convertedResult = result as JsonResult<DropPoint>;
            Assert.AreEqual(DropPoint.Address, convertedResult.Content.Address);
            Assert.AreEqual(DropPoint.Name, convertedResult.Content.Name);
            Assert.AreEqual(DropPoint.UserId, convertedResult.Content.UserId);
        }
    }
}
