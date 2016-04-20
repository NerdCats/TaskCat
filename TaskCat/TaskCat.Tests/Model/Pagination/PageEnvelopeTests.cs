namespace TaskCat.Model.Pagination.Tests.Model.Pagination
{
    using Lib.Utility;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using TaskCat.Tests.TestData;

    [TestFixture(TestOf = typeof(PageEnvelope<TestEntity>))]
    public class PageEnvelopeTests
    {
        List<TestEntity> TestDataCollection;
        string TestRoute = "/test/route";
        int Page = 0;
        int PageSize = 10;

        Dictionary<string, string> OtherParams;

        [SetUp]
        public void Setup()
        {
            TestDataCollection = new List<TestEntity>();
            for (int i = 0; i < 5; i++)
            {
                TestDataCollection.Add(new TestEntity());
            }

            OtherParams = new Dictionary<string, string>();
            OtherParams["param1"] = "paramvalue1";
            OtherParams["param2"] = "paramvalue2";
        }

        [Test]
        public void Test_PageEnvelope_Creation()
        {
            Mock<IPagingHelper> pagingHelperMock = new Mock<IPagingHelper>();
            pagingHelperMock.Setup(x => x.GeneratePageUrl(TestRoute, Page, PageSize, OtherParams)).Returns(string.Concat(TestRoute, "?", "param1=paramvalue1", "&", "param2=paramvalue2"));

            PageEnvelope<TestEntity> pageEnvelope = new PageEnvelope<TestEntity>(TestDataCollection.Count, Page, PageSize, TestRoute, TestDataCollection, new HttpRequestMessage(), OtherParams);
            pageEnvelope.paginationHelper = pagingHelperMock.Object;
            Assert.IsNotNull(pageEnvelope.data);
            Assert.That(pageEnvelope.data.Count() == 5);
            Assert.IsNotNull(pageEnvelope.pagination);
            Assert.That(pageEnvelope.pagination.Total == pageEnvelope.data.Count());
            Assert.AreEqual(Page, pageEnvelope.pagination.Page);
            Assert.AreEqual(Page * PageSize + 1, pageEnvelope.pagination.Start);
            Assert.AreEqual(PageSize, pageEnvelope.pagination.PageSize);
            Assert.AreEqual(TestDataCollection.Count(), pageEnvelope.pagination.Returned);
            Assert.AreEqual((int)Math.Ceiling((double)TestDataCollection.Count / PageSize), pageEnvelope.pagination.TotalPages);

            // TODO: #11 Page URL generation is not tested yet, we need a way to test that
        }


    }
}