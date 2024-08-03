using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeckDJ.Models
{
    public class Deck
    {
        [Key]
        public int DeckId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public string DeckName { get; set; }

        public ICollection<Audio> Audios { get; set; }
    }

    public class DeckDto
    {
        [Key]
        public int DeckId { get; set; }
        public string UserId { get; set; }
        public string DeckName { get; set; }
        public string AudioName { get; set; }

    }
}