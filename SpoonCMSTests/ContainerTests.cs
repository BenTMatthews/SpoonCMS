using System;
using Xunit;
using SpoonCMS.Classes;
using System.Collections.Generic;
using SpoonCMS.Exceptions;

namespace SpoonCMSTests
{
    public class ContainerTests
    {
        [Fact]
        public void ValidDefaultContainerDefaultValuesTest() {
            var container = new Container();

            Assert.True(container.Active);
            Assert.InRange(container.Created, DateTime.Now.AddMinutes(-1), DateTime.Now);
            Assert.Empty(container.Items);
            Assert.NotNull(container.Items);
        }

        [Fact]
        public void ValidNamedContainerDefaultValuesTest()
        {
            var container = new Container("Container");

            Assert.True(container.Active);
            Assert.InRange(container.Created, DateTime.Now.AddMinutes(-1), DateTime.Now);
            Assert.Empty(container.Items);
            Assert.NotNull(container.Items);
            Assert.Equal("Container", container.Name);
        }

        [Fact]
        public void ValidContainerItemDictionarySetTest()
        {
            var container = new Container();
            var itemList = new Dictionary<string,ContentItem>();
            itemList.Add("existingItem", new ContentItem("existingItem"));

            container.Items = itemList;

            Assert.NotEmpty(container.Items);
            Assert.Equal("existingItem", container.GetItem().Name);
        }

        [Fact]
        public void ValidContainerMaximumContentItemsTest()
        {
            var container = new Container();
            var random = new Random(999999999);

            for (int i = 0; i < 100; i++)
            {
                var item = new ContentItem(random.Next().ToString());
                container.AddItem(item);
            }

            Action test = () => { container.AddItem(new ContentItem("lastItemTest")); };

            var exception = Record.Exception(test);
            Assert.NotNull(exception);
            Assert.IsType<CountExceededException>(exception);
        }

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

        [Fact]
        public void ValidContainerValidContentHighestPriorityItemTest()
        {
            var Container = new Container();
            Container.AddItem(new ContentItem("existingItem") { Priority = 100 });
            Container.AddItem(new ContentItem("otherExistingItem") { Priority = 50 });

            var contentItem = Container.GetItem();

            Assert.Equal("otherExistingItem", contentItem.Name);
        }
        
        [Fact]
        public void ValidContainerSetNewContentItemTest()
        {
            var Container = new Container();
            Container.AddItem(new ContentItem("existingItem") { Value = "Initial value" });

            Container.SetItem("existingItem", new ContentItem { Value = "New value" });

            var contentItem = Container.GetItem();
            Assert.Equal("New value", contentItem.Value);
        }
    }
}
