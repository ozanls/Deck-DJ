﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using DeckDJ.Models;

namespace DeckDJ.Controllers
{
    public class ComboPieceDataController : ApiController
    {
        private ApplicationDbContext  db = new ApplicationDbContext();

        /// <summary>
        /// Adds a Combo Piece to the system
        /// </summary>
        /// <param name="NewComboPiece">JSON FORM DATA of a combo piece definition</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Combo piece definition ID, Combo piece definition Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ComboPieceData/AddComboPiece
        /// FORM DATA: Combo piece definition JSON Object
        /// </example>
        [ResponseType(typeof(ComboPiece))]
        [HttpPost]
        [Route("api/ComboPieceData/AddComboPiece")]
        [Authorize]
        public IHttpActionResult AddComboPiece(ComboPiece NewComboPiece)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.ComboPieces.Add(NewComboPiece);
            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Returns all Combo Pieces in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Combo Pieces in the database.
        /// </returns>
        /// <example>
        /// GET: api/ComboPieceData/ListComboPiece
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/ListComboPiece")]
        public IEnumerable<ComboPieceDto> ListComboPiece()
        {
            List<ComboPiece> ComboPieceList = db.ComboPieces.ToList();
            List<ComboPieceDto> ComboPieceDtosList = new List<ComboPieceDto>();

            ComboPieceList.ForEach(c => ComboPieceDtosList.Add(new ComboPieceDto()
            {
                ComboPieceId = c.ComboPieceId,
                CardId = c.CardId,
                DeckId = c.DeckId,
                copies = c.copies
            }));
            return ComboPieceDtosList;
        }

        /// <summary>
        /// Returns a specific Combo Piece in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A Combo Piece in the system matching up to the Combo Piece ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Combo Piece</param>
        /// <example>
        /// GET: api/ComboPieceData/FindComboPiece/1
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/FindComboPiece/{id}")]
        [ResponseType(typeof(ComboPieceDto))]
        public IHttpActionResult FindComboPiece(int id)
        {
            ComboPiece ComboPiece = db.ComboPieces.Find(id);
            ComboPieceDto ComboPieceDto = new ComboPieceDto()
            {
                ComboPieceId = ComboPiece.ComboPieceId,
                CardId = ComboPiece.CardId,
                DeckId = ComboPiece.DeckId,
                copies = ComboPiece.copies
            };
            if (ComboPiece == null)
            {
                return NotFound();
            }
            return Ok(ComboPieceDto);
        }

        /// <summary>
        /// Returns all combo pieces in the system that belong to a specific deck.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Combo pieces in the system with a that belong to a specific deck
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The id of the deck containing the combo pieces</param>
        /// <example>
        /// GET: api/ComboPieceData/GetComboPieces/1
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/GetComboPieces/{id}")]
        public IEnumerable<ComboPieceDto> GetComboPieces(int id)
        {
            List<ComboPiece> ComboPieces = db.ComboPieces.Where(c => c.DeckId == id).ToList();
            List<ComboPieceDto> ComboPiecesDtos = new List<ComboPieceDto>();
            ComboPieces.ForEach(c => ComboPiecesDtos.Add(new ComboPieceDto()
            {
                ComboPieceId = c.ComboPieceId,
                CardId = c.CardId,
                DeckId = c.DeckId,
                copies = c.copies
            }));
            return ComboPiecesDtos;
        }

        /// <summary>
        /// Returns all combo pieces in the system that belong to a specific deck.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Combo pieces in the system with a that belong to a specific deck
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The id of the deck containing the combo pieces</param>
        /// <example>
        /// GET: api/ComboPieceData/GetCombosInDeck/1
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/GetCombosInDeck/{id}")]
        public IEnumerable<ComboPieceDto> GetCombosInDeck(int id)
        {
            var ComboPieces = db.ComboPieces.Join(db.Cards, combo => combo.CardId, card => card.Id, (combo, card) => new { combo, card.Name, card.Number }).Where(c => c.combo.DeckId == id).ToList();
            List<ComboPieceDto> ComboPiecesDtos = new List<ComboPieceDto>();
            foreach (var combo in ComboPieces)
            {
                ComboPiecesDtos.Add(new ComboPieceDto()
                {
                    ComboPieceId = combo.combo.ComboPieceId,
                    CardId = combo.combo.CardId,
                    CardNumber = combo.Number,
                    CardName = combo.Name,
                    DeckId = combo.combo.DeckId,
                    copies = combo.combo.copies
                });
            }
            return ComboPiecesDtos;
        }

        /// <summary>
        /// Returns all combo pieces in the system that use a specific card.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Combo pieces in the system with a that use a specific card
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The id of the card used as a combo pieces</param>
        /// <example>
        /// GET: api/ComboPieceData/GetCombosForCard/1
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/GetCombosForCard/{id}")]
        public IEnumerable<ComboPieceDto> GetCombosForCard(int id)
        {
            var ComboPieces = db.ComboPieces.Join(db.Decks, combo => combo.DeckId, deck => deck.DeckId, (combo, deck) => new { combo, deck.DeckName, deck.UserId }).Where(c => c.combo.CardId == id).ToList();
            List<ComboPieceDto> ComboPiecesDtos = new List<ComboPieceDto>();
            foreach (var combo in ComboPieces)
            {
                ComboPiecesDtos.Add(new ComboPieceDto()
                {
                    ComboPieceId = combo.combo.ComboPieceId,
                    DeckId = combo.combo.DeckId,
                    DeckName = combo.DeckName,
                    UserId = combo.UserId,
                    copies = combo.combo.copies
                });
            }
            return ComboPiecesDtos;
        }

