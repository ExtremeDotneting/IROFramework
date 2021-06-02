namespace IROFramework.Web.Dto.AuthDto
{
    public class LoginResponseResponse: IDevCommentResponse
    {
        public string RefreshToken { get; set; }

        public string AccessToken { get; set; }

        public string Comment { get; set; } = $"Token automatically added to your cookie params.";
    }
}