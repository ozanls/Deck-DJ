using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AudioApplication.Models

{
    public class Audio
    {
        [Key]
        public int AudioId { get; set; }
        public string AudioName { get; set; }
        public string AudioURL { get; set; }
        public int AudioLength { get; set; }
        public DateTime AudioTimestamp { get; set; }
        public int AudioStreams { get; set; }
        public int AudioUploaderId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }

    // Data Transfer Object (DTO) allows us to package the information for each model

    public class AudioDto
    {
        public int AudioId { get; set; }
        public string AudioName { get; set; }
        public string AudioURL { get; set; }
        public int AudioLength { get; set; }
        public DateTime AudioTimestamp { get; set; }
        public int AudioStreams { get; set; }
        public int AudioUploaderId { get; set; }
        public string CategoryName { get; set; }
    }
}