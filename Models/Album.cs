using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class Album
    {
        public Album()
        {
            AlbumItems = new HashSet<AlbumItem>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Path { get; set; }
        public DateTime GmtCreate { get; set; }
        public DateTime GmtModify { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<AlbumItem> AlbumItems { get; set; }
    }
}
