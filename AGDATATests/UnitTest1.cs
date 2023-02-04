using AGDATA.Controllers;
using AGDATA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace AGDATATests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly IMemoryCache _memoryCache;
        [TestMethod]
        public async void TestMethod1()
        {
            var mockSet = new Mock<DbSet<Person>>();

            var mockContext = new Mock<PersonContext>();
            mockContext.Setup(m => m.Persons).Returns(mockSet.Object);

            var personsController = new PersonsController(mockContext.Object, _memoryCache);
            await personsController.PostPerson(new Person());

            mockSet.Verify(m => m.Add(It.IsAny<Person>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }


    }
}