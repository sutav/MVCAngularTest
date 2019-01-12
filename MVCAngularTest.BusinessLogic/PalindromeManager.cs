using MVCAngularTest.WebModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCAngularTest.BusinessLogic
{
    public class PalindromeManager: IPalindromeManager
    {
        public List<PalindromeModel> GetPalindromeList() {
          
            List<PalindromeModel> palindromes = new List<PalindromeModel>();
            palindromes.Add(new PalindromeModel() { Id = 1, PalindromeWord = "Was it a cat I saw" });
            palindromes.Add(new PalindromeModel() { Id = 1, PalindromeWord = @"Don't nod" });
            palindromes.Add(new PalindromeModel() { Id = 1, PalindromeWord = "Radar" });
            palindromes.Add(new PalindromeModel() { Id = 1, PalindromeWord = "No lemon, no melon" });
            return palindromes;

        }
        public void SavePalindrome(PalindromeModel palindrome) {
        }
    }
}
