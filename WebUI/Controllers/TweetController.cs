using System;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
	public class TweetController : Controller
	{
		 public ActionResult Create(string text, string username)
		 {
		 	var user = new UserRepository().Get(username);
		 	new TweetRepository().Create(new Tweet
		 	                             	{
		 	                             		Date = DateTime.UtcNow,
												Text = text,
												UserId = user.Id
		 	                             	});
		 	return Redirect("/");
		 }
	}
}