namespace Ipitup_backend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    public AuthController(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login( LoginRequest request)
    {
        var user = await _userRepository.AuthenticateAsync(request.Email, request.Password);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });
        var token = _jwtService.GenerateToken(user);
        return Ok(new { token, user });
    }
}
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
