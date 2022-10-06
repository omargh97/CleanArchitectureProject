using Demo.ViewModel;

namespace Demo.WebUi.IService
{
    public interface IAuthenticationRepository
    {
        public Task<bool> Signin(SigninVM signin);
        public Task<bool> Login(LoginRequestVM loginRequest);
        public Task Logout();
    }
}
