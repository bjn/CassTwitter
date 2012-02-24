using System;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
	public class UserController : Controller
	{
		 public ActionResult Create(string username)
		 {
		 	User user = new User();
		 	user.Id = Guid.NewGuid();
		 	user.Username = username;
			new UserRepository().Add(user);
		 	return RedirectToAction("Index");
		 }

		public ActionResult Index()
		{
			return View();
		}
	}
}