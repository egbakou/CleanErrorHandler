using CleanErrorHandler.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanErrorHandler.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController(ILogger<ProductsController> logger, IProductService productService)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var data = productService.GetProducts();
        return Ok(data);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        // Customization in AddProblemDetails in Program.cs will not be applied here
        /*var problemDetails = new ProblemDetails
        {
            Title = "Invalid product id",
            Status = StatusCodes.Status404NotFound,
            Detail = "Product id must be between 1 and 10"
        };
        return BadRequest(problemDetails);*/

        // Customization in AddProblemDetails in Program.cs will be applied here
        /*return Problem(
            title: "Invalid product id",
            detail: "Product id must be between 1 and 10",
            statusCode: StatusCodes.Status400BadRequest
        );*/

        // Customization in AddProblemDetails in Program.cs will be applied here
        /*return NotFound();*/

        // Customization in AddProblemDetails in Program.cs will not be applied here
        //return NotFound("Product id must be between 1 and 10");
        
        var product = productService.GetProductById(id);
        if (product is null)
        {
            return NotFound();
        }
        
        return Ok(product);
    }
}