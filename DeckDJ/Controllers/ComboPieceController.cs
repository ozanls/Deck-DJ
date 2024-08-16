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
using Microsoft.AspNet.Identity;

namespace DeckDJ.Controllers
{
    public class ComboPieceController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ComboPieceController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44357/api/");
        }

        private bool isOwner(int id)
        {
            string url = "ComboPieceData/FindComboPiece/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ComboPieceDto selectedComboPiece = response.Content.ReadAsAsync<ComboPieceDto>().Result;
            
            url = "DeckData/FindDeck/" + selectedComboPiece.DeckId;
            response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            if (selectedDeck.UserId != User.Identity.GetUserId())
            {
                return false;
            }
            return true;
        }

        // GET: ComboPiece/New
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();
            return View();
        }

        // POST: ComboPiece/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(ComboPiece comboPiece)
        {
            string url = "DeckData/FindDeck/" + comboPiece.DeckId;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            if (selectedDeck.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("List"); ;
            }

            url = "ComboPieceData/AddComboPiece";
            if (!isOwner(comboPiece.DeckId))
            {
                return RedirectToAction("List");
            }

            string jsonpayload = jss.Serialize(comboPiece);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return Error();
            }
        }

        // GET: ComboPiece/List
        public ActionResult List()
        {
            GetApplicationCookie();
            string url = "ComboPieceData/ListComboPiece";
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result.ToString());

            IEnumerable<ComboPieceDto> comboPieces = response.Content.ReadAsAsync<IEnumerable<ComboPieceDto>>().Result;

            return View(comboPieces);
        }

        // GET: ComboPiece/Details/1
        public ActionResult Details(int id)
        {
            string url = "ComboPieceData/FindComboPiece/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            ComboPieceDto selectedComboPiece = response.Content.ReadAsAsync<ComboPieceDto>().Result;

            return View(selectedComboPiece);
        }

        // GET: ComboPiece/Edit/1
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();
            string url = "ComboPieceData/FindComboPiece/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ComboPieceDto selectedComboPiece = response.Content.ReadAsAsync<ComboPieceDto>().Result;
            if (!isOwner(id))
            {
                return RedirectToAction("Details/"+id);
            }

            return View(selectedComboPiece);
        }

        // POST: ComboPiece/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, ComboPiece comboPiece)
        {
            GetApplicationCookie();
            if (!isOwner(id))
            {
                return RedirectToAction("Details/" + id);
            }
            string url = "ComboPieceData/UpdateComboPiece/" + id;
            string jsonpayload = jss.Serialize(comboPiece);
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

        // GET: ComboPiece/Delete/1
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            if (!isOwner(id))
            {
                return RedirectToAction("Details/" + id);
            }
            string url = "ComboPieceData/FindComboPiece/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ComboPieceDto selectedComboPiece = response.Content.ReadAsAsync<ComboPieceDto>().Result;
            return View(selectedComboPiece);

        }

        // POST: ComboPiece/Delete/1
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            if (!isOwner(id))
            {
                return RedirectToAction("Details/" + id);
            }
            string url = "ComboPieceData/DeleteComboPiece/" + id;
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

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
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
    }
}
