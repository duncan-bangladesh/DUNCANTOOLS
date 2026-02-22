using DuncanTool.Api.Model;

namespace DuncanTool.Api.Service
{
    public interface IWeighbridgeScaleDataService
    {
        Task<List<WeighbridgeScaleData>> GetAllWeighbridgeScaleData();
        Task<int> SaveScaleData(WeighbridgeScaleData model);
    }
}
