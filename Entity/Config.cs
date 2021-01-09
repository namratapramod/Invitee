using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class Config 
    {
        public int Id { get; set; }
        public string FireBaseToken { get; set; }
        [Range(0,50)]
        public int FreeVideoOnRegister { get; set; }
        public string PlaystoreVersionNumber { get; set; }
    }
}