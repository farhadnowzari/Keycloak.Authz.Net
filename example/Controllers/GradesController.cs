using Keycloak.Authz.Net.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace University.Service.Controllers;

[ApiController]
[Route("grades")]
public class GradesController : ControllerBase
{
    [Authz(["grade_{id}#delete"])]
    [HttpDelete("{id}")]
    public string DeleteGrade(string id)
    {
        return $"Grade with id:{id} was requested to be deleted!";
    }
}