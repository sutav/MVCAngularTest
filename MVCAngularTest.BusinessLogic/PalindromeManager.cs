using MVCAngularTest.Core;
using MVCAngularTest.DataAccess.Models;
using MVCAngularTest.WebModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCAngularTest.BusinessLogic
{
    public class PalindromeManager: IPalindromeManager
    {
        private readonly IRepository<Palindrome> _repository;
        public PalindromeManager(IRepository<Palindrome> repository) {
            this._repository = repository;

        }
        public List<PalindromeModel> GetPalindromeList() {
            List<PalindromeModel> palindromes = new List<PalindromeModel>();
            if (ConfigurationManager.AppSettings["ConnectToDb"] == "true")
            {
                var res = _repository.Table.OrderByDescending(p=>p.Id).ToList();
                palindromes =  res.Select(p => new PalindromeModel()
                {
                    Id = p.Id,
                    PalindromeWord = p.PalindromeWord
                }).ToList();
            }
            else
            {
                palindromes.Add(new PalindromeModel() { Id = 1, PalindromeWord = "Was it a cat I saw" });
                palindromes.Add(new PalindromeModel() { Id = 1, PalindromeWord = @"Don't nod" });
                palindromes.Add(new PalindromeModel() { Id = 1, PalindromeWord = "Radar" });
                palindromes.Add(new PalindromeModel() { Id = 1, PalindromeWord = "No lemon, no melon" });

            }
            return palindromes;
        }
        public void SavePalindrome(PalindromeModel palindrome) {
            Palindrome newPalindrome = new Palindrome()
            {
                Id = palindrome.Id,
                PalindromeWord = palindrome.PalindromeWord
            };
            _repository.Insert(newPalindrome);

        }
    }
}
