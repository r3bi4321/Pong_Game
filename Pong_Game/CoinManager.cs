using MongoDB.Driver;

namespace Pong_Game.Helpers
{
    public static class CoinManager
    {
        private static readonly MongoClient client = new MongoClient("mongodb://localhost:27017");
        private static readonly IMongoDatabase database = client.GetDatabase("PongDB");
        private static readonly IMongoCollection<User> users = database.GetCollection<User>("Login");
        private static readonly IMongoCollection<StoreItem> storeItems = database.GetCollection<StoreItem>("StoreItems");

        public static void AddCoinsToUser(string username, int amount)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var update = Builders<User>.Update.Inc(u => u.Coins, amount);
            users.UpdateOne(filter, update);
        }

        public static bool TryBuyItem(string username, string itemTag, int price)
        {
            var filterUser = Builders<User>.Filter.Eq(u => u.Username, username);
            var user = users.Find(filterUser).FirstOrDefault();

            if (user != null && user.Coins >= price)
            {
                var updateUser = Builders<User>.Update.Inc(u => u.Coins, -price);
                users.UpdateOne(filterUser, updateUser);

                var filterItem = Builders<StoreItem>.Filter.And(
                    Builders<StoreItem>.Filter.Eq(i => i.Username, username),
                    Builders<StoreItem>.Filter.Eq(i => i.Tag, itemTag)
                );
                var updateItem = Builders<StoreItem>.Update.Set(i => i.Bought, true);
                storeItems.UpdateOne(filterItem, updateItem);

                return true;
            }
            return false;
        }

        public static int GetUserCoins(string username)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var user = users.Find(filter).FirstOrDefault();
            return user?.Coins ?? 0;
        }
    }
}
