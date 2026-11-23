using Microsoft.AspNetCore.Mvc;
using Orders.Api.Application;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrdersController(
    PlaceOrderHandler placeOrder,
    GetOrderHandler getOrder,
    CancelOrderHandler cancelOrder) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Place(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var id = await placeOrder.HandleAsync(command, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var order = await getOrder.HandleAsync(new GetOrderQuery(id), cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var found = await cancelOrder.HandleAsync(new CancelOrderCommand(id), cancellationToken);
        return found ? NoContent() : NotFound();
    }
}
