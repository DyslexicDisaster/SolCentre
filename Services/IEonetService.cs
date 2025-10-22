using System.Collections.Generic;
using System.Threading.Tasks;
using SolCentre.Models;

namespace SolCentre.Services
{
    public interface IEonetService
    {
        Task<IReadOnlyList<EonetEvent>> GetEventsAsync(int limit = 50, int? days = null, string source = null, string status = null);
        Task<EonetEvent?> GetEventByIdAsync(string id, CancellationToken ct = default);
    }
}
