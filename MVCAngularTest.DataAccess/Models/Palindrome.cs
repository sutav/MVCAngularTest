using MVCAngularTest.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCAngularTest.DataAccess.Models
{
    public partial class Palindrome : BaseEntity
    {      
        public int Id { get; set; }    
        public string PalindromeWord { get; set; }

    }
}
