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
    public class Wedding

    {
        [Key]
        public int WeddingId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(45)]
        public string WedderOne { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(45)]
        public string WedderTwo { get; set; }



        [Required]
        [NoPastDate(ErrorMessage = "Date must be a future date")]
        public DateTime Date { get; set; }

        [Required]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }


        public User Guest { get; set; }

        // public RSVP rsvps { get; set; }

        public List<RSVP> RSVPs { get; set; }



        // public User Creator { get; set; }



    }
    public class NoPastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value <= DateTime.Now)
                return new ValidationResult("Date must be in the future");
            return ValidationResult.Success;
        }
    }


}