using CashFlow.Application.UseCases.Expenses.Delete;
using CashFlow.Application.UseCases.Expenses.GetAll;
using CashFlow.Application.UseCases.Expenses.GetById;
using CashFlow.Application.UseCases.Expenses.Register;
using CashFlow.Application.UseCases.Expenses.Update;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;


namespace CashFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExpensesController : ControllerBase
{

    //Criar uma despesa -------------------------------------------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterExpense), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    
    public async Task<IActionResult> Register([FromServices] IRegisterExpenseUseCase useCase, [FromBody] RequestExpenseJson request)
    {

        var response = await useCase.Execute(request);

        return Created(string.Empty, response);

    }

    //Retorna todas as despesas ---------------------------------------------------------------------------------
    [HttpGet]
    [ProducesResponseType(typeof(ResponseExpenseJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]

    public async Task<IActionResult> GetAllExpenses([FromServices] IGetAllExpenseUseCase useCase)
    {
        var response = await useCase.Execute();

        if (response.Expenses.Count != 0)
            return Ok(response);

        return NoContent();

    }

    //retorna uma despesa especifica através do ID -----------------------------------------------------------------
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(ResponseExpenseByIdJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromServices] IGetExpenseByIdUseCase useCase, [FromRoute] long id)
    {
        var response = await useCase.Execute(id);

        return Ok(response);
    }

    // Deleta uma despesa especifica através do ID -------------------------------------------------------------------
    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromServices] IDeleteExpenseUseCase useCase, [FromRoute] long id)
    {
        await useCase.Execute(id);
        
        return NoContent();
    }

    // Atualiza uma despesa especifica atraves do ID ---------------------------------------------------------------------
    [HttpPut]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromServices] IUpdateExpenseUseCase useCase, [FromRoute] long id, [FromBody] RequestExpenseJson request)
    {
        await useCase.Execute(id, request);

        return NoContent();
    }

}
