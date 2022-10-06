using Demo.ViewModel;
using Demo.WebUi.IService;
using Microsoft.AspNetCore.Components;

namespace Demo.WebUi.Pages.Authentication
{
    public partial class Signin
    {
        [Inject]
        protected IAuthenticationRepository _authenticationRepository { get; set; }
        [Inject]
        protected NavigationManager _navigationManager { get; set; }

        protected SigninVM SigninModel = new SigninVM();
        protected bool isFlailed = false;

        protected async Task HandleSignin()
        {
            var response = await _authenticationRepository.Signin(SigninModel);
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
