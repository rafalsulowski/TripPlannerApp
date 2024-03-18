using Microsoft.AspNetCore.Mvc;
using TripPlanner.Services.CultureService;
using TripPlanner.Models.DTO.CultureDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;
using Microsoft.AspNetCore.Authorization;

namespace TripPlanner.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = ProjectConfiguration.HideContorller)]
    public class CultureController : ControllerBase
    {
        private readonly ICultureService _CultureService;

        public CultureController(ICultureService CultureService)
        {
            _CultureService = CultureService;
        }

        [HttpGet]
        public async Task<ActionResult<RepositoryResponse<List<CultureDTO>>>> Get()
        {
            var response = await _CultureService.GetCulturesAsync();
            List<CultureDTO> res = response.Data.Select(u => (CultureDTO)u).ToList();
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RepositoryResponse<CultureDTO>>> GetById(int id)
        {
            var response = await _CultureService.GetCultureAsync(u => u.Id == id);
            CultureDTO res = response.Data;
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<RepositoryResponse<bool>>> Create([FromBody] CreateCultureDTO Culture)
        {
            var resp = await _CultureService.GetCultureAsync(u => u.Name == Culture.Name);
            if (resp.Data != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Istnieje już taka nota kulturowa o nazwie = {Culture.Name}" };
            }

            Culture newCulture = Culture;

            var response = await _CultureService.CreateCulture(newCulture);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPut("{CultureId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Edit(int CultureId, [FromBody] CreateCultureDTO Culture)
        {
            var resp2 = await _CultureService.GetCultureAsync(u => u.Id == CultureId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje taka nota kulturowa o id = {CultureId}" };
            }
            var resp = await _CultureService.GetCultureAsync(u => u.Name == Culture.Name && u.Id != CultureId);
            if (resp.Data != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Istnieje już taka nota kulturowa o nazwie = {Culture.Name}" };
            }

            Culture elem = Culture;
            elem.Id = CultureId;

            var response = await _CultureService.UpdateCulture(elem);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Delete(int id)
        {
            var response = await _CultureService.DeleteCulture(new Culture() { Id = id });
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
