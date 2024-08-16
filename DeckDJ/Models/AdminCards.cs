using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckDJ.Models
{
    public class AdminCards
    {
        public bool IsAdmin { get; set; }

        public CardDto Card { get; set; }

        public IEnumerable<CardDto> Cards { get; set; }

        public int PageNum { get; set; }

        public String PageSummary { get; set; }

    }
}