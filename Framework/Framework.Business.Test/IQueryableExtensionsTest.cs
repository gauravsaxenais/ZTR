namespace ZTR.Framework.Business.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using ZTR.Framework.Business.Test.FixtureSetup.DataAccess.Entities;
    using ZTR.Framework.DataAccess;
    using Xunit;

    public class IQueryableExtensionsTest
    {
        [Fact]
        public void OrderEntitiesByModelsOrder_ShouldReturnEntitiesInTheSameModelOrder_Successful()
        {
            List<MockModelTwo> mockDbData = new List<MockModelTwo>();
            for (int temp = 1; temp < 10; temp++)
            {
                mockDbData.Add(new MockModelTwo()
                {
                    Id = temp,
                    Name = "Name " + temp
                });
            }

            List<MockModel> mockCreateModelData = new List<MockModel>
            {
                new MockModel() { Id = 8, Name = "Name 8" },
                new MockModel() { Id = 9, Name = "Name 9" },
                new MockModel() { Id = 1, Name = "Name 1" },
                new MockModel() { Id = 6, Name = "Name 6" },
                new MockModel() { Id = 7, Name = "Name 7" },
                new MockModel() { Id = 4, Name = "Name 4" },
                new MockModel() { Id = 5, Name = "Name 5" },
                new MockModel() { Id = 2, Name = "Name 2" },
                new MockModel() { Id = 3, Name = "Name 3" }
            };

            var finalModels = mockDbData.AsQueryable().OrderEntitiesByModelsOrder(mockCreateModelData, m => m.Name, m => m.Id, m => m.Name);
            for (int i = 0; i < mockCreateModelData.Count(); i++)
            {
                Assert.True(mockCreateModelData[i].Id == finalModels[i]);
            }
        }
    }
}
