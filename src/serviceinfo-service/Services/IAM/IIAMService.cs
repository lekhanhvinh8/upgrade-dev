namespace ServiceInfoService.Sevices.IAM
{
    public interface IIAMService
    {
        Task<string?> GetCachedToken();
    }
}