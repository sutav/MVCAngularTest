using MVCAngularTest.BusinessLogic;
using MVCAngularTest.BusinessLogic.Log;
using MVCAngularTest.WebModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MVC_Angular_Test.Controllers
{
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class PalindromeController : ApiController
    {
        private readonly ILogManager _logger;
        private readonly IPalindromeManager _palindromeManager;
        public PalindromeController(IPalindromeManager palindromeManager, ILogManager logger) {
            _palindromeManager = palindromeManager;
            _logger = logger;
        }
        [Route("Palindrome/GetPalindromes")]
        [HttpGet]
        public IHttpActionResult GetPalindromes()
        {
            try
            {
                var palindrtomes = _palindromeManager.GetPalindromeList();                           
                return Content(HttpStatusCode.OK, palindrtomes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PalindrtomeController.GetPalindrtomes :: Unhandled exception");
                return Content(HttpStatusCode.InternalServerError, new { status = "Failed", error = ex.ToString() });
            }
        }
        [Route("Palindrome/SavePalindrome")]
        [HttpPost]
        public IHttpActionResult SavePalindrome(PalindromeModel palindrome)
        {
            try
            {
                _palindromeManager.SavePalindrome(palindrome);
                return Content(HttpStatusCode.OK, new { status = "Success", data = "Palindrome saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PalindromeController.SaveBidTracker :: Unhandled exception");
                return Content(HttpStatusCode.InternalServerError, new { status = "Failed", error = ex.ToString() });
            }
        }
    }
}