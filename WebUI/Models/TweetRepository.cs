using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentCassandra.Types;

namespace WebUI.Models
{
	public class Tweet
	{
		public string Text { get; set; }
		public Guid UserId { get; set; }
		public DateTime Date { get; set; }

		public string UserName { get; set; }
	}

	public class TweetRepository
	{
		public void Create(Tweet tweet)
		{
			using (var db = new CassandraContextFactory().Get())
			{
				var family = db.GetColumnFamily("tweets");

				var tweetId = Guid.NewGuid();
				dynamic record = family.CreateRecord(tweetId);
				record.userid = tweet.UserId;
				record.text = tweet.Text;
				record.date = tweet.Date;

				Regex reg = new Regex(@"((mailto\:|(news|(ht|f)tp(s?))\://){1}\S+)");
				var match = reg.Match(tweet.Text);
				if (match.Success)
				{
					string url = match.Value;
					var urlFamily = db.GetColumnFamily("urls");

					var urlRow = urlFamily.CreateRecord(url);
					urlRow.TrySetColumn(tweet.UserId, tweetId);

					db.Attach(urlRow);
					
					var userUrlFamily = db.GetSuperColumnFamily("user_urls");
					dynamic userUrlRow = userUrlFamily.CreateRecord(tweet.UserId);
					db.Attach(userUrlRow);

					var superColumn = userUrlRow.CreateSuperColumn();
					superColumn.TrySetColumn(tweetId, "");

					userUrlRow[url] = superColumn;
				}

				db.Attach(record);
				db.SaveChanges();
			}
		}

		public IEnumerable<Tweet> GetAll(string username = null)
		{
			using (var db = new CassandraContextFactory().Get())
			{
				string query = string.Empty;
				if (username != null)
				{
					var user = new UserRepository().Get(username);
					query = "select * from tweets where userid='" + user.Id + "'";
				} else
				{
					query = "select * from tweets";
				}

				var tweets = db.ExecuteQuery(query);
				var repo = new UserRepository();
				List<Tweet> tw = new List<Tweet>();
				foreach (dynamic tweet in tweets)
				{
					int x = 4;
					var id = tweet.UserId;
					tw.Add( new Tweet
					             	{
					             		Date = tweet.date,
					             		Text = tweet.text,
					             		UserId = tweet.userid,
									//	UserName = repo.Get().Username
					             	});
				}
				return tw;
			}
			
		}
	}
}