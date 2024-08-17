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
using System.Web;
using System.IO;

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
        [Route("api/AudioData/ListAudios")]
        public IEnumerable<AudioDto> ListAudios()
        {
            List<Audio> Audios = db.Audios.ToList();
            List<AudioDto> AudioDtos = new List<AudioDto>();

            Audios.ForEach(a => AudioDtos.Add(new AudioDto
            {
                AudioId = a.AudioId,
                AudioName = a.AudioName,
                AudioLength = a.AudioLength,
                AudioTimestamp = a.AudioTimestamp,
                AudioStreams = a.AudioStreams,
                AudioUploaderId = a.AudioUploaderId,
                AudioExtension = a.AudioExtension,
                AudioHasAudio = a.AudioHasAudio,
                CategoryName = a.Category.CategoryName,
                CategoryId = a.Category.CategoryId
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
        [Route("api/AudioData/ListAudioForCategory/{id}")]
        public IHttpActionResult ListAudioForCategory(int id)
        {
            List<Audio> Audios = db.Audios.Where(a=>a.CategoryId==id).ToList();
            List<AudioDto> AudioDtos = new List<AudioDto>();

            Audios.ForEach(a => AudioDtos.Add(new AudioDto()
            {
                AudioId = a.AudioId,
                AudioName = a.AudioName,
                AudioLength = a.AudioLength,
                AudioTimestamp = a.AudioTimestamp,
                AudioStreams = a.AudioStreams,
                AudioUploaderId = a.AudioUploaderId,
                AudioHasAudio = a.AudioHasAudio,
                AudioExtension = a.AudioExtension,
                CategoryName = a.Category.CategoryName,
                CategoryId = a.Category.CategoryId
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
        /// GET: api/audiodata/ListAudioForDeck/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AudioDto))]
        [Route("api/AudioData/ListAudioForDeck/{id}")]
        public IHttpActionResult ListAudioForDeck(int id)
        {
            List<Audio> Audios = db.Audios.Where(a => a.Decks.Any(d => d.DeckId == id)).ToList();
            List<AudioDto> AudioDtos = new List<AudioDto>();

            Audios.ForEach(a => AudioDtos.Add(new AudioDto()
            {
                AudioId = a.AudioId,
                AudioName = a.AudioName,
                AudioLength = a.AudioLength,
                AudioTimestamp = a.AudioTimestamp,
                AudioStreams = a.AudioStreams,
                AudioUploaderId = a.AudioUploaderId,
                AudioHasAudio = a.AudioHasAudio,
                AudioExtension = a.AudioExtension,
                CategoryName = a.Category.CategoryName,
                CategoryId = a.Category.CategoryId
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
        [Route("api/AudioData/ListAudioNotForDeck/{id}")]
        public IHttpActionResult ListAudioNotForDeck(int id)
        {
            List<Audio> Audios = db.Audios.Where(a => !a.Decks.Any(d => d.DeckId == id)).ToList();
            List<AudioDto> AudioDtos = new List<AudioDto>();

            Audios.ForEach(a => AudioDtos.Add(new AudioDto()
            {
                AudioId = a.AudioId,
                AudioName = a.AudioName,
                AudioLength = a.AudioLength,
                AudioTimestamp = a.AudioTimestamp,
                AudioStreams = a.AudioStreams,
                AudioUploaderId = a.AudioUploaderId,
                AudioHasAudio = a.AudioHasAudio,
                AudioExtension = a.AudioExtension,
                CategoryName = a.Category.CategoryName,
                CategoryId = a.Category.CategoryId
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
        [Route("api/AudioData/FindAudio/{id}")]
        public IHttpActionResult FindAudio(int id)
        {
            Audio Audio = db.Audios.Find(id);
            AudioDto AudioDto = new AudioDto()
            {
                AudioId = Audio.AudioId,
                AudioName = Audio.AudioName,
                AudioLength = Audio.AudioLength,
                AudioTimestamp = Audio.AudioTimestamp,
                AudioStreams = Audio.AudioStreams,
                AudioUploaderId = Audio.AudioUploaderId,
                AudioHasAudio = Audio.AudioHasAudio, 
                AudioExtension = Audio.AudioExtension,
                CategoryName = Audio.Category.CategoryName,
                CategoryId = Audio.Category.CategoryId
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
        [Route("api/AudioData/UpdateAudio/{id}")]
        [Authorize]
        public IHttpActionResult UpdateAudio(int id, [FromBody]  Audio audio)
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
            db.Entry(audio).Property(a => a.AudioHasAudio).IsModified = false;
            db.Entry(audio).Property(a => a.AudioExtension).IsModified = false;

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
        [Route("api/AudioData/AddAudio")]
        [Authorize]
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

        /// <summary>
        /// Deletes an audio from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the audio</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/AudioData/DeleteAudio/1
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Audio))]
        [HttpPost]
        [Route("api/AudioData/DeleteAudio/{id}")]
        [Authorize]
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

        /// <summary>
        /// Saves uploaded audio and associates it with the audio system with matching ID.
        /// </summary>
        /// <param name="id">The primary key of the audio data to associate</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/AudioData/UploadAudioData/1
        /// FORM DATA: Audio file
        /// </example>
        [HttpPost]
        [Route("api/AudioData/UploadAudioData/{id}")]

        public IHttpActionResult UploadAudioData(int id)
        {

            bool hasAudio = false;
            string AudioExtension = "";
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numFiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numFiles);

                //Check if a file is posted
                if (numFiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var audioData = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (audioData.ContentLength > 0)
                    {
                        // Establish valid file types
                        var valTypes = new[] { "mp3", "wav", "ogg" };
                        var extension = Path.GetExtension(audioData.FileName).Substring(1);

                        //Check the extension of the file
                        if (valTypes.Contains(extension))
                        {
                            try
                            {
                                // file name is the audio id
                                string fn = id + "." + extension;

                                // get a direct file path to ~/Content/AudioData/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/AudioData/"), fn);

                                // Save the file
                                audioData.SaveAs(path);

                                // If the file is saved successfully, set the audio extension and hasAudio to true
                                hasAudio = true;
                                AudioExtension = extension;

                                // Update the audio record in the database
                                Audio SelectedAudio = db.Audios.Find(id);
                                SelectedAudio.AudioHasAudio = hasAudio;
                                SelectedAudio.AudioExtension = AudioExtension;
                                db.Entry(SelectedAudio).State = EntityState.Modified;

                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Audio Data was not saved successfully.");
                                Debug.WriteLine("Error: " + ex);
                            }
                        }
                    }
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
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