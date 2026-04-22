using System.Security.Claims;
using FluentValidation;
using GoodHamburger.Domain.DTOs;
using GoodHamburger.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IValidator<CreateOrderDto> _createValidator;
    private readonly IValidator<UpdateOrderDto> _updateValidator;
    
    public OrdersController(
        IOrderService orderService,
        IValidator<CreateOrderDto> createValidator,
        IValidator<UpdateOrderDto> updateValidator)
    {
        _orderService = orderService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    
    // GET api/orders - lista todos os pedidos do usuário logado
    [HttpGet]
    public async Task<ActionResult<List<OrderResponseDto>>> GetAll()
    {
        var userId = GetUserId();
        var orders = await _orderService.GetAllOrdersByUserAsync(userId);
        return Ok(orders);
    }
    
    // GET api/orders/5 - busca pedido por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        var userId = GetUserId();
        var order = await _orderService.GetOrderByIdAsync(id, userId);
        
        if (order == null)
        {
            return NotFound(new { message = "Pedido não encontrado" });
        }
        
        return Ok(order);
    }
    
    // POST api/orders - cria novo pedido
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create([FromBody] CreateOrderDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });
        }
        
        var userId = GetUserId();
        
        try
        {
            var order = await _orderService.CreateOrderAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    // PUT api/orders/5 - atualiza pedido existente
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderResponseDto>> Update(int id, [FromBody] UpdateOrderDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });
        }
        
        var userId = GetUserId();
        
        try
        {
            var order = await _orderService.UpdateOrderAsync(id, dto, userId);
            return Ok(order);
        }
        catch (InvalidOperationException)
        {
            return NotFound(new { message = "Pedido não encontrado" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    // DELETE api/orders/5 - remove pedido
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = GetUserId();
        
        try
        {
            await _orderService.DeleteOrderAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    // pega ID do usuário autenticado do token JWT
    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("User ID not found in token");
    }
}
