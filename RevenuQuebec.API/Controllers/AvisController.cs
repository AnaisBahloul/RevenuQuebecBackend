// Controllers/AvisController.cs
using Microsoft.AspNetCore.Mvc;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using RevenuQuebec.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevenuQuebec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvisController : ControllerBase
    {
        private readonly IGestionAvisService _avisService;

        public AvisController(IGestionAvisService avisService)
        {
            _avisService = avisService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avis>>> GetAll()
        {
            var avis = await _avisService.ListerAvis();
            return Ok(avis);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Avis>> GetById(int id)
        {
            var avis = await _avisService.ConsulterAvis(id);
            if (avis == null)
                return NotFound();
            return Ok(avis);
        }

        [HttpGet("declaration/{declarationId}")]
        public async Task<ActionResult<Avis>> GetByDeclarationId(int declarationId)
        {
            var avis = await _avisService.ConsulterAvisParDeclaration(declarationId);
            if (avis == null)
                return NotFound();
            return Ok(avis);
        }
    }
}