using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Using our action filter 
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {

    }
}