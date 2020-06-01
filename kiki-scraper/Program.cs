using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using kiki.scraper.Crawlers;
using kiki.shared;

namespace kiki.scraper {
    class Program {
        static async Task Main(string[] args) {
            var builders = new Dictionary<string, Func<KiKiDBContext, int, Crawler>> {
                ["tx"] = (db, interval) => new TxKeyboard(db, interval),
            };

            var db = new KiKiDBContext();
            var interval = 60;
            var crawlers = new List<Crawler>();
            foreach (var arg in args) {
                if (!builders.TryGetValue(arg, out var builder)) {
                    Console.WriteLine($"model not found: {arg}");
                    return;
                }
                crawlers.Add(builder(db, interval));
            }

            foreach (var crawler in crawlers) {
                await crawler.Init();
            }

            foreach (var crawler in crawlers) {
                crawler.ManualCrawl();
            }

            Console.ReadLine();
        }
    }
}