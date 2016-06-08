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
    using Data.Entity.Identity;
    using System.Collections.Generic;
    using TaskCat.Model.Pagination;
    using System.Net.Http;
    using System.Linq;
    [TestFixture]
    public class DropPointControllerTest
    {
        private Mock<ClaimsIdentity> ClaimsIdentityMock;
        private DropPointController Controller;
        private DropPoint DropPoint;
        private Mock<IDropPointService> DropPointServiceMock;
        private Mock<IPrincipal> IIPrincipalMock;

        private string testUserId = "123456";
        private string testDropPointName = "test drop point";
        private DefaultAddress testAddress = new DefaultAddress("test_formatted_address", new Point(2, 1));
        private string testUserName = "testUserName";
        private string testAuthType = "test_auth_type";

        [SetUp]
        public void Setup()
        {
            this.DropPointServiceMock = new Mock<IDropPointService>();
            this.IIPrincipalMock = new Mock<IPrincipal>();
            this.ClaimsIdentityMock = new Mock<ClaimsIdentity>();

            this.Controller = new DropPointController(DropPointServiceMock.Object);
        }

        [Test]
        public async Task Test_Post_DropPoint_As_User()
        {
            DropPoint = new DropPoint(
                testUserId,
                testDropPointName,
                testAddress);

            DropPointServiceMock.Setup(x => x.Insert(It.IsAny<DropPoint>())).Returns<DropPoint>((x) => Task.FromResult(x));

            SetupAuth();
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_ADMINISTRATOR)).Returns(false);
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)).Returns(false);

            Controller.User = IIPrincipalMock.Object;

            var result = await Controller.Post(DropPoint);
            Assert.IsInstanceOf<JsonResult<DropPoint>>(result);

            var convertedResult = result as JsonResult<DropPoint>;
            Assert.IsNotNull(convertedResult.Content);
            Assert.AreEqual(DropPoint.Address, convertedResult.Content.Address);
            Assert.AreEqual(DropPoint.Name, convertedResult.Content.Name);
            Assert.AreEqual(DropPoint.UserId, convertedResult.Content.UserId);
        }

        [Test]
        public async Task Test_Post_DropPoint_As_User_With_Other_UserId()
        {
            DropPoint = new DropPoint(
                "456789",
                testDropPointName,
                testAddress);

            DropPointServiceMock.Setup(x => x.Insert(It.IsAny<DropPoint>())).Returns<DropPoint>((x) => Task.FromResult(x));

            SetupAuth();
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_ADMINISTRATOR)).Returns(false);
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)).Returns(false);

            Controller.User = IIPrincipalMock.Object;

            var result = await Controller.Post(DropPoint);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Test_Post_DropPoint_As_Administrator_With_Other_UserId()
        {
            DropPoint = new DropPoint(
                "456789",
                testDropPointName,
                testAddress);

            DropPointServiceMock.Setup(x => x.Insert(It.IsAny<DropPoint>()))
                .Returns<DropPoint>((x) => Task.FromResult(x));

            SetupAuth();
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_ADMINISTRATOR)).Returns(true);
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)).Returns(true);

            Controller.User = IIPrincipalMock.Object;
            var result = await Controller.Post(DropPoint);
            Assert.IsInstanceOf<JsonResult<DropPoint>>(result);

            var convertedResult = result as JsonResult<DropPoint>;
            Assert.IsNotNull(convertedResult.Content);
            Assert.AreEqual(DropPoint.Address, convertedResult.Content.Address);
            Assert.AreEqual(DropPoint.Name, convertedResult.Content.Name);
            Assert.AreEqual(DropPoint.UserId, convertedResult.Content.UserId);
        }

        [Test]
        public async Task Test_Search_DropPoint_As_User()
        {
            DropPoint = new DropPoint(
                testUserId,
                testDropPointName,
                testAddress);
            var dropPoints = new List<DropPoint>() { DropPoint };

            DropPointServiceMock.Setup(x => x.SearchDropPoints(testUserId, It.IsAny<string>())).ReturnsAsync(dropPoints);

            SetupAuth();
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_ADMINISTRATOR)).Returns(false);
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)).Returns(false);

            Mock<HttpRequestMessage> httpRequestMock = new Mock<HttpRequestMessage>();

            Controller.User = IIPrincipalMock.Object;
            Controller.Request = httpRequestMock.Object;

            var result = await Controller.Search("test_query", testUserId);
            Assert.IsInstanceOf<JsonResult<PageEnvelope<DropPoint>>>(result);
            Assert.IsNotNull(result);

            var convertedResult = result as JsonResult<PageEnvelope<DropPoint>>;
            Assert.AreEqual(1, convertedResult.Content.data.Count());
            Assert.AreEqual(DropPoint, convertedResult.Content.data.First());
        }

        [Test]
        public async Task Test_Search_DropPoint_As_Administrator_With_Other_UserId()
        {
            DropPoint = new DropPoint(
                "23456",
                testDropPointName,
                testAddress);
            var dropPoints = new List<DropPoint>() { DropPoint };

            DropPointServiceMock.Setup(x => x.SearchDropPoints(testUserId, It.IsAny<string>()))
                .ReturnsAsync(dropPoints);

            SetupAuth();
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_ADMINISTRATOR)).Returns(true);
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)).Returns(true);

            Mock<HttpRequestMessage> httpRequestMock = new Mock<HttpRequestMessage>();

            Controller.User = IIPrincipalMock.Object;
            Controller.Request = httpRequestMock.Object;

            var result = await Controller.Search("test_query", testUserId);
            Assert.IsInstanceOf<JsonResult<PageEnvelope<DropPoint>>>(result);
            
            var convertedResult = result as JsonResult<PageEnvelope<DropPoint>>;
            Assert.IsNotNull(convertedResult);
            Assert.AreEqual(1, convertedResult.Content.data.Count());
            Assert.AreEqual(DropPoint, convertedResult.Content.data.First());
        }

        [Test]
        public async Task Test_Search_DropPoint_As_User_With_Other_UserId()
        {
            DropPoint = new DropPoint(
                "23456",
                testDropPointName,
                testAddress);
            var dropPoints = new List<DropPoint>() { DropPoint };

            DropPointServiceMock.Setup(x => x.SearchDropPoints(testUserId, It.IsAny<string>()))
                .ReturnsAsync(dropPoints);

            SetupAuth();
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_ADMINISTRATOR)).Returns(false);
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)).Returns(false);

            Mock<HttpRequestMessage> httpRequestMock = new Mock<HttpRequestMessage>();

            Controller.User = IIPrincipalMock.Object;
            Controller.Request = httpRequestMock.Object;

            var result = await Controller.Search("test_query", "23456");
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Test_Update_DropPoint_As_User()
        {
            DropPoint = new DropPoint(
                testUserId,
                testDropPointName,
                testAddress)
            {
                Id = "test_dropPoint_id"
            };
            var dropPoints = new List<DropPoint>() { DropPoint };

            DropPointServiceMock.Setup(x => x.Update(DropPoint, testUserId))
                .Returns<DropPoint, string>((x, id) => Task.FromResult(x));

            SetupAuth();
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_ADMINISTRATOR)).Returns(false);
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)).Returns(false);

            Mock<HttpRequestMessage> httpRequestMock = new Mock<HttpRequestMessage>();

            Controller.User = IIPrincipalMock.Object;
            Controller.Request = httpRequestMock.Object;

            var result = await Controller.Put(DropPoint);
            Assert.IsInstanceOf<JsonResult<DropPoint>>(result);
            
            var convertedResult = result as JsonResult<DropPoint>;
            Assert.IsNotNull(convertedResult.Content);
            Assert.AreEqual(DropPoint.Address, convertedResult.Content.Address);
            Assert.AreEqual(DropPoint.UserId, convertedResult.Content.UserId);
            Assert.AreEqual("test_dropPoint_id", convertedResult.Content.Id);
            Assert.AreEqual(DropPoint.Name, convertedResult.Content.Name);
        }

        [Test]
        public async Task Test_Update_DropPoint_As_User_With_Other_UserId()
        {
            DropPoint = new DropPoint(
                "23456",
                testDropPointName,
                testAddress)
            {
                Id = "test_dropPoint_id"
            };
            var dropPoints = new List<DropPoint>() { DropPoint };

            DropPointServiceMock.Setup(x => x.Update(DropPoint, testUserId))
                .Returns<DropPoint, string>((x, id) => Task.FromResult(x));

            SetupAuth();
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_ADMINISTRATOR)).Returns(false);
            IIPrincipalMock.Setup(x => x.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)).Returns(false);

            Mock<HttpRequestMessage> httpRequestMock = new Mock<HttpRequestMessage>();

            Controller.User = IIPrincipalMock.Object;
            Controller.Request = httpRequestMock.Object;

            var result = await Controller.Put(DropPoint);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        private void SetupAuth()
        {
            ClaimsIdentityMock.As<IIdentity>();
            ClaimsIdentityMock.SetupGet(x => x.AuthenticationType).Returns(testAuthType);
            ClaimsIdentityMock.SetupGet(x => x.IsAuthenticated).Returns(true);
            ClaimsIdentityMock.SetupGet(x => x.Name).Returns(testUserName);
            ClaimsIdentityMock.Setup(x => x.FindFirst(It.IsAny<string>())).Returns(new Claim(testAuthType, testUserId));
            IIPrincipalMock.SetupGet(x => x.Identity).Returns(ClaimsIdentityMock.Object);
        }
    }
}
