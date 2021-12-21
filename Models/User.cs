using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class User
    {
        public User()
        {
            Albums = new HashSet<Album>();
            Avatars = new HashSet<Avatar>();
            Comments = new HashSet<Comment>();
            FriendApplyFroms = new HashSet<FriendApply>();
            FriendApplyTos = new HashSet<FriendApply>();
            FriendMies = new HashSet<Friend>();
            FriendOthers = new HashSet<Friend>();
            Likes = new HashSet<Like>();
            News = new HashSet<News>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public byte? Sexual { get; set; }
        public DateTime? Birthday { get; set; }
        public string Hometown { get; set; }
        public string College { get; set; }
        public string Dept { get; set; }
        public string Major { get; set; }
        public string Grade { get; set; }
        public string Fmusic { get; set; }
        public string Fhobby { get; set; }
        public string Fbook { get; set; }
        public string Fmovie { get; set; }
        public string Fgame { get; set; }
        public string Fanime { get; set; }
        public string Fsport { get; set; }
        public byte BeenAudit { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<Avatar> Avatars { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FriendApply> FriendApplyFroms { get; set; }
        public virtual ICollection<FriendApply> FriendApplyTos { get; set; }
        public virtual ICollection<Friend> FriendMies { get; set; }
        public virtual ICollection<Friend> FriendOthers { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<News> News { get; set; }
    }
}
