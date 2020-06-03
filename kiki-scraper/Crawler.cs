using System.Threading.Tasks;
using System.Timers;
using kiki.shared;
using MongoDB.Bson;

namespace kiki.scraper {
    public abstract class Crawler {
        public int Interval;

        protected Logger Logger;

        private KiKiDBContext db;
        protected ObjectId communityId;
        private string communityName;
        private Timer timer;

        public Crawler(KiKiDBContext dBContext, int interval, string communityName) {
            Interval = interval;
            db = dBContext;
            this.communityName = communityName;

            Logger = new Logger(communityName);

            timer = new Timer(interval);
            timer.Elapsed += async (a, b) => await CrawlRecent();
        }

        public async Task Init() {
            var community = await db.GetCommunityOfName(communityName);
            communityId = community.Id;
        }

        public void Start() {
            timer.Start();
            CrawlRecent().ConfigureAwait(false);
        }

        public void Stop() {
            timer.Stop();
        }

        public void ManualCrawl() {
            CrawlAll().ConfigureAwait(false);
        }

        protected abstract Task CrawlAll();

        protected abstract Task CrawlRecent();

        protected async Task<bool> NeedToUpdatePost(string postId, int commentCount) {
            var exist = await db.GetPostOfPostId(communityId, postId);
            return exist == null || exist.CommentCount != commentCount;
        }

        protected async Task<bool> PostFound(Post post) {
            Logger.PostFound(post);
            var exist = await db.GetPostOfPostId(communityId, post.PostId);
            if (exist != null) {
                Logger.UpdatePost(post);
                post.Id = exist.Id;
                await db.UpdatePost(post);
                return false;
            } else {
                Logger.AddPost(post);
                await db.AddPost(post);
                return true;
            }
        }
    }
}