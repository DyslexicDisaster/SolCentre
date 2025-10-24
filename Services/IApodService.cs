using System;
using System.Threading;
using System.Threading.Tasks;
using SolCentre.Models;

namespace SolCentre.Services
{
    public interface IApodService
    {
        Task<ApodModel> GetApodAsync(DateTime? date = null, CancellationToken ct = default);
    }
}
