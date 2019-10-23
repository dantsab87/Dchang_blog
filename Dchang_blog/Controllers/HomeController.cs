using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dchang_blog.Helpers;
using Dchang_blog.Models;
using PagedList;
using PagedList.Mvc;

namespace Dchang_blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 3; // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);

            return View(blogList.ToPagedList(pageNumber, pageSize));

            //var listPosts = db.BlogPosts.AsQueryable();
            //return View(listPosts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

        public IQueryable<BlogPost> IndexSearch(string searchStr) 
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                                      p.BlogPostBody.Contains(searchStr) ||
                                      p.Comments.Any(c => c.CommentBody.Contains(searchStr) ||
                                                     c.Author.FirstName.Contains(searchStr) ||
                                                     c.Author.LastName.Contains(searchStr) ||
                                                     c.Author.DisplayName.Contains(searchStr) ||
                                                     c.Author.Email.Contains(searchStr)));
            }
            else 
            {
                result = db.BlogPosts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await EmailHelper.ComposeEmailAsync(model);
                    return View(new EmailModel());

                    //var to = ConfigurationManager.AppSettings["emailfrom"];
                    //var from = $"{model.FromEmail}<{to}>";

                    //var email = new MailMessage(from, to)
                    //{
                    //    Subject = model.Subject,
                    //    Body = $"You have received an email from your Portfolio <br/> {model.Body}",
                    //    IsBodyHtml = true
                    //};

                    //var svc = new PersonalEmail();
                    //await svc.SendAsync(email);

                    //return View(new EmailModel());
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                    Debug.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }
    }
}