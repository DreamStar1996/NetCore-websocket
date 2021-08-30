using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model
{
   public abstract class TreeID<E>: ID
    {
        public TreeID()
        {
            Childs = new HashSet<E>();
        }
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        [Display(Name = "上级")]
        [JsonIgnore]
        public virtual E Parent { get; set; }
        public virtual ICollection<E> Childs { get; set; }
    }
}
