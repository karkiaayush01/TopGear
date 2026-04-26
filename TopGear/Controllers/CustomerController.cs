using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.CustomerDTO;
using TopGear.Application.Interfaces;


namespace TopGear.Controllers
{

    [ApiController]
    [Route("api/Customers")]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerRequest request)
        {
            var result = await customerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var result = await customerService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request)
        {
            await customerService.UpdateAsync(id, request);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await customerService.DeactivateAsync(id);
            return NoContent();
        }
    }
}
