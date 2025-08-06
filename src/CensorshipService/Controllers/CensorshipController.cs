using CensorshipService.DTOs;
using CensorshipService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CensorshipService.Controllers
{
    [ApiController]
    [Route("api/v1/censor/")]
    public class CensorshipController(ITextCensorshipService censorshipService) : ControllerBase
    {
        private readonly ITextCensorshipService _censorshipService = censorshipService;

        [Authorize(Roles = "Admin")]
        [HttpPost("check")]
        public IActionResult Check([FromBody] CheckTextDto checkText)
        {
            var hasBanned = _censorshipService.ContainsBannedWords(checkText.Text);
            return Ok(hasBanned);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("censtext")]
        public IActionResult TextCensor([FromBody] CheckTextDto checkText)
        {
            var censored = _censorshipService.CensorText(checkText.Text);
            return Ok(censored);
        }
    }
}
