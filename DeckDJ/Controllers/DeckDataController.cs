using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Description;
using DeckDJ.Models;


namespace DeckDJ.Controllers
{
    public class DeckDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Adds a Deck to the system
        /// </summary>
        /// <param name="deck">JSON FORM DATA of a deck</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Deck ID, Deck Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/DeckData/AddDeck
        /// FORM DATA: Deck JSON Object
        /// </example>
        [ResponseType(typeof(Deck))]
        [HttpPost]
        [Route("api/DeckData/AddDeck/")]
        public IHttpActionResult AddDeck(Deck NewDeck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Decks.Add(NewDeck);
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        //POST: /api/DeckData/CreateDeck
        [ResponseType(typeof(Deck))]
        [HttpPost]
        [Route("api/DeckData/CreateDeck/")]
        public int CreateDeck(Deck NewDeck)
        {
            if (!ModelState.IsValid)
            {
                return -1;
            }
            db.Decks.Add(NewDeck);
            db.SaveChanges();
            return NewDeck.DeckId;
        }

        /// <summary>
        /// Returns all decks in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all decks in the database.
        /// </returns>
        /// <example>
        /// GET: api/DeckData/ListDecks
        /// </example>
        [HttpGet]
        [Route("api/DeckData/ListDecks")]
        public IEnumerable<DeckDto> ListDecks()
        {
            List<Deck> DeckList = db.Decks.ToList();
            List<DeckDto> DeckDtoList = new List<DeckDto>();

            DeckList.ForEach(d => DeckDtoList.Add(new DeckDto()
            {
                DeckId = d.DeckId,
                UserId = d.UserId,
                DeckName = d.DeckName
            }));
            return DeckDtoList;
        }

        ///<summary>
        /// Returns all decks associated with a audio
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all decks related to the audio
        /// </returns>
        ///<param name="id">The primary key of the specified audio</param>
        /// <example>
        /// GET: api/DeckData/ListDecksForAudio/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AudioDto))]
        [Route("api/DeckData/ListDecksForAudio/{id}")]
        public IHttpActionResult ListDecksForAudio(int id)
        {
            List<Deck> Decks = db.Decks.Where(a => a.Audios.Any(d => d.AudioId == id)).ToList();
            List<DeckDto> DeckDtos = new List<DeckDto>();

            Decks.ForEach(a => DeckDtos.Add(new DeckDto()
            {
                DeckId = a.DeckId,
                DeckName = a.DeckName,
                UserId = a.UserId
            }));
            return Ok(DeckDtos);
        }

        ///<summary>
        /// Returns all decks associated with a audio
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all decks related to the audio
        /// </returns>
        ///<param name="id">The primary key of the specified audio</param>
        /// <example>
        /// GET: api/DeckData/ListDecksForAudio/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AudioDto))]
        [Route("api/DeckData/ListDecksNotForAudio/{id}")]
        public IHttpActionResult ListDecksNotForAudio(int id)
        {
            List<Deck> Decks = db.Decks.Where(a => !a.Audios.Any(d => d.AudioId == id)).ToList();
            List<DeckDto> DeckDtos = new List<DeckDto>();

            Decks.ForEach(a => DeckDtos.Add(new DeckDto()
            {
                DeckId = a.DeckId,
                DeckName = a.DeckName,
                UserId = a.UserId
            }));
            return Ok(DeckDtos);
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
        /// GET: api/audiodata/listaudios
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AudioDto))]
        [Route("api/DeckData/ListAudioNotForDeck/{id}")]
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
        /// Returns a specific deck in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A deck in the system matching up to the deck ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the deck</param>
        /// <example>
        /// GET: api/DeckData/FindDeck/1
        /// </example>
        [HttpGet]
        [Route("api/DeckData/FindDeck/{id}")]
        [ResponseType(typeof(DeckDto))]
        public IHttpActionResult FindDeck(int id)
        {
            Deck deck = db.Decks.Find(id);
            DeckDto deckDto = new DeckDto()
            {
                DeckId = deck.DeckId,
                UserId = deck.UserId,
                DeckName = deck.DeckName

            };
            if (deck == null)
            {
                return NotFound();
            }
            return Ok(deckDto);
        }


        /// <summary>
        /// Returns all deck in the system that belong to a specific user.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Deck in the system with a that belong to a specific user
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The id of the owner of the deck</param>
        /// <example>
        /// GET: api/DeckData/UserDecks/1231as-asdas546
        /// </example>
        [HttpGet]
        [Route("api/DeckData/UserDecks/{id}")]
        public IEnumerable<DeckDto> UserDecks(string id)
        {
            List<Deck> Decks = db.Decks.Where(d => d.UserId.Equals(id)).ToList();
            List<DeckDto> DeckDtos = new List<DeckDto>();
            Decks.ForEach(d => DeckDtos.Add(new DeckDto()
            {
                DeckId = d.DeckId,
                UserId = d.UserId,
                DeckName = d.DeckName
            }));
            return DeckDtos;
        }

        /// <summary>
        /// Updates a particular deck in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Deck ID primary key</param>
        /// <param name="deck">JSON FORM DATA of an deck</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/DeckData/UpdateDeck/1
        /// FORM DATA: Deck JSON Object
        /// </example>
        [ResponseType(typeof(Deck))]
        [HttpPost]
        [Route("api/DeckData/UpdateDeck/{id}")]
        public IHttpActionResult UpdateDeck(int id, [FromBody] Deck deck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != deck.DeckId)
            {
                return BadRequest();
            }

            db.Entry(deck).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(db.Decks.Count(e => e.DeckId == id) > 0))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Associates a particular audio with a particular deck
        /// </summary>
        /// <param name="deckid">The deck ID primary key</param>
        /// <param name="audioid">The audio ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/DeckData/AssociateDeckWithAudio/9/1
        /// </example>
        [HttpPost]
        [Route("api/DeckData/AssociateDeckWithAudio/{deckid}/{audioid}")]
        public IHttpActionResult AssociateDeckWithAudio(int deckid, int audioid)
        {

            Deck SelectedDeck = db.Decks.Include(d => d.Audios).Where(d => d.DeckId == deckid).FirstOrDefault();
            Audio SelectedAudio = db.Audios.Find(audioid);

            if (SelectedDeck == null || SelectedAudio == null)
            {
                return NotFound();
            }

            SelectedDeck.Audios.Add(SelectedAudio);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a deck and a particular audio
        /// </summary>
        /// <param name="deckid">The deck ID primary key</param>
        /// <param name="audioid">The audio ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/DeckData/UnAssociateDeckwithAudio/9/1
        /// </example>
        [HttpPost]
        [Route("api/DeckData/UnAssociateDeckwithAudio/{deckid}/{audioid}")]
        public IHttpActionResult UnAssociateDeckWithAudio(int DeckId, int AudioId)
        {

            Deck SelectedDeck = db.Decks.Include(a => a.Audios).Where(d => d.DeckId == DeckId).FirstOrDefault();
            Audio SelectedAudio = db.Audios.Find(AudioId);

            if (SelectedDeck == null || SelectedAudio == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input deck id is: " + DeckId);
            Debug.WriteLine("selected deck name is: " + SelectedDeck.DeckName);
            Debug.WriteLine("input audio id is: " + AudioId);
            Debug.WriteLine("selected audio name is: " + SelectedAudio.AudioName);

            //todo: verify that the audio actually is keeping track of the deck

            SelectedDeck.Audios.Remove(SelectedAudio);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// Removes an association between a particular audio and a particular deck
        /// </summary>
        /// <param name="deckid">The deck ID primary key</param>
        /// <param name="audioid">The audio ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/DeckData/UnassociateDeckwithAudio/9/1
        /// </example>
        [HttpPost]
        [Route("api/DeckData/UnassociateDeckwithAudio/{deckid}/{audioid}")]
        public IHttpActionResult UnassociateDeckwithAudio(int deckid, int audioid)
        {

            Deck SelectedDeck = db.Decks.Include(a => a.Audios).Where(a => a.DeckId == deckid).FirstOrDefault();
            Audio SelectedAudio = db.Audios.Find(audioid);

            if (SelectedDeck == null || SelectedAudio == null)
            {
                return NotFound();
            }

            //todo: verify that the audio actually is keeping track of the deck

            SelectedDeck.Audios.Remove(SelectedAudio);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a deck from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the deck</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/DeckData/DeleteDeck/1
        /// FORM DATA: (empty)
        /// </example>
        [HttpPost]
        [Route("api/DeckData/DeleteDeck/{id}")]
        public IHttpActionResult DeleteDeck(int id)
        {
            Deck deck = db.Decks.Find(id);
            if (deck == null)
            {
                return NotFound();
            }
            db.Decks.Remove(deck);
            db.SaveChanges();
            return Ok();
        }
    }
}