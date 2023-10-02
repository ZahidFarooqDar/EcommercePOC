using EcommercePOCThirdPartyAPI.Data;
using EcommercePOCThirdPartyAPI.DomainModals;
using Microsoft.EntityFrameworkCore;

namespace EcommercePOCThirdPartyAPI.Controllers.Common
{
    public class DatabaseSeeder<T> where T : DbContext
    {
        #region SetupDatabaseWithSeedData
        /*public void SetupDatabaseWithSeedData(ModelBuilder modelBuilder)
        {
            var defaultCreatedBy = "SeedAdmin";
            SeedDummyData(modelBuilder, defaultCreatedBy);
        }*/
        #endregion SetupDatabaseWithSeedData

        #region SetupDatabaseWithTestData
        public async Task<bool> SetupDatabaseWithTestData(T context)
        {
            var defaultCreatedBy = "SeedAdmin";
            var defaultUpdatedBy = "UpdateAdmin";
            var apiDb = context as ProjectEcommerceContext;
            if (apiDb != null)
            {
                SeedBuyers(apiDb, defaultCreatedBy, defaultUpdatedBy);

                SeedSellers(apiDb, defaultCreatedBy, defaultUpdatedBy);
               
                return true;
            }
            return false;
        }
        #endregion SetupDatabaseWithTestData

        #region Db Tables

        #region Seed Sellers
        private void SeedSellers(ProjectEcommerceContext apiDb, string defaultCreatedBy, string defaultUpdatedBy)
        {
            var sellers = new List<Seller>()
            {
                new Seller()
                {
                    SellerId = Guid.NewGuid().ToString(),
                    Name = "Seller 1",
                    Email = "seller1@gmail.com"

                },
                new Seller()
                {
                    SellerId = Guid.NewGuid().ToString(),
                    Name = "Seller 2",
                    Email = "seller1@gmail.com"
                }
            };
            apiDb.Sellers.AddRange(sellers);
            apiDb.SaveChanges();
        }
        #endregion Seed Sellers

        #region Seed Buyers
        private void SeedBuyers(ProjectEcommerceContext apiDb, string defaultCreatedBy, string defaultUpdatedBy)
        {
            var buyers = new List<Buyer>()
            {
                new Buyer()
                {
                    BuyerId = Guid.NewGuid().ToString(),
                    Name = "Buyer 1",
                    Email = "buyer1@email.com"
                },
                new Buyer()
                {
                    BuyerId = Guid.NewGuid().ToString(),
                    Name = "Buyer 2",
                    Email = "buyer2@email.com"
                }
            };
            apiDb.Buyers.AddRange(buyers);
            apiDb.SaveChanges();
        }
        #endregion Seed Sellers
        #endregion SetupDatabaseWithTestData
    }
}
