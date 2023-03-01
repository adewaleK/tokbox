using ExamRoom.BlackBox.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Data;
using OpenTokSDK;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;

namespace ExamRoom.BlackBox.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        public IOptions<TokBoxSettings> _config { get; }
        public OpenTok OpenTok { get; protected set; }
        public SessionController(IOptions<TokBoxSettings> config)
        {
            _config = config;
        }

        [HttpGet("session-page")]

        public ContentResult GetSessionPage()
        {
            var html = System.IO.File.ReadAllText(@"./assets/session.html");
            return base.Content(html, "text/html");
        }

        [HttpGet("index-page")]
        public ContentResult GetIndexPage()
        {
            var html = System.IO.File.ReadAllText(@"./assets/index.html");
            return base.Content(html, "text/html");
        }

        [HttpPost()]
        public ActionResult<SessionResponse> CreateSession()
        {
            MediaMode mediaMode = MediaMode.ROUTED;
            ArchiveMode archiveMode = ArchiveMode.ALWAYS;
            OpenTokService();
            var session = OpenTok.CreateSession(String.Empty, mediaMode, archiveMode);
            string apiKeyString = _config.Value.API_KEY;
            int apiKey;
            
            apiKey = Convert.ToInt32(apiKeyString);
            var res = new SessionResponse();
            var sessionId = session.Id;
            res.SessionId = sessionId;
            res.Token = OpenTok.GenerateToken(sessionId);

            res.API_KEY = apiKey;
            return Ok(res);
        }
        private void OpenTokService()
        {
            int apiKey = 0;
            string apiSecret = null;
            try
            {
                string apiKeyString = _config.Value.API_KEY;
                apiSecret = _config.Value.API_SECRET;
                apiKey = Convert.ToInt32(apiKeyString);

                if (apiKey == 0 || apiSecret == null)
                {
                    Console.WriteLine(
                        "The OpenTok API Key and API Secret were not set in the application configuration. " +
                        "Set the values in App.config and try again. (apiKey = {0}, apiSecret = {1})", apiKey, apiSecret);
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            
            this.OpenTok = new OpenTok(apiKey, apiSecret);
        }
    }
}
