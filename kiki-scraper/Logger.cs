using System;
using kiki.shared;

namespace kiki.scraper {
    public class Logger {
        public readonly string Name;

        public Logger(string name) {
            Name = name;
        }

        public void Debug(string content) {
            LogDebug(content);
        }

        public void StartCrawlAll() {
            LogInfo("Start Crawl All");
        }

        public void FinishCrawlAll() {
            LogInfo("Finished Crawl All");
        }

        public void StartCrawlRecent() {
            LogInfo("Start Crawl Recent");
        }

        public void FinishCrawlRecent() {
            LogInfo("Finished Crawl Recent");
        }

        public void StartCrawlPage(int page) {
            LogInfo($"Start Crawl Page {page}");
        }

        public void FinishCrawlPage(int page) {
            LogInfo($"Finished Crawl Page {page}");
        }

        public void PostFoundAtPage(int page, int postCount) {
            LogInfo($"{postCount} posts found at page {page}");
        }

        public void SkipPost(string postId) {
            LogInfo($"Post skipped: {postId}");
        }

        public void PostFound(Post post) {
            LogInfo($"Post found: {post.PostId}");
        }

        public void UpdatePost(Post post) {
            LogInfo($"Post updated: {post.PostId}");
        }

        public void AddPost(Post post) {
            LogInfo($"Post add: {post.PostId}");
        }

        public void PostParseError(Exception exception) {
            LogError("Parse Error");
            LogError(exception.ToString());
        }

        private void LogDebug(string content) => Log("DEBG", content);
        private void LogInfo(string content) => Log("INFO", content);
        private void LogWarning(string content) => Log("WARN", content);
        private void LogError(string content) => Log("ERRR", content);

        private void Log(string status, string content) {
            var namePadded = $"[{Name}]";
            var timeStamp = DateTime.Now.ToString("s");
            Console.WriteLine($"[{timeStamp}] {namePadded,6} [{status}] {content}");
        }
    }
}