using System.ComponentModel.DataAnnotations;

namespace Blazor_wasm_jwt.Auth.DTOs
{
    public class AuthenticationRequestDTO
    {


        public string UserName { get; set; } = "";


        public string Password { get; set; } = "";


    }
}
