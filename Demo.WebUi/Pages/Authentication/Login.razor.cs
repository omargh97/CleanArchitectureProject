using Demo.ViewModel;
using Demo.WebUi.IService;
using Microsoft.AspNetCore.Components;

namespace Demo.WebUi.Pages.Authentication
{
    public partial class Login
    {
        [Inject]
        protected IAuthenticationRepository _authenticationRepository { get; set; }
        [Inject]
        protected NavigationManager _navigationManager { get; set; }

        protected LoginRequestVM loginModel = new LoginRequestVM();
        protected bool isFlailed = false;

        protected async Task HandleLogin()
        {
            var response = await _authenticationRepository.Login(loginModel);
            if (response)
            {
                _navigationManager.NavigateTo("/Login");
            }
            else
            {
                isFlailed = true;

            }
        }
    }
}
