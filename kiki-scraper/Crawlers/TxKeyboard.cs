using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using kiki.shared;
using HtmlAgilityPack;

namespace kiki.scraper.Crawlers {
    public class TxKeyboard : Crawler {
        public TxKeyboard(KiKiDBContext dBContext, int interval) : base(dBContext, interval, "txkeyboard") {
        }

        protected override async Task CrawlAll() {
            Logger.StartCrawlAll();
            var (postCount, allNewPosts) = await CrawlPage(1);
            for (var i = 2; i <= postCount; i++) {
                await CrawlPage(i);
            }
            Logger.FinishCrawlAll();
        }

        protected override async Task CrawlRecent() {
            Logger.StartCrawlRecent();
            var i = 1;
            while (true) {
                var (postCount, allNewPosts) = await CrawlPage(i);
                if (postCount < i || !allNewPosts) {
                    break;
                }
                i += 1;
            }
            Logger.FinishCrawlRecent();
        }

        private async Task<(int pageCount, bool allNewPosts)> CrawlPage(int page) {
            Logger.StartCrawlPage(page);
            var doc = await Html.ParseGetAsync(BuildNoticePageUrl(page));
            var posts = doc.DocumentNode.SelectNodes("//div[contains(@class, 'list_text_title')]");
            var pageCount = doc.DocumentNode.SelectNodes("//ul[contains(@class, 'pagination')]/li/a[not(contains(@class, 'disabled'))]").Count;
            var allNewWithoutNotice = true;
            Logger.PostFoundAtPage(page, posts.Count);
            foreach (var post in posts) {
                try {
                    var a = post.SelectSingleNode("li[contains(@class, 'tit')]/a");
                    var href = a.GetAttributeValue("href", "");
                    var idx = HttpUtility.ParseQueryString(href).Get("idx");
                    var notice = a.SelectSingleNode("em[contains(@class, 'notice')]");
                    var isNotice = !notice.GetAttributeValue("style", "").Contains("none");
                    var title = a.SelectSingleNode("span").InnerText.Trim();
                    var author = post.SelectSingleNode("li[contains(@class, 'author')]").InnerText.Trim();
                    var time = DateTime.Parse(post.SelectSingleNode("li[@class='time']").GetAttributeValue("title", "").Trim());
                    var views = int.Parse(post.SelectSingleNode("li[contains(@class, 'views')]").ChildNodes[2].InnerText.Trim());
                    var commentCount = int.Parse(post.SelectSingleNode("span[contains(@class, 'comment-count')]/span").InnerText.Trim());

                    Logger.Debug($"post: {idx} {isNotice} {author} {time} {views} {commentCount}");

                    if (!await NeedToUpdatePost(idx, commentCount)) {
                        Logger.SkipPost(idx);
                        if (!isNotice) {
                            allNewWithoutNotice = false;
                        }
                        continue;
                    }

                    var postUrl = BuildNoticePostUrl(idx);
                    var contentHtml = await Html.ParseGetAsync(postUrl);
                    var content = ParseContent(contentHtml);
                    var comments = ParseComments(contentHtml);

                    var p = new Post {
                        Community = communityId,
                        ScrappedDate = DateTime.Now,
                        Url = postUrl,
                        PostId = idx,
                        Title = title,
                        Author = author,
                        WrittenDate = time,
                        Content = content,
                        Views = views,
                        CommentCount = commentCount,
                        Comments = comments,
                    };

                    var isNewPost = await PostFound(p);
                    if (!isNotice) {
                        allNewWithoutNotice = allNewWithoutNotice && isNewPost;
                    }
                } catch (Exception ex) {
                    Logger.PostParseError(ex);
                }
            }
            Logger.FinishCrawlPage(page);
            return (pageCount, allNewWithoutNotice);
        }

        private static string BuildNoticePageUrl(int page) {
            return $"https://txkeyboard.com/21/?page={page}";
        }

        private static string BuildNoticePostUrl(string idx) {
            return $"https://txkeyboard.com/21/?bmode=view&idx={idx}";
        }

        private static string ParseContent(HtmlDocument html) {
            var texts = html.DocumentNode.SelectNodes("//div[contains(@class, 'board_txt_area')]//p");
            return string.Join('\n', texts.Nodes().Select(p => p.InnerText));
        }

        private static Comment[] ParseComments(HtmlDocument html) {
            var root = html.DocumentNode.SelectSingleNode("//div[@id='comment_container']");
            return ParseComments(root);
        }

        private static Comment[] ParseComments(HtmlNode node) {
            var comments = node.SelectNodes("div[contains(@class, 'comment')]");
            if (comments == null || comments.Count == 0) {
                return new Comment[0];
            }
            var result = new List<Comment>();
            foreach (var c in comments) {
                var main = c.SelectSingleNode("div[contains(@class, 'main_comment')]");
                var subRoot = c.SelectSingleNode("div[contains(@class, 'sub_comment')]//div[contains(@class, 'comment_comment_wrap')]");
                var subComments = subRoot == null ? new Comment[0] : ParseComments(subRoot);

                if (main == null) {
                    result.Add(new Comment {
                        Deleted = true,
                        SubComments = subComments,
                    });
                    continue;
                }

                var header = main.SelectSingleNode(".//div[contains(@class, 'write')]");
                var author = header.ChildNodes[0].InnerText.Trim();
                var time = DateTime.Parse(header.SelectSingleNode("span").InnerText.Trim());
                var content = main.SelectSingleNode(".//div[contains(@class, 'comment_area')]/div").InnerText.Trim();

                result.Add(new Comment {
                    Deleted = false,
                    Author = author,
                    WrittenDate = time,
                    Content = content,
                    SubComments = subComments,
                });
            }

            return result.ToArray();
        }
    }
}