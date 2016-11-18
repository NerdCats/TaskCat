namespace TaskCat.Tests.Lib.UtilityTests
{
    using Common.Utility;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Web;

    [TestFixture(TestOf = typeof(UrlExtensions))]
    public class UrlExtensionsTests
    {
        [Test]
        public void Test_ToQueryString()
        {
            var queryParams = new Dictionary<string, string>()
            {
                { "param1", "value1" },
                { "param2", "value2" }
            };

            var queryString = UrlExtensions.ToQuerystring(queryParams);
            Assert.IsNotNull(queryString);
            Assert.AreEqual("?param1=value1&param2=value2", queryString);
        }

        [Test]
        public void Test_ToQueryString_With_UrlEscaping()
        {
            var queryParams = new Dictionary<string, string>()
            {
                { "param1", "value1&" },
                { "param2", "value2&" }
            };

            var queryString = UrlExtensions.ToQuerystring(queryParams);
            Assert.IsNotNull(queryString);
            var compareUrl = $"?param1={HttpUtility.UrlEncode("value1&")}&param2={HttpUtility.UrlEncode("value2&")}";
            Assert.AreEqual(compareUrl, queryString);
        }
    }
}
