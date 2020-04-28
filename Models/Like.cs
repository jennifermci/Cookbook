using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Cookbook.Models
{
    public class Like
    {
        [Key]
        public int LikeId {get; set;}
        [Required]

        public int UserTipId {get;set;}
        // public int UserCommentId {get;set;}

        // public RegisterUser UserCommentLikes {get;set;}
        public RegisterUser UserTipLikes {get;set;}
        // public int CommentId {get;set;}
        // public Comment Comment {get;set;}
        public int TipId {get;set;}
        public Tip Tip {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } =  DateTime.Now;

    }
}