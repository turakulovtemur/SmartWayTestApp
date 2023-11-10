namespace SmartWayTestAppplication.Services
{
    public interface IDownLoadLinkService
    {
        public Task<string> CreateLink(Guid[] fileIds, long userId, CancellationToken token);
        public Task<byte[]> GetFileToDownload(Guid linkId, CancellationToken token);
    }
}
