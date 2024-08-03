using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DeckDJ.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeckDJ.Controllers
{
    public class AudioController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AudioController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44357/api/");
        }


        // GET: Audio/List 
        public ActionResult List()
        {
            // curl api/AudioData/ListAudios

            string url = "audiodata/listaudios";

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<AudioDto> Audios = response.Content.ReadAsAsync<IEnumerable<AudioDto>>().Result;

            return View(Audios);
        }

        //GET: Audio/Details/1
        [Authorize]
        public ActionResult Details(int id)
        {

            DetailsAudio ViewModel = new DetailsAudio();

            string url = "AudioData/FindAudio/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            AudioDto selectedAudio = response.Content.ReadAsAsync<AudioDto>().Result;

            ViewModel.Audio = selectedAudio;

            url = "DeckData/ListDecksForAudio/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<DeckDto> audios = response.Content.ReadAsAsync<IEnumerable<DeckDto>>().Result;

            ViewModel.Decks = audios;

            url = "DeckData/ListDecksNotForAudio/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<DeckDto> otherAudios = response.Content.ReadAsAsync<IEnumerable<DeckDto>>().Result;

            ViewModel.OtherDecks = otherAudios;

            return View(ViewModel);
        }

        // POST: Audio/Create
        [HttpPost]
        public ActionResult Create(Audio audio)
        {
            Debug.WriteLine("the json payload is:");

            string url = "audiodata/addaudio";
            string jsonpaylod = jss.Serialize(audio);

            Debug.WriteLine(jsonpaylod);

            HttpContent content =new StringContent(jsonpaylod);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url,content).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        

        // GET: Audio/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "audiodata/findaudio/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            AudioDto selectedaudio = response.Content.ReadAsAsync<AudioDto>().Result;

            return View(selectedaudio);
        }

        // POST: Audio/Update/5
        [HttpPost]
        public ActionResult Update(int id, Audio audio)
        {
            try
            {
                Debug.WriteLine("The new audio info is:");
                Debug.WriteLine(audio.AudioName);
                Debug.WriteLine(audio.AudioURL);
                Debug.WriteLine(audio.AudioLength);
                Debug.WriteLine(audio.AudioTimestamp);
                Debug.WriteLine(audio.AudioStreams);
                Debug.WriteLine(audio.AudioUploaderId);
                Debug.WriteLine(audio.CategoryId);

                // serialize into JSON, send the request to the API

                string url = "audiodata/updateaudio/" + id;

                string jsonpayload = jss.Serialize(audio);
                Debug.WriteLine(jsonpayload);

                HttpContent content = new StringContent(jsonpayload);

                //POST: api/audiodata/updateaudio/{id}
                content.Headers.ContentType.MediaType = "application/json";
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                Debug.WriteLine(content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details/" + id);
                }
                else
                {
                    return RedirectToAction("Error");
                }
                
            }

            catch
            {
                return View();
            }
        }

        // GET: Audio/New
        public ActionResult New()
        {
            string url = "https://localhost:44357/api/categorydata/listcategories";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CategoryDto> categories = JsonConvert.DeserializeObject<IEnumerable<CategoryDto>>(response.Content.ReadAsStringAsync().Result);
            return View(categories);

        }


        // GET: Audio/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "audiodata/findaudio/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AudioDto selectedaudio = response.Content.ReadAsAsync<AudioDto>().Result;
            return View(selectedaudio);
        }


        // POST: Audio/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                string url = "audiodata/deleteaudio/" + id;
                HttpContent content = new StringContent("");
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                return RedirectToAction("List");
            }
            catch
            {
                return View();
            }
        }

    }
}