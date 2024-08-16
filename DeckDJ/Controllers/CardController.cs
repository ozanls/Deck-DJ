using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DeckDJ.Models;

namespace DeckDJ.Controllers
{
    public class CardController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CardController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44357/api/CardData/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Card/New
        [Authorize(Roles ="Admin")]
        public ActionResult New()
        {
            return View();
        }

        // POST: Card/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Card card)
        {
            
            string url = "AddCard";
        
            string jsonpayload = jss.Serialize(card);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            GetApplicationCookie();

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return Error();
            }
        }

        // GET: Card/List?PageNum={PageNum}
        public ActionResult List(int PageNum = 0)
        {
            ListCards ViewModel = new ListCards();

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            string url = "GetCardCount";
            HttpResponseMessage response = client.GetAsync(url).Result;

            int CardCount = response.Content.ReadAsAsync<int>().Result;
            int PerPage = 12;
            int MaxPage = (int)Math.Ceiling((decimal)CardCount / PerPage) - 1;

            if(MaxPage < 0) MaxPage = 0;
            if(PageNum < 0) PageNum = 0;
            if(PageNum > MaxPage) PageNum = MaxPage;

            int StartIndex = PerPage * PageNum;

            ViewModel.PageNum = PageNum;
            ViewModel.PageSummary = " " + (PageNum + 1) + " of " + (MaxPage + 1) + " ";

            url = "ListCardsPage/" + StartIndex + "/"+PerPage;
            response = client.GetAsync(url).Result;

            IEnumerable<CardDto> cards = response.Content.ReadAsAsync<IEnumerable<CardDto>>().Result;

            ViewModel.Cards = cards;

            return View(ViewModel);
        }

        // GET: Card/Details/1
        public ActionResult Details(int id)
        {
            string url = "FindCard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            CardDto selectedCard = response.Content.ReadAsAsync<CardDto>().Result;

            return View(selectedCard);
        }

        // GET: Card/Edit/1
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            string url = "FindCard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CardDto selectedCard = response.Content.ReadAsAsync<CardDto>().Result;
            return View(selectedCard);
        }

        // POST: Card/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Card card)
        {

            string url = "UpdateCard/" + id;
            string jsonpayload = jss.Serialize(card);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details/" +id);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Card/DeleteConfirm/1
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindCard/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CardDto selectedCard = response.Content.ReadAsAsync<CardDto>().Result;
            return View(selectedCard);

        }

        // POST: Card/Delete/1
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            string url = "DeleteCard/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