        /// <summary>
        /// Returns all cards in the system that belong to a specific deck.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Cards in the system with a that belong to a specific deck
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="Deckid">The id of the deck containing the cards</param>
        /// <example>
        /// GET: api/ComboPieceData/GetCardsInDeck/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CardDto))]
        [Route("api/ComboPieceData/GetCardsInDeck/{id}")]
        public IEnumerable<ComboPieceDto> GetCardsInDeck(int Deckid)
        {
            var ComboPieces = db.ComboPieces.Join(db.Cards, combo => combo.CardId, card => card.Id, (combo, card) => new { combo, card}).Where(c => c.combo.DeckId == Deckid).ToList();
            List<ComboPieceDto> ComboPieceDtos = new List<ComboPieceDto>();
            foreach (var comboPiece in ComboPieces)
            {
                ComboPieceDtos.Add(new ComboPieceDto()
                {
                    CardId = comboPiece.card.Id,
                    CardNumber = comboPiece.card.Number,
                    CardName = comboPiece.card.Name,
                    copies = comboPiece.combo.copies,
                    ComboPieceId = comboPiece.combo.ComboPieceId,
                    DeckId = comboPiece.combo.DeckId
                });
            }
            return ComboPieceDtos;
        }

        /// <summary>
        /// Returns all cards in the system that do not belong to a specific deck.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Cards in the system with that do not belong to a specific deck
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="Deckid">The id of the deck not containing the cards</param>
        /// <example>
        /// GET: api/ComboData/GetCardsNotInDeck/1
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/GetCardsNotInDeck/{id}")]
        public IEnumerable<CardDto> GetCardsNotInDeck(int Deckid)
        {
            var Cards = db.ComboPieces.Join(db.Cards, combo => combo.CardId, card => card.Id, (combo, card) => new { combo, card }).Where(c => c.combo.DeckId != Deckid).ToList();
            List<CardDto> CardDtos = new List<CardDto>();
            foreach (var card in Cards)
            {
                CardDtos.Add(new CardDto()
                {
                    Id = card.card.Id,
                    Number = card.card.Id,
                    Name = card.card.Name,
                    CardFrame = card.card.CardFrame,
                    CardType = card.card.CardType,
                    Level = card.card.Level,
                    Attribute = card.card.Attribute,
                    Scale = card.card.Id,
                    Attack = card.card.Id,
                    Defense = card.card.Id
                });
            }
            return CardDtos;
        }

        /// <summary>
        /// Returns all Decks in the system that contains a specific card.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Decks in the system with a that contain a specific card
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="Cardid">The id of the card found in the decks pieces</param>
        /// <example>
        /// GET: api/ComboData/GetDecksForCard/1
        /// </example>
        [HttpGet]
        [Route("api/ComboPieceData/GetDecksForCard/{id}")]
        public IEnumerable<DeckDto> GetDecksForCard(int Cardid)
        {
            var Decks = db.ComboPieces.Join(db.Decks, combo => combo.CardId, deck => deck.DeckId, (combo, deck) => new { combo, deck}).Where(c => c.combo.CardId == Cardid).ToList();
            List<DeckDto> DeckDtos = new List<DeckDto>();
            foreach (var deck in Decks)
            {
                DeckDtos.Add(new DeckDto()
                {
                    DeckId = deck.deck.DeckId,
                    UserId = deck.deck.UserId
                });
            }
            return DeckDtos;
        }


        /// <summary>
        /// Updates a particular combo piece in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the combo piece ID primary key</param>
        /// <param name="deck">JSON FORM DATA of an combo piece</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/ComboPieceData/UpdateComboPiece/1
        /// FORM DATA: Deck JSON Object
        /// </example>
        [ResponseType(typeof(ComboPiece))]
        [HttpPost]
        [Route("api/ComboPieceData/UpdateComboPiece/{id}")]
        [Authorize]
        public IHttpActionResult UpdateComboPiece(int id, [FromBody] ComboPiece ComboPiece)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ComboPiece.ComboPieceId)
            {
                return BadRequest();
            }

            db.Entry(ComboPiece).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(db.ComboPieces.Count(e => e.ComboPieceId == id) > 0))
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
        /// Deletes a combo piece from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the combo piece</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/ComboPieceData/DeleteComboPiece/1
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(ComboPiece))]
        [HttpPost]
        [Route("api/ComboPieceData/DeleteComboPiece/{id}")]
        [Authorize]
        public IHttpActionResult DeleteComboPiece(int id)
        {
            ComboPiece ComboPiece = db.ComboPieces.Find(id);
            if (ComboPiece == null)
            {
                return NotFound();
            }
            db.ComboPieces.Remove(ComboPiece);
            db.SaveChanges();
            return Ok();
        }

    }
}
