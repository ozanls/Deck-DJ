using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckDJ.Models
{
    public class DetailsDeck
    {
        public DeckDto Deck { get; set; }

        public IEnumerable<ComboPieceDto> ComboPieces { get; set; }

        public IEnumerable<AudioDto> Audios { get; set; }

        public IEnumerable<AudioDto> OtherAudios { get; set; }

    }
}