using System.Threading.Tasks;
using System.Timers;
using kiki.shared;
using MongoDB.Bson;

namespace kiki.scraper {
    public abstract class Crawler {
        public int Interval;

        private KiKiDBContext db;
        protected ObjectId communityId;
        private string communityName;
        private Timer timer;

        public Crawler(KiKiDBContext dBContext, int interval, string communityName) {
            Interval = interval;
            db = dBContext;
            this.communityName = communityName;

            timer = new Timer(interval);
            timer.Elapsed += async (a, b) => await Crawl();
        }

        public async Task Init() {
            var community = await db.GetCommunityOfName(communityName);
            communityId = community.Id;
        }

        public void Start() {
            timer.Start();
        }

        public void Stop() {
            timer.Stop();
        }

        public void ManualCrawl() {
            Crawl().ConfigureAwait(false);
        }

        protected abstract Task Crawl();

        protected async Task PostFound(Post post) {
            System.Console.WriteLine($"post {post.PostId} found");
            var exist = await db.GetPostOfPostId(post.PostId);
            if (exist != null) {
                System.Console.WriteLine("post updated");
                post.Id = exist.Id;
                await db.UpdatePost(post);
            } else {
                System.Console.WriteLine("post add");
                await db.AddPost(post);
            }
        }
    }
}