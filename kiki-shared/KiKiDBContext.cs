using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace kiki.shared {
    public class KiKiDBContext {
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<Community> communities;
        private readonly IMongoCollection<Post> posts;

        private static FilterDefinitionBuilder<Community> CommunityFilter => Builders<Community>.Filter;
        private static FilterDefinitionBuilder<Post> PostFilter => Builders<Post>.Filter;

        public KiKiDBContext() {
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            db = client.GetDatabase("KiKiDB");
            communities = db.GetCollection<Community>("Communities");
            posts = db.GetCollection<Post>("Posts");
        }

        public async Task<List<Community>> GetAllCommunities() {
            return await FilterCommunities(CommunityFilter.Empty);
        }

        public async Task<Community> GetCommunityOfName(string name) {
            var result = await FilterCommunity(CommunityFilter.Eq(c => c.Name, name));
            if (result == null) {
                await AddCommunity(new Community { Name = name });
                return await GetCommunityOfName(name);
            }
            return result;
        }

        public async Task AddCommunity(Community community) {
            await communities.InsertOneAsync(community);
        }

        public async Task RemoveCommunity(string name) {
            await communities.DeleteOneAsync(CommunityFilter.Eq(p => p.Name, name));
        }

        public async Task<List<Post>> GetAllPosts() {
            return await FilterPosts(PostFilter.Empty);
        }

        public async Task<Post> GetPostOfPostId(ObjectId communityId, string postId) {
            return await FilterPost(PostFilter.Eq(p => p.PostId, postId) & PostFilter.Eq(p => p.Community, communityId));
        }

        public async Task<List<Post>> GetPostsOfCommunity(string community) {
            var communityId = (await GetCommunityOfName(community)).Id;
            return await FilterPosts(PostFilter.Eq(p => p.Community, communityId));
        }

        public async Task AddPost(Post post) {
            await posts.InsertOneAsync(post);
        }

        public async Task RemovePost(string postId) {
            await posts.DeleteOneAsync(PostFilter.Eq(p => p.PostId, postId));
        }

        public async Task UpdatePost(Post post) {
            var filter = PostFilter.Eq(p => p.PostId, post.PostId);
            await posts.ReplaceOneAsync(filter, post);
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
            return await docs.SingleOrDefaultAsync();
        }
    }
}