using System;
using System.Collections.Generic;
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

				dynamic record = family.CreateRecord(tweet.Date);
				record.UserId = tweet.UserId;
				record.Text = tweet.Text;
				record.Date = tweet.Date;

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
					query = "select * from tweets where UserID='" + user.Id + "'";
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
					             		Date = tweet.Date,
					             		Text = tweet.Text,
					             		UserId = tweet.UserId,
									//	UserName = repo.Get().Username
					             	});
				}
				return tw;
			}
			
		}
	}
}