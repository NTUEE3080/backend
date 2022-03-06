using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitaPairing.Auth;
using PitaPairing.Database;
using PitaPairing.Errors;

namespace PitaPairing.Domain.Semester
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SemesterController : ControllerBase
    {
        private readonly CoreDbContext _db;
        private readonly ILogger<SemesterController> _l;

        public SemesterController(CoreDbContext db, ILogger<SemesterController> l)
        {
            _db = db;
            _l = l;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SemesterRes>> GetAll()
        {
            try
            {
                var semesters = _db.Semester.Select(x => x.ToResp()).ToArray();
                return Ok(semesters);
            }
            catch (Exception e)
            {
                if (e is not BaseErrorException) _l.LogCritical(e, "failed to list all semesters");
                throw;
            }
        }

        [HttpPost("current")]
        [Authorize(Policy = AuthPolicy.Admin)]
        public async Task<ActionResult> SetAsCurrent([FromBody] CreateSemesterReq r)
        {
            try
            {
                var exist = await _db.Semester.AnyAsync(x => x.Id == r.Semester);
                if (!exist) throw new NotFoundError("Semester Not Found", $"Semester {r} does not exist");
                foreach (var c in _db.Semester)
                {
                    if (c.Id != r.Semester)
                        c.Current = false;
                    else
                        c.Current = true;
                }

                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                // Add Conflict handling here
                if (e is not BaseErrorException) _l.LogCritical(e, "failed to list all semesters");
                throw;
            }
        }

        [HttpPost]
        [Authorize(Policy = AuthPolicy.Admin)]
        public async Task<ActionResult<SemesterRes>> CreateSemester([FromBody] CreateSemesterReq r)
        {
            try
            {
                var d = new SemesterData()
                {
                    Id = r.Semester,
                    Current = false
                };
                var added = await _db.Semester.AddAsync(d);
                await _db.SaveChangesAsync();
                return Ok(added.Entity.ToResp());
            }
            catch (Exception e)
            {
                // Add Conflict handling here
                if (e is not BaseErrorException) _l.LogCritical(e, "failed to list all semesters");
                throw;
            }
        }

        [HttpGet("/current")]
        public async Task<ActionResult<string>> GetCurrent()
        {
            try
            {
                var semester = await _db.Semester
                    .FirstOrDefaultAsync(x => x.Current);
                if (semester == null)
                    throw new NotFoundError("No Current Semester", "No semester is currently marked as 'current'");
                return Ok(semester.Id);
            }
            catch (Exception e)
            {
                if (e is not BaseErrorException) _l.LogCritical(e, "failed to list all semesters");
                throw;
            }
        }
    }
}