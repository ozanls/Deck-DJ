using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DeckDJ.Models;
using System.Diagnostics;

namespace DeckDJ.Controllers
{
    public class AudioDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ///<summary>
        /// Returns all audios in the database
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all audios in the database
        /// </returns>
        /// <example>
        /// GET: api/audiodata/listaudios
        /// </example>
        [HttpGet]
        public IEnumerable<AudioDto> ListAudios()
        {
            List<Audio> Audios = db.Audios.ToList();
            List<AudioDto> AudioDtos = new List<AudioDto>();

            Audios.ForEach(a => AudioDtos.Add(new AudioDto
            {
                AudioId = a.AudioId,
                AudioName = a.AudioName,
                AudioURL = a.AudioURL,
                AudioLength = a.AudioLength,
                AudioTimestamp = a.AudioTimestamp,
                AudioStreams = a.AudioStreams,
                AudioUploaderId = a.AudioUploaderId,
                CategoryName = a.Category.CategoryName
            }));
            return AudioDtos;
        }



        ///<summary>
        /// Returns all audios associated with a category
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all audios related to the category
        /// </returns>
        ///<param name="id">The primary key of the specified category</param>
        /// <example>
        /// GET: api/audiodata/listaudioforcategory/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AudioDto))]

        public IHttpActionResult ListAudioForCategory(int id)
        {
            List<Audio> Audios = db.Audios.Where(a=>a.CategoryId==id).ToList();
            List<AudioDto> AudioDtos = new List<AudioDto>();

            Audios.ForEach(a => AudioDtos.Add(new AudioDto()
            {
                AudioId = a.AudioId,
                AudioName = a.AudioName,
                AudioURL = a.AudioURL,
                AudioLength = a.AudioLength,
                AudioTimestamp = a.AudioTimestamp,
                AudioStreams = a.AudioStreams,
                AudioUploaderId = a.AudioUploaderId,
                CategoryName = a.Category.CategoryName
            }));
            return Ok(AudioDtos);
        }

        ///<summary>
        /// Returns all audios associated with a deck
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all audios related to the deck
        /// </returns>
        ///<param name="id">The primary key of the specified deck</param>
        /// <example>
        /// GET: api/audiodata/listaudios
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AudioDto))]

        public IHttpActionResult ListAudioForDeck(int id)
        {
            List<Audio> Audios = db.Audios.Where(a => a.Decks.Any(d => d.DeckId == id)).ToList();
            List<AudioDto> AudioDtos = new List<AudioDto>();

            Audios.ForEach(a => AudioDtos.Add(new AudioDto()
            {
                AudioId = a.AudioId,
                AudioName = a.AudioName,
                AudioURL = a.AudioURL,
                AudioLength = a.AudioLength,
                AudioTimestamp = a.AudioTimestamp,
                AudioStreams = a.AudioStreams,
                AudioUploaderId = a.AudioUploaderId,
                CategoryName = a.Category.CategoryName
            }));
            return Ok(AudioDtos);
        }

        ///<summary>
        /// Returns all audios not associated with a deck
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all audios not related to the deck
        /// </returns>
        ///<param name="id">The primary key of the specified deck</param>
        /// <example>
        /// GET: api/audiodata/listaudionotfordeck/id
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AudioDto))]

        public IHttpActionResult ListAudioNotForDeck(int id)
        {
            List<Audio> Audios = db.Audios.Where(a => a.Decks.Any(d => d.DeckId != id)).ToList();
            List<AudioDto> AudioDtos = new List<AudioDto>();

            Audios.ForEach(a => AudioDtos.Add(new AudioDto()
            {
                AudioId = a.AudioId,
                AudioName = a.AudioName,
                AudioURL = a.AudioURL,
                AudioLength = a.AudioLength,
                AudioTimestamp = a.AudioTimestamp,
                AudioStreams = a.AudioStreams,
                AudioUploaderId = a.AudioUploaderId,
                CategoryName = a.Category.CategoryName
            }));
            return Ok(AudioDtos);
        }


        /// <summary>
        /// Returns a specified audio
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An audio in the system matching up to the audio ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the audio</param>
        /// <example>
        /// GET: api/audiodata/findaudio/5
        /// </example>
        [ResponseType(typeof(Audio))]
        [HttpGet]
        public IHttpActionResult FindAudio(int id)
        {
            Audio Audio = db.Audios.Find(id);
            AudioDto AudioDto = new AudioDto()
            {
                AudioId = Audio.AudioId,
                AudioName = Audio.AudioName,
                AudioURL = Audio.AudioURL,
                AudioLength = Audio.AudioLength,
                AudioTimestamp = Audio.AudioTimestamp,
                AudioStreams = Audio.AudioStreams,
                AudioUploaderId = Audio.AudioUploaderId,
                CategoryName = Audio.Category.CategoryName
            };
            if (Audio == null)
            {
                return NotFound();
            }

            return Ok(AudioDto);
        }


        /// <summary>
        /// Updates a particular audio in the system with POST data input
        /// </summary>
        /// <param name="id"></param>
        /// <param name="audio">JSON Form data of a audio</param>
        /// <returns>
        /// HEADER:204 (success, no content response)
        /// or
        /// HEADER:400 (Bad Request)
        /// or
        /// HEADER:404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/audiodata/updateaudio/5
        /// FORM DATA: audio JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateAudio(int id, Audio audio)
        {
            Debug.WriteLine("I have reached the audio update method");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model state is invalid");

                return BadRequest(ModelState);
            }

            if (id != audio.AudioId)
            {
                Debug.WriteLine("Id is invalid");
                Debug.WriteLine("GET Parameter: " + id);
                Debug.WriteLine("POST Parameter: " + audio.AudioId);
                return BadRequest();
            }

            db.Entry(audio).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AudioExists(id))
                {
                    Debug.WriteLine("Audio not found");
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
        /// Adds an audio to the database
        /// </summary>
        /// <param name="audio"></param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Audio ID, Audio Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/AudioData/AddAudio 
        /// FORM DATA: Audio JSON Object
        /// </example>
        [HttpPost]
        public IHttpActionResult AddAudio(Audio audio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Audios.Add(audio);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = audio.AudioId }, audio);
        }

        // POST: api/AudioData/DeleteAudio/5
        [ResponseType(typeof(Audio))]
        [HttpPost]
        public IHttpActionResult DeleteAudio(int id)
        {
            Audio audio = db.Audios.Find(id);
            if (audio == null)
            {
                return NotFound();
            }

            db.Audios.Remove(audio);
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

        private bool AudioExists(int id)
        {
            return db.Audios.Count(e => e.AudioId == id) > 0;
        }
    }
}