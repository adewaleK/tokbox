using ExamRoom.BlackBox.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Data;
using OpenTokSDK;

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


        [HttpPost("CreateSession")]
        public ActionResult<SessionResponse> CreateSession(string name, MediaMode mediaMode = MediaMode.ROUTED, ArchiveMode archiveMode = ArchiveMode.ALWAYS)
        {
            OpenTokService();
            var session = OpenTok.CreateSession(String.Empty, mediaMode, archiveMode);

            var res = new SessionResponse();
            res.SessionId = session.Id;
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
