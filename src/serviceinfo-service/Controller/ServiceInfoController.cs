using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ServiceInfoService.Sevices.Http;
using ServiceInfoService.Sevices.IAM;

namespace OrderServiceQuery.API.Controllers.OrderController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceInfoController : ControllerBase
    {
        private readonly IIAMService _iamService;
        private readonly IHttpService _httpService;
        private readonly IValidator<Input> _validator;

        public ServiceInfoController(IIAMService iamService, IHttpService httpService, IValidator<Input> validator)
        {
            _iamService = iamService;
            _httpService = httpService;
            _validator = validator;
        }

        [HttpGet]
        [Route("ConnectToOrder")]
        public async Task<IActionResult> ConnectToOrder()
        {
            var token = await _iamService.GetCachedToken();

            if (token == null)
            {
                return Ok("failure");
            }

            var response = await _httpService.SendJsonRequestAsync("https://localhost:7120/api/order/Unstable", new {}, new Dictionary<string, string> {
                { "Authorization", "bearer " + token }
            });

            return Ok(response.StatusCode);
        }

        [HttpPost]
        [Route("FluentValidation")]
        public async Task<IActionResult> FluentValidation(Input input)
        {
            var validatorResult = await _validator.ValidateAsync(input);
            if (!validatorResult.IsValid)
            {
                return BadRequest();
            }

            return Ok();
        }

        public class Input
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public int Age { get; set; }
        }

        public class UserValidator : AbstractValidator<Input>
        {
            public UserValidator()
            {
                RuleFor(user => user.Name)
                    .NotNull().WithMessage("Not null")
                    .NotEmpty().WithMessage("Name is required.")
                    .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

                RuleFor(user => user.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Invalid email format.");

                RuleFor(user => user.Age)
                    .GreaterThan(0).WithMessage("Age must be a positive number.")
                    .LessThan(120).WithMessage("Age must be less than 120.");
            }
        }

    }
}
