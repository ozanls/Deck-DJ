using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DeckDJ.Models;

namespace DeckDJ.Controllers
{
    public class CategoryController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CategoryController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44357/api/categorydata/");
        }

        // GET: Category/List 
        public ActionResult List()
        {
            // curl api/CategoryData/ListCategories

            string url = "listcategories";

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<CategoryDto> Categories = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;

            return View(Categories);
        }

        // GET: Category/Show/{id}
        public ActionResult Show(int id)
        {
            // curl api/CategoryData/FindCategory/{id}

            string url = "findcategory/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            CategoryDto Category = response.Content.ReadAsAsync<CategoryDto>().Result;

            // Views/Category/Show.cshtml
            return View(Category);
        }

        // POST: Category/Create
        [HttpPost]
        public ActionResult Create(Category category)
        {
            Debug.WriteLine("the json payload is:");

            string url = "addcategory";
            string jsonpaylod = jss.Serialize(category);

            Debug.WriteLine(jsonpaylod);

            HttpContent content = new StringContent(jsonpaylod);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("List");
            }
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "findcategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            CategoryDto selectedcategory = response.Content.ReadAsAsync<CategoryDto>().Result;

            return View(selectedcategory);
        }

        // POST: Category/Update/5
        [HttpPost]
        public ActionResult Update(int id, Category category)
        {
            try
            {
                Debug.WriteLine("The new category info is:");
                Debug.WriteLine(category.CategoryId);
                Debug.WriteLine(category.CategoryName);
                // serialize into JSON, send the request to the API

                string url = "UpdateCategory/" + id;

                string jsonpayload = jss.Serialize(category);
                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                Debug.WriteLine(content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Show", new { id = id });
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

        // GET: Category/New
        public ActionResult New()
        {

            return View();
        }

        // GET: Category/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findcategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CategoryDto selectedcategory = response.Content.ReadAsAsync<CategoryDto>().Result;
            return View(selectedcategory);
        }

        // POST: Category/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                string url = "deletecategory/" + id;
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