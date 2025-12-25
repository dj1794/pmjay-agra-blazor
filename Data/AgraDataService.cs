using Microsoft.EntityFrameworkCore;

namespace PmjayAgra.Blazor.Data;

public class AgraDataService
{
    private readonly AgraDbContext _db;

    public AgraDataService(AgraDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<int> GetTotalAsync()
    {
        return await _db.Agra1.CountAsync();
    }

    // Server-side filtering and paging
    public async Task<List<Dictionary<string, object?>>> GetPageAsync(int page, int pageSize,
        string? block = null, string? village = null, string? ru = null, string? sourceType = null,
        string? memberStatus = null, string? familyStatus = null, string? search = null)
    {
        int offset = (page -1) * pageSize;

        IQueryable<Agra1Dto> q = _db.Agra1.AsNoTracking();

        q = ApplyFilters(q, block, village, ru, sourceType, memberStatus, familyStatus, search);

        var list = await q.Skip(offset).Take(pageSize).ToListAsync();

        var results = new List<Dictionary<string, object?>>();
        foreach (var dto in list)
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(dto.src_family_id)] = dto.src_family_id,
                [nameof(dto.src_member_id)] = dto.src_member_id,
                [nameof(dto.name)] = dto.name,
                [nameof(dto.relation)] = dto.relation,
                [nameof(dto.father_guardian_name)] = dto.father_guardian_name,
                [nameof(dto.rural_urban_flag)] = dto.rural_urban_flag,
                [nameof(dto.district_name)] = dto.district_name,
                [nameof(dto.blockname)] = dto.blockname,
                [nameof(dto.source_address)] = dto.source_address,
                [nameof(dto.addressasperadhaarofapproved)] = dto.addressasperadhaarofapproved,
                [nameof(dto.village_ward_lgd_code)] = dto.village_ward_lgd_code,
                [nameof(dto.source_type)] = dto.source_type,
                [nameof(dto.card_status_member)] = dto.card_status_member,
                [nameof(dto.card_status_family)] = dto.card_status_family,
                [nameof(dto.memberbelongtozeropovery)] = dto.memberbelongtozeropovery,
            };
            results.Add(row);
        }

        return results;
    }

    // EF Core method to fetch typed DTOs (with filters)
    public async Task<List<Agra1Dto>> GetSelectedColumnsAsync(int page, int pageSize,
        string? block = null, string? village = null, string? ru = null, string? sourceType = null,
        string? memberStatus = null, string? familyStatus = null, string? search = null)
    {
        IQueryable<Agra1Dto> q = _db.Agra1.AsNoTracking();
        q = ApplyFilters(q, block, village, ru, sourceType, memberStatus, familyStatus, search);
        return await q.Skip((page -1) * pageSize).Take(pageSize).ToListAsync();
    }

    // New: compute summary totals from the database with filters
    public async Task<SummaryDto> GetFilteredSummaryAsync(string? block = null, string? village = null, string? ru = null, string? sourceType = null,
        string? memberStatus = null, string? familyStatus = null, string? search = null)
    {
        IQueryable<Agra1Dto> q = _db.Agra1.AsNoTracking();
        q = ApplyFilters(q, block, village, ru, sourceType, memberStatus, familyStatus, search);

        // Total members = total rows matching filters
        var totalMembers = await q.CountAsync();

        // Total families = distinct non-empty src_family_id
        var totalFamilies = await q.Where(a => !string.IsNullOrEmpty(a.src_family_id))
            .Select(a => a.src_family_id)
            .Distinct()
            .CountAsync();

        // Covered members = count where card_status_member == 'Approved' (case-insensitive)
        var coveredMembers = await q.Where(a => !string.IsNullOrEmpty(a.card_status_member) && a.card_status_member.ToUpper() == "APPROVED").CountAsync();

        // Covered families = distinct src_family_id where any member in family has card_status_member == 'Approved'
        var coveredFamilies = await q.Where(a => !string.IsNullOrEmpty(a.src_family_id) && !string.IsNullOrEmpty(a.card_status_member) && a.card_status_member.ToUpper() == "APPROVED")
            .Select(a => a.src_family_id)
            .Distinct()
            .CountAsync();

        return new SummaryDto
        {
            TotalMembers = totalMembers,
            TotalFamilies = totalFamilies,
            CoveredMembers = coveredMembers,
            CoveredFamilies = coveredFamilies
        };
    }

    private static IQueryable<Agra1Dto> ApplyFilters(IQueryable<Agra1Dto> q,
        string? block, string? village, string? ru, string? sourceType,
        string? memberStatus, string? familyStatus, string? search)
    {
        if (!string.IsNullOrWhiteSpace(block))
        {
            var ub = block.ToUpper();
            q = q.Where(a => a.blockname != null && a.blockname.ToUpper() == ub);
        }

        if (!string.IsNullOrWhiteSpace(village))
        {
            var uv = village.ToUpper();
            q = q.Where(a => a.village_ward_lgd_code != null && a.village_ward_lgd_code.ToUpper().Contains(uv));
        }

        if (!string.IsNullOrWhiteSpace(ru))
        {
            var ur = ru.ToUpper();
            q = q.Where(a => a.rural_urban_flag != null && a.rural_urban_flag.ToUpper() == ur);
        }

        if (!string.IsNullOrWhiteSpace(sourceType))
        {
            var us = sourceType.ToUpper();
            q = q.Where(a => a.source_type != null && a.source_type.ToUpper() == us);
        }

        if (!string.IsNullOrWhiteSpace(memberStatus))
        {
            var um = memberStatus.ToUpper();
            q = q.Where(a => a.card_status_member != null && a.card_status_member.ToUpper() == um);
        }

        if (!string.IsNullOrWhiteSpace(familyStatus))
        {
            var uf = familyStatus.ToUpper();
            q = q.Where(a => a.card_status_family != null && a.card_status_family.ToUpper() == uf);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToUpper();
            q = q.Where(a => (a.name != null && a.name.ToUpper().Contains(s)) || (a.src_family_id != null && a.src_family_id.ToUpper().Contains(s)));
        }

        return q;
    }
}
