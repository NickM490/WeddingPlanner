
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

    public class RSVP
    {
        public int RSVPId { get; set; }
        public int UserId { get; set; }
        public int WeddingId { get; set; }
        public Wedding Wedding { get; set; }
        public User Guest { get; set; }
    }

}