using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WLR.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();
}
