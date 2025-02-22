using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskMgtSysMVC.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult AssignTask()
        {
            return View();
        }
        public ActionResult UpdateTask()
        {
            return View();
        }
        public ActionResult ProjectManagement()
        {
            return View();
        }
        public ActionResult UserProductivity()
        {
            return View();
        }
    }
}