using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [ApiController]
    [EnableCors(ApiConstants.ApiAllowAllOriginsPolicy)]
    public class ApiControllerBase : ControllerBase
    {
    }
}
