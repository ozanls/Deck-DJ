﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Owin.Security;
using DeckDJ.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Diagnostics;
namespace DeckDJ.Controllers
{
    public class DeckController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static DeckController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44357/api/");
        }

        // GET: Deck/New
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();
            return View();
        }

        // POST: Deck/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Deck deck)
        {
            GetApplicationCookie();
            string url = "DeckData/AddDeck";

            string jsonpayload = jss.Serialize(deck);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

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

        // GET: Deck/List
        public ActionResult List()
        {
            GetApplicationCookie();

            AdminDecks ViewModel = new AdminDecks();

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            string url = "DeckData/ListDecks";
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine(response.Content.ReadAsStringAsync().Result.ToString());

            ViewModel.Decks = response.Content.ReadAsAsync<IEnumerable<DeckDto>>().Result;

            return View(ViewModel);
        }

        //GET: Deck/Details/1
        public ActionResult Details(int id)
        {
            GetApplicationCookie();

            AdminDecks ViewModel = new AdminDecks();

            

            string url = "DeckData/FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin") || User.Identity.GetUserId() == selectedDeck.UserId) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            ViewModel.Deck = selectedDeck;

            url = "ComboPieceData/GetCombosInDeck/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ComboPieceDto> comboPieces = response.Content.ReadAsAsync<IEnumerable<ComboPieceDto>>().Result;

            ViewModel.ComboPieces = comboPieces;

            url = "AudioData/ListAudioForDeck/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<AudioDto> audios = response.Content.ReadAsAsync<IEnumerable<AudioDto>>().Result;

            ViewModel.Audios = audios;

            url = "AudioData/ListAudioNotForDeck/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<AudioDto> otherAudios = response.Content.ReadAsAsync<IEnumerable<AudioDto>>().Result;

            ViewModel.OtherAudios = otherAudios;

            return View(ViewModel);
        }

        //GET: Deck/Edit/1
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();
            string url = "DeckData/FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            if (selectedDeck.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Details/" + id);
            }

            return View(selectedDeck);
        }

        //POST: Deck/Update/1
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Deck deck)
        {
            GetApplicationCookie();

            string url = "DeckData/FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            if (selectedDeck.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Details/" + id);
            }

            url = "DeckData/UpdateDeck/" + id;
            string jsonpayload = jss.Serialize(deck);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details/" + id);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        //POST: Deck/Associate/{DeckId}/{AudioId}
        [HttpPost]
        [Authorize]
        public ActionResult Associate(int id, int AudioId)
        {
            GetApplicationCookie();

            string url = "DeckData/FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            if (selectedDeck.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Details/" + id);
            }
            //call our api to associate deck with audio
            url = "DeckData/AssociateDeckwithAudio/" + id + "/" + AudioId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //POST: Deck/Unassociate/{DeckId}/{AudioId}
        [HttpGet]
        [Authorize]
        public ActionResult Unassociate(int id, int AudioId)
        {
            GetApplicationCookie();

            string url = "DeckData/FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            if (selectedDeck.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Details/" + id);
            }
            //call our api to unassociate deck with audio
            url = "DeckData/UnassociateDeckwithAudio/" + id + "/" + AudioId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        // GET: Deck/Delete/1
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            string url = "DeckData/FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;
            if (selectedDeck.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Details/" + id);
            }
            return View(selectedDeck);

        }

        // POST: Deck/Delete/1
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();

            string url = "DeckData/FindDeck/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            DeckDto selectedDeck = response.Content.ReadAsAsync<DeckDto>().Result;

            if (selectedDeck.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Details/" + id);
            }
            url = "DeckData/DeleteDeck/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            response = client.PostAsync(url, content).Result;

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
