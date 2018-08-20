using System;
using Xunit;
using SpoonCMS.Classes;

namespace SpoonCMSTests
{
    public class ContainerTests
    {
        [Fact]
        public void ValidContainerValidContentItemTest()
        {
            var Container = new Container();
            Container.AddItem(new ContentItem("existingItem"));

            var contentItem = Container.GetItem("existingItem");
            var expectedItem = typeof(ContentItem);

            Assert.IsType(expectedItem, contentItem);
        }

        [Fact]
        public void ValidContainerInvalidContentItemTest()
        {
            var Container = new Container();

            var contentItem = Container.GetItem("nonExistingItem");
            var expectedItem = typeof(NotFoundContentItem);

            Assert.IsType(expectedItem, contentItem);
        }
    }
}
