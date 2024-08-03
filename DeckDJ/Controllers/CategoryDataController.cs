using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DeckDJ.Models;

namespace CategoryApplication.Controllers
{
    public class CategoryDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ///<summary>
        /// Returns all categories in the database
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all categories in the database
        /// </returns>
        /// <example>
        /// GET: api/categorydata/listcategories
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CategoryDto))]
        [Route("api/CategoryData/ListCategories")]
        public IHttpActionResult ListCategories()
        {
            List<Category> Categories = db.Categories.ToList();
            List<CategoryDto> CategoryDtos = new List<CategoryDto>();
            Categories.ForEach(c => CategoryDtos.Add(new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }));
            return Ok(CategoryDtos);
        }


        /// <summary>
        /// Returns a specified category
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A category in the system matching up to the category ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the category</param>
        /// <example>
        /// GET: api/categorydata/findcategory/5
        /// </example>
        [ResponseType(typeof(CategoryDto))]
        [HttpGet]
        [Route("api/CategoryData/FindCategory/{id}")]
        public IHttpActionResult FindCategory(int id)
        {
            Category Category = db.Categories.Find(id);
            CategoryDto CategoryDto = new CategoryDto()
            {
                CategoryId = Category.CategoryId,
                CategoryName = Category.CategoryName,
            };
            if (Category == null)
            {
                return NotFound();
            }

            return Ok(CategoryDto);
        }

        /// <summary>
        /// Updates a particular category in the system with POST data input
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category">JSON Form data of a category</param>
        /// <returns>
        /// HEADER:204 (success, no content response)
        /// or
        /// HEADER:400 (Bad Request)
        /// or
        /// HEADER:404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/categorydata/updatecategory/5
        /// FORM DATA: category JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/CategoryData/UpdateCategory/{id}")]
        public IHttpActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            Debug.WriteLine("I have reached the category update method");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model state is invalid");

                return BadRequest(ModelState);
            }

            if (id != category.CategoryId)
            {
                Debug.WriteLine("Id is invalid");
                Debug.WriteLine("GET Parameter: " + id);
                Debug.WriteLine("POST Parameter: " + category.CategoryId);
                return BadRequest();
            }

            db.Entry(category).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    Debug.WriteLine("Category not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Debug.WriteLine("None of the conditions triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Adds a category to the database
        /// </summary>
        /// <param name="category"></param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Category ID, Category Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/CategoryData/AddCategory
        /// FORM DATA: Category JSON Object
        /// </example>
        [ResponseType(typeof(Category))]
        [HttpPost]
        [Route("api/CategoryData/AddCategory")]
        public IHttpActionResult AddCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Debug.WriteLine(category);
            db.Categories.Add(category);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = category.CategoryId }, category);
        }


        /// <summary>
        /// Deletes a species from the system by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (not found)
        /// </returns>
        /// <example>
        /// POST: api/categorydata/deletecategory/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Category))]
        [HttpPost]
        [Route("api/CategoryData/DeleteCategory/{id}")]
        public IHttpActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return Ok();
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(c => c.CategoryId == id) > 0;
        }
    }
}
