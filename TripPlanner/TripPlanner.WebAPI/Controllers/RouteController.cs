using Microsoft.AspNetCore.Mvc;
using TripPlanner.Services.RouteService;
using TripPlanner.Services.UserService;
using TripPlanner.Services.TourService;
using TripPlanner.Models.DTO.RouteDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.RouteModels;
using Route = TripPlanner.Models.Models.RouteModels.Route;
using Microsoft.AspNetCore.Authorization;

namespace TripPlanner.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = ProjectConfiguration.HideContorller)]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _RouteService;
        private readonly IUserService _UserService;
        private readonly ITourService _TourService;

        public RouteController(IRouteService RouteService, IUserService userService, ITourService tourService)
        {
            _RouteService = RouteService;
            _UserService = userService;
            _TourService = tourService;
        }

        [HttpGet]
        public async Task<ActionResult<RepositoryResponse<List<RouteDTO>>>> Get()
        {
            var response = await _RouteService.GetRoutesAsync();
            List<RouteDTO> res = response.Data.Select(u => (RouteDTO)u).ToList();
            return Ok(res);
        }

        [HttpGet("{routeId}/GetWholeDir")]
        public async Task<ActionResult<RepositoryResponse<string>>> GetWholeUrl(int routeId)
        {
            var response = await _RouteService.GetRouteAsync(u => u.Id == routeId, "Stopovers");
            if(response.Data == null)
            {
                return new RepositoryResponse<string> { Data = "", Success = false, Message = $"Nie istnieje trasa o id = {routeId}" };
            }

            Models.Models.RouteModels.Route route = response.Data;
            //zbudowac url i wyslac

            return Ok(new RepositoryResponse<string> { Data = "whole url", Message = "", Success = true});
        }

        [HttpGet("{routeId}/GetDirToNextStop/{nextStopoverId}")]
        public async Task<ActionResult<RepositoryResponse<string>>> Get(int routeId, int nextStopoverId)
        {
            var response = await _RouteService.GetRouteAsync(u => u.Id == routeId, "Stopovers");
            if (response.Data == null)
            {
                return new RepositoryResponse<string> { Data = "", Success = false, Message = $"Nie istnieje trasa o id = {routeId}" };
            }

            Route route = response.Data;
            //zbudowac url i wyslac

            return Ok(new RepositoryResponse<string> { Data = "part of url", Message = "", Success = true });
        }

        [HttpGet("GetWithStopovers/{id}")]
        public async Task<ActionResult<RepositoryResponse<RouteDTO>>> GetWithStopovers(int id)
        {
            var response = await _RouteService.GetRouteAsync(u => u.Id == id, "Stopovers");
            RouteDTO res = response.Data;
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RepositoryResponse<RouteDTO>>> GetById(int id)
        {
            var response = await _RouteService.GetRouteAsync(u => u.Id == id);
            RouteDTO res = response.Data;
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<RepositoryResponse<bool>>> Create([FromBody] CreateRouteDTO Route)
        {
            var resp = await _TourService.GetTourAsync(u => u.Id == Route.TourId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wycieczka o id = {Route.TourId}" };
            }
            if (resp.Data.Routes.FirstOrDefault(u => u.Name == Route.Name) != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Dana wycieczka posiada już trase o nazwie = {Route.Name}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == Route.UserId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {Route.UserId}" };
            }

            Route newRoute = Route;

            var response = await _RouteService.CreateRoute(newRoute);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return NotFound(response.Message);
            }
        }

        [HttpGet("{RouteId}/Stopovers")]
        public async Task<ActionResult<RepositoryResponse<List<StopoverDTO>>>> GetStopover(int RouteId)
        {
            var response = await _RouteService.GetStopoversAsync(u => u.RouteId == RouteId);
            List<StopoverDTO> res = response.Data.Select(u => (StopoverDTO)u).ToList();
            return Ok(res);
        }

        [HttpGet("{RouteId}/Stopover/{stopoverId}")]
        public async Task<ActionResult<RepositoryResponse<StopoverDTO>>> GetStopoverById(int RouteId, int stopoverId)
        {
            var response = await _RouteService.GetStopoverAsync(u => u.RouteId == RouteId && u.Id == stopoverId);
            StopoverDTO res = response.Data;
            return Ok(res);
        }

        [HttpPost("addStopover")]
        public async Task<ActionResult<RepositoryResponse<bool>>> AddStopover([FromBody] CreateStopoverDTO Route)
        {
            var resp2 = await _RouteService.GetRouteAsync(u => u.Id == Route.RouteId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje trasa o id = {Route.RouteId}" };
            }
            var resp3 = await _RouteService.GetStopoversAsync(u => u.RouteId == Route.RouteId);
            if (resp3.Data != null)
            {
                var user = resp3.Data.Find(u => u.Name == Route.Name);
                if (user != null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Trasa zawiera już postoj o nazwie = {Route.Name}" };
            }

            Stopover elem = Route;

            var response = await _RouteService.AddStopoverToRoute(elem);
            return Ok(response.Data);
        }

        [HttpPut("{RouteId}/editStopover/{stopoverId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Edit(int RouteId, int stopoverId, [FromBody] EditStopoverDTO stopover)
        {
            var resp2 = await _RouteService.GetRouteAsync(u => u.Id == RouteId, "Stopovers");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje trasa o id = {RouteId}" };
            }
            var resp3 = await _RouteService.GetStopoverAsync(u => u.Id == stopoverId);
            if (resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje postoj o id = {stopoverId}" };
            }

            var res = resp2.Data.Stopovers.FirstOrDefault(u => u.Name == stopover.Name && u.Id != stopoverId);
            if (res != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Trasa zawiera już postoj o nazwie = {res.Name}" };
            }

            Stopover newStopover = stopover;
            newStopover.RouteId = RouteId;
            newStopover.Id = stopoverId;

            var response = await _RouteService.UpdateStopover(newStopover);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return NotFound(response.Message);
            }
        }

        [HttpDelete("{RouteId}/deleteStopover/{stopoverId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteStopover(int RouteId, int stopoverId)
        {
            var resp2 = await _RouteService.GetRouteAsync(u => u.Id == RouteId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje trasa o id = {RouteId}" };
            }
            var resp3 = await _RouteService.GetStopoverAsync(u => u.Id == stopoverId);
            if (resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje postoj o id = {stopoverId}" };
            }

            Stopover elem = resp3.Data;
            elem.RouteId = RouteId;
            elem.Id = stopoverId;

            var response = await _RouteService.DeleteStopoverFromRoute(elem);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return NotFound(response.Message);
            }
        }

        [HttpPut("{RouteId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Edit(int RouteId, [FromBody] EditRouteDTO Route)
        {
            var resp2 = await _RouteService.GetRouteAsync(u => u.Id == RouteId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje trasa o id = {RouteId}" };
            }

            Route elem = resp2.Data;
            elem.Name = Route.Name;
            elem.StartLocation = Route.StartLocation;
            elem.ArriveLocation = Route.ArriveLocation;
            elem.StartDate = Route.StartDate;
            elem.ArriveDate = Route.ArriveDate;
            elem.Id = RouteId;

            var response = await _RouteService.UpdateRoute(elem);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return NotFound(response.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Delete(int id)
        {
            var response = await _RouteService.DeleteRoute(new Route() { Id = id });
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return NotFound(response.Data);
            }
        }
    }
}
