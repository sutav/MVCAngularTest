using MVCAngularTest.WebModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCAngularTest.BusinessLogic
{
    public interface IPalindromeManager
    {
        List<PalindromeModel> GetPalindromeList();
        void SavePalindrome(PalindromeModel palindrome);
    }
}
