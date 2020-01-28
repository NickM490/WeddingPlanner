using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace Wedding_Planner.Models

{

    public class User
    {

        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(45)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(45)]
        public string LastName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }


        [DataType(DataType.Password)]
        [Required]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string Password { get; set; }


        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public List<RSVP> ResponsesGiven { get; set; }

    }

    public class LoginUser
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string LoginEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string LoginPassword { get; set; }
    }
}



