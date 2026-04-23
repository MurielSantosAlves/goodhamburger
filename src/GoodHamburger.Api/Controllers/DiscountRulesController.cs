using GoodHamburger.Domain.DTOs;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DiscountRulesController : ControllerBase
{
    private readonly IDiscountRuleRepository _discountRuleRepository;
    
    public DiscountRulesController(IDiscountRuleRepository discountRuleRepository)
    {
        _discountRuleRepository = discountRuleRepository;
    }
    
    // GET api/discountrules - retorna todas as regras de desconto
    [HttpGet]
    public async Task<ActionResult<List<DiscountRuleDto>>> GetAll()
    {
        var rules = await _discountRuleRepository.GetAllAsync();
        var ruleDtos = rules.Select(r => new DiscountRuleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            DiscountPercentage = r.DiscountPercentage,
            RequiresSandwich = r.RequiresSandwich,
            RequiresSideDish = r.RequiresSideDish,
            RequiresDrink = r.RequiresDrink,
            IsActive = r.IsActive,
            Priority = r.Priority,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
        
        return Ok(ruleDtos);
    }
    
    // GET api/discountrules/active - retorna apenas regras ativas
    [HttpGet("active")]
    public async Task<ActionResult<List<DiscountRuleDto>>> GetActive()
    {
        var rules = await _discountRuleRepository.GetAllActiveAsync();
        var ruleDtos = rules.Select(r => new DiscountRuleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            DiscountPercentage = r.DiscountPercentage,
            RequiresSandwich = r.RequiresSandwich,
            RequiresSideDish = r.RequiresSideDish,
            RequiresDrink = r.RequiresDrink,
            IsActive = r.IsActive,
            Priority = r.Priority,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
        
        return Ok(ruleDtos);
    }
    
    // GET api/discountrules/{id} - retorna uma regra específica
    [HttpGet("{id}")]
    public async Task<ActionResult<DiscountRuleDto>> GetById(int id)
    {
        var rule = await _discountRuleRepository.GetByIdAsync(id);
        if (rule == null)
            return NotFound(new { message = "Regra de desconto não encontrada" });
        
        var ruleDto = new DiscountRuleDto
        {
            Id = rule.Id,
            Name = rule.Name,
            Description = rule.Description,
            DiscountPercentage = rule.DiscountPercentage,
            RequiresSandwich = rule.RequiresSandwich,
            RequiresSideDish = rule.RequiresSideDish,
            RequiresDrink = rule.RequiresDrink,
            IsActive = rule.IsActive,
            Priority = rule.Priority,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt
        };
        
        return Ok(ruleDto);
    }
    
    // POST api/discountrules - cria uma nova regra
    [HttpPost]
    public async Task<ActionResult<DiscountRuleDto>> Create([FromBody] CreateDiscountRuleDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var rule = new DiscountRule
        {
            Name = createDto.Name,
            Description = createDto.Description,
            DiscountPercentage = createDto.DiscountPercentage,
            RequiresSandwich = createDto.RequiresSandwich,
            RequiresSideDish = createDto.RequiresSideDish,
            RequiresDrink = createDto.RequiresDrink,
            IsActive = createDto.IsActive,
            Priority = createDto.Priority,
            CreatedAt = DateTime.UtcNow
        };
        
        var createdRule = await _discountRuleRepository.CreateAsync(rule);
        
        var ruleDto = new DiscountRuleDto
        {
            Id = createdRule.Id,
            Name = createdRule.Name,
            Description = createdRule.Description,
            DiscountPercentage = createdRule.DiscountPercentage,
            RequiresSandwich = createdRule.RequiresSandwich,
            RequiresSideDish = createdRule.RequiresSideDish,
            RequiresDrink = createdRule.RequiresDrink,
            IsActive = createdRule.IsActive,
            Priority = createdRule.Priority,
            CreatedAt = createdRule.CreatedAt,
            UpdatedAt = createdRule.UpdatedAt
        };
        
        return CreatedAtAction(nameof(GetById), new { id = ruleDto.Id }, ruleDto);
    }
    
    // PUT api/discountrules/{id} - atualiza uma regra existente
    [HttpPut("{id}")]
    public async Task<ActionResult<DiscountRuleDto>> Update(int id, [FromBody] UpdateDiscountRuleDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var existingRule = await _discountRuleRepository.GetByIdAsync(id);
        if (existingRule == null)
            return NotFound(new { message = "Regra de desconto não encontrada" });
        
        existingRule.Name = updateDto.Name;
        existingRule.Description = updateDto.Description;
        existingRule.DiscountPercentage = updateDto.DiscountPercentage;
        existingRule.RequiresSandwich = updateDto.RequiresSandwich;
        existingRule.RequiresSideDish = updateDto.RequiresSideDish;
        existingRule.RequiresDrink = updateDto.RequiresDrink;
        existingRule.IsActive = updateDto.IsActive;
        existingRule.Priority = updateDto.Priority;
        
        var updatedRule = await _discountRuleRepository.UpdateAsync(existingRule);
        
        var ruleDto = new DiscountRuleDto
        {
            Id = updatedRule.Id,
            Name = updatedRule.Name,
            Description = updatedRule.Description,
            DiscountPercentage = updatedRule.DiscountPercentage,
            RequiresSandwich = updatedRule.RequiresSandwich,
            RequiresSideDish = updatedRule.RequiresSideDish,
            RequiresDrink = updatedRule.RequiresDrink,
            IsActive = updatedRule.IsActive,
            Priority = updatedRule.Priority,
            CreatedAt = updatedRule.CreatedAt,
            UpdatedAt = updatedRule.UpdatedAt
        };
        
        return Ok(ruleDto);
    }
    
    // DELETE api/discountrules/{id} - remove uma regra
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existingRule = await _discountRuleRepository.GetByIdAsync(id);
        if (existingRule == null)
            return NotFound(new { message = "Regra de desconto não encontrada" });
        
        await _discountRuleRepository.DeleteAsync(id);
        return NoContent();
    }
}
