using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace auth
{
	public class ApplicationUser
	{
		[Key]
        public int id { get; }
        [MaxLength(11)]
        public string phone { get; set; }
        [MaxLength(100)]
        public string password { get; set; }
        [MaxLength(50)]
        public string userName { get; set; }
        [MaxLength(6)]
        public string code { get; set; }
        public bool ifValidate { get; set; }
		public DateTime createdAt { get; set; }
	}
}