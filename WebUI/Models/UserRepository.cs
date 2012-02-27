using System;
using System.Collections.Generic;
using System.Linq;
using FluentCassandra;
using FluentCassandra.Types;

namespace WebUI.Models
{
	public class UserRepository
	{
		 public void Add(User user)
		 {
		 	using (var db = new CassandraContextFactory().Get())
		 	{
		 		var family = db.GetColumnFamily<UTF8Type>("users");

		 		dynamic record = family.CreateRecord(user.Id);
		 		record.username = user.Username;
				
				db.Attach(record);
				db.SaveChanges();
		 	}
		 }

		public IEnumerable<User> GetAll()
		{
			using (var db = new CassandraContextFactory().Get())
			{
				var users = db.ExecuteQuery("select * from users");
				foreach (dynamic user in users)
				{
					yield return new User
					             	{
					             		Username = user.username
					             	};
				}
			}
		}

		public User Get(string username)
		{
			using (var db = new CassandraContextFactory().Get())
			{
				var users = db.ExecuteQuery("select * from users where username='"+username+"'");
				dynamic user = users.First();
				return new User { Username = user.Username, Id = user.Key};
			}
		}

		public User Get(Guid id)
		{
			using (var db = new CassandraContextFactory().Get())
			{
				var users = db.ExecuteQuery("select * from users where key='" + id + "'");
				dynamic user = users.First();
				return new User { Username = user.Username, Id = user.Key };
			}
		}
	}
}