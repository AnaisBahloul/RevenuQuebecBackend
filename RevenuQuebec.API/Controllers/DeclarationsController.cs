// Controllers/DeclarationsController.cs
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
    public class DeclarationsController : ControllerBase
    {
        private readonly IGestionDeclarationService _declarationService;

        public DeclarationsController(IGestionDeclarationService declarationService)
        {
            _declarationService = declarationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Declaration>>> GetAll()
        {
            var declarations = await _declarationService.ListerDeclarations();
            return Ok(declarations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Declaration>> GetById(int id)
        {
            var declaration = await _declarationService.ConsulterDeclaration(id);
            if (declaration == null)
                return NotFound();
            return Ok(declaration);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Declaration declaration)
        {
            if (declaration == null)
                return BadRequest();

            await _declarationService.AddDeclaration(declaration);
            return CreatedAtAction(nameof(GetById), new { id = declaration.Id }, declaration);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Declaration declaration)
        {
            if (id != declaration.Id)
                return BadRequest();

            await _declarationService.UpdateDeclarationAsync(declaration);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var declaration = await _declarationService.ConsulterDeclaration(id);
            if (declaration == null)
                return NotFound();

            await _declarationService.DeleteDeclaration(declaration);
            return NoContent();
        }
    }
}