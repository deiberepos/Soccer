using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data;
using Soccer.Web.Data.Entities;

namespace Soccer.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly DataContext _context;

        public TeamsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public IEnumerable<TeamEntity> GetTeams()
        {
            return _context.Teams.OrderBy(t=>t.Name);
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teamEntity = await _context.Teams.FindAsync(id);

            if (teamEntity == null)
            {
                return NotFound();
            }

            return Ok(teamEntity);
        }

        private bool TeamEntityExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}