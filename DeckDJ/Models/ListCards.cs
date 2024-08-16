using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckDJ.Models
{
    public class ListCards
    {
        public bool IsAdmin { get; set; }

        public int PageNum { get; set; }

        public String PageSummary { get; set; }

        public IEnumerable<CardDto> Cards { get; set; }

    }
}