using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace kiki.shared {
    public class KiKiDBContext {
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<Community> communities;
        private readonly IMongoCollection<Post> posts;

        private static FilterDefinitionBuilder<Community> CommunityFilter => Builders<Community>.Filter;
        private static FilterDefinitionBuilder<Post> PostFilter => Builders<Post>.Filter;

        public KiKiDBContext() {
            var client = new MongoClient("mongo://127.0.0.1:27017");
            db = client.GetDatabase("KiKiDB");
            communities = db.GetCollection<Community>("Communities");
            posts = db.GetCollection<Post>("Posts");
        }

        public async Task<List<Community>> GetAllCommunities() {
            return await FilterCommunities(CommunityFilter.Empty);
        }

        public async Task<Community> GetCommunityOfId(int id) {
            return await FilterCommunity(CommunityFilter.Eq(c => c.Id, id));
        }

        public async Task AddCommunity(Community community) {
            await communities.InsertOneAsync(community);
        }

        public async Task RemoveCommunity(int id) {
            await communities.DeleteOneAsync(CommunityFilter.Eq(p => p.Id, id));
        }

        public async Task<List<Post>> GetAllPosts() {
            return await FilterPosts(PostFilter.Empty);
        }

        public async Task<Post> GetPostOfId(int id) {
            return await FilterPost(PostFilter.Eq(c => c.Id, id));
        }

        public async Task<List<Post>> GetPostsOfCommunity(int id) {
            return await FilterPosts(PostFilter.Eq(p => p.Community, id));
        }

        public async Task AddPost(Post post) {
            await posts.InsertOneAsync(post);
        }

        public async Task RemovePost(int id) {
            await posts.DeleteOneAsync(PostFilter.Eq(p => p.Id, id));
        }

        private async Task<List<Community>> FilterCommunities(FilterDefinition<Community> filter) {
            var docs = await communities.FindAsync(filter);
            return await docs.ToListAsync();
        }

        private async Task<Community> FilterCommunity(FilterDefinition<Community> filter) {
            var docs = await communities.FindAsync(filter);
            return await docs.FirstOrDefaultAsync();
        }

        private async Task<List<Post>> FilterPosts(FilterDefinition<Post> filter) {
            var docs = await posts.FindAsync(filter);
            return await docs.ToListAsync();
        }

        private async Task<Post> FilterPost(FilterDefinition<Post> filter) {
            var docs = await posts.FindAsync(filter);
            return await docs.FirstOrDefaultAsync();
        }
    }
}