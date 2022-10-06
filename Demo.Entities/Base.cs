using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Demo.Entities
{
    public partial class Base
    {
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Nullable<DateTime> EditedDate { get; set; }

        [JsonIgnore]
        public Nullable<DateTime> DeletedDate { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; } = false;
    }
}

