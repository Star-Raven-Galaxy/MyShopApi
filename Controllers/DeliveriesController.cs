// Controllers/DeliveriesController.cs
using Microsoft.AspNetCore.Mvc;
using MyShopApi.Models.DTO;
using MyShopApi.Services;

namespace MyShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveriesController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            var delivery = await _deliveryService.GetDeliveryByOrderIdAsync(orderId);
            return delivery == null ? NotFound() : Ok(delivery);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DeliveryCreateDto dto)
        {
            try
            {
                var delivery = await _deliveryService.CreateDeliveryAsync(dto);
                return CreatedAtAction(nameof(GetByOrder), new { orderId = delivery.Id }, delivery);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDeliveryDto dto)
        {
            var success = await _deliveryService.UpdateDeliveryAsync(id, dto.Address, dto.DeliveryDate);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _deliveryService.DeleteDeliveryAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }

    public class UpdateDeliveryDto
    {
        public string Address { get; set; } = "";
        public DateTime DeliveryDate { get; set; }
    }
}