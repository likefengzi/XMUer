using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class AlbumItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string AlbumId { get; set; }
        public DateTime GmtCreate { get; set; }
        public DateTime GmtModify { get; set; }


    }
}
