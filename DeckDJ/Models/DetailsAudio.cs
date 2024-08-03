using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckDJ.Models
{
    public class DetailsAudio
    {
        public AudioDto Audio { get; set; }

        public IEnumerable<DeckDto> Decks { get; set; }

        public IEnumerable<DeckDto> OtherDecks { get; set; }
    }
}