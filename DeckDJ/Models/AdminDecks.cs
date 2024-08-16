using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckDJ.Models
{
    public class AdminDecks
    {
        public bool IsAdmin { get; set; }

        public DeckDto Deck { get; set; }

        public IEnumerable<DeckDto> Decks { get; set; }

        public IEnumerable<ComboPieceDto> ComboPieces { get; set; }

        public IEnumerable<AudioDto> Audios { get; set; }

        public IEnumerable<AudioDto> OtherAudios { get; set; }
    }
}